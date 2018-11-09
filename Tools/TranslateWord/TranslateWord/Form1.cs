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

        static int[] long2doubleInt(long a)
        {
            int a1 = (int)(a & uint.MaxValue);
            int a2 = (int)(a >> 32);
            return new int[] { a1, a2 };
        }

        static long doubleInt2long(int a1, int a2)
        {
            long b = a2;
            b = b << 32;
            b = b | (uint)a1;
            return b;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            long a = doubleInt2long(-100, -300);
            int[] ary = long2doubleInt(a);

            a = doubleInt2long(-100, -300);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            XiaoNiu.apikey = textBoxKey.Text;
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
