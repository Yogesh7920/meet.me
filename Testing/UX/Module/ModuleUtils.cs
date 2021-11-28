/// <author>Raswanth Murugan</author>
/// <created>23/11/2021</created>
/// <summary>
///  Utility functions for 

using System.Windows.Threading;

namespace Testing.UX.Module
{
    public class ModuleUtils
    {
        // Utility function to get the dispatcher for unit testing.
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