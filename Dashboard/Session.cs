using System.IO;
using System.Diagnostics;

namespace Dashboard
{
    public class Session
    {
        public Session()
        {
            TraceListener();
        }

        private static void TraceListener()
        {
            Stream traceFile = File.Create("trace.txt");
            Trace.Listeners.Add(new TextWriterTraceListener(traceFile));
            Trace.AutoFlush = true;
            Trace.IndentSize = 4;
        }
    }
}