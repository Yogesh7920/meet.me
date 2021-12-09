/// <author> Rajeev Goyal </author>
/// <created> 9/11/2021 </created>
/// <summary>
/// This file contains the SummaryData class used for storing the summary of the session and 
/// sending it to the client (from server).
/// </summary>
/// 

namespace Dashboard
{
    /// <summary>
    ///     Class to store the summary, it also
    ///     implemets the IReceivedFromServer interface so that it
    ///     can be sent to the client side (from the server side)
    /// </summary>
    public class SummaryData
    {
        public string summary;

        /// <summary>
        ///     Default Constructor, necessary for serialization
        /// </summary>
        public SummaryData()
        {
        }

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