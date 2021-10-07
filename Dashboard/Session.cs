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

        /// <summary>
        /// The TraceListener function initiates the trace listener for the whole solution.
        /// It creates a trace.txt where all the traces will be logged.
        /// </summary>
        private static void TraceListener()
        {
            Stream traceFile = File.Create("trace.txt");
            Trace.Listeners.Add(new TextWriterTraceListener(traceFile));
            Trace.AutoFlush = true;
            Trace.IndentSize = 4;
        }
    }
}