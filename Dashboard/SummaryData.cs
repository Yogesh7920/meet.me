namespace Dashboard
{
    /// <summary>
    ///     Class to store the summary, it also
    ///     implemets the IReceivedFromServer interface so that it
    ///     can be sent to the client side (from the server side)
    /// </summary>
    public class SummaryData : IRecievedFromServer
    {
        public string summary;

        /// <summary>
        ///     Constructor to initialize the field summary with
        ///     a given string.
        /// </summary>
        /// <param name="chatSummary"></param>
        public SummaryData(string chatSummary)
        {
            summary = chatSummary;
        }
    }
}