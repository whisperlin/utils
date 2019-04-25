using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExcelToJson
{
    class JsonWriter
    {
        string m_rowPrefix;

        public JsonWriter(string rowPrefix)
        {
            m_rowPrefix = rowPrefix;
        }

        public bool Write(string path, List<Row> rows)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path, false, Encoding.UTF8))
                {
                    file.WriteLine("[");

                    for (int i = 0; i < rows.Count; ++i)
                    {
                        file.Write(m_rowPrefix);
                        file.Write(rows[i].ToString());

                        string postfix = (i < rows.Count - 1) ? "," : "";
                        file.WriteLine(postfix);
                    }

                    file.WriteLine("]");

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("- Can not write file : {0} ({1})", path, e.Message);
                return false;
            }
        }
    }
}
