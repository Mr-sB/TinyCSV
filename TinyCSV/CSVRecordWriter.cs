using System;
using System.Collections.Generic;

namespace TinyCSV
{
    public class CSVRecordWriter
    {
        public readonly List<string> Cells;
        public readonly char CellSeparator;

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
        /// <param name="cellSeparator">CSV cells separator.</param>
        public CSVRecordWriter(CSVRecordReader csvRecordReader, char cellSeparator = CSVDataHelper.CommaCharacter) : this(csvRecordReader.CellArray, cellSeparator)
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
                throw new CSVException("Index was out of range!", e);
            }
        }
        
        public override string ToString()
        {
            return Cells.GetCSVEncodeRow(CellSeparator);
        }
    }
}