using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCSV
{
    public class CSVRecordReader
    {
        public readonly Dictionary<string, string> CellDict;
        public readonly string[] CellArray;
        public readonly string RawRecord;
        public char CellSeparator;
        private StringBuilder mStringBuilder;

        /// <summary>
        /// Create a CSVRecordReader.
        /// </summary>
        /// <param name="headers">Headres</param>
        /// <param name="record">CSV row data.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        public CSVRecordReader(string[] headers, string record, char cellSeparator = CSVDataHelper.CommaCharacter)
        {
            RawRecord = record;
            CellSeparator = cellSeparator;
            int column = headers.Length;
            CellDict = new Dictionary<string, string>(column);
            CellArray = RawRecord.GetCSVDecodeRow(cellSeparator, column).ToArray();
            for (int i = 0, cellLen = CellArray.Length; i < column; i++)
            {
                if(!CellDict.ContainsKey(headers[i]))
                    CellDict.Add(headers[i], i < cellLen ? CellArray[i] : string.Empty);
                else
                    Console.WriteLine("Has same header: " + headers[i]);
            }
        }

        public string GetDecodeRow()
        {
            return GetDecodeRow(CellSeparator);
        }
        
        public string GetDecodeRow(char cellSeparator)
        {
            if (mStringBuilder == null)
                mStringBuilder = new StringBuilder();
            for (int i = 0, len = CellArray.Length; i < len; i++)
            {
                mStringBuilder.Append(CellArray[i]);
                if(i < len - 1)
                    mStringBuilder.Append(cellSeparator);
            }
            string decodeCSV = mStringBuilder.ToString();
            mStringBuilder.Length = 0;
            return decodeCSV;
        }
        
        public override string ToString()
        {
            return GetDecodeRow();
        }
    }
}