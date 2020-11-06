using NUnit.Framework;

namespace Config.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var s = "[a]\nb=c";
            var config = new INIConfig(s);
            Assert.That(config["a", "b"], Is.EqualTo("c"));
            Assert.That(config["a", "c"], Is.Null);
            Assert.That(config["aa", "b"], Is.Null);
        }

        [Test]
        public void Test2()
        {
            var s = ";comment a\n[aaa]\r\n;comment b  \nabc=def\r\nghi=jkl\n\r\n    \n\n\nmno=pqr";
            var config = new INIConfig(s);
            Assert.That(config["aaa", "abc"], Is.EqualTo("def"));
            Assert.That(config["aaa", "ghi"], Is.EqualTo("jkl"));
            Assert.That(config["aaa", "mno"], Is.EqualTo("pqr"));
        }

        [Test]
        public void Test3()
        {
            var s = "[a]\n[b]\n[c]\nd=f\n[g]\nh=i\nj=k";
            var config = new INIConfig(s);
            Assert.That(config["c", "d"], Is.EqualTo("f"));
            Assert.That(config["g", "h"], Is.EqualTo("i"));
            Assert.That(config["g", "j"], Is.EqualTo("k"));
        }
    }
}