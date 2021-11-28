/// <author>Suchitra Yechuri</author>
/// <created>23/11/2021</created>
/// <summary>
///     This file contains some mock objects which can
///     be used to simulate tests for the networking module.
/// </summary>

using System.Security.Permissions;
using System.Windows;
using System.Windows.Threading;

namespace Testing.UX.Chat
{
    public class ChatUtils
    {
        //https://docs.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatcher.pushframe?view=windowsdesktop-6.0
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
