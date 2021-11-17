namespace Dashboard.Server.SessionManagement
{
    public interface IUXServerSessionManager
    {
        /// <summary>
        /// Returns the credentials required to 
        /// Join or start the meeting
        /// </summary>
        /// <returns> A MeetingCredentials Object </returns>
        MeetingCredentials GetPortsAndIPAddress();
    }
}
