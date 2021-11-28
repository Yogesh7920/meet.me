/// <author>Suchitra Yechuri</author>
/// <created>23/11/2021</created>
/// <summary>
///     Utility functions for unit testing.
/// </summary>

using System.Windows.Threading;

namespace Testing.UX.Chat
{
    public class ChatUtils
    {
        // Utility function to get the dispatcher for unit testing.
        public static class DispatcherUtil
        {
            public static void DoEvents()
            {
                var frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }

            private static object ExitFrame(object frame)
            {
                ((DispatcherFrame)frame).Continue = false;
                return null;
            }
        }
    }
}
