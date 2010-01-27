using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace netPongDebugClient
{
    public partial class netPongClientSettingsForm : Form
    {
        private static string strLog = "";
        private static bool bLoggedInStatus = false;
        private static ArrayList tmpNamesArray = null;
        private static string strInvPlayerName = "";
        private static string m_strUserName = "";

        public string strInvitationName
        {
            get { return strInvPlayerName; }
            set { strInvPlayerName = value; if (this.InvokeRequired) this.Invoke(new MethodInvoker(invitationRecieved)); else invitationRecieved(); }
        }

        public ArrayList playerOnline
        {
            get { return tmpNamesArray; }
            set { tmpNamesArray = new ArrayList(value); Invoke(new MethodInvoker(refreshPlayersList)); }
        }

        public string strLogString
        {
            get { return strLog; }
            set { strLog = value; Invoke(new MethodInvoker(addLogTextMethod)); }
        }

        public bool bLoggedIn
        {
            get { return bLoggedInStatus; }
            set { bLoggedInStatus = value; Invoke(new MethodInvoker(setLoggedInMethod)); }
        }
        
        public netPongClientSettingsForm()
        {
            InitializeComponent();
        }

        public static void Init( string strUsername, string strServerIP, int nPort)
        {
            m_strUserName = strUsername;
            netPongClientProgram.m_chatWindow = new netPongClientChatForm(m_strUserName);
            netPongClientProgram.m_cNetClient = new netPongClient(strUsername, strServerIP, nPort);
        }

        private void login_Click(object sender, EventArgs e)
        {
            if (bLoggedInStatus)
            {
                netPongServerIP.Enabled = true;
                username.Enabled = true;
                netPongServerPort.Enabled = true;
                chatWindowbutton.Enabled = false;
                netPongClientProgram.m_chatWindow.Hide();
                netClientTcpSocket.SendCloseConnectionCommand();
            }
            else
            {
                netPongServerIP.Enabled = false;
                username.Enabled = false;
                netPongServerPort.Enabled = false;
                chatWindowbutton.Enabled = true;
                Init(username.Text, netPongServerIP.Text, 2410);
            }
            
        }

        private void addLogTextMethod()
        {
            LogOutput.Text += strLog;
            strLog = "";
        }

        private void setLoggedInMethod()
        {
            playerListView.Clear();
            playerListView.Enabled = bLoggedInStatus;

            if (bLoggedInStatus)
                login.Text = "logout";
            else
                login.Text = "login";
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

        private void invitationRecieved()
        {
            DialogResult res = MessageBox.Show(this,strInvPlayerName + " invites you for a pong challenge.\r\nDo you want to start a match ?", "player invitation recieved", MessageBoxButtons.YesNo);
            if (res.Equals(DialogResult.Yes))
            {
                netClientTcpSocket.SendInvitePlayerCommand(strInvPlayerName);
                netPongClientProgram.m_cNetClient.StartGameSession();
            }
            else if (res.Equals(DialogResult.No))
            {
                netClientTcpSocket.SendRejectInviteCommand(strInvPlayerName);
            }
        }

        private void listplayersbutton_Click(object sender, EventArgs e)
        {
            netClientTcpSocket.SendListPlayersCommand();
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = playerListView.SelectedItems;
            if (items.Count != 1)
                return;
                        
            netClientTcpSocket.SendInvitePlayerCommand(items[0].Text);
        }

        private void sendMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = playerListView.SelectedItems;
            if (items.Count != 1)
                return;

            netPongClientProgram.m_chatWindow.Show();
            netClientTcpSocket.SendListPlayersCommand();
            netPongClientProgram.m_chatWindow.strMsgRecv = items[0].Text;
        }

        private void chatWindowbutton_Click(object sender, EventArgs e)
        {
            netPongClientProgram.m_chatWindow.Show();
            netClientTcpSocket.SendListPlayersCommand();
        }

        private void gamesettingsButton_Click(object sender, EventArgs e)
        {
            netPongDirectXSettingsForm dirXSetForm = new netPongDirectXSettingsForm();
            dirXSetForm.Show();
        }

        private void netPongClientSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            netPongClientProgram.ExitNetPongClient();
        }
    }
}