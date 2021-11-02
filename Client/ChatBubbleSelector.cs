using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public class ChatBubbleSelector : DataTemplateSelector
    {
        public DataTemplate toBubble { get; set; }
        public DataTemplate fromBubble { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Message message = item as Message;
            if (message.tofrom == true)
            {
                return toBubble;
            }
            else
            {
                return fromBubble;
            }
        }
    }
}
