using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCSV
{
    /// <summary>
    /// Write csv table.
    /// </summary>
    public class CSVTableWriter
    {
        public readonly List<CSVRecordWriter> Headers;
        public int HeaderRow => Headers.Count;
        public readonly List<CSVRecordWriter> Records;
        public int RecordRow => Records.Count;
        public char CellSeparator;
        private StringBuilder mStringBuilder;

        /// <summary>
        /// Create a empty CSVTableWriter.
        /// </summary>
        /// <param name="cellSeparator">CSV cells separator.</param>
        public CSVTableWriter(char cellSeparator = CSVDataHelper.CommaCharacter)
        {
            Headers = new List<CSVRecordWriter>();
            Records = new List<CSVRecordWriter>();
            CellSeparator = cellSeparator;
        }

        /// <summary>
        /// Create a CSVTableWriter by csv content.
        /// </summary>
        /// <param name="svContent">CSV content.</param>
        /// <param name="headerRow">Header Row.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="supportCellMultiline">If true, support multiline cells but slower, otherwise not support multiline cells but faster.</param>
        /// <param name="readRecordCount">Read how many record rows. Negative means all records.</param>
        public CSVTableWriter(string svContent, int headerRow, char cellSeparator = CSVDataHelper.CommaCharacter, bool supportCellMultiline = true, int readRecordCount = -1) : this(cellSeparator)
        {
            string[] rows = svContent.GetCSVRowArray(cellSeparator, supportCellMultiline, readRecordCount >= 0 ? readRecordCount + headerRow : -1);
            int rowsLength = rows.Length;
            Headers.Capacity = headerRow;
            for (int i = 0; i < headerRow; i++)
                Headers.Add(i < rowsLength ? new CSVRecordWriter(rows[i], cellSeparator) : new CSVRecordWriter(cellSeparator));
            if (rowsLength > headerRow)
            {
                //Remove headers
                Records.Capacity = rowsLength - headerRow;
                for (int i = headerRow; i < rowsLength; i++)
                    Records.Add(new CSVRecordWriter(rows[i], cellSeparator, Headers[0].Column));
            }
        }
        
        /// <summary>
        /// Create a CSVTableWriter by CSVTableReader.
        /// </summary>
        /// <param name="csvTableReader">CSVTableReader.</param>
        /// <param name="readRecordCount">Read how many record rows. Negative means all records.</param>
        public CSVTableWriter(CSVTableReader csvTableReader, int readRecordCount = -1) : this(csvTableReader.CellSeparator)
        {
            Headers.Capacity = csvTableReader.HeaderRow;
            for (int i = 0; i < csvTableReader.HeaderRow; i++)
                Headers.Add(new CSVRecordWriter(csvTableReader.Headers[i]));
            if (readRecordCount != 0)
            {
                var records = csvTableReader.Records;
                var recordCount = records.Length;
                var count = readRecordCount > 0 ? Math.Min(recordCount, readRecordCount) : recordCount;
                Records.Capacity = count;
                for (int i = 0; i < count; i++)
                    Records.Add(new CSVRecordWriter(records[i]));
            }
        }
        
        /// <summary>
        /// Create a CSVTableWriter by CSVTableReader.
        /// </summary>
        /// <param name="csvTableReader">CSVTableReader.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="readRecordCount">Read how many record rows. Negative means all records.</param>
        public CSVTableWriter(CSVTableReader csvTableReader, char cellSeparator, int readRecordCount = -1) : this(cellSeparator)
        {
            Headers.Capacity = csvTableReader.HeaderRow;
            for (int i = 0; i < csvTableReader.HeaderRow; i++)
                Headers.Add(new CSVRecordWriter(csvTableReader.Headers[i], cellSeparator));
            if (readRecordCount != 0)
            {
                var records = csvTableReader.Records;
                var recordCount = records.Length;
                var count = readRecordCount > 0 ? Math.Min(recordCount, readRecordCount) : recordCount;
                Records.Capacity = count;
                for (int i = 0; i < count; i++)
                    Records.Add(new CSVRecordWriter(records[i], cellSeparator));
            }
        }

        public CSVTableWriter AddHeader(CSVRecordWriter csvRecordWriter)
        {
            Headers.Add(csvRecordWriter);
            return this;
        }

        public CSVTableWriter AddRecord(CSVRecordWriter csvRecordWriter)
        {
            Records.Add(csvRecordWriter);
            return this;
        }

        public CSVTableWriter RemoveHeader(int index)
        {
            try
            {
                Headers.RemoveAt(index);
                return this;
            }
            catch (Exception e)
            {
                throw new CSVException(e);
            }
        }
        
        public CSVTableWriter RemoveRecord(int index)
        {
            try
            {
                Records.RemoveAt(index);
                return this;
            }
            catch (Exception e)
            {
                throw new CSVException(e);
            }
        }

        /// <summary>
        /// Get a csv form string.
        /// </summary>
        /// <param name="newLineStyle">NewLineStyle.</param>
        /// <returns>CSV table.</returns>
        public string GetEncodeTable(NewLineStyle newLineStyle = NewLineStyle.Environment)
        {
            return GetEncodeTable(CellSeparator, newLineStyle);
        }
        
        /// <summary>
        /// Get a csv form string.
        /// </summary>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="newLineStyle">NewLineStyle.</param>
        /// <returns></returns>
        public string GetEncodeTable(char cellSeparator, NewLineStyle newLineStyle = NewLineStyle.Environment)
        {
            if (mStringBuilder == null)
                mStringBuilder = new StringBuilder();
            else
                mStringBuilder.Clear();
            string newLine = newLineStyle.GetNewLine();
            foreach (var header in Headers)
            {
                mStringBuilder.Append(header.GetEncodeRow(cellSeparator));
                mStringBuilder.Append(newLine);
            }
            foreach (var record in Records)
            {
                mStringBuilder.Append(record.GetEncodeRow(cellSeparator));
                mStringBuilder.Append(newLine);
            }
            string encodeCSV = mStringBuilder.ToString();
            mStringBuilder.Clear();
            return encodeCSV;
        }
        
        public override string ToString()
        {
            return GetEncodeTable();
        }
    }
}