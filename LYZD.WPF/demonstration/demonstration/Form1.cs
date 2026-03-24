using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace demonstration
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        RegisterHelper Register = new RegisterHelper();
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Register.GetMNum();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string Reg = Register.MD5Decrypt(textBox1.Text);
                if (textBox2.Text == Reg)
                {
                    System.Windows.Forms.MessageBox.Show("注册成功！", "信息");
                    RegistryKey retkey = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("lzd").CreateSubKey("lzd.INI").CreateSubKey(textBox2.Text);
                    retkey.SetValue("UserName", "Rsoft");
                    this.Close();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("注册码错误！", "警告");
                    textBox2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
