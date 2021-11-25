namespace Content
{
    internal class IdGenerator
    {
        private static int _messageId;
        private static int _chatContextId;

        /// <summary>
        /// Generates a unique id for messages.
        /// </summary>
        /// <returns>Returns the generated unique id.</returns>
        public static int getMessageId()
        {
            int prevId = _messageId;
            _messageId++;
            return prevId;
        }

        /// <summary>
        /// Resets the uniqie id for messages.
        /// </summary>
        public static void resetMessageId()
        {
            _messageId = 0;
        }

        /// <summary>
        /// Generates unique id for chat contexts.
        /// </summary>
        /// <returns>Returns the generated unique id.</returns>
        public static int getChatContextId()
        {
            int prevId = _chatContextId;
            _chatContextId++;
            return prevId;
        }

        /// <summary>
        /// Resets the unique id for chat contexts
        /// </summary>
        public static void resetChatContextId()
        {
            _chatContextId = 0;
        }
    }
}