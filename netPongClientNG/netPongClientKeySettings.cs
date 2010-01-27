using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.DirectX.DirectInput;

namespace netPongClientNG
{
    public enum netPongClientActionKeys
    {
        /*paddle movement keys*/
        NP_KEY_PADDLE_LEFT,
        NP_KEY_PADDLE_RIGHT,

        /*world movement keys*/
        NP_KEY_WORLD_MOVE_UP,
        NP_KEY_WORLD_MOVE_DOWN,
        NP_KEY_WORLD_MOVE_LEFT,
        NP_KEY_WORLD_MOVE_RIGHT,
        NP_KEY_WORLD_MOVE_FORWARDS,
        NP_KEY_WORLD_MOVE_BACKWARDS,

        /*world rotation keys*/
        NP_KEY_WORLD_ROTATE_X_PLUS,
        NP_KEY_WORLD_ROTATE_X_MINUS,
        NP_KEY_WORLD_ROTATE_Y_PLUS,
        NP_KEY_WORLD_ROTATE_Y_MINUS,
        NP_KEY_WORLD_ROTATE_Z_PLUS,
        NP_KEY_WORLD_ROTATE_Z_MINUS,

        /*other actions*/
        NP_RESET_CAMERA_WORLD_POSITION
    }

    public partial class netPongClientKeySettings : Form
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        public netPongClientKeySettings()
        {
            InitializeComponent();
        }

        private void paddleRightButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_PADDLE_RIGHT);
        }

        private void paddleLeftButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_PADDLE_LEFT);
        }

        private void worldForwardsButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_MOVE_FORWARDS);
        }

        private void worldLeftButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_MOVE_LEFT);
        }

        private void resetCameraButtom_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_RESET_CAMERA_WORLD_POSITION);
        }

        private void worldRightButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_MOVE_RIGHT);
        }

        private void worldBackwardsButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_MOVE_BACKWARDS);
        }

        private void rotateXPlusButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_ROTATE_X_PLUS);
        }

        private void rotateYPlusButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_ROTATE_Y_PLUS);
        }

        private void rotateZPlusButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_ROTATE_Z_PLUS);
        }

        private void rotateXMinusButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_ROTATE_X_MINUS);
        }

        private void rotateYMinusButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_ROTATE_Y_MINUS);
        }

        private void rotateZMinusButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_ROTATE_Z_MINUS);
        }

        private void resetDefaultButton_Click(object sender, EventArgs e)
        {
            IniWriteValue("./keys.ini", "paddlemovement", "NP_KEY_PADDLE_LEFT", "71");
            IniWriteValue("./keys.ini", "paddlemovement", "NP_KEY_PADDLE_RIGHT", "73");

            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_UP", "78");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_DOWN", "74");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_LEFT", "75");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_RIGHT", "77");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_FORWARDS", "72");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_BACKWARDS", "80");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_X_PLUS", "200");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_X_MINUS", "208");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Y_PLUS", "203");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Y_MINUS", "205");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Z_PLUS", "201");
            IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Z_MINUS", "209");

            IniWriteValue("./keys.ini", "other", "NP_RESET_CAMERA_WORLD_POSITION", "76");
        }

        private void worldMoveUpButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_MOVE_UP);
        }

        private void worldMoveDownButton_Click(object sender, EventArgs e)
        {
            new netPongClientKeySetterForm(netPongClientActionKeys.NP_KEY_WORLD_MOVE_DOWN);
        }

        private string IniReadValue(string strFilePath, string Section, string regkey)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, regkey, "", temp, 255, strFilePath);

            int ix = int.Parse(temp.ToString());

            Type theType = typeof(Key);
            MemberInfo[] keyMembers = theType.GetMembers();

            foreach (MemberInfo entry in keyMembers)
            {
                if ((entry.MemberType.ToString() == "Field"))
                    if ((entry.Name != "value__"))
                    {
                        Key theKey = (Key)Enum.Parse(typeof(Key), entry.Name);
                        if ((int)theKey == ix)
                            return entry.Name;
                    }
            }

            return "";
        }

        private string IniReadIntValue(string strFilePath, string Section, string regkey)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, regkey, "", temp, 255, strFilePath);

            int ix = int.Parse(temp.ToString());

            Type theType = typeof(Key);
            MemberInfo[] keyMembers = theType.GetMembers();

            foreach (MemberInfo entry in keyMembers)
            {
                if ((entry.MemberType.ToString() == "Field"))
                    if ((entry.Name != "value__"))
                    {
                        Keys theKey = (Keys)Enum.Parse(typeof(Key), entry.Name);
                        if ((int)theKey == ix)
                            return temp.ToString();
                    }
            }

            return "";
        }

        public void InitKeyList()
        {
            //keyBindingListView.;

            ListViewItem item = new ListViewItem();  
            item.Text = "move paddle left";
            item.SubItems.Add(IniReadValue("./keys.ini", "paddlemovement", "NP_KEY_PADDLE_LEFT"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "move paddle right";
            item.SubItems.Add(IniReadValue("./keys.ini", "paddlemovement", "NP_KEY_PADDLE_RIGHT"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "move world up";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_UP"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "move world down";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_DOWN"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "move world left";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_LEFT"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "move world right";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_RIGHT"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "move world forwards";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_FORWARDS"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "move world backwards";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_BACKWARDS"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "rotate world x +";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_X_PLUS"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "rotate world x -";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_X_MINUS"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "rotate world y -";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Y_PLUS"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "rotate world y -";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Y_MINUS"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "rotate world z +";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Z_PLUS"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "rotate world z -";
            item.SubItems.Add(IniReadValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Z_MINUS"));
            keyBindingListView.Items.Add(item);

            item = new ListViewItem();
            item.Text = "reset camera position";
            item.SubItems.Add(IniReadValue("./keys.ini", "other", "NP_RESET_CAMERA_WORLD_POSITION"));
            keyBindingListView.Items.Add(item);
        }

        private void IniWriteValue(string strFilePath, string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, strFilePath);
        }

        private void netPongClientKeySettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hide the form...
            this.Hide();
            // Cancel the close...
            e.Cancel = true;
        }

        public void LoadKeySettings()
        {
            netPongClientGlobals.hKeyBindings.Clear();

            netPongClientGlobals.hKeyBindings.Add("NP_RESET_CAMERA_WORLD_POSITION", IniReadIntValue("./keys.ini", "other", "NP_RESET_CAMERA_WORLD_POSITION"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_ROTATE_Z_MINUS", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Z_MINUS"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_ROTATE_Z_PLUS", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Z_PLUS"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_ROTATE_Y_MINUS", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Y_MINUS"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_ROTATE_Y_PLUS", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Y_PLUS"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_ROTATE_X_MINUS", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_X_MINUS"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_ROTATE_X_PLUS", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_X_PLUS"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_MOVE_BACKWARDS", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_BACKWARDS"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_MOVE_FORWARDS", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_FORWARDS"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_MOVE_RIGHT", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_RIGHT"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_MOVE_LEFT", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_LEFT"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_MOVE_DOWN", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_DOWN"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_WORLD_MOVE_UP", IniReadIntValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_UP"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_PADDLE_RIGHT", IniReadIntValue("./keys.ini", "paddlemovement", "NP_KEY_PADDLE_RIGHT"));
            netPongClientGlobals.hKeyBindings.Add("NP_KEY_PADDLE_LEFT", IniReadIntValue("./keys.ini", "paddlemovement", "NP_KEY_PADDLE_LEFT"));
        }
    }
}