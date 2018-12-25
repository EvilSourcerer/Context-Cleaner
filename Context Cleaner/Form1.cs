using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using Microsoft.Win32;
using System.Net;
using System.IO;

namespace Context_Cleaner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /**                 START PHASE UWU                  **/
        private void Form1_Load(object sender, EventArgs e)
        {
            checkPrivelages();
            string[] extensions = Registry.ClassesRoot.GetSubKeyNames();
            for (int i = 0; i < extensions.Length; i++)
            {
                if (extensions[i].StartsWith(".")) treeView1.Nodes[0].Nodes.Add(extensions[i]);
            }
            WebRequest request = WebRequest.Create("https://www.simpodex.com/ContextCleaner/update.php");
            WebResponse output = request.GetResponse();
            string ver = new StreamReader(output.GetResponseStream()).ReadToEnd();
            if (ver!="1.0")
            {
                
                DialogResult updateresponse=MessageBox.Show("You are " + (float.Parse(ver)*10-float.Parse("1.0")*10) + " update(s) behind. Would you like to download updates?","Update notification",MessageBoxButtons.YesNo);
                if(updateresponse==DialogResult.Yes)
                {
                    WebClient downloader = new WebClient();
                    downloader.DownloadFileAsync(new Uri("https://www.simpodex.com/contextcleaner/ContextCleaner.exe"), "contextcleanerupdate.exe");
                    downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
                }
            }

        }
        private void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Update updater = new Update(this);
            updater.Show();
        }
        private void checkPrivelages()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                if (!isElevated)
                {
                    MessageBox.Show("Unfortunately, you don't seem to have the privelages to run this program -_-");
                }
            }
        }
        /**                 START PHASE UWU                  **/




        /**                 REFRESH LISTBOXES                **/
        public void refreshFileContexts(string target)
        {
            try
            {
                RegistryKey targetextension = Registry.ClassesRoot.OpenSubKey(target, true);
                string defaults = targetextension.GetValue(null).ToString();
                listBox1.Items.Clear();
                RegistryKey actualshit = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\" + defaults + @"\shell");
                string[] results = actualshit.GetSubKeyNames();
                for (int i = 0; i < results.Length; i++)
                {
                    listBox1.Items.Add(results[i]);
                }
            }
            catch
            {
                listBox1.Items.Clear();
                listBox1.Items.Add("No context menu items found");
            }
        }
        public void refreshFolderContexts()
        {
            listBox1.Items.Clear();
            string[] shit = Registry.ClassesRoot.OpenSubKey(@"Directory\shell", true).GetSubKeyNames();
            for (int i = 0; i < shit.Length; i++)
            {
                bool enabled = Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + shit[i]).GetValue("LegacyDisable") == null;
                listBox1.Items.Add(shit[i] + " - " + enabled.ToString());
            }
        }
        public void refreshGlobal()
        {
            listBox1.Items.Clear();
            string[] shellexvals = Registry.ClassesRoot.OpenSubKey(@"*\shell\", true).GetSubKeyNames();
            for (int i = 0; i < shellexvals.Length; i++)
            {
                bool enabled = Registry.ClassesRoot.OpenSubKey(@"*\shell\" + shellexvals[i]).GetValue("LegacyDisable") == null;
                listBox1.Items.Add(shellexvals[i] + " - " + enabled.ToString());
            }
        }
        public void refreshDesktopItems()
        {
            listBox1.Items.Clear();
            string[] shellexvals = Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\", true).GetSubKeyNames();
            for (int i = 0; i < shellexvals.Length; i++)
            {
                bool enabled = Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + shellexvals[i]).GetValue("LegacyDisable") == null;
                listBox1.Items.Add(shellexvals[i] + " - " + enabled.ToString());
            }
        }
        /**                 REFRESH LISTBOXES                **/




        /**            DISABLE FUNCTIONS. TRUST ME IT ISN'T LEGACY CODE!             **/
        private void legacydisable()
        {
            string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
            bool enabled = Registry.ClassesRoot.OpenSubKey(@"*\shell\" + selecteditem,true).GetValue("LegacyDisable") == null;
            if (enabled)
            {
                int oldindex = listBox1.SelectedIndex;
                Registry.ClassesRoot.OpenSubKey(@"*\shell\" + selecteditem, true).SetValue("LegacyDisable", "");
                refreshGlobal();
                listBox1.SelectedIndex = oldindex;
            }
        }
        private void legacyenable()
        {
            string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
            bool enabled = Registry.ClassesRoot.OpenSubKey(@"*\shell\" + selecteditem,true).GetValue("LegacyDisable") == null;
            if (!enabled)
            {
                int oldindex = listBox1.SelectedIndex;
                Registry.ClassesRoot.OpenSubKey(@"*\shell\" + selecteditem, true).DeleteValue("LegacyDisable");
                refreshGlobal();
                listBox1.SelectedIndex = oldindex;
            }
        }
        private void legacydisable_folder()
        {
            string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
            bool enabled = Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + selecteditem,true).GetValue("LegacyDisable") == null;
            if (enabled)
            {
                int oldindex = listBox1.SelectedIndex;
                Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + selecteditem, true).SetValue("LegacyDisable", "");
                refreshFolderContexts();
                listBox1.SelectedIndex = oldindex;
            }
        }
        private void legacyenable_folder()
        {
            string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
            bool enabled = Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + selecteditem,true).GetValue("LegacyDisable") == null;
            if (!enabled)
            {
                int oldindex = listBox1.SelectedIndex;
                Registry.ClassesRoot.OpenSubKey(@"Directory\shell\" + selecteditem, true).DeleteValue("LegacyDisable");
                refreshFolderContexts();
                listBox1.SelectedIndex = oldindex;
            }
        }
        private void legacydisable_desktop()
        {
            string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
            bool enabled = Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + selecteditem,true).GetValue("LegacyDisable") == null;
            if (enabled)
            {
                int oldindex = listBox1.SelectedIndex;
                Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + selecteditem, true).SetValue("LegacyDisable", "");
                refreshDesktopItems();
                listBox1.SelectedIndex = oldindex;
            }
        }
        private void legacyenable_desktop()
        {
            string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
            bool enabled = Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + selecteditem,true).GetValue("LegacyDisable") == null;
            if (!enabled)
            {
                int oldindex = listBox1.SelectedIndex;
                Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\" + selecteditem, true).DeleteValue("LegacyDisable");
                refreshDesktopItems();
                listBox1.SelectedIndex = oldindex;
            }
        }
        /**            DISABLE FUNCTIONS. TRUST ME IT ISN'T LEGACY CODE!             **/




        /**          PERMANENT DELETE FUNCTIONS.           **/

        private void deleteglobal()
        {
            try
            {
                string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
                DialogResult yesno = MessageBox.Show("Are you sure you want to delete this entry? THIS CANNOT BE UNDONE!", "warning", MessageBoxButtons.YesNo);
                if (yesno == DialogResult.Yes)
                {
                    Registry.ClassesRoot.OpenSubKey(@"*\shell\", true).DeleteSubKeyTree(selecteditem);
                    refreshGlobal();
                }
            }
            catch { }
        }
        private void deletefolder()
        {
            try
            {
                string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
                DialogResult yesno = MessageBox.Show("Are you sure you want to delete this entry? THIS CANNOT BE UNDONE!", "warning", MessageBoxButtons.YesNo);
                if (yesno == DialogResult.Yes)
                {
                    Registry.ClassesRoot.OpenSubKey(@"Directory\shell\", true).DeleteSubKeyTree(selecteditem);
                    refreshFolderContexts();
                }
            }
            catch { }
        }
        private void deletedesktop()
        {
            try
            {
                string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
                DialogResult yesno = MessageBox.Show("Are you sure you want to delete this entry? THIS CANNOT BE UNDONE!", "warning", MessageBoxButtons.YesNo);
                if (yesno == DialogResult.Yes)
                {
                    Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell\", true).DeleteSubKeyTree(selecteditem);
                    refreshDesktopItems();
                }
            }
            catch { }
        }
        private void deletefile()
        {
            try
            {
                string target = treeView1.SelectedNode.Text;
                string selecteditem = listBox1.SelectedItem.ToString().Replace(" - False", "").Replace(" - True", "");
                RegistryKey targetextension = Registry.ClassesRoot.OpenSubKey(target, true);
                string defaults = targetextension.GetValue(null).ToString();
                RegistryKey actualshit = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\" + defaults + @"\shell",true);
                DialogResult yesno = MessageBox.Show("Are you sure you want to delete this entry? THIS CANNOT BE UNDONE!", "warning", MessageBoxButtons.YesNo);
                if (yesno == DialogResult.Yes)
                {
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\" + defaults + @"\shell", true).DeleteSubKeyTree(selecteditem);
                    refreshFileContexts(target);
                }
            }
            catch { }
        }

        /**          PERMANENT DELETE FUNCTIONS.           **/

        private string mode = "global"; // NO TOUCH! THIS SAYS WHICH PART OF REGISTRY TO MODIFY!
        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string selected = treeView1.SelectedNode.Text;
            if(selected!="Context Items")
            {
                if(selected=="Global Context Items")
                {
                    refreshGlobal();
                    mode = "global";
                    button2.Enabled = true;
                    button3.Enabled = true;
                }
                else if(selected=="Folder Context Items")
                {
                    refreshFolderContexts();
                    mode = "folder";
                    button2.Enabled = true;
                    button3.Enabled = true;
                }
                else if (selected == "Desktop Context Items")
                {
                    refreshDesktopItems();
                    mode = "desktop";
                    button2.Enabled = true;
                    button3.Enabled = true;
                }
                else
                {
                    refreshFileContexts(selected);
                    mode = "file";
                    button2.Enabled = false;
                    button3.Enabled = false;
                }
            }
            else
            {
                listBox1.Items.Clear();
                listBox1.Items.Add("View context items using the tree view on the left");
            }
        } // big brains stuff happens here


        private void button1_Click(object sender, EventArgs e)
        {
            AddContext newinstance = new AddContext(treeView1.SelectedNode.Text, this);
            newinstance.Show();
        } // Allows you to add context items


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if(mode=="global") legacydisable();
                if (mode == "folder") legacydisable_folder();
                if (mode == "desktop") legacydisable_desktop();
            }
            catch { }
        } // this will disable shit


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if(mode=="global") legacyenable();
                if (mode == "folder") legacyenable_folder();
                if (mode == "desktop") legacyenable_desktop();
            }
            catch { }
        } // this will enable shit


        private void button4_Click(object sender, EventArgs e)
        {
            if (mode == "global") deleteglobal();
            if (mode == "folder") deletefolder();
            if (mode == "desktop") deletedesktop();
            if (mode == "file") deletefile();
        } // this will permanently delete shit
    }
}
