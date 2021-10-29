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
        
        /// <summary>
        /// The TraceListener function initiates the trace listener for the whole solution.
        /// It creates a trace.txt where all the traces will be logged.
        /// </summary>
        public void TraceListener()
        {
            Stream traceFile = File.Create("trace.txt");
            Trace.Listeners.Add(new TextWriterTraceListener(traceFile));
            Trace.AutoFlush = true;
            Trace.IndentSize = 4;
        }
    }
}