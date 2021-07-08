using System;
using System.Threading;
using System.Windows.Forms;

namespace NetSend
{
    public class MessageForm : Form
    {
        private Button button_Send;
        private Button button_Cancel;
        private TextBox textBox_Message;
        private Label label_Message;
        private ComboBox comboBox_Recipients;
        private CheckBox checkBox_ReadConfirmation;
        private Label label_Recipient;
        
        private delegate void SafeCallDelegate(string[] Recipients);

        public MessageForm()
        {
            InitializeComponent();

            Text = Program.AppName + " - Neue Nachricht";

            AsyncUDPClient.Send(MessageObject.BrowseRequestString);
        }

        private void InitializeComponent()
        {
            this.button_Send = new System.Windows.Forms.Button();
            this.textBox_Message = new System.Windows.Forms.TextBox();
            this.label_Message = new System.Windows.Forms.Label();
            this.label_Recipient = new System.Windows.Forms.Label();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.comboBox_Recipients = new System.Windows.Forms.ComboBox();
            this.checkBox_ReadConfirmation = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button_Send
            // 
            this.button_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Send.Location = new System.Drawing.Point(260, 12);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(90, 28);
            this.button_Send.TabIndex = 4;
            this.button_Send.Text = "Senden";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // textBox_Message
            // 
            this.textBox_Message.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Message.Location = new System.Drawing.Point(12, 94);
            this.textBox_Message.Multiline = true;
            this.textBox_Message.Name = "textBox_Message";
            this.textBox_Message.Size = new System.Drawing.Size(338, 129);
            this.textBox_Message.TabIndex = 2;
            // 
            // label_Message
            // 
            this.label_Message.AutoSize = true;
            this.label_Message.Location = new System.Drawing.Point(12, 71);
            this.label_Message.Margin = new System.Windows.Forms.Padding(3);
            this.label_Message.Name = "label_Message";
            this.label_Message.Size = new System.Drawing.Size(72, 17);
            this.label_Message.TabIndex = 7;
            this.label_Message.Text = "Nachricht:";
            // 
            // label_Recipient
            // 
            this.label_Recipient.AutoSize = true;
            this.label_Recipient.Location = new System.Drawing.Point(12, 12);
            this.label_Recipient.Margin = new System.Windows.Forms.Padding(3);
            this.label_Recipient.Name = "label_Recipient";
            this.label_Recipient.Size = new System.Drawing.Size(81, 17);
            this.label_Recipient.TabIndex = 6;
            this.label_Recipient.Text = "Empfänger:";
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.Location = new System.Drawing.Point(260, 46);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(90, 28);
            this.button_Cancel.TabIndex = 5;
            this.button_Cancel.Text = "Abbrechen";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // comboBox_Recipients
            // 
            this.comboBox_Recipients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_Recipients.FormattingEnabled = true;
            this.comboBox_Recipients.Location = new System.Drawing.Point(12, 35);
            this.comboBox_Recipients.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.comboBox_Recipients.Name = "comboBox_Recipients";
            this.comboBox_Recipients.Size = new System.Drawing.Size(225, 24);
            this.comboBox_Recipients.TabIndex = 1;
            // 
            // checkBox_ReadConfirmation
            // 
            this.checkBox_ReadConfirmation.AutoSize = true;
            this.checkBox_ReadConfirmation.Location = new System.Drawing.Point(102, 70);
            this.checkBox_ReadConfirmation.Name = "checkBox_ReadConfirmation";
            this.checkBox_ReadConfirmation.Size = new System.Drawing.Size(135, 21);
            this.checkBox_ReadConfirmation.TabIndex = 3;
            this.checkBox_ReadConfirmation.Text = "Lesebestätigung";
            this.checkBox_ReadConfirmation.UseVisualStyleBackColor = true;
            // 
            // MessageForm
            // 
            this.ClientSize = new System.Drawing.Size(362, 235);
            this.Controls.Add(this.checkBox_ReadConfirmation);
            this.Controls.Add(this.comboBox_Recipients);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.label_Recipient);
            this.Controls.Add(this.label_Message);
            this.Controls.Add(this.textBox_Message);
            this.Controls.Add(this.button_Send);
            this.Icon = global::NetSend.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(380, 280);
            this.Name = "MessageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NetSend - Neue Nachricht";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MessageForm_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void MessageForm_Closing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            if (checkBox_ReadConfirmation.Checked)
            {
                MainContext.SendMessage(MessageType.MessageWithConfirmation, comboBox_Recipients.Text, textBox_Message.Text);
            }
            else
            {
                MainContext.SendMessage(MessageType.Message, comboBox_Recipients.Text, textBox_Message.Text);
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void UpdateCombobox(string[] Recipients)
        {
            if (comboBox_Recipients.InvokeRequired)
            {
                var d = new SafeCallDelegate(UpdateCombobox);
                comboBox_Recipients.Invoke(d, new object[] { Recipients });
            }
            else
            {
                comboBox_Recipients.BeginUpdate();
                comboBox_Recipients.Items.Clear();
                //comboBox_Recipients.Items.Add("*");
                comboBox_Recipients.Items.AddRange(Recipients);
                comboBox_Recipients.EndUpdate();
            }
        }
    }
}
