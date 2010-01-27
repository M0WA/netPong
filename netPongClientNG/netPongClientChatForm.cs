using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace netPongClientNG
{
    public partial class netPongClientChatForm : Form
    {
        private string m_strUsername = "";
        private string strTmpUsername = "";
        private string strTmpRecvName = "";
        private ArrayList tmpArray;

        public ArrayList playersArray
        {
            get 
            { 
                return tmpArray; 
            }
            set 
            { 
                tmpArray = value; 
                Invoke(new MethodInvoker(refreshPlayerList));
            }
        }

        public ArrayList AddChatMessage
        {
            get { return tmpArray; }
            set
            {
                tmpArray = value;
                Invoke(new MethodInvoker(addChatArray));
            }
        }

        public string strMsgRecv
        {
            get { return strTmpRecvName; }
            set
            {
                strTmpRecvName = value;
                Invoke(new MethodInvoker(setReciever));
            }
        }

        public string strUserName
        {
            get { return m_strUsername; }
            set
            {
                strTmpUsername = value;
                Invoke(new MethodInvoker(setUsername));
            }
        }


        public netPongClientChatForm()
        {
            InitializeComponent();
            CreateHandle();
            Hide();
        }

        private void refreshPlayerList()
        {
            userComboBox.Items.Clear();

            for (int i = 0; i < tmpArray.Count; i++)
            {
                string strPlayer = (string)tmpArray[i];
                if (!strPlayer.Equals(m_strUsername))
                    userComboBox.Items.Add(strPlayer);  
                
            }            
        }

        private void setReciever()
        {
            userComboBox.SelectedValue = strTmpRecvName;

            for (int i = 0; i < userComboBox.Items.Count; i++)
            {
                if ( strTmpRecvName.Equals((string)userComboBox.Items[i]) )
                {
                    userComboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void setUsername()
        {
            m_strUsername = strTmpUsername;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {

            string username = (string)userComboBox.SelectedItem;
            string message = inputRichTextBox.Text;

            if ( (username == null) || (username.Equals("")) || message.Equals("") )
                return;

            netPongClientTcpSocket.SendTextMessageCommand(username, message);
            AddLocalChatArray(message);
            inputRichTextBox.Text = "";
        }

        private void addChatArray()
        {
            DateTime today = DateTime.Now;
            string strChatMessage = "[" + today.ToString("HH:mm:ss") + "][" + ((string)tmpArray[0]) + "]: " + ((string)tmpArray[1]) + "\r\n";
            chatRichTextBox.Text += strChatMessage;
            this.Show();
        }

        private void AddLocalChatArray(string message)
        {
            DateTime today = DateTime.Now;
            string strChatMessage = "[" + today.ToString("HH:mm:ss") + "][" + m_strUsername + "]: " + message + "\r\n";
            chatRichTextBox.Text += strChatMessage;
            this.Show();
        }

        private void netPongClientChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hide the form...
            this.Hide();
            // Cancel the close...
            e.Cancel = true;
        }
    }
}