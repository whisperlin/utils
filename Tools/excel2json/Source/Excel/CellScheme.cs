using Excel;

namespace ExcelToJson
{
    class CellScheme
    {
        public enum eType
        {
            None,
            Int,
            Float,
            String,
            Bool,
            Comment,
        }

        public eType Type
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public CellScheme()
        {
            Type = eType.None;
            Name = "";
        }

        public bool ReadName(IExcelDataReader reader, int i)
        {
            if (reader == null)
                return false;

            if (reader.Read())

            Name = reader.GetString(i);
            return true;
        }

        public bool ReadType(IExcelDataReader reader, int i)
        {
            if (reader == null)
                return false;

            Name = reader.GetString(i);
            return true;
        }
    }

}
