using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslateWord;

namespace TranslateWord
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "D:\\";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "doc|*.docx";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                WordDoc.LoadDoc(openFileDialog.FileName);
                 
                //MessageBox.Show(openFileDialog.FileName);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(XiaoNiu.TranslateWord("how are you."));
        }
    }
}
