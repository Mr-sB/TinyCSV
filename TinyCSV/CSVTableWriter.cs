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
        public readonly List<string> Headers;
        public readonly List<string> Descriptions;
        public readonly List<CSVRecordWriter> Records;
        public char CellSeparator;
        private StringBuilder mStringBuilder;

        /// <summary>
        /// Create a empty CSVTableWriter.
        /// </summary>
        /// <param name="cellSeparator">CSV cells separator.</param>
        public CSVTableWriter(char cellSeparator = CSVDataHelper.CommaCharacter)
        {
            Headers = new List<string>();
            Descriptions = new List<string>();
            Records = new List<CSVRecordWriter>();
            CellSeparator = cellSeparator;
        }

        /// <summary>
        /// Create a CSVTableWriter by csv content.
        /// </summary>
        /// <param name="svContent">CSV content.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="supportCellMultiline">If true, support multiline cells but slower, otherwise not support multiline cells but faster.</param>
        /// <param name="readRecordCount">Read how many record rows. Negative means all records.</param>
        public CSVTableWriter(string svContent, char cellSeparator = CSVDataHelper.CommaCharacter, bool supportCellMultiline = true, int readRecordCount = -1)
        {
            string[] rows = svContent.GetCSVRowArray(cellSeparator, supportCellMultiline, readRecordCount >= 0 ? readRecordCount + CSVDataHelper.HeaderInfoRowCount : readRecordCount);
            CellSeparator = cellSeparator;
            int rowsLength = rows.Length;
            Headers = rowsLength > 0 ? rows[0].GetCSVDecodeRow(cellSeparator) : new List<string>();
            Descriptions = rowsLength > 1 ? rows[1].GetCSVDecodeRow(cellSeparator, Headers.Count) : new List<string>();
            if (rowsLength > CSVDataHelper.HeaderInfoRowCount)
            {
                //Remove header info rows.
                Records = new List<CSVRecordWriter>(rowsLength - CSVDataHelper.HeaderInfoRowCount);
                for (int i = CSVDataHelper.HeaderInfoRowCount; i < rowsLength; i++)
                    Records.Add(new CSVRecordWriter(rows[i], cellSeparator, Headers.Count));
            }
            else
                Records = new List<CSVRecordWriter>();
        }
        
        /// <summary>
        /// Create a CSVTableWriter by CSVTableReader.
        /// </summary>
        /// <param name="csvTableReader">CSVTableReader.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="readRecordCount">Read how many record rows. Negative means all records.</param>
        public CSVTableWriter(CSVTableReader csvTableReader, char cellSeparator = CSVDataHelper.CommaCharacter, int readRecordCount = -1)
        {
            CellSeparator = cellSeparator;
            Headers = new List<string>(csvTableReader.Headers);
            Descriptions = new List<string>(csvTableReader.Descriptions);
            if (readRecordCount == 0)
                Records = new List<CSVRecordWriter>();
            else
            {
                var records = csvTableReader.Records;
                var recordCount = records.Length;
                var count = readRecordCount > 0 ? Math.Min(recordCount, readRecordCount) : recordCount;
                Records = new List<CSVRecordWriter>(count);
                for (int i = 0; i < count; i++)
                    Records.Add(new CSVRecordWriter(records[i]));
            }
        }

        public CSVTableWriter AddHeader(string header)
        {
            Headers.Add(header);
            return this;
        }

        public CSVTableWriter AddDescription(string description)
        {
            Descriptions.Add(description);
            return this;
        }

        public CSVTableWriter AddRecord(CSVRecordWriter csvRecordWriter)
        {
            Records.Add(csvRecordWriter);
            //Assign this.CellSeparator to CSVRecordWriter.CellSeparator.
            csvRecordWriter.CellSeparator = CellSeparator;
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
                throw new CSVException("Index was out of range!", e);
            }
        }

        public CSVTableWriter RemoveDescription(int index)
        {
            try
            {
                Descriptions.RemoveAt(index);
                return this;
            }
            catch (Exception e)
            {
                throw new CSVException("Index was out of range!", e);
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
                throw new CSVException("Index was out of range!", e);
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
            mStringBuilder.Append(Headers.GetCSVEncodeRow(cellSeparator));
            mStringBuilder.Append(newLine);
            mStringBuilder.Append(Descriptions.GetCSVEncodeRow(cellSeparator));
            mStringBuilder.Append(newLine);
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