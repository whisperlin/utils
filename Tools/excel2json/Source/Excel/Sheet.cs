using System;
using System.Collections.Generic;
using System.Diagnostics;
using Excel;

namespace ExcelToJson
{
    class Sheet
    {
        List<CellScheme> m_schemes = new List<CellScheme>();
        List<Row> m_rows = new List<Row>();

        public string Name
        {
            get; private set;
        }

        public Sheet(string name)
        {
            Name = name;
        }

        public bool Load(IExcelDataReader reader)
        {
            Console.WriteLine("Load sheet \"{0}\"", Name);

            Debug.Assert(reader != null);

            m_schemes.Clear();
            m_rows.Clear();

            if (!ReadFieldNames(reader))
                return false;

            if (!ReadFieldTypes(reader))
                return false;

            if (!ReadRows(reader))
                return false;

            return true;
        }

        bool ReadFieldNames(IExcelDataReader reader)
        {
            if (!reader.Read())
            {
                Console.WriteLine("Sheet \"{0}\" does not have field name row", Name);
                return false;
            }

            for (int i = 0; i < reader.FieldCount; ++i)
            {
                CellScheme scheme = new CellScheme();
                scheme.Name = reader.GetString(i);
                m_schemes.Add(scheme);
            }

            return true;
        }

        bool ReadFieldTypes(IExcelDataReader reader)
        {
            if (!reader.Read())
            {
                Console.WriteLine("Sheet \"{0}\" does not have field type row", Name);
                return false;
            }

            if (reader.FieldCount != m_schemes.Count)
            {
                Console.WriteLine("Sheet \"{0}\"s field name and type count are different", Name);
                return false;
            }

            HashSet<string> typeNames = new HashSet<string>(
                Enum.GetNames(typeof(CellScheme.eType)),
                StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < m_schemes.Count; ++i)
            {
                string text = reader.GetString(i);
                if (!typeNames.Contains(text))
                {
                    Console.WriteLine("Sheet \"{0}\" has invalid type \"{1}\"", Name, text);
                    return false;
                }

                CellScheme.eType type = CellScheme.eType.None;
                if (!Enum.TryParse(text, true, out type))
                    return false;

                m_schemes[i].Type = type;
            }

            return true;
        }

        bool ReadRows(IExcelDataReader reader)
        {
            uint rowIndex = 0;

            while (reader.Read())
            {
                List<string> columns = new List<string>();

                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    string text = reader.GetString(i);
                    columns.Add(text);
                }

                Row row = new Row(rowIndex++, m_schemes);
                if (!row.Parse(columns))
                    return false;

                m_rows.Add(row);
            }

            return true;
        }

        public bool Save(string path)
        {
            Console.WriteLine("Save sheet \"{0}\" to \"{1}\"", Name, path);

            JsonWriter writer = new JsonWriter("\t");
            return writer.Write(path, m_rows);
        }
    }
}
