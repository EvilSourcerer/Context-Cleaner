using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
namespace Context_Cleaner
{
    public partial class AddContext : Form
    {
        public AddContext(string input,Form1 main)
        {
            InitializeComponent();
            mode = input;
            mainform = main;
        }
        public static string mode = "";
        public static Form1 mainform;
        private void AddContext_Load(object sender, EventArgs e)
        {
            if(mode=="Global Context Items")
            {
                label1.Text = "Add to Global Context Items";
            }
            else if(mode.StartsWith("."))
            {
                label1.Text = "Add to " + mode + " context items";
                textBox2.Enabled = false;
            }
            else if(mode=="Folder Context Items")
            {
                label1.Text = "Add to " + mode;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mode == "Global Context Items")
            {
                Registry.ClassesRoot.OpenSubKey(@"*\shell\", true).CreateSubKey(textBox1.Text);
                Registry.ClassesRoot.OpenSubKey(@"*\shell\" + textBox1.Text, true).SetValue(null, textBox1.Text);
                Registry.ClassesRoot.OpenSubKey(@"*\shell\" + textBox1.Text, true).SetValue("Icon", textBox2.Text);
                Registry.ClassesRoot.OpenSubKey(@"*\shell\" + textBox1.Text, true).CreateSubKey("command", true);
                Registry.ClassesRoot.OpenSubKey(@"*\shell\" + textBox1.Text + @"\command", true).SetValue(null, textBox3.Text);
                mainform.refreshGlobal();
                Close();
            }

            else if (mode == "Folder Context Items")
            {
                Registry.ClassesRoot.OpenSubKey(@"Directory\shell", true).CreateSubKey(textBox1.Text);
                Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + textBox1.Text, true).SetValue(null,textBox1.Text);
                Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + textBox1.Text, true).SetValue("Icon", textBox2.Text);
                Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + textBox1.Text, true).CreateSubKey("command");
                Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + textBox1.Text + @"\command", true).SetValue(null, textBox3.Text);
                mainform.refreshFolderContexts();
                Close();
            }
            else if (mode == "Desktop Context Items")
            {
                Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\", true).CreateSubKey(textBox1.Text);
                Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + textBox1.Text, true).SetValue(null, textBox1.Text);
                Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + textBox1.Text, true).SetValue("Icon", textBox2.Text);
                Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + textBox1.Text, true).CreateSubKey("command");
                Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + textBox1.Text + @"\command", true).SetValue(null, textBox3.Text);
                mainform.refreshDesktopItems();
                Close();
            }
            else
            {
                    RegistryKey targetextension = Registry.ClassesRoot.OpenSubKey(mode, true);
                    string defaults = targetextension.GetValue(null).ToString();
                    RegistryKey actualshit = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\" + defaults + @"\shell",true);
                    actualshit.CreateSubKey(textBox1.Text);
                    actualshit.OpenSubKey(textBox1.Text,true).CreateSubKey("command");
                    actualshit.OpenSubKey(textBox1.Text + @"\command", true).SetValue(null, textBox3.Text);
                    mainform.refreshFileContexts(mode);
                    Close();
            }
        }
    }
}
