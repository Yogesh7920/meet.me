using Content;

namespace Dashboard.Server.Telemetry
{
    public interface ITelemetry
    {
        /// <summary>
        ///    simiplifies all_messages into easily plotable data.
        /// </summary>
        /// <params name="allMessages"> Array of ChatContext object, which contains info about threads </params>
        void SaveAnalytics(ChatContext[] allMessages);

        /// <summary>
        ///    Returns the analysed data to be passed to the UI
        /// </summary>
        /// <params name="allMessages"> The chat data of all the threads </params>
        /// <returns> SessionAnalytics object </returns>
        SessionAnalytics GetTelemetryAnalytics(ChatContext[] allMessages);
    }
}
