namespace Content
{
    /// <summary>
    /// Message Type - File or Chat
    /// </summary>
    public enum MessageType
    {
        File,
        Chat
    }

    /// <summary>
    /// Message Event - Update, NewMessage, Star, Download
    /// </summary>
    public enum MessageEvent
    {
        Update,
        NewMessage,
        Star,
        Download
    }
}