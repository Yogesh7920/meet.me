using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public class ChatBubbleSelector : DataTemplateSelector
    {
        public DataTemplate ToMsgBubble { get; set; }
        public DataTemplate FromMsgBubble { get; set; }
        public DataTemplate ToFileBubble { get; set; }
        public DataTemplate FromFileBubble { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Message message = item as Message;
            if (message.ToFrom == true)
            {
                if (message.Type == true)
                {
                    return ToMsgBubble;
                }
                else
                {
                    return ToFileBubble;
                }
            }
            else
            {
                if (message.Type == true)
                {
                    return FromMsgBubble;
                }
                else
                {
                    return FromFileBubble;
                }
            }
        }
    }
}
