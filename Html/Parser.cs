using Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Html
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(Token t) : base($"Invalid token {t.Line} index: {t.LineIndex}") { }
    }

    public class Parser
    {
        const int WHITE_SPACE = 0;
        const int DOCTYPE = 1;
        const int BEGIN_TAG = 2;
        const int COMMENT = 3;
        const int ATTRIBUTE = 4;
        const int TEXT = 5;

        readonly string[] AUTOCLOSE_TAGS = new string[] { "DOCTYPE", "br", "hr","base","col","command","embed","img","input","keygen","link","meta","param","source","track","wbr" };

        ILog _log;

        public Parser(ILog log)
        {
            _log = log;
        }

        public List<Node> Parse(string s)
        {
            Node node = new Root();
            var mode = WHITE_SPACE;
            try
            {
                var tokens = Tokenizer.Tokenize(s);
                using var tokens_enumerator = tokens.GetEnumerator();
                using var tokentuple_enumerator = TokenTupler(tokens_enumerator).GetEnumerator();

                TokenTuple tp;
                while (tokentuple_enumerator.MoveNext())
                {
                    tp = tokentuple_enumerator.Current;

                    switch(mode)
                    {
                        case WHITE_SPACE:
                            if (tp.t.ID == Tokenizer.TOKEN_OPEN_ANGLE_BANG && tp.t1.ID == Tokenizer.TOKEN_NAME && string.Equals(tp.t1.Content, "DOCTYPE", StringComparison.OrdinalIgnoreCase))
                            {
                                PushNode(new DocType(), ref node);
                                tokentuple_enumerator.MoveNext();
                                mode = BEGIN_TAG;
                            }
                            else if (tp.t.ID == Tokenizer.TOKEN_OPEN_ANGLE && tp.t1.ID == Tokenizer.TOKEN_NAME)
                            {
                                PushNode(new Element { tagName = tp.t1.Content }, ref node);
                                tokentuple_enumerator.MoveNext();
                                mode = BEGIN_TAG;
                            }
                            else if (tp.t.ID == Tokenizer.TOKEN_OPEN_END_TAG && tp.t1.ID == Tokenizer.TOKEN_NAME)
                            {
                                if (!string.Equals(tp.t1.Content, node.tagName, StringComparison.OrdinalIgnoreCase)) throw new InvalidTokenException(tp.t);
                                tokentuple_enumerator.MoveNext();
                                tokentuple_enumerator.MoveNext();
                                tp = tokentuple_enumerator.Current;
                                if (tp.t.ID != Tokenizer.TOKEN_CLOSE_ANGLE) throw new InvalidTokenException(tp.t);
                                node = node.Parent;
                            }
                            else
                            {
                                PushNode(new Text { Value = tp.t.Content }, ref node);
                                mode = TEXT;
                            }
                            break;
                        case BEGIN_TAG:
                            __BEGIN_TAG:
                            if (tp.t.ID == Tokenizer.TOKEN_WHITESPACE)
                            {
                                // ignored
                            }
                            else if (tp.t.ID == Tokenizer.TOKEN_NAME)
                            {
                                PushNode(new Attribute { Key = tp.t.Content }, ref node);
                                mode = ATTRIBUTE;
                            }
                            else if (tp.t.ID == Tokenizer.TOKEN_CLOSE_ANGLE)
                            {
                                if (AUTOCLOSE_TAGS.Contains(node.tagName, StringComparer.OrdinalIgnoreCase))
                                {
                                    node = node.Parent;
                                }
                                mode = WHITE_SPACE;
                            }
                            else if (tp.t.ID == Tokenizer.TOKEN_SELF_CLOSE_TAG)
                            {
                                node = node.Parent;
                                mode = WHITE_SPACE;
                            }
                            else
                            {
                                throw new InvalidTokenException(tp.t);
                            }
                            break;
                        case ATTRIBUTE:
                            switch(tp.t.ID)
                            {
                                case Tokenizer.TOKEN_EQUALS:
                                    switch(tp.t1.ID)
                                    {
                                        case Tokenizer.TOKEN_DBL_QUOTED_TEXT:
                                        case Tokenizer.TOKEN_SNGL_QUOTED_TEXT:
                                        case Tokenizer.TOKEN_NAME:
                                            ((Attribute)node).Value = tp.t1.Content;
                                            node = node.Parent;
                                            tokentuple_enumerator.MoveNext();
                                            mode = BEGIN_TAG;
                                            break;
                                        default:
                                            throw new InvalidTokenException(tp.t);
                                    }    
                                    break;
                                default:
                                    node = node.Parent;
                                    mode = BEGIN_TAG;
                                    goto __BEGIN_TAG;
                            }
                            break;
                        case TEXT:
                            if (tp.t.ID == Tokenizer.TOKEN_OPEN_END_TAG || tp.t.ID == Tokenizer.TOKEN_OPEN_ANGLE)
                            {
                                node = node.Parent;
                                goto case WHITE_SPACE;
                            }
                            ((Text)node).Value = tp.t.Content;
                            break;
                        default:
                            throw new InvalidTokenException(tp.t);
                    }
                }

                PopToRoot(ref node);
                _log.LogDebug(WriteNodeHierarchy(node));
                return node.Children;
            }
            catch //(Exception ex)
            {
                PopToRoot(ref node);
                _log.LogDebug(WriteNodeHierarchy(node));
                throw;
            }
        }

        static IEnumerable<TokenTuple> TokenTupler(IEnumerator<Token> tokens)
        {
            Token t, t1;
            if (tokens.MoveNext())
            {
                t1 = tokens.Current;
                while (tokens.MoveNext())
                {
                    t = t1;
                    t1 = tokens.Current;

                    yield return new TokenTuple { t = t, t1 = t1 };
                }
                yield return new TokenTuple { t = t1 };
            }
        }

        static void PushNode(Node node, ref Node parent)
        {
            node.Parent = parent;

            if (node is Attribute)
                parent.Attributes.Add((Attribute)node);
            else
                parent.Children.Add(node);

            parent = node;
        }

        static void PopToRoot(ref Node node)
        {
            while (node.Parent != null)
                node = node.Parent;
        }

        static string WriteNodeHierarchy(Node node)
        {
            var sb = new StringBuilder();
            WriteNodeHierarchy(node, 0, new StringWriter(sb));
            return sb.ToString();
        }

        static void WriteNodeHierarchy(Node node, int indent, TextWriter writer)
        {
            writer.Write(new string(' ', indent));
            writer.Write(node.NodeName);
            writer.Write(' ');
            writer.WriteLine(node.tagName);
            foreach(var child in node.Children)
            {
                WriteNodeHierarchy(child, indent + 2, writer);
            }
        }
    }
}
