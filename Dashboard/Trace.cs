/// <author>Yogesh</author>
/// <created>26/10/2021</created>

using System;
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
            
            var configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folderPath = Path.Combine(configPath, "meet.me");
            Directory.CreateDirectory(folderPath);
            var path = Path.Combine(folderPath, @"trace.txt");
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