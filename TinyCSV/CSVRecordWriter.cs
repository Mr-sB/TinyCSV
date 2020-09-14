using System.Collections.Generic;

namespace TinyCSV
{
    public class CSVRecordWriter
    {
        public readonly List<string> Cells;

        public CSVRecordWriter()
        {
            Cells = new List<string>();
        }

        public CSVRecordWriter(string record, int capacity = 0)
        {
            Cells = record.GetCSVDecodeRow(capacity);
        }
        
        public CSVRecordWriter(IEnumerable<string> cells)
        {
            Cells = new List<string>(cells);
        }

        public CSVRecordWriter(CSVRecordReader csvRecordReader) : this(csvRecordReader.CellArray)
        {
        }

        public void AddCell(string cell)
        {
            Cells.Add(cell);
        }
        
        public override string ToString()
        {
            return Cells.GetCSVEncodeRow();
        }
    }
}