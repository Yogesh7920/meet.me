using System;
using System.Diagnostics;
using System.IO;

namespace Dashboard
{
    public class Session
    {
        public Session()
        {
            TraceListener();
        }
        private void TraceListener()
        {
            Stream traceFile = File.Create("trace.txt");
            Trace.Listeners.Add(new TextWriterTraceListener(traceFile));
            Trace.AutoFlush = true;
            Trace.IndentSize = 4;
        }
    }
}