namespace Html
{
    public class Attribute : Node
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public override int Type => Node.ATTRIBUTE;
    }
}
