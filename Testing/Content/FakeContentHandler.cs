/// <author>Vishesh Munjal</author>
/// <created>24/11/2021</created>
/// <summary>
/// This file mimics the function of a ContentHandler in the Content in order to test its functions
/// </summary>
using Content;
using System.Collections.Generic;

public class FakeContentHandler: IContentClient
{
    private MessageData _messageData;
    private List<ChatContext> _chatContexts;

    public FakeContentHandler()
    {
        _messageData = new MessageData();
        _chatContexts = new List<ChatContext>();
    }

    /// <summary>
    ///     Getter for the user id associated with the instance
    /// </summary>
    /// <returns>User id associated with the instance</returns>
    public int GetUserId()
    {
        return -1;
    }

    /// <summary>
    ///     Sends chat or file message to specified clients
    /// </summary>
    /// <param name="toSend">Message to send. In case of file, toSend.message should contain file path</param>
    public void CSend(SendMessageData toSend)
    {
        return;
    }

    /// <summary>
    ///     Download a file message to specified file path on the client machine
    /// </summary>
    /// <param name="messageId">Message Id corresponding to the file to be downloaded</param>
    /// <param name="savePath">Path to which the downloaded file will be saved</param>
    public void CDownload(int messageId, string savePath)
    {
        return;
    }

    /// <summary>
    ///     Star a message which prioritises it to be included in dashboard summary
    /// </summary>
    /// <param name="messageId">Message Id of message to be starred</param>
    public void CMarkStar(int messageId)
    {
        return;
    }

    /// <summary>
    ///     Update a previously sent chat message
    /// </summary>
    /// <param name="messageId">Messsage Id of the chat message to be updated</param>
    /// <param name="newMessage">New updated chat message</param>
    public void CUpdateChat(int messageId, string newMessage)
    {
        return;
    }

    /// <summary>
    ///     Subscribe to content module for listening to received messages
    /// </summary>
    /// <param name="subscriber">An implementation of the IContentListener interface</param>
    public void CSubscribe(IContentListener subscriber)
    {
        return;
    }
/// <summary>
///     Get the thread corresponding to a thread id
/// </summary>
/// <param name="threadId">Id of the requested thread</param>
/// <returns>Thread object corresponding to specified thread Id</returns>
    public ChatContext CGetThread(int threadId)
    {
        return null;
    }

    public void Reset()
    {
        _messageData = new MessageData();
        _chatContexts = new List<ChatContext>();
    }

    public void OnReceive(MessageData msg)
    {
        Reset();
        _messageData = msg;
    }

    public MessageData GetOnReceive()
    {
        return _messageData;
    }

    public void Notify(List<ChatContext> list)
    {
        _chatContexts = list;
    }

    public List<ChatContext> GetNotify()
    {
        return _chatContexts;
    }
}