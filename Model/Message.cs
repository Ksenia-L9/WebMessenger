using System.Data;

namespace Model
{
    /// <summary>
    /// Message instance class.
    /// </summary>
    public class Message
    {
        public string Subject { get; }
        public string Mess { get; }
        public int SenderId { get; }
        public int ReceiverId { get; }

        public Message(string subject, string mess, int senderId, int receiverId)
        {
            Subject = subject;
            Mess = mess;
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
}