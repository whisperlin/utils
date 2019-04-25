using System.Web;

namespace ExcelToJson
{
    abstract class Cell
    {
        protected CellScheme m_scheme;

        public uint ColumnIndex
        {
            get; private set;
        }

        public virtual bool IsSerializable
        {
            get { return true; }
        }

        public Cell(uint columnIndex, CellScheme scheme)
        {
            ColumnIndex = columnIndex;
            m_scheme = scheme;
        }

        public abstract bool Parse(string text);

        public static Cell Create(uint columnIndex, CellScheme scheme)
        {
            switch (scheme.Type)
            {
                case CellScheme.eType.Int:
                    return new CellInt(columnIndex, scheme);
                case CellScheme.eType.Float:
                    return new CellFloat(columnIndex, scheme);
                case CellScheme.eType.Bool:
                    return new CellBool(columnIndex, scheme);
                case CellScheme.eType.String:
                    return new CellString(columnIndex, scheme);
                case CellScheme.eType.Comment:
                    return new CellComment(columnIndex, scheme);
                default:
                    return null;
            }
        }
    }

    class CellInt : Cell
    {
        int m_value = 0;

        public CellInt(uint columnIndex, CellScheme scheme)
            : base(columnIndex, scheme)
        {
        }

        public override bool Parse(string text)
        {
            return int.TryParse(text, out m_value);
        }

        public override string ToString()
        {
            return string.Format("\"{0}\":{1}", m_scheme.Name, m_value.ToString());
        }
    }

    class CellFloat : Cell
    {
        float m_value = 0.0f;

        public CellFloat(uint columnIndex, CellScheme scheme)
            : base(columnIndex, scheme)
        {
        }

        public override bool Parse(string text)
        {
            return float.TryParse(text, out m_value);
        }

        public override string ToString()
        {
            return string.Format("\"{0}\":{1}", m_scheme.Name, m_value.ToString());
        }
    }

    class CellBool : Cell
    {
        bool m_value = false;

        public CellBool(uint columnIndex, CellScheme scheme)
            : base(columnIndex, scheme)
        {
        }

        public override bool Parse(string text)
        {
            if (bool.TryParse(text, out m_value))
                return true;

            int value = 0;
            if (int.TryParse(text, out value))
            {
                m_value = (value != 0);
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\":{1}", m_scheme.Name, m_value ? 1 : 0);
        }
    }

    class CellString : Cell
    {
        string m_value = "";

        public CellString(uint columnIndex, CellScheme scheme)
            : base(columnIndex, scheme)
        {
        }

        public override bool Parse(string text)
        {
            m_value = EscapeForJson(text);
            return true;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\":\"{1}\"", m_scheme.Name, m_value.ToString());
        }

        static string EscapeForJson(string s)
        {
            return HttpUtility.JavaScriptStringEncode(s);
        }
    }

    class CellComment : Cell
    {
        public override bool IsSerializable
        {
            get { return false; }
        }

        public CellComment(uint columnIndex, CellScheme scheme)
            : base(columnIndex, scheme)
        {
        }

        public override bool Parse(string text)
        {
            return true;
        }

        public override string ToString()
        {
            return "";
        }
    }
}
