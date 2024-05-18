using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WinformThreadingFix
{
    public partial class Form1 : Form
    {
        Client client;
        public Form1()
        {
            InitializeComponent();
            client = new Client();
            if (client.Connect())
            {
                client.ReceivedMessage += UpdateTextField;
            }
            else
            {
                treeView1.Nodes.Add("I am a server now because I couldn't connect to one!");
                treeView1.Invalidate();
                textBox1.ReadOnly = true;
                Thread thread = new Thread(() => { Server server = new Server(); });
                thread.IsBackground = true;
                thread.Start();
            }
        }

        void UpdateTextField(string message)
        {
            treeView1.Invoke((MethodInvoker)(() => { treeView1.Nodes.Add(DateTime.Now.ToString(), message); treeView1.Invalidate(); }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                client.SendMessage(((TextBox)sender).Text);
                ((TextBox)sender).Text = "";
            }
        }
    }
}
