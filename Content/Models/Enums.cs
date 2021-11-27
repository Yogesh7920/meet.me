namespace Content
{
    /// <summary>
    ///     Message Type - File, Chat, or a request for message history until now
    /// </summary>
    public enum MessageType
    {
        File,
        Chat,
        HistoryRequest
    }

    /// <summary>
    ///     Message Event - Update, NewMessage, Star, Download
    /// </summary>
    public enum MessageEvent
    {
        Update,
        NewMessage,
        Star,
        Download
    }
}