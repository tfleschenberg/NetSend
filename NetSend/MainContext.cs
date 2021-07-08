using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetSend
{
    public class MainContext : ApplicationContext
    {
        private static List<MessageObject> MessageQueue;
        private static List<string> RecipientList;
        private static Timer Timer;
        private static MessageForm MessageForm;
        private static NotifyIcon NotifyIcon;
        private static ContextMenu ContextMenu;
        private static readonly MenuItem MenuNewMessageItem;
        private static readonly MenuItem MenuExitItem;

        static MainContext()
        {
            MessageQueue = new List<MessageObject>();
            RecipientList = new List<string>();

            Timer = new Timer();
            Timer.Interval = 5000;
            Timer.Tick += new EventHandler(OnTimerTick);

            AsyncUDPClient.onReceived += OnReceived;
            AsyncUDPClient.Bind(18);

            MessageForm = new MessageForm();

            NotifyIcon = new NotifyIcon();
            ContextMenu = new ContextMenu();
            
            MenuNewMessageItem = new MenuItem("Neue Nachricht", MenuNewMessageItem_Click);
            MenuExitItem = new MenuItem("Beenden", MenuExitItem_Click);

            ContextMenu.MenuItems.Add(MenuNewMessageItem);
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(MenuExitItem);
            
            NotifyIcon.MouseClick += NotifyIcon_MouseClick;

            NotifyIcon.ContextMenu = ContextMenu;
            NotifyIcon.Icon = global::NetSend.Properties.Resources.Icon;
            NotifyIcon.Text = Program.AppName;
            
            NotifyIcon.Visible = true;
        }

        private static void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MenuNewMessageItem_Click(sender, e);
            }
        }

        private static void MenuNewMessageItem_Click(object sender, EventArgs e)
        {
            MessageForm.Show();
            MessageForm.Focus();
        }

        public static void MenuExitItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        public static void Exit()
        {
            AsyncUDPClient.Send(MessageObject.LogOffString);
            Timer.Stop();
            AsyncUDPClient.Close();
            MessageQueue.Clear();
            MessageForm.Close();
            MessageForm.Dispose();
            NotifyIcon.Visible = false;
            Application.Exit();
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            if (MessageQueue.Count.Equals(0))
            {
                Timer.Stop();
            }
            else
            {
                for (int i = MessageQueue.Count - 1; i >= 0; i--)
                {
                    if (MessageQueue[i].RetryCount < 10)
                    {
                        AsyncUDPClient.Send(MessageQueue[i].MessageString);
                        MessageQueue[i].IncRetryCount();
                    }
                    else
                    {
                        RemoveSenderFromRecipientList(MessageQueue[i].Recipient);
                        MessageQueue.RemoveAt(i);
                    }
                }
            }
        }

        private static void OnReceived(object Sender, string ReceivedMessage)
        {
            MessageObject messageObject = MessageObject.TryParse(ReceivedMessage);

            if (messageObject == null) return;

            // Ignore own Messages
            if (messageObject.Sender.Equals(MessageObject.DefaultSender)) return;

            switch (messageObject.MessageType)
            {
                case MessageType.Message:
                case MessageType.MessageWithConfirmation:
                    AddSenderToRecipientList(messageObject.Sender);
                    if (messageObject.Recipient.Equals(String.Empty) || messageObject.Recipient.Equals("*"))
                    {
                        AsyncUDPClient.Send(messageObject.MessageReceivedString);
                        MessageBox.Show("Nachricht von " + messageObject.Sender + " an " + "*" + " am " + messageObject.Time.ToLocalTime().ToString() + "\n\n" + messageObject.Message, "NetSend", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                    else if (messageObject.Recipient.Equals(Environment.UserName) || messageObject.Recipient.Equals(MessageObject.DefaultSender))
                    {
                        AsyncUDPClient.Send(messageObject.MessageReceivedString);
                        MessageBox.Show("Nachricht von " + messageObject.Sender + " an " + messageObject.Recipient + " am " + messageObject.Time.ToLocalTime().ToString() + "\n\n" + messageObject.Message, "NetSend", MessageBoxButtons.OK, MessageBoxIcon.None);
                        if (messageObject.MessageType == MessageType.MessageWithConfirmation)
                        {
                            AsyncUDPClient.Send(messageObject.MessageReadString);
                        }
                    }
                    break;

                case MessageType.MessageReceived: //Remove Message from Queue
                    AddSenderToRecipientList(messageObject.Sender);
                    if (messageObject.Recipient.Equals(MessageObject.DefaultSender))
                    {
                        DequeueMessage(messageObject);
                    }
                    break;

                case MessageType.MessageRead: //Message was read
                    AddSenderToRecipientList(messageObject.Sender);
                    if (messageObject.Recipient.Equals(MessageObject.DefaultSender))
                    {
                        MessageBox.Show("Nachricht vom " + messageObject.Time.ToLocalTime().ToString() + " an " + messageObject.Sender + " wurde gelesen!", "NetSend", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                    break;

                case MessageType.BrowseRequest: //Send Name and Machine
                    AddSenderToRecipientList(messageObject.Sender);
                    AsyncUDPClient.Send(messageObject.BrowseReplyString);
                    break;

                case MessageType.BrowseReply: //Add Sender to Recplist
                    AddSenderToRecipientList(messageObject.Sender);
                    break;

                case MessageType.LogOff: //Remove Sender from Recplist
                    RemoveSenderFromRecipientList(messageObject.Sender);
                    break;

                default:
                    break;
            }
        }

        public static void SendMessage(MessageType MessageType, string Recipient, string Message)
        {
            MessageObject messageObject = new MessageObject(MessageType, Recipient, Message);
            AsyncUDPClient.Send(messageObject.MessageString);
            MessageQueue.Add(messageObject);
            Timer.Start();
        }

        private static void DequeueMessage(MessageObject message)
        {
            for (int i = MessageQueue.Count - 1; i >= 0; i--)
            {
                if (MessageQueue[i].Time.Equals(message.Time))
                {
                    MessageQueue.RemoveAt(i);
                }
            }
        }

        private static void RemoveSenderFromRecipientList(string Sender)
        {
            if (RecipientList.Contains(Sender))
            {
                RecipientList.Remove(Sender);
            }
        }
         
        private static void AddSenderToRecipientList(string Sender)
        {
            RemoveSenderFromRecipientList(Sender);

            RecipientList.Add(Sender);
            RecipientList.Sort();

            MessageForm.UpdateCombobox(RecipientList.ToArray());
        }
    }
}
