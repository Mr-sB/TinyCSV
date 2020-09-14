using System;
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
        
        public void RemoveCell(int index)
        {
            try
            {
                Cells.RemoveAt(index);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public override string ToString()
        {
            return Cells.GetCSVEncodeRow();
        }
    }
}