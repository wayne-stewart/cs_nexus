using System.Text;

namespace Html
{
    public class Text : Node
    {
        StringBuilder sb = new StringBuilder();

        public string Value { 
            get 
            { 
                return sb.ToString(); 
            } 
            set 
            { 
                sb.Append(value); 
            }
        }

        public override int Type => Node.TEXT;
    }
}
