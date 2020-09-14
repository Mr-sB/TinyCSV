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
        private StringBuilder mStringBuilder;

        public CSVTableWriter()
        {
            Headers = new List<string>();
            Descriptions = new List<string>();
            Records = new List<CSVRecordWriter>();
        }

        public CSVTableWriter(string svContent)
        {
            string[] rows = svContent.GetCSVRows();
            int recordLen = rows.Length;
            Headers = recordLen > 0 ? rows[0].GetCSVDecodeRow() : new List<string>();
            Descriptions = recordLen > 1 ? rows[1].GetCSVDecodeRow(Headers.Count) : new List<string>();
            if (recordLen > 2)
            {
                //Remove the first and second lines.
                Records = new List<CSVRecordWriter>(recordLen - 2);
                for (int i = 2; i < recordLen; i++)
                    Records.Add(new CSVRecordWriter(rows[i], Headers.Count));
            }
            else
                Records = new List<CSVRecordWriter>();
        }
        
        public CSVTableWriter(CSVTableReader csvTableReader)
        {
            Headers = new List<string>(csvTableReader.Headers);
            Descriptions = new List<string>(csvTableReader.Descriptions);
            var records = csvTableReader.Records;
            int recordCount = records.Length;
            Records = new List<CSVRecordWriter>(recordCount);
            for (int i = 0; i < recordCount; i++)
                Records.Add(new CSVRecordWriter(records[i]));
        }

        public void AddHeader(string header)
        {
            Headers.Add(header);
        }

        public void AddDescription(string description)
        {
            Descriptions.Add(description);
        }

        public void AddRecord(CSVRecordWriter csvRecordWriter)
        {
            Records.Add(csvRecordWriter);
        }

        public void RemoveHeader(int index)
        {
            try
            {
                Headers.RemoveAt(index);
            }
            catch (Exception e)
            {
                throw new CSVException("Index was out of range!", e);
            }
        }

        public void RemoveDescription(int index)
        {
            try
            {
                Descriptions.RemoveAt(index);
            }
            catch (Exception e)
            {
                throw new CSVException("Index was out of range!", e);
            }
        }
        
        public void RemoveRecord(int index)
        {
            try
            {
                Records.RemoveAt(index);
            }
            catch (Exception e)
            {
                throw new CSVException("Index was out of range!", e);
            }
        }
        
        /// <summary>
        /// Get a csv form string.
        /// </summary>
        public override string ToString()
        {
            if (mStringBuilder == null)
                mStringBuilder = new StringBuilder();
            mStringBuilder.AppendLine(Headers.GetCSVEncodeRow());
            mStringBuilder.AppendLine(Descriptions.GetCSVEncodeRow());
            foreach (var record in Records)
                mStringBuilder.AppendLine(record.ToString());
            string encodeCSV = mStringBuilder.ToString();
            mStringBuilder.Length = 0;
            return encodeCSV;
        }
    }
}