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
        bool waitCallBack = false;
        int[] BD = new int[] {9600,115200,1200,2400,4800,19200,38400,57600,230400};

        string prid_RE = @"[a-zA-Z0-9]{6}";
        string panid_RE = @"[0-9]{3}";
        string characters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string node_id;

        static string flagment = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboCOMsetting();
            comboBDsetting();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" & comboBox2.Text != "" & isConCOM == false)
            {
                serialPort1.PortName = comboBox1.Text;
                try
                {
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.Open();
                    richTextBox1.AppendText(comboBox1.Text + "に接続しました\n");
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    button1.Text = "接続断";
                    isConCOM = true;
                }
                catch
                {
                    richTextBox1.AppendText("問題が発生しました．\n接続を確認してください\n");
                    com_close();
                }
            }else if(isConCOM == true){

                richTextBox1.AppendText(comboBox1.Text + ":接続を断ちます\n");
                com_close();
                isConCOM = false;
            }
            else
            {
                richTextBox1.AppendText("COMポート,ボーレートを選択してください\n");
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

        private void comboBDsetting()
        {
            comboBox2.Items.Clear();
            for(int i = 0;i < BD.Length; i++)
            {
                comboBox2.Items.Add(BD[i]);
            }
        }
 
        private void com_close()
        {
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            button1.Enabled = true;
            button1.Text = "接続";

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
                label1.Text = "PAN and IM ID:";
                label2.Text = "設置建物名:";
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked == true)
            {
                label1.Text = "";
                label2.Text = "";
                textBox3.Enabled = false;
                textBox2.Enabled = false;
                checkBox1.Checked = false;
                checkBox3.Checked = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox3.Checked == true)
            {
                label1.Text = "PAN and IM ID:";
                label2.Text = "センサ設置位置:";
                textBox2.Enabled = true;
                textBox3.Enabled = true;
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
                if(Regex.IsMatch(textBox1.Text,prid_RE) & textBox1.Text.Length < 7)
                {
                    string CD = Directory.GetCurrentDirectory();
                    string root = @CD + "\\data\\" + textBox1.Text;
                    string[] files = Directory.GetDirectories(@CD + "\\data", "*", SearchOption.AllDirectories);
                    bool chkRoot = false;

                    var random = new Random();
                    node_id = characters[random.Next(characters.Length)] + random.Next(0, 999).ToString("D3");

                    for (int i = 0;i < files.Length; i++)
                    {
                        if(root == files[i])
                        {
                            chkRoot = true;
                            rtb("フォルダの存在を確認\n");
                        }
                    }
                    if (chkRoot == false)
                    {
                        Directory.CreateDirectory(root);
                        FileStream bld = File.Create(root + "\\bld.csv");
                        FileStream sen = File.Create(root + "\\sensor.csv");
                        rtb("フォルダが存在しません\n新しく作製します\n");
                    }

                    if (Regex.IsMatch(textBox3.Text,panid_RE) &(textBox3.Text.Length < 4))
                    {
                        try
                        {
                            if (checkBox1.Checked == true)
                            {
                                using (StreamWriter sw = new StreamWriter(@"data\" + textBox1.Text + "\\bld.csv", true, Encoding.GetEncoding("shift_jis")))
                                {
                                    sw.WriteLine(textBox3.Text + "," + textBox2.Text + ",2,0,0");
                                }
                            }
                            else if (checkBox3.Checked == true)
                            {
                                using (StreamWriter sw = new StreamWriter(@"data\" + textBox1.Text + "\\sensor.csv", true, Encoding.GetEncoding("shift_jis")))
                                {
                                    sw.WriteLine(node_id + "," + textBox2.Text + "," + textBox3.Text);
                                }
                            }
                            else { }
                            rtb("ファイル書き込み完了\n");
                        }
                        catch { }
                        if((checkBox1.Checked ^ checkBox2.Checked ^ checkBox3.Checked) & textBox3.Text.Length < 14)
                        {
                            rtb("XBee3と仮想シリアル通信を開始\n");
                            //testXBee();
                            //rebootXBee();
                            //settingXBee();
                        }
                        else
                        {
                            rtb("XBeeのロールが選択されていないか\n建物かセンサの名前が長すぎます\n");
                        }
                    }
                    else
                    {
                        rtb("PAN IDは000から999までが使用可能です\n確認してください\n");
                    }
                }
                else
                {
                    rtb("premises IDは0-9,a-z,A-Zが使用可能です\n6文字指定です\n");
                }
            }
            else
            {
                richTextBox1.AppendText("COMが接続されていません\n");
            }
        }

        delegate void SetTextCallBack(string text);
        private void res(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallBack d = new SetTextCallBack(res);
                BeginInvoke(d, new object[] { text });
            }
            else
            {
                if (text.Contains("\r\n")){
                    if (flagment != "")
                    {
                        text = flagment + text;
                        flagment = "";
                    }
                    richTextBox1.Focus();
                    richTextBox1.AppendText(text);

                    if (text.Contains("OK"))
                    {
                        waitCallBack = false;
                    }

                }
                else
                {
                    flagment += text;
                }
            }
        }

        private void rtb(string x)
        {
            richTextBox1.Focus();
            richTextBox1.AppendText(x);
        }

        private void testXBee()
        {
            setFlagAndWrite("+++");
            while(waitCallBack == true) { }
            setFlagAndWrite("AT\r");
            while(waitCallBack == true) { }
            serialPort1.Write("ATCN\r");
        }
        private void rebootXBee()
        {
            setFlagAndWrite("+++\r");
            while(waitCallBack == true) { }
            serialPort1.Write("ATWR\r");
            serialPort1.Write("ATBD7\r");
            serialPort1.Write("ATCN\r");
            try
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                serialPort1.Close();
                serialPort1.BaudRate = 115200;
                serialPort1.Open();
                rtb("XBee再起動，COM再接続完了\nボーレートを115200に変更\n");
            }
            catch
            {
                rtb("再接続時にエラー\n");
            }
        }

        private void settingXBee()
        {
            setFlagAndWrite("+++");
            while(waitCallBack == true) { }
            serialPort1.Write("ATID " + textBox3.Text + "\r");
            serialPort1.Write("ATNJ FF\r");

            if(checkBox1.Checked == true)
            {
                serialPort1.Write("ATCE 1\r");
                serialPort1.Write("ATDL FFFF\r");
                serialPort1.Write("ATAP 2\r");
            }else if(checkBox2.Checked == true)
            {
                serialPort1.Write("ATJV 1\r");
                serialPort1.Write("ATDL FFFF\r");
                serialPort1.Write("ATAP 2\r");
            }
            else
            {
                serialPort1.Write("ATJV 1\r");
                serialPort1.Write("ATAP 4\r");
                serialPort1.Write("ATSM 6\r");
                serialPort1.Write("ATPS 1\r");
                serialPort1.Write("ATD1 3\r");
                serialPort1.Write("ATD2 5\r");
                var random = new Random();
                serialPort1.Write("ATNI"+node_id+"\r");
            }

            serialPort1.Write("ATCN\r");
            rtb("XBee設定完了！！\n");
        }

        private void setFlagAndWrite(string y){
            waitCallBack = true;
            serialPort1.Write(y);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string received_data = sp.ReadExisting();

            if(received_data.IndexOf("\r\n") >= 0)
            {
                res(received_data);
            }
            if(received_data.Length > 1024)
            {
                received_data = string.Empty;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string CD = Directory.GetCurrentDirectory();
            string root = @CD + "\\data";
            System.Diagnostics.Process.Start(root);
        }
    }
}
