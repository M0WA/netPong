using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Microsoft.Win32;

namespace netPongDebugClient
{
    public partial class netPongDirectXSettingsForm : Form
    {
        private int[] m_nHeight = null;
        private int[] m_nWidth = null;
        private int[] m_nDisplayFormat = null;

        public netPongDirectXSettingsForm()
        {
            InitializeComponent();
            InitGUI();
        }

        private void InitGUI()
        {
            AdapterListCollection adapList = Microsoft.DirectX.Direct3D.Manager.Adapters;
            string[] strItems = new string[adapList.Count];
            for (int i = 0; i < adapList.Count; i++)
                strItems[i] = adapList[i].Information.Description;

            adapterComboBox.Items.AddRange(strItems);
            try
            {

                RegistryKey adapKey = Registry.CurrentUser.OpenSubKey("Software\\netPongClient\\Adapter");
                if (adapKey != null)
                {
                    adapterComboBox.SelectedIndex = (int)adapKey.GetValue("ID", 0);
                    windowedCheckBox.Checked = Convert.ToBoolean(adapKey.GetValue("Fullscreen", 0));
                }
                else
                {
                    adapterComboBox.SelectedIndex = 0;
                }

            }
            catch (Exception)
            {

            }
        }

        private void adapterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedAdapter = adapterComboBox.SelectedIndex;
            AdapterListCollection adapList = Microsoft.DirectX.Direct3D.Manager.Adapters;

            DisplayModeCollection disMods = adapList[nSelectedAdapter].SupportedDisplayModes;
            string[] strResolutions = new string[disMods.Count];

            m_nHeight = new int[disMods.Count];
            m_nWidth = new int[disMods.Count];
            m_nDisplayFormat = new int[disMods.Count];

            IEnumerator disModEnum = (IEnumerator)disMods.GetEnumerator();
            int i = 0;
            int nSelected = 0;
            int nSavedWidth = 0;
            int nSavedHeight = 0;
            int nSavedFormat = 0;
            try
            {
                RegistryKey adapKey = Registry.CurrentUser.OpenSubKey("Software\\netPongClient\\Adapter");
                if (adapKey != null)
                {
                    nSavedWidth = (int)adapKey.GetValue("Width", 0);
                    nSavedHeight = (int)adapKey.GetValue("Height", 0);
                    nSavedFormat = (int)adapKey.GetValue("Format", 0);
                }
            }
            catch (Exception)
            {
            }

            while ( disModEnum.MoveNext() )
            {
                DisplayMode disMod = ((DisplayMode)disModEnum.Current);
                if ((nSavedWidth == disMod.Width) && (nSavedHeight == disMod.Height) && (nSavedFormat == (int)disMod.Format))
                    nSelected = i;

                m_nHeight[i] = disMod.Height;
                m_nWidth[i] = disMod.Width;
                m_nDisplayFormat[i] = (int)disMod.Format;
                strResolutions[i] = disMod.Width + " x " + disMod.Height + " " + disMod.Format.ToString() + " ( " + disMod.RefreshRate + " Hz )";
                i++;
            }

            resolutionComboBox.Items.Clear();
            resolutionComboBox.Items.AddRange(strResolutions);
            resolutionComboBox.SelectedIndex = nSelected;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey adapKey = Registry.CurrentUser.OpenSubKey("Software\\netPongClient\\Adapter" ,true);
                if (adapKey == null)
                    adapKey = Registry.CurrentUser.CreateSubKey("Software\\netPongClient\\Adapter");
                adapKey.SetValue("ID", adapterComboBox.SelectedIndex);
                adapKey.SetValue("Width", m_nWidth[resolutionComboBox.SelectedIndex]);
                adapKey.SetValue("Height", m_nHeight[resolutionComboBox.SelectedIndex]);
                adapKey.SetValue("Format", m_nDisplayFormat[resolutionComboBox.SelectedIndex]);
                adapKey.SetValue("Fullscreen", windowedCheckBox.Checked, RegistryValueKind.DWord);     
            }
            catch (Exception)
            {
            }

            Close();
        }
    }
}