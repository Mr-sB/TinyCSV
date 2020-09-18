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
        public CSVTableWriter(string svContent, char cellSeparator = CSVDataHelper.CommaCharacter, bool supportCellMultiline = true)
        {
            string[] rows = svContent.GetCSVRowArray(cellSeparator, supportCellMultiline);
            CellSeparator = cellSeparator;
            int recordLen = rows.Length;
            Headers = recordLen > 0 ? rows[0].GetCSVDecodeRow(cellSeparator) : new List<string>();
            Descriptions = recordLen > 1 ? rows[1].GetCSVDecodeRow(cellSeparator, Headers.Count) : new List<string>();
            if (recordLen > 2)
            {
                //Remove the first and second lines.
                Records = new List<CSVRecordWriter>(recordLen - 2);
                for (int i = 2; i < recordLen; i++)
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
        public CSVTableWriter(CSVTableReader csvTableReader, char cellSeparator = CSVDataHelper.CommaCharacter)
        {
            CellSeparator = cellSeparator;
            Headers = new List<string>(csvTableReader.Headers);
            Descriptions = new List<string>(csvTableReader.Descriptions);
            var records = csvTableReader.Records;
            int recordCount = records.Length;
            Records = new List<CSVRecordWriter>(recordCount);
            for (int i = 0; i < recordCount; i++)
                Records.Add(new CSVRecordWriter(records[i]));
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