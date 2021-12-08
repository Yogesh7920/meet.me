/// <author>Yogesh</author>
/// <created>26/10/2021</created>

using System.Diagnostics;
using System.IO;

namespace Dashboard
{
    public class TraceManager
    {
        private Stream traceFile;

        public TraceManager()
        {
            TraceListener();
        }

        /// <summary>
        ///     The TraceListener function initiates the trace listener for the whole solution.
        ///     It creates a trace.txt where all the traces will be logged.
        /// </summary>
        public void TraceListener()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"trace.txt");
            traceFile = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            Trace.Listeners.Add(new TextWriterTraceListener(traceFile));
            Trace.AutoFlush = true;
            Trace.IndentSize = 4;
        }

        ~TraceManager()
        {
            traceFile.Close();
        }
    }
}