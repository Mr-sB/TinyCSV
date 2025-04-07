using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCSV
{
    public static class CSVDataHelper
    {
        public const char DoubleQuoteCharacter = '\"';
        public const char CommaCharacter = ',';
        
        public static readonly string[] NewLineSeparators = {NewLineHelper.UnixNewLine, NewLineHelper.NonUnixNewLine};
        public static readonly int NewLineSeparatorsLength = NewLineSeparators.Length;
        public static readonly string[] EmptyStringArray = new string[0];
        
        /// <summary>
        /// Split csv table by \n or \r\n.
        /// </summary>
        /// <param name="csvContent">CSV content.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="supportCellMultiline">If true, support multiline cell but slower, otherwise not support multiline cell but faster.</param>
        /// <param name="rowCount">Get how many rows. Negative means all rows.</param>
        /// <returns>CSV rows.</returns>
        public static string[] GetCSVRowArray(this string csvContent, char cellSeparator = CommaCharacter, bool supportCellMultiline = true, int rowCount = -1)
        {
            if (string.IsNullOrEmpty(csvContent) || rowCount == 0) return EmptyStringArray;
            //Split by \n or \r\n.
            if (!supportCellMultiline)
                return rowCount > 0
                    ? csvContent.Split(NewLineSeparators, rowCount, StringSplitOptions.RemoveEmptyEntries)
                    : csvContent.Split(NewLineSeparators, StringSplitOptions.RemoveEmptyEntries);
            return csvContent.GetCSVRowList(cellSeparator, true, rowCount).ToArray();
        }
        
        /// <summary>
        /// Split csv table by \n or \r\n.
        /// </summary>
        /// <param name="csvContent">CSV content.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="supportCellMultiline">If true, support multiline cell but slower, otherwise not support multiline cell but faster.</param>
        /// <param name="rowCount">Get how many rows. Negative means all rows.</param>
        /// <returns>CSV rows.</returns>
        public static List<string> GetCSVRowList(this string csvContent, char cellSeparator = CommaCharacter, bool supportCellMultiline = true, int rowCount = -1)
        {
            if (string.IsNullOrEmpty(csvContent) || rowCount == 0) return new List<string>();
            //Split by \n or \r\n.
            if (!supportCellMultiline)
                return new List<string>(rowCount > 0
                    ? csvContent.Split(NewLineSeparators, rowCount, StringSplitOptions.RemoveEmptyEntries)
                    : csvContent.Split(NewLineSeparators, StringSplitOptions.RemoveEmptyEntries));

            var rows = rowCount < 0 ? new List<string>() : new List<string>(rowCount);

            StringBuilder stringBuilder = new StringBuilder();
            bool isCellBeginning = true;
            bool cellNeedEscape = false;//If true, all characters need to read, include new line.
            bool passEvenDoubleQuotes = false;
            for (int csvIndex = 0, len = csvContent.Length; csvIndex < len; csvIndex++)
            {
                char ch = csvContent[csvIndex];
                if (ch == DoubleQuoteCharacter)
                {
                    //The cell start with \", then all characters need to read, include new line.
                    if (isCellBeginning)
                        cellNeedEscape = true;
                    else if (cellNeedEscape)
                        passEvenDoubleQuotes = !passEvenDoubleQuotes;
                    stringBuilder.Append(ch);
                }
                else if (ch == cellSeparator)
                {
                    //Do not need escape or pass even double quotes character means the cell is end.
                    //csv字段内的\"必定是偶数个，而需要转义的情况下\"变为\"\"，所以尾部必定有个落单的\"与起始的\"形成一对，所以包含在字段内的\,前面必定有奇数个\"
                    stringBuilder.Append(ch);
                    if (!cellNeedEscape || passEvenDoubleQuotes)
                    {
                        isCellBeginning = true;
                        cellNeedEscape = false;
                        passEvenDoubleQuotes = false;
                        continue;
                    }
                }
                else if (cellNeedEscape && !passEvenDoubleQuotes) //Need escape and pass odd double quotes character means the cell is not end.
                    stringBuilder.Append(ch);
                else
                {
                    //Judge whether it is new line.
                    bool isNewLine = false;
                    for (int separatorsIndex = 0; separatorsIndex < NewLineSeparatorsLength; separatorsIndex++)
                    {
                        string newLineSeparator = NewLineSeparators[separatorsIndex];
                        int newLineSeparatorLen = newLineSeparator.Length;
                        if (ch == newLineSeparator[0] && csvIndex + newLineSeparatorLen <= len)
                        {
                            isNewLine = true;
                            for (int separatorIndex = 1; separatorIndex < newLineSeparatorLen; separatorIndex++)
                            {
                                if (csvContent[csvIndex + separatorIndex] == newLineSeparator[separatorIndex]) continue;
                                isNewLine = false;
                                break;
                            }

                            if (isNewLine)
                            {
                                //Skip empty row.
                                if (stringBuilder.Length != 0)
                                {
                                    rows.Add(stringBuilder.ToString());
                                    if (rowCount > 0 && rows.Count >= rowCount)
                                        return rows;
                                }
                                stringBuilder.Clear();
                                //Skip new line string.
                                csvIndex += newLineSeparatorLen - 1;
                                break;
                            }
                        }
                    }
                    if (!isNewLine)
                        stringBuilder.Append(ch);
                    else
                    {
                        // Reset to new line status
                        isCellBeginning = true;
                        cellNeedEscape = false;
                        passEvenDoubleQuotes = false;
                        continue;
                    }
                }
                isCellBeginning = false;
            }
            //The last line maybe does not have new line separator.
            //Skip empty row.
            if (stringBuilder.Length != 0)
                rows.Add(stringBuilder.ToString());
            stringBuilder.Clear();
            return rows;
        }
        
        /// <summary>
        /// Decode csv row content.
        /// </summary>
        /// <param name="rowContent">CSV row content.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <param name="capacity">List capacity.</param>
        /// <returns>Cell list.</returns>
        public static List<string> GetCSVDecodeRow(this string rowContent, char cellSeparator = CommaCharacter, int capacity = 0)
        {
            List<string> cellValues = new List<string>(capacity);
            StringBuilder cellValueBuilder = new StringBuilder();
            bool isCellBeginning = true;
            bool cellNeedEscape = false;
            bool canAddEscapeDoubleQuote = false;
            foreach (var ch in rowContent)
            {
                if (ch == DoubleQuoteCharacter)
                {
                    if (isCellBeginning) //The cell start with \", then all \" need escape(change to \"\") and add \" to cell's beginning and ending.
                        cellNeedEscape = true;
                    else if(cellNeedEscape)
                    {
                        if (canAddEscapeDoubleQuote)
                            cellValueBuilder.Append(ch);
                        canAddEscapeDoubleQuote = !canAddEscapeDoubleQuote;
                    }
                    else
                        cellValueBuilder.Append(ch);
                }
                else if(ch == cellSeparator)
                {
                    //Do not need escape or can not add escape character \" means the cell is end.
                    //能添加\"的时候代表已经经过了偶数个\"（canAddEscapeDoubleQuote从false变为true代表经过了奇数次变化，加上字段起始的\",所以是偶数个\"）
                    //csv字段内的\"必定是偶数个，而需要转义的情况下\"变为\"\"，所以尾部必定有个落单的\"与起始的\"形成一对，所以包含在字段内的\,前面必定有奇数个\"
                    if (!cellNeedEscape || canAddEscapeDoubleQuote)
                    {
                        cellValues.Add(cellValueBuilder.ToString());
                        cellValueBuilder.Clear();
                        isCellBeginning = true;
                        cellNeedEscape = false;
                        canAddEscapeDoubleQuote = false;
                        continue;
                    }
                    cellValueBuilder.Append(ch);
                }
                else
                    cellValueBuilder.Append(ch);
                isCellBeginning = false;
            }
            cellValues.Add(cellValueBuilder.ToString());
            cellValueBuilder.Clear();
            return cellValues;
        }

        /// <summary>
        /// Encode cells to csv form.
        /// </summary>
        /// <param name="cellList">Cell list.</param>
        /// <param name="cellSeparator">CSV cells separator.</param>
        /// <returns>Encode row.</returns>
        public static string GetCSVEncodeRow(this List<string> cellList, char cellSeparator = CommaCharacter)
        {
            if (cellList == null || cellList.Count == 0) return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            Queue<int> doubleQuoteInsertIndices = new Queue<int>();
            for (int cellsIndex = 0, count = cellList.Count; cellsIndex < count; cellsIndex++)
            {
                var cell = cellList[cellsIndex];
                bool cellNeedEscape = false;
                int cellStartCharIndex = stringBuilder.Length;
                for (int cellIndex = 0, len = cell != null ? cell.Length : 0; cellIndex < len; cellIndex++)
                {
                    char ch = cell[cellIndex];
                    if (ch == DoubleQuoteCharacter)
                    {
                        if (cellIndex == 0)
                        {
                            cellNeedEscape = true;
                            stringBuilder.Append(DoubleQuoteCharacter);//Add \" to beginning.
                        }
                        stringBuilder.Append(ch);
                        if (cellNeedEscape)
                            stringBuilder.Append(ch);// \" change to \"\"
                        else//Record escape character \" insert index.
                            doubleQuoteInsertIndices.Enqueue(stringBuilder.Length + doubleQuoteInsertIndices.Count);
                    }
                    else if (ch == cellSeparator)
                    {
                        if (!cellNeedEscape)
                        {
                            //The cell has comma character, so the preceding \" needs to be changed to \"\" and add \" in the cell's beginning.
                            cellNeedEscape = true;
                            while (doubleQuoteInsertIndices.Count > 0)
                                stringBuilder.Insert(doubleQuoteInsertIndices.Dequeue(), DoubleQuoteCharacter);
                            stringBuilder.Insert(cellStartCharIndex, DoubleQuoteCharacter);
                        }
                        stringBuilder.Append(ch);
                    }
                    else if (cellNeedEscape)
                        stringBuilder.Append(ch);
                    else
                    {
                        //Judge whether it is new line.
                        bool isNewLine = false;
                        for (int separatorsIndex = 0; separatorsIndex < NewLineSeparatorsLength; separatorsIndex++)
                        {
                            string newLineSeparator = NewLineSeparators[separatorsIndex];
                            int newLineSeparatorLen = newLineSeparator.Length;
                            if (ch == newLineSeparator[0] && cellIndex + newLineSeparatorLen <= len)
                            {
                                isNewLine = true;
                                for (int separatorIndex = 1; separatorIndex < newLineSeparatorLen; separatorIndex++)
                                {
                                    if (cell[cellIndex + separatorIndex] == newLineSeparator[separatorIndex]) continue;
                                    isNewLine = false;
                                    break;
                                }

                                if (isNewLine)
                                {
                                    //The cell has new line separator, so the preceding \" needs to be changed to \"\" and add \" in the cell's beginning.
                                    cellNeedEscape = true;
                                    while (doubleQuoteInsertIndices.Count > 0)
                                        stringBuilder.Insert(doubleQuoteInsertIndices.Dequeue(), DoubleQuoteCharacter);
                                    stringBuilder.Insert(cellStartCharIndex, DoubleQuoteCharacter);
                                    //Add
                                    for (int separatorIndex = 0; separatorIndex < newLineSeparatorLen; separatorIndex++)
                                        stringBuilder.Append(cell[cellIndex + separatorIndex]);
                                    //Skip new line string.
                                    cellIndex += newLineSeparatorLen - 1;
                                    break;
                                }
                            }
                        }
                        if (!isNewLine)
                            stringBuilder.Append(ch);
                    }
                }
                doubleQuoteInsertIndices.Clear();
                if(cellNeedEscape)
                    stringBuilder.Append(DoubleQuoteCharacter);//Add \" to the ending.
                if(cellsIndex != count - 1)
                    stringBuilder.Append(cellSeparator);//Not the last cell, then add cell separator. 
            }
            return stringBuilder.ToString();
        }

        public static void Clear(this StringBuilder stringBuilder)
        {
            try
            {
                //Clear. After .NETFramework v4.0 can replace to Clear method;
                stringBuilder.Length = 0;
            }
            catch (Exception e)
            {
                throw new CSVException(e);
            }
        }
        
        /// <summary>
        /// Split string by string array separators.
        /// </summary>
        public static List<string> StringSplit(this string input, string[] separators, StringSplitOptions options)
        {
            if (string.IsNullOrEmpty(input) || separators == null || separators.Length == 0) return new List<string> {input};
            List<string> outputs = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            int inputLen = input.Length;
            int separatorsLen = separators.Length;
            for (int inputIndex = 0; inputIndex < inputLen; inputIndex++)
            {
                char ch = input[inputIndex];
                //Judge whether it is separator.
                bool isSeparator = false;
                for (int separatorsIndex = 0; separatorsIndex < separatorsLen; separatorsIndex++)
                {
                    string separator = separators[separatorsIndex];
                    int separatorLen = separator.Length;
                    if (ch == separator[0] && inputIndex + separatorLen <= inputLen)
                    {
                        isSeparator = true;
                        for (int separatorIndex = 1; separatorIndex < separatorLen; separatorIndex++)
                        {
                            if (input[inputIndex + separatorIndex] == separator[separatorIndex]) continue;
                            isSeparator = false;
                            break;
                        }
                        if (isSeparator)
                        {
                            //Skip empty entry if need.
                            if(options == StringSplitOptions.None || stringBuilder.Length != 0)
                                outputs.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            //Skip separator.
                            inputIndex += separatorLen - 1;
                            break;
                        }
                    }
                }
                if(!isSeparator)
                    stringBuilder.Append(ch);
            }
            //Skip empty entry if need.
            if(options == StringSplitOptions.None || stringBuilder.Length != 0)
                outputs.Add(stringBuilder.ToString());
            stringBuilder.Clear();
            return outputs;
        }
    }
}