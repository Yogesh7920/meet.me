using System.Security.Permissions;
using System.Windows.Threading;

namespace Testing.UX.Chat
{
    public class ChatUtils
    {
        public static class DispatcherUtil
        {
#pragma warning disable SYSLIB0003 // Type or member is obsolete
            [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
#pragma warning restore SYSLIB0003 // Type or member is obsolete
            public static void DoEvents()
            {
                var frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                                                         new DispatcherOperationCallback(ExitFrame), frame);
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
