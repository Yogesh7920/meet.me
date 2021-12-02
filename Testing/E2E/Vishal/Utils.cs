using System.Windows.Threading;

namespace Testing.E2E.Vishal
{
    public class Utils
    {
        public static class DispatcherUtil
        {
            public static void DoEvents()
            {
                var frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                    new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }

            private static object ExitFrame(object frame)
            {
                ((DispatcherFrame) frame).Continue = false;
                return null;
            }
        }
    }
}