using System;

namespace NetSend
{
    public class MessageObject
    {
        private static char DefaultProtocolVersion = 'C';
        public static string DefaultSender = Environment.UserName + "@" + Environment.MachineName;

        public MessageObject(MessageType MessageType, string Recipient, string Message)
        {
            this.ProtocolVersion = DefaultProtocolVersion;
            this.MessageType = MessageType;
            this.Time = DateTime.Now.ToUniversalTime();
            this.Sender = DefaultSender;
            this.Recipient = Recipient.Replace("\0", String.Empty);
            this.Message = Message.Replace("\0", String.Empty);
            this.RetryCount = 0;
        }

        private MessageObject(char ProtocolVersion, MessageType MessageType, DateTime DateTime, string Sender, string Recipient, string Message)
        {
            this.ProtocolVersion = ProtocolVersion;
            this.MessageType = MessageType;
            this.Time = DateTime;
            this.Sender = Sender.Replace("\0", String.Empty);
            this.Recipient = Recipient.Replace("\0", String.Empty);
            this.Message = Message.Replace("\0", String.Empty);
            this.RetryCount = 0;
        }

        public static MessageObject TryParse(string ReceivedMessage)
        {
            try
            {
                if (ReceivedMessage[0] != DefaultProtocolVersion) return null;

                if (ReceivedMessage.Length < 13) return null;

                string[] fields = ReceivedMessage.Substring(10).Split('\0');

                if (fields.Length != 4) return null;

                return new MessageObject(
                    ReceivedMessage[0],
                    (MessageType)ReceivedMessage[1],
                    new DateTime(BitConverter.ToInt64(System.Text.Encoding.GetEncoding(437).GetBytes(ReceivedMessage.Substring(2, 8)), 0)),
                    fields[0],
                    fields[1],
                    fields[2]
                    );
            }
            catch
            {
                return null;
            }
        }

        public char ProtocolVersion { get; }

        public MessageType MessageType { get; }

        public DateTime Time { get; }

        public string Sender { get; }

        public string Recipient { get; }

        public string Message { get; }

        public string MessageString
        {
            get
            {
                return
                    ProtocolVersion.ToString() +
                    ((char)MessageType).ToString() +
                    System.Text.Encoding.GetEncoding(437).GetString(BitConverter.GetBytes(Time.Ticks)) + 
                    Sender + "\0" +
                    Recipient + "\0" +
                    Message + "\0";
            }
        }

        public string MessageReceivedString
        {
            get
            {
                return
                    ProtocolVersion.ToString() +
                    ((char)MessageType.MessageReceived).ToString() +
                    System.Text.Encoding.GetEncoding(437).GetString(BitConverter.GetBytes(Time.Ticks)) +
                    DefaultSender + "\0" +
                    Sender + "\0" +
                    Message + "\0";
            }
        }

        public string MessageReadString
        {
            get
            {
                return
                    ProtocolVersion.ToString() +
                    ((char)MessageType.MessageRead).ToString() +
                    System.Text.Encoding.GetEncoding(437).GetString(BitConverter.GetBytes(Time.Ticks)) +
                    DefaultSender + "\0" +
                    Sender + "\0" +
                    Message + "\0";
            }
        }

        public static string BrowseRequestString
        {
            get
            {
                return
                    DefaultProtocolVersion.ToString() +
                    ((char)MessageType.BrowseRequest).ToString() +
                    System.Text.Encoding.GetEncoding(437).GetString(BitConverter.GetBytes(DateTime.Now.ToUniversalTime().Ticks)) +
                    DefaultSender + "\0" +
                    "\0" +
                    "\0";
            }
        }

        public string BrowseReplyString
        {
            get
            {
                return
                    ProtocolVersion.ToString() +
                    ((char)MessageType.BrowseReply).ToString() +
                    System.Text.Encoding.GetEncoding(437).GetString(BitConverter.GetBytes(Time.Ticks)) +
                    DefaultSender + "\0" +
                    Sender + "\0" +
                    Message + "\0";
            }
        }

        public static string LogOffString
        {
            get
            {
                return
                    DefaultProtocolVersion.ToString() +
                    ((char)MessageType.LogOff).ToString() +
                    System.Text.Encoding.GetEncoding(437).GetString(BitConverter.GetBytes(DateTime.Now.ToUniversalTime().Ticks)) +
                    DefaultSender + "\0" +
                    "\0" +
                    "\0";
            }
        }

        public int RetryCount { get; private set; }

        public void IncRetryCount()
        {
            RetryCount++;
        }
    }

    public enum MessageType
    {
        Message = 1,
        MessageWithConfirmation = 2,
        MessageReceived = 3,
        MessageRead = 4,
        BrowseRequest = 5,
        BrowseReply = 6,
        LogOff = 7
    }
}
