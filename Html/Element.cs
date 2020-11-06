using System.Collections.Generic;

namespace Html
{
    public class Element : Node
    {
        string _tagName;
        public override string tagName { get => _tagName; set { _tagName = value; } }

        public override int Type => Node.ELEMENT;
    }
}
