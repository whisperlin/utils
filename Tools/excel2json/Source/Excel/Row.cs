using System;
using System.Collections.Generic;
using Excel;

namespace ExcelToJson
{
    class Row
    {
        uint m_rowIndex = 0;
        List<Cell> m_cells = null;

        public uint RowIndex
        {
            get; private set;
        }

        public uint LineNumber
        {
            get { return RowIndex + 2; }
        }

        public Row(uint rowIndex, List<CellScheme> schemes)
        {
            RowIndex = rowIndex;
            m_cells = new List<Cell>();

            for (int i = 0; i < schemes.Count; ++i)
            {
                CellScheme scheme = schemes[i];
                m_cells.Add(Cell.Create((uint)i, scheme));
            }
        }

        public bool Parse(List<string> columnTexts)
        {
            if (columnTexts.Count != m_cells.Count)
                return false;

            for (int i = 0; i < columnTexts.Count; ++i)
            {
                Cell cell = m_cells[i];
                string text = columnTexts[i];

                if (cell == null)
                    return false;

                if (!cell.IsSerializable || text == null)
                    continue;

                if (!cell.Parse(text))
                {
                    Console.WriteLine("Parse failed Line {0}, Column {1} : {2}", m_rowIndex + 2, cell.ColumnIndex + 1, text);
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            List<string> cells = new List<string>();
            foreach (Cell cell in m_cells)
            {
                if (cell.IsSerializable)
                    cells.Add(cell.ToString());
            }

            string total = string.Join(", ", cells);
            return string.Format("{{{0}}}", total);
        }
    }
}
