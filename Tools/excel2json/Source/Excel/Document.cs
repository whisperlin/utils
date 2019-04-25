using System;
using System.IO;
using System.Collections.Generic;
using Excel;

namespace ExcelToJson
{
    class Document
    {
        List<Sheet> m_sheets = new List<Sheet>();

        public Document()
        {
        }

        public bool Load(string path)
        {
            Console.WriteLine("Load file \"{0}\"", path);

            m_sheets.Clear();

            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    if (reader == null)
                    {
                        Console.WriteLine("Can not make reader");
                        return false;
                    }

                    do
                    {
                        Sheet sheet = new Sheet(reader.Name);
                        if (!sheet.Load(reader))
                            return false;

                        m_sheets.Add(sheet);
                    }
                    while (reader.NextResult());

                    Console.WriteLine("- Success\r\n");
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("- Can not read file : {0} ({1})\r\n", path, e.Message);
                return false;
            }
        }

        public bool Save(string pathIn)
        {
            Console.WriteLine("Save file \"{0}\"", pathIn);

            if (m_sheets.Count == 1)
            {
                Sheet sheet = m_sheets[0];
                if (!m_sheets[0].Save(pathIn))
                    return false;
            }
            else
            {
                foreach (Sheet sheet in m_sheets)
                {
                    string postfix = sheet.Name.Replace(' ', '_');

                    int index = pathIn.LastIndexOf('.');
                    if (index < 0)
                        index = pathIn.Length - 1;

                    string path = pathIn.Insert(index, string.Format("_{0}", postfix));

                    if (!sheet.Save(path))
                        return false;
                }
            }

            Console.WriteLine("- Success\r\n");
            return true;
        }
    }
}
