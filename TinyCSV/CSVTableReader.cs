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

        public CSVTableReader(string svContent)
        {
            RawCSVContent = svContent;
            string[] rows = RawCSVContent.GetCSVRows();
            int recordLen = rows.Length;
            Headers = recordLen > 0 ? rows[0].GetCSVDecodeRow().ToArray() : new string[0];
            Column = Headers.Length;
            Descriptions = recordLen > 1 ? rows[1].GetCSVDecodeRow(Column).ToArray() : new string[0];
            if (recordLen > 2)
            {
                //Remove the first and second lines.
                Records = new CSVRecordReader[recordLen - 2];
                for (int i = 2; i < recordLen; i++)
                    Records[i - 2] = new CSVRecordReader(Headers, rows[i]);
            }
            else
                Records = new CSVRecordReader[0];
            RecordRow = Records.Length;
        }
    }
}