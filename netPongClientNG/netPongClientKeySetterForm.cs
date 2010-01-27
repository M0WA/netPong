using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.DirectX.DirectInput;

namespace netPongClientNG
{
    public partial class netPongClientKeySetterForm : Form
    {
        private netPongClientActionKeys m_keyToSet;
        private Microsoft.DirectX.DirectInput.Device m_Keyb = null;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        public netPongClientKeySetterForm(netPongClientActionKeys keyToSet)
        {
            InitializeComponent();
            m_keyToSet = keyToSet;
            InitKeyboard();
            Show();
        }

        private void InitKeyboard()
        {
            m_Keyb = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
            m_Keyb.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
            m_Keyb.Acquire();
        }

        private void netPongClientKeySetterForm_KeyUp(object sender, KeyEventArgs e)
        {
            /*
            Key[] keys = m_Keyb.GetPressedKeys();

            
             * */
        }

        private void IniWriteValue(string strFilePath, string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, strFilePath);
        }

        private void netPongClientKeySetterForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void netPongClientKeySetterForm_KeyDown(object sender, KeyEventArgs e)
        {
            Key[] keys = m_Keyb.GetPressedKeys();
            if (keys.Length != 1)
                Close();

            int nKeyValue = (int)keys[0];

            string strKeyValue = nKeyValue.ToString();
            switch (m_keyToSet)
            {
                case netPongClientActionKeys.NP_KEY_PADDLE_LEFT:
                    IniWriteValue("./keys.ini", "paddlemovement", "NP_KEY_PADDLE_LEFT", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_PADDLE_RIGHT:
                    IniWriteValue("./keys.ini", "paddlemovement", "NP_KEY_PADDLE_RIGHT", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_MOVE_UP:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_UP", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_MOVE_DOWN:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_DOWN", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_MOVE_LEFT:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_LEFT", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_MOVE_RIGHT:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_RIGHT", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_MOVE_FORWARDS:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_FORWARDS", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_MOVE_BACKWARDS:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_MOVE_BACKWARDS", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_ROTATE_X_PLUS:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_X_PLUS", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_ROTATE_X_MINUS:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_X_MINUS", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_ROTATE_Y_PLUS:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Y_PLUS", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_ROTATE_Y_MINUS:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Y_MINUS", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_ROTATE_Z_PLUS:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Z_PLUS", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_KEY_WORLD_ROTATE_Z_MINUS:
                    IniWriteValue("./keys.ini", "worldmovement", "NP_KEY_WORLD_ROTATE_Z_MINUS", strKeyValue);
                    break;
                case netPongClientActionKeys.NP_RESET_CAMERA_WORLD_POSITION:
                    IniWriteValue("./keys.ini", "other", "NP_RESET_CAMERA_WORLD_POSITION", strKeyValue);
                    break;
                default:
                    break;
            }

            netPongClientGlobals.hKeySettings.LoadKeySettings();
            Close();
        }
    }
}