using System.Text;

namespace TinyCSV
{
    /// <summary>
    /// Read csv table.
    /// </summary>
    public class CSVTableReader
    {
        public readonly CSVRecordReader[] Headers;
        public readonly int HeaderRow;
        public readonly CSVRecordReader[] Records;
        public readonly int RecordRow;
        public char CellSeparator;
        private StringBuilder mStringBuilder;

        /// <summary>
        /// Create a CSVTableReader by csv content.
        /// </summary>
        /// <param name="svContent">CSV content.</param>
        /// <param name="headerRow">Header Row.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="supportCellMultiline">If true, support multiline cells but slower, otherwise not support multiline cells but faster.</param>
        /// <param name="readRecordCount">Read how many record rows. Negative means all records.</param>
        public CSVTableReader(string svContent, int headerRow, char cellSeparator = CSVDataHelper.CommaCharacter, bool supportCellMultiline = true, int readRecordCount = -1)
        {
            HeaderRow = headerRow;
            CellSeparator = cellSeparator;
            string[] rows = svContent.GetCSVRowArray(cellSeparator, supportCellMultiline, readRecordCount >= 0 ? readRecordCount + headerRow : readRecordCount);
            int rowsLength = rows.Length;
            Headers = new CSVRecordReader[headerRow];
            for (int i = 0; i < headerRow; i++)
                Headers[i] = i < rowsLength ? new CSVRecordReader(rows[i], cellSeparator) : new CSVRecordReader();
            if (rowsLength > headerRow)
            {
                //Remove headers
                Records = new CSVRecordReader[rowsLength - headerRow];
                for (int i = headerRow; i < rowsLength; i++)
                    Records[i - headerRow] = new CSVRecordReader(rows[i], cellSeparator, Headers[0].Column);
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
            foreach (var header in Headers)
            {
                mStringBuilder.Append(header.GetDecodeRow(cellSeparator));
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