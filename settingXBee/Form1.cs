using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace settingXBee
{
    public partial class Form1 : Form
    {
        bool isConCOM = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboCOMsetting();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                serialPort1.PortName = comboBox1.Text;
                try
                {
                    serialPort1.Open();
                    richTextBox1.AppendText(comboBox1.Text + "に接続しました\n");
                    comboBox1.Enabled = false;
                    button1.Enabled = false;
                }
                catch
                {
                    richTextBox1.AppendText("問題が発生しました．\n接続を確認してください\n");
                    com_close();
                }
            }
            else
            {
                richTextBox1.AppendText("COMポートを選択してください\n");
            }
        }

        private void comboCOMsetting()
        {
            comboBox1.Items.Clear();
            string[] port = SerialPort.GetPortNames();
            for(int i = 0;i< port.Length; i++)
            {
                comboBox1.Items.Add(port[i]);
            }
        }
 
        private void com_close()
        {
            comboBox1.Enabled = true;
            button1.Enabled = true;

            try
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Close();
            }
            catch { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox3.Checked = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox3.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            com_close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(isConCOM == true)
            {

            }
            else
            {
                richTextBox1.AppendText("COMが接続されていません\n");
            }
        }

    }
}
