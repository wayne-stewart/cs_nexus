using System;
using System.Collections.Generic;
using System.Text;

namespace Html
{
    public class Tokenizer
    {
        public const int TOKEN_OPEN_ANGLE       = 1;  // <
        public const int TOKEN_CLOSE_ANGLE      = 2;  // >
        public const int TOKEN_OPEN_END_TAG     = 3;  // </
        public const int TOKEN_SELF_CLOSE_TAG   = 4;  // />
        public const int TOKEN_OPEN_ANGLE_BANG  = 5;  // <!
        public const int TOKEN_NAME             = 6;  // abc
        public const int TOKEN_DBL_QUOTED_TEXT  = 7;  // "how are you"
        public const int TOKEN_WHITESPACE       = 8;
        public const int TOKEN_EQUALS           = 9;  // =
        public const int TOKEN_SNGL_QUOTED_TEXT = 10; // 'how are you'
        public const int TOKEN_DOUBLE_DASH      = 11; // --

        public static IEnumerable<Token> Tokenize(string s)
        {
            var char_enumerator = EnumerateChars(s);
            return EnumerateTokens(char_enumerator);
        }

        static IEnumerable<CharTuple> EnumerateChars(string s)
        {
            var s_len = s.Length - 1;
            var s_index = 0;
            var s_line = 1;
            var s_line_index = 0;
            for (; s_index < s_len; s_index++)
            {
                yield return new CharTuple 
                { 
                    c = s[s_index], 
                    c1 = s[s_index + 1], 
                    index = s_index, 
                    line = s_line, 
                    line_index = s_line_index 
                };
                if (s[s_index] == '\n')
                {
                    s_line++;
                    s_line_index = 0;
                }
                else
                {
                    s_line_index++;
                }
            }
            yield return new CharTuple 
            { 
                c = s[s_index], 
                c1 = (char)0, 
                index = s_index,
                line = s_line,
                line_index = s_line_index
            };
        }

        static IEnumerable<Token> EnumerateTokens(IEnumerable<CharTuple> tuples)
        {
            var sb = new StringBuilder();
            var tuple_enumerator = tuples as IEnumerator<CharTuple>;
            foreach (var tuple in tuples)
            {
                var token = new Token { Line = tuple.line, LineIndex = tuple.line_index };

                if (tuple.c == '<' && tuple.c1 == '/')
                {
                    tuple_enumerator.MoveNext();
                    token.ID = TOKEN_OPEN_END_TAG;
                    token.Content = "</";
                }
                else if (tuple.c == '<' && tuple.c1 == '!')
                {
                    tuple_enumerator.MoveNext();
                    token.ID = TOKEN_OPEN_ANGLE_BANG;
                    token.Content = "<!";
                }
                else if (tuple.c == '/' && tuple.c1 == '>')
                {
                    tuple_enumerator.MoveNext();
                    token.ID = TOKEN_SELF_CLOSE_TAG;
                    token.Content = "/>";
                }
                else if (tuple.c == '-' && tuple.c1 == '-')
                {
                    tuple_enumerator.MoveNext();
                    token.ID = TOKEN_DOUBLE_DASH;
                    token.Content = "--";
                }
                else if (tuple.c == '<')
                {
                    token.ID = TOKEN_OPEN_ANGLE;
                    token.Content = "<";
                }
                else if (tuple.c == '>')
                {
                    token.ID = TOKEN_CLOSE_ANGLE;
                    token.Content = ">";
                }
                else if (tuple.c == '=')
                {
                    token.ID = TOKEN_EQUALS;
                    token.Content = "=";
                }
                else if (char.IsLetter(tuple.c))
                {
                    sb.Clear();
                    sb.Append(tuple.c);
                    var local = tuple;
                    while (char.IsLetterOrDigit(local.c1))
                    {
                        tuple_enumerator.MoveNext();
                        local = tuple_enumerator.Current;
                        sb.Append(local.c);
                    }
                    token.ID = TOKEN_NAME;
                    token.Content = sb.ToString();
                }
                else if (char.IsWhiteSpace(tuple.c))
                {
                    sb.Clear();
                    sb.Append(tuple.c);
                    var local = tuple;
                    while (char.IsWhiteSpace(local.c1))
                    {
                        tuple_enumerator.MoveNext();
                        local = tuple_enumerator.Current;
                        sb.Append(local.c);
                    }
                    token.ID = TOKEN_WHITESPACE;
                    token.Content = sb.ToString();
                }
                else if (tuple.c == '"')
                {
                    sb.Clear();
                    tuple_enumerator.MoveNext();
                    var local = tuple_enumerator.Current;
                    while (local.c != '"')
                    {
                        sb.Append(local.c);
                        tuple_enumerator.MoveNext();
                        local = tuple_enumerator.Current;
                    }
                    token.ID = TOKEN_DBL_QUOTED_TEXT;
                    token.Content = sb.ToString();
                }
                else if (tuple.c == '\'')
                {
                    sb.Clear();
                    tuple_enumerator.MoveNext();
                    var local = tuple_enumerator.Current;
                    while (local.c != '\'')
                    {
                        sb.Append(local.c);
                        tuple_enumerator.MoveNext();
                        local = tuple_enumerator.Current;
                    }
                    token.ID = TOKEN_SNGL_QUOTED_TEXT;
                    token.Content = sb.ToString();
                }
                else
                {
                    throw new Exception($"Unsupported char encountered line: {token.Line} index: {token.LineIndex}");
                }

                yield return token;
            }
        }


    }
}
