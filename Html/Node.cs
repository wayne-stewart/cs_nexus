using System.Collections.Generic;

namespace Html
{
    public abstract class Node
    {
        public const int ROOT = 0;
        public const int ELEMENT = 1;
        public const int DOCTYPE = 2;
        public const int ATTRIBUTE = 3;
        public const int TEXT = 4;

        List<Node> _children;
        List<Attribute> _attributes;

        public Node Parent { get; set; }

        public bool Open { get; set; } = true;

        public abstract int Type { get; }
        
        public virtual string tagName { get { return string.Empty; } set { } }

        public string NodeName => this.GetType().Name;

        public List<Node> Children => (_children ?? (_children = new List<Node>()));

        public List<Attribute> Attributes => (_attributes ?? (_attributes = new List<Attribute>()));
    }
}
