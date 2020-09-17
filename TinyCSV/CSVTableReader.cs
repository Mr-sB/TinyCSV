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
        public readonly char CellSeparator;
        private StringBuilder mStringBuilder;

        /// <summary>
        /// Create a CSVTableReader by csv content.
        /// </summary>
        /// <param name="svContent">CSV content.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="supportCellMultiline">If true, support multiline cells but slower, otherwise not support multiline cells but faster.</param>
        public CSVTableReader(string svContent, char cellSeparator = CSVDataHelper.CommaCharacter, bool supportCellMultiline = true)
        {
            RawCSVContent = svContent;
            CellSeparator = cellSeparator;
            string[] rows = RawCSVContent.GetCSVRowArray(cellSeparator, supportCellMultiline);
            int recordLen = rows.Length;
            Headers = recordLen > 0 ? rows[0].GetCSVDecodeRow(cellSeparator).ToArray() : new string[0];
            Column = Headers.Length;
            Descriptions = recordLen > 1 ? rows[1].GetCSVDecodeRow(cellSeparator, Column).ToArray() : new string[0];
            if (recordLen > 2)
            {
                //Remove the first and second lines.
                Records = new CSVRecordReader[recordLen - 2];
                for (int i = 2; i < recordLen; i++)
                    Records[i - 2] = new CSVRecordReader(Headers, rows[i], cellSeparator);
            }
            else
                Records = new CSVRecordReader[0];
            RecordRow = Records.Length;
        }

        public override string ToString()
        {
            if (mStringBuilder == null)
                mStringBuilder = new StringBuilder();
            for (int i = 0, len = Headers.Length; i < len; i++)
            {
                mStringBuilder.Append(Headers[i]);
                if(i < len - 1)
                    mStringBuilder.Append(CellSeparator);
                else
                    mStringBuilder.Append(Environment.NewLine);
            }
            for (int i = 0, len = Descriptions.Length; i < len; i++)
            {
                mStringBuilder.Append(Descriptions[i]);
                if(i < len - 1)
                    mStringBuilder.Append(CellSeparator);
                else
                    mStringBuilder.Append(Environment.NewLine);
            }
            foreach (var record in Records)
                mStringBuilder.AppendLine(record.ToString());
            string decodeCSV = mStringBuilder.ToString();
            mStringBuilder.Length = 0;
            return decodeCSV;
        }
    }
}