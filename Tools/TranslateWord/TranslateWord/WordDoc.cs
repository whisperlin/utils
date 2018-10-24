using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;
using System.Windows.Forms;

namespace TranslateWord
{
    public class WordDoc
    {
 
  

        static string Translate(string text)
        {
            return   "("+XiaoNiu.TranslateWord(text)+")";
        }
        static string TryTranslate(  string text, ref string temp)
        {
            int id = text.IndexOf('.');
            if (id >= 0)
            {
                string form = text.Substring(0, id+1);
                string back = text.Substring(id+1);
                string  t = temp + form;
                temp = "";
                return form + Translate(t) + TryTranslate(back,ref temp);

            }
            else
            {
                return text;
            }
        }
        //_Paragraph.ParentNode;
        static void PrintParagraph(Paragraph _Paragraph )
        {
 
            if (_Paragraph.Runs != null)
            {
                var arys = _Paragraph.Runs.ToArray();
                string t_text = "";
                for (int j = 0, len = arys.Length; j < len; j++)
                {
                    Run r = arys[j];
                    string text = r.GetText();
                    if (j == len - 1)
                    {
                        if (text.Length < 1 || text[text.Length - 1] != '.')
                            text = text + ".";
                    }
                    //System.Console.WriteLine(text);
                    r.Text = TryTranslate(text,ref t_text);

                    t_text += text;
                    

                }
                 
            }
        }
        public static void LoadDoc(string path)
        {
            Document doc = new Document(path);
            NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);

            foreach (Paragraph _Paragraph in paragraphs.OfType<Paragraph>())
            {
                PrintParagraph(_Paragraph);
            }
            //foreach (Section _Section in doc.Sections)
            //{
            //    var body = _Section.Body;
            //    foreach (var node in body.ChildNodes)
            //    {
            //        if (node is Paragraph)
            //        {
            //            var _Paragraph = (Paragraph)node;
            //            PrintParagraph(_Paragraph);

            //        }
            //        if (node is Aspose.Words.Tables.Table)
            //        {
            //            var _Tab = (Aspose.Words.Tables.Table)node;
            //            foreach (var _row in _Tab.Rows)
            //            {
            //                _row.
            //               //foreach( var _cel in _row.)
            //            }

            //        }
            //    }
            //} 

            doc.Save(path+"modify.docx");
            MessageBox.Show("翻译完成");
        }
    }
}
