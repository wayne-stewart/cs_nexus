namespace Html
{
    public class DocType : Node
    {
        public override int Type => Node.DOCTYPE;

        public override string tagName { get => "DOCTYPE"; set { } }
    }
}
