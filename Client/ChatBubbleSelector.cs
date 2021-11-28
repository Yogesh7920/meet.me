/// <author>Suchitra Yechuri</author>
/// <created>13/10/2021</created>
/// <summary>
///     This file contains the code for ChatBubbleSelector which helps selecting the Data template according
///     to whether the message is a chat or file and whether it is send or received.
/// </summary>

using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public class ChatBubbleSelector : DataTemplateSelector
    {
        /// <summary>
        ///     The sent chat message data template
        /// </summary>
        public DataTemplate ToMsgBubble { get; set; }

        /// <summary>
        ///     The received chat message data template
        /// </summary>
        public DataTemplate FromMsgBubble { get; set; }

        /// <summary>
        ///     The sent file message data template
        /// </summary>
        public DataTemplate ToFileBubble { get; set; }

        /// <summary>
        ///     The received file message data template
        /// </summary>
        public DataTemplate FromFileBubble { get; set; }

        /// <summary>
        ///     Checks whether the message is send or received and whether 
        ///     it's a chat or a file and returns the appropriate template
        /// </summary>
        /// <param name="item"> The message object </param>
        /// <param name="container"> Container of the message object </param>
        /// <returns> Appropriate DataTemplate </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Message message = item as Message;

            if (message.ToFrom)
            {
                return message.Type == true ? ToMsgBubble : ToFileBubble;
            }
            else
            {
                return message.Type == true ? FromMsgBubble : FromFileBubble;
            }
        }
    }
}
