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
        private StringBuilder mStringBuilder;

        public CSVRecordReader(string[] headers, string record)
        {
            RawRecord = record;
            int column = headers.Length;
            CellDict = new Dictionary<string, string>(column);
            CellArray = RawRecord.GetCSVDecodeRow(column).ToArray();
            for (int i = 0, cellLen = CellArray.Length; i < column; i++)
            {
                if(!CellDict.ContainsKey(headers[i]))
                    CellDict.Add(headers[i], i < cellLen ? CellArray[i] : string.Empty);
                else
                    Console.WriteLine("Has same header: " + headers[i]);
            }
        }

        public override string ToString()
        {
            if (mStringBuilder == null)
                mStringBuilder = new StringBuilder();
            for (int i = 0, len = CellArray.Length; i < len; i++)
            {
                mStringBuilder.Append(CellArray[i]);
                if(i < len - 1)
                    mStringBuilder.Append(',');
            }
            string decodeCSV = mStringBuilder.ToString();
            mStringBuilder.Length = 0;
            return decodeCSV;
        }
    }
}