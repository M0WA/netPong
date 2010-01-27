using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace netPongClientNG
{
    public partial class netPongClientMessageForm : Form
    {
        private object m_objLock = new object();
        private string m_strLog = "";
        public string strLogString
        {
            get 
            {
                string strReturn = "";
                lock (m_objLock)
                {
                    strReturn = m_strLog;
                }
                return strReturn; 
            }
            set 
            { 
                lock (m_objLock)
                {
                    m_strLog = value; Invoke(new MethodInvoker(addLogTextMethod));
                } 
            }
        }
        
        public netPongClientMessageForm()
        {
            InitializeComponent();
        }


        private void addLogTextMethod()
        {
            richTextBox.Text += m_strLog;
            m_strLog = "";
        }

        private void netPongClientMessageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hide the form...
            this.Hide();
            // Cancel the close...
            e.Cancel = true;
        }
    }
}