using System;
using System.Text;

namespace TinyCSV
{
    /// <summary>
    /// Read csv table.
    /// </summary>
    public class CSVTableReader
    {
        public readonly string RawCSVContent;
        public readonly string[] Headers;
        public readonly string[] Descriptions;
        public readonly CSVRecordReader[] Records;
        public readonly int Column;
        public readonly int RecordRow;
        public char CellSeparator;
        private StringBuilder mStringBuilder;

        /// <summary>
        /// Create a CSVTableReader by csv content.
        /// </summary>
        /// <param name="svContent">CSV content.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="supportCellMultiline">If true, support multiline cells but slower, otherwise not support multiline cells but faster.</param>
        /// <param name="readRecordCount">Read how many record rows. Negative means all records.</param>
        public CSVTableReader(string svContent, char cellSeparator = CSVDataHelper.CommaCharacter, bool supportCellMultiline = true, int readRecordCount = -1)
        {
            RawCSVContent = svContent;
            CellSeparator = cellSeparator;
            string[] rows = RawCSVContent.GetCSVRowArray(cellSeparator, supportCellMultiline, readRecordCount >= 0 ? readRecordCount + CSVDataHelper.HeaderInfoRowCount : readRecordCount);
            int rowsLength = rows.Length;
            Headers = rowsLength > 0 ? rows[0].GetCSVDecodeRow(cellSeparator).ToArray() : new string[0];
            Column = Headers.Length;
            Descriptions = rowsLength > 1 ? rows[1].GetCSVDecodeRow(cellSeparator, Column).ToArray() : new string[0];
            if (rowsLength > CSVDataHelper.HeaderInfoRowCount)
            {
                //Remove the first and second lines.
                Records = new CSVRecordReader[rowsLength - CSVDataHelper.HeaderInfoRowCount];
                for (int i = CSVDataHelper.HeaderInfoRowCount; i < rowsLength; i++)
                    Records[i - CSVDataHelper.HeaderInfoRowCount] = new CSVRecordReader(Headers, rows[i], cellSeparator);
            }
            else
                Records = new CSVRecordReader[0];
            RecordRow = Records.Length;
        }

        /// <summary>
        /// Get decode csv string.
        /// </summary>
        public string GetDecodeTable(NewLineStyle newLineStyle = NewLineStyle.Environment)
        {
            return GetDecodeTable(CellSeparator, newLineStyle);
        }

        /// <summary>
        /// Get decode csv string.
        /// </summary>
        public string GetDecodeTable(char cellSeparator, NewLineStyle newLineStyle = NewLineStyle.Environment)
        {
            if (mStringBuilder == null)
                mStringBuilder = new StringBuilder();
            else
                mStringBuilder.Clear();
            string newLine = newLineStyle.GetNewLine();
            for (int i = 0, len = Headers.Length; i < len; i++)
            {
                mStringBuilder.Append(Headers[i]);
                if(i < len - 1)
                    mStringBuilder.Append(cellSeparator);
                else
                    mStringBuilder.Append(newLine);
            }
            for (int i = 0, len = Descriptions.Length; i < len; i++)
            {
                mStringBuilder.Append(Descriptions[i]);
                if(i < len - 1)
                    mStringBuilder.Append(cellSeparator);
                else
                    mStringBuilder.Append(newLine);
            }

            foreach (var record in Records)
            {
                mStringBuilder.Append(record.GetDecodeRow(cellSeparator));
                mStringBuilder.Append(newLine);
            }
            string decodeTable = mStringBuilder.ToString();
            mStringBuilder.Clear();
            return decodeTable;
        }
        
        public override string ToString()
        {
            return GetDecodeTable();
        }
    }
}