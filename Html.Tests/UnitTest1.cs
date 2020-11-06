using Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

namespace Html.Tests
{
    public class Tests
    {
        ILog log = new DebugLogger();

        [Test]
        public void TokenTest_1()
        {
            var tokens = Tokenizer.Tokenize("<!DOCTYPE html><html lang=\"en\">").ToArray();
            Assert.That(tokens.Length, Is.EqualTo(12));

            var index = 0;
            Assert.That(tokens[index].ID, Is.EqualTo(Tokenizer.TOKEN_OPEN_ANGLE_BANG));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_NAME));
            Assert.That(tokens[index].Content, Is.EqualTo("DOCTYPE"));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_WHITESPACE));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_NAME));
            Assert.That(tokens[index].Content, Is.EqualTo("html"));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_CLOSE_ANGLE));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_OPEN_ANGLE));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_NAME));
            Assert.That(tokens[index].Content, Is.EqualTo("html"));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_WHITESPACE));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_NAME));
            Assert.That(tokens[index].Content, Is.EqualTo("lang"));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_EQUALS));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_DBL_QUOTED_TEXT));
            Assert.That(tokens[index].Content, Is.EqualTo("en"));
            Assert.That(tokens[++index].ID, Is.EqualTo(Tokenizer.TOKEN_CLOSE_ANGLE));
        }

        [Test]
        public void Test1()
        {
            var sut = new Parser(log);
            var nodes = sut.Parse("<html>");
            Assert.That(nodes.Count, Is.EqualTo(1));
            Assert.That(nodes[0].GetType(), Is.EqualTo(typeof(Element)));
            Assert.That(((Element)nodes[0]).tagName, Is.EqualTo("html"));
        }

        [Test]
        public void Test2() {
            var sut = new Parser(log);
            var nodes = sut.Parse("<!DOCTYPE><html>");
            Assert.That(nodes.Count, Is.EqualTo(2));
            Assert.That(nodes[0].GetType(), Is.EqualTo(typeof(DocType)));
            Assert.That(nodes[1].GetType(), Is.EqualTo(typeof(Element)));
            Assert.That(((Element)nodes[1]).tagName, Is.EqualTo("html"));
        }

        [Test]
        public void Test3()
        {
            var sut = new Parser(log);
            var nodes = sut.Parse("<!DOCTYPE html><html lang=\"en\">");
            Assert.That(nodes.Count, Is.EqualTo(2));
            Assert.That(nodes[0].GetType(), Is.EqualTo(typeof(DocType)));
            Assert.That(nodes[0].Attributes.Count, Is.EqualTo(1));
            Assert.That(nodes[0].Attributes[0].Key, Is.EqualTo("html"));
            Assert.That(nodes[1].GetType(), Is.EqualTo(typeof(Element)));
            Assert.That(((Element)nodes[1]).tagName, Is.EqualTo("html"));
            Assert.That(nodes[1].Attributes.Count, Is.EqualTo(1));
            Assert.That(nodes[1].Attributes[0].Key, Is.EqualTo("lang"));
            Assert.That(nodes[1].Attributes[0].Value, Is.EqualTo("en"));
        }

        [Test]
        public void Test4()
        {
            var sut = new Parser(log);
            var nodes = sut.Parse("<!DOCTYPE html><html lang=\"en\"><body><h1>green smoothies</h1><hr>are great</body></html>");
            Assert.That(nodes.Count, Is.EqualTo(2));
            Assert.That(nodes[0].GetType(), Is.EqualTo(typeof(DocType)));
            Assert.That(nodes[0].Attributes.Count, Is.EqualTo(1));
            Assert.That(nodes[0].Attributes[0].Key, Is.EqualTo("html"));


            Assert.That(nodes[1].GetType(), Is.EqualTo(typeof(Element)));
            Assert.That(((Element)nodes[1]).tagName, Is.EqualTo("html"));
            Assert.That(nodes[1].Attributes.Count, Is.EqualTo(1));
            Assert.That(nodes[1].Attributes[0].Key, Is.EqualTo("lang"));
            Assert.That(nodes[1].Attributes[0].Value, Is.EqualTo("en"));

            Assert.That(nodes[1].Children.Count, Is.EqualTo(1));
            Assert.That(((Element)nodes[1].Children[0]).tagName, Is.EqualTo("body"));

            var body = nodes[1].Children[0] as Element;
            Assert.That(body.Children.Count, Is.EqualTo(3));
            var h1 = body.Children[0] as Element;
            var hr = body.Children[1] as Element;
            var text_are_great = body.Children[2] as Text;
            Assert.That(h1.tagName, Is.EqualTo("h1"));
            Assert.That(((Text)h1.Children[0]).Value, Is.EqualTo("green smoothies"));
            Assert.That(hr.tagName, Is.EqualTo("hr"));
            Assert.That(text_are_great.Value, Is.EqualTo("are great"));
        }

        [Test]
        public void Test5()
        {
            var sut = new Parser(log);
            var nodes = sut.Parse("<!DOCTYPE><html><body><input type=checkbox checked=checked><input type=text disabled=disabled value='single quoted value' /><p>my paragraph");
            var doctype = nodes[0] as DocType;
            var html = nodes[1] as Element;
            var body = html.Children[0] as Element;
            var input1 = body.Children[0] as Element;
            var input2 = body.Children[1] as Element;
            var p = body.Children[2] as Element;
            var p_text = p.Children[0] as Text;


            Assert.That(doctype, Is.Not.Null);
            Assert.That(html, Is.Not.Null);
            Assert.That(body, Is.Not.Null);
            Assert.That(input1, Is.Not.Null);
            Assert.That(input2, Is.Not.Null);
            Assert.That(p, Is.Not.Null);
            Assert.That(p_text, Is.Not.Null);

            Assert.That(input1.Attributes[0].Key, Is.EqualTo("type"));
            Assert.That(input1.Attributes[0].Value, Is.EqualTo("checkbox"));
            Assert.That(input1.Attributes[1].Key, Is.EqualTo("checked"));
            Assert.That(input1.Attributes[1].Value, Is.EqualTo("checked"));

            Assert.That(input2.Attributes[0].Key, Is.EqualTo("type"));
            Assert.That(input2.Attributes[0].Value, Is.EqualTo("text"));
            Assert.That(input2.Attributes[1].Key, Is.EqualTo("disabled"));
            Assert.That(input2.Attributes[1].Value, Is.EqualTo("disabled"));
            Assert.That(input2.Attributes[2].Key, Is.EqualTo("value"));
            Assert.That(input2.Attributes[2].Value, Is.EqualTo("single quoted value"));

            Assert.That(p_text.Value, Is.EqualTo("my paragraph"));
        }

        [Test]
        public void Test6()
        {
            using var stream = Assembly.GetAssembly(typeof(Tests)).GetManifestResourceStream("Html.Tests.test6.html");
            using var reader = new StreamReader(stream);
            var html = reader.ReadToEnd();

            var sut = new Parser(log);
            var nodes = sut.Parse(html);
        }
    }
}



