using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace netPongClientNG
{
    public partial class netPongClientLoginForm : Form
    {
        private static bool bLoggedInStatus = false;
        private static ArrayList tmpNamesArray = null;
        private static string m_strUserName = "";

        public ArrayList playerOnline
        {
            get { return tmpNamesArray; }
            set { tmpNamesArray = new ArrayList(value); Invoke(new MethodInvoker(refreshPlayersList)); }
        }


        public netPongClientLoginForm()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, EventArgs e)
        {
            if (bLoggedInStatus)
            {
                netPongServerIP.Enabled = true;
                username.Enabled = true;
                netPongServerPort.Enabled = true;
                netPongClientTcpSocket.SendCloseConnectionCommand();
            }
            else
            {
                netPongClientGlobals.hChatForm.strUserName = username.Text;
                netPongServerIP.Enabled = false;
                username.Enabled = false;
                netPongServerPort.Enabled = false;
                Init(username.Text, netPongServerIP.Text, 2410);
            }
        }

        public static void Init(string strUsername, string strServerIP, int nPort)
        {
            m_strUserName = strUsername;
            //netPongClientProgram.m_chatWindow = new netPongClientChatForm(m_strUserName);
            netPongClientGlobals.hTCPControlSock = new netPongClientTcpSocket(strUsername, strServerIP, nPort);
        }


        private void refreshPlayersList()
        {
            playerListView.Clear();
            for (int i = 0; i < tmpNamesArray.Count; i++)
            {
                string strName = (string)tmpNamesArray[i];
                if (!strName.Equals(m_strUserName))
                {
                    ListViewItem item = playerListView.Items.Add(strName, 0);
                }
            }
        }

        private void setLoggedInMethod()
        {
            playerListView.Clear();
            playerListView.Enabled = bLoggedInStatus;


            if (bLoggedInStatus)
                login.Text = "logout";
            else
            {
                netPongServerIP.Enabled = true;
                username.Enabled = true;
                netPongServerPort.Enabled = true;
                login.Text = "login";
            }
        }

        public bool bLoggedIn
        {
            get { return bLoggedInStatus; }
            set { bLoggedInStatus = value; Invoke(new MethodInvoker(setLoggedInMethod)); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            netPongClientDirectXSettingsForm form = new netPongClientDirectXSettingsForm();
            form.Show();
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = playerListView.SelectedItems;
            if (items.Count != 1)
                return;

            netPongClientTcpSocket.SendInvitePlayerCommand(items[0].Text);
        }

        private void sendMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            netPongClientGlobals.hChatForm.Show();
        }

        private void keySettingsButton_Click(object sender, EventArgs e)
        {
            netPongClientGlobals.hKeySettings.Show();
        }

        private void netPongClientLoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hide the form...
            this.Hide();
            // Cancel the close...
            e.Cancel = true;
        }
    }
}