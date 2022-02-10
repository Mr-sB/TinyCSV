using System.Text;

namespace TinyCSV
{
    public class CSVRecordReader
    {
        public readonly string[] CellArray;
        public char CellSeparator;
        private StringBuilder mStringBuilder;

        /// <summary>
        /// Create a CSVRecordReader.
        /// </summary>
        /// <param name="record">CSV row data.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="capacity">List capacity.</param>
        public CSVRecordReader(string record, char cellSeparator = CSVDataHelper.CommaCharacter, int capacity = 0)
        {
            CellSeparator = cellSeparator;
            CellArray = record.GetCSVDecodeRow(cellSeparator, capacity).ToArray();
        }

        public string GetDecodeRow()
        {
            return GetDecodeRow(CellSeparator);
        }
        
        public string GetDecodeRow(char cellSeparator)
        {
            if (mStringBuilder == null)
                mStringBuilder = new StringBuilder();
            else
                mStringBuilder.Clear();
            for (int i = 0, len = CellArray.Length; i < len; i++)
            {
                mStringBuilder.Append(CellArray[i]);
                if(i < len - 1)
                    mStringBuilder.Append(cellSeparator);
            }
            string decodeCSV = mStringBuilder.ToString();
            mStringBuilder.Clear();
            return decodeCSV;
        }
        
        public override string ToString()
        {
            return GetDecodeRow();
        }
    }
}