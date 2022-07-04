using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TinyCSV
{
    public class CSVRecordReader : IEnumerable<string>
    {
        public readonly string[] Cells;
        public readonly int Column;
        public char CellSeparator;
        private StringBuilder mStringBuilder;

        /// <summary>
        /// Create a CSVRecordReader.
        /// </summary>
        /// <param name="cellSeparator">CSV cells separator.</param>
        public CSVRecordReader(char cellSeparator = CSVDataHelper.CommaCharacter)
        {
            CellSeparator = cellSeparator;
            Cells = CSVDataHelper.EmptyStringArray;
            Column = 0;
        }
        
        /// <summary>
        /// Create a CSVRecordReader.
        /// </summary>
        /// <param name="record">CSV row data.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="capacity">List capacity.</param>
        public CSVRecordReader(string record, char cellSeparator = CSVDataHelper.CommaCharacter, int capacity = 0)
        {
            CellSeparator = cellSeparator;
            Cells = record != null ? record.GetCSVDecodeRow(cellSeparator, capacity).ToArray() : CSVDataHelper.EmptyStringArray;
            Column = Cells.Length;
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
            for (int i = 0, len = Cells.Length; i < len; i++)
            {
                mStringBuilder.Append(Cells[i]);
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
        
        public IEnumerator<string> GetEnumerator()
        {
            return new Enumerator(Cells);
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public struct Enumerator : IEnumerator<string>
        {
            private readonly string[] _array;
            private int _index;
            private string _current;

            public Enumerator(string[] array)
            {
                _array = array;
                _index = 0;
                _current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_index < _array.Length)
                {
                    _current = _array[_index];
                    _index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                _index = _array.Length + 1;
                _current = default;
                return false;
            }

            public string Current => _current;

            object IEnumerator.Current => _current;

            void IEnumerator.Reset()
            {
                _index = 0;
                _current = default;
            }
        }
    }
}