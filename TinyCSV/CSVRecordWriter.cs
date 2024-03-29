using System;
using System.Collections;
using System.Collections.Generic;

namespace TinyCSV
{
    public class CSVRecordWriter : IEnumerable<string>
    {
        public readonly List<string> Cells;
        public int Column => Cells.Count;
        public char CellSeparator;

        /// <summary>
        /// Create a empty CSVRecordWriter.
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// </summary>
        public CSVRecordWriter(char cellSeparator = CSVDataHelper.CommaCharacter)
        {
            Cells = new List<string>();
            CellSeparator = cellSeparator;
        }

        /// <summary>
        /// Create a CSVRecordWriter by CSV row data.
        /// </summary>
        /// <param name="record">CSV row data.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="capacity">List capacity.</param>
        public CSVRecordWriter(string record, char cellSeparator = CSVDataHelper.CommaCharacter, int capacity = 0)
        {
            Cells = record.GetCSVDecodeRow(cellSeparator, capacity);
            CellSeparator = cellSeparator;
        }
        
        /// <summary>
        /// Create a CSVRecordWriter by cells.
        /// </summary>
        /// <param name="cells">IEnumerable cells.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        public CSVRecordWriter(IEnumerable<string> cells, char cellSeparator = CSVDataHelper.CommaCharacter)
        {
            Cells = new List<string>(cells);
            CellSeparator = cellSeparator;
        }

        /// <summary>
        /// Create a CSVRecordWriter by CSVRecordReader.
        /// </summary>
        /// <param name="csvRecordReader">CSVRecordReader.</param>
        public CSVRecordWriter(CSVRecordReader csvRecordReader) : this(csvRecordReader.Cells, csvRecordReader.CellSeparator)
        {
        }
        
        /// <summary>
        /// Create a CSVRecordWriter by CSVRecordReader.
        /// </summary>
        /// <param name="csvRecordReader">CSVRecordReader.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        public CSVRecordWriter(CSVRecordReader csvRecordReader, char cellSeparator) : this(csvRecordReader.Cells, cellSeparator)
        {
        }

        public CSVRecordWriter Add(string cell)
        {
            Cells.Add(cell);
            return this;
        }
        
        public CSVRecordWriter RemoveAt(int index)
        {
            try
            {
                Cells.RemoveAt(index);
                return this;
            }
            catch (Exception e)
            {
                throw new CSVException(e);
            }
        }

        public string GetEncodeRow()
        {
            return Cells.GetCSVEncodeRow(CellSeparator);
        }
        
        public string GetEncodeRow(char cellSeparator)
        {
            return Cells.GetCSVEncodeRow(cellSeparator);
        }

        public override string ToString()
        {
            return GetEncodeRow();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Cells.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}