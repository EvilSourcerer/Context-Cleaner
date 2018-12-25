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
namespace Context_Cleaner
{
    public partial class Update : Form
    {
        public Update(Form1 form)
        {
            InitializeComponent();
            target = form;
        }
        Form1 target;
        private void Update_Load(object sender, EventArgs e)
        {
            File.Replace("contextcleanerupdate.exe", "Context Cleaner.exe","oldver.exe.bak");
            target.Close();
            Close();
        }
    }
}
