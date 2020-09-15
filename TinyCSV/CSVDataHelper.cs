using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCSV
{
    public static class CSVDataHelper
    {
        public const char DoubleQuoteCharacter = '\"';
        public const char CommaCharacter = ',';
        public static string[] NewLineSeparators = {"\n", "\r\n"};
        public static int NewLineSeparatorsLength = NewLineSeparators.Length;
        
        /// <summary>
        /// Split csv table by \n or \r\n.
        /// </summary>
        /// <param name="csvContent">CSV content.</param>
        /// <param name="supportCellMultiline">If true, support multiline cell but slower, otherwise not support multiline cell but faster.</param>
        /// <returns>CSV rows.</returns>
        public static List<string> GetCSVRows(this string csvContent, bool supportCellMultiline)
        {
            if (string.IsNullOrEmpty(csvContent)) return new List<string>();
            //Split by \n or \r\n.
            if (!supportCellMultiline)
                return csvContent.StringSplit(NewLineSeparators, StringSplitOptions.RemoveEmptyEntries);
            
            List<string> rows = new List<string>();

            StringBuilder stringBuilder = new StringBuilder();
            bool isCellBeginning = true;
            bool cellNeedEscape = false;//If true, all characters need to read, include new line.
            bool passEvenDoubleQuotes = false;
            for (int csvIndex = 0, len = csvContent.Length; csvIndex < len; csvIndex++)
            {
                char ch = csvContent[csvIndex];
                switch (ch)
                {
                    case DoubleQuoteCharacter:
                        //The cell start with \", then all characters need to read, include new line.
                        if (isCellBeginning)
                            cellNeedEscape = true;
                        else if (cellNeedEscape)
                            passEvenDoubleQuotes = !passEvenDoubleQuotes;
                        stringBuilder.Append(ch);
                        break;
                    case CommaCharacter:
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
                        break;
                    default:
                        //Need escape and pass odd double quotes character means the cell is not end.
                        if (cellNeedEscape && !passEvenDoubleQuotes)
                            stringBuilder.Append(ch);
                        else
                        {
                            //Judge whether it is new line.
                            bool isNewLine = false;
                            for (int separatorsIndex = 0; separatorsIndex < NewLineSeparatorsLength; separatorsIndex++)
                            {
                                string separator = NewLineSeparators[separatorsIndex];
                                int separatorLen = separator.Length;
                                if (ch == separator[0] && csvIndex + separatorLen <= len)
                                {
                                    isNewLine = true;
                                    for (int separatorIndex = 1; separatorIndex < separatorLen; separatorIndex++)
                                    {
                                        if (csvContent[csvIndex + separatorIndex] == separator[separatorIndex]) continue;
                                        isNewLine = false;
                                        break;
                                    }
                                    if (isNewLine)
                                    {
                                        //Skip empty row.
                                        if(stringBuilder.Length != 0)
                                            rows.Add(stringBuilder.ToString());
                                        stringBuilder.Clear();
                                        //Skip new line string.
                                        csvIndex += separatorLen - 1;
                                        isCellBeginning = true;
                                        cellNeedEscape = false;
                                        passEvenDoubleQuotes = false;
                                        break;
                                    }
                                }
                            }
                            if(!isNewLine)
                                stringBuilder.Append(ch);
                        }
                        break;
                }
                isCellBeginning = false;
            }
            //The last line maybe does not have new line separator.
            //Skip empty row.
            if(stringBuilder.Length != 0)
                rows.Add(stringBuilder.ToString());
            stringBuilder.Clear();
            return rows;
        }
        
        /// <summary>
        /// Decode csv row content.
        /// </summary>
        public static List<string> GetCSVDecodeRow(this string rowContent, int capacity = 0)
        {
            List<string> cellValues = new List<string>(capacity);
            StringBuilder cellValueBuilder = new StringBuilder();
            bool isCellBeginning = true;
            bool cellNeedEscape = false;
            bool canAddEscapeDoubleQuote = false;
            foreach (var ch in rowContent)
            {
                switch (ch)
                {
                    case DoubleQuoteCharacter:
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
                        break;
                    case CommaCharacter:
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
                        break;
                    default:
                        cellValueBuilder.Append(ch);
                        break;
                }
                isCellBeginning = false;
            }
            cellValues.Add(cellValueBuilder.ToString());
            cellValueBuilder.Clear();
            return cellValues;
        }

        /// <summary>
        /// Encode cells to csv form.
        /// </summary>
        public static string GetCSVEncodeRow(this List<string> cellList)
        {
            if (cellList == null || cellList.Count == 0) return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            Queue<int> doubleQuoteInsertIndices = new Queue<int>();
            for (int cellsIndex = 0, count = cellList.Count; cellsIndex < count; cellsIndex++)
            {
                var cell = cellList[cellsIndex];
                bool cellNeedEscape = false;
                int cellStartCharIndex = stringBuilder.Length;
                for (int cellIndex = 0, len = cell.Length; cellIndex < len; cellIndex++)
                {
                    char ch = cell[cellIndex];
                    switch (ch)
                    {
                        case DoubleQuoteCharacter:
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
                            break;
                        case CommaCharacter:
                            if (!cellNeedEscape)
                            {
                                //The cell has comma character, so the preceding \" needs to be changed to \"\" and add \" in the cell's beginning.
                                cellNeedEscape = true;
                                while (doubleQuoteInsertIndices.Count > 0)
                                    stringBuilder.Insert(doubleQuoteInsertIndices.Dequeue(), DoubleQuoteCharacter);
                                stringBuilder.Insert(cellStartCharIndex, DoubleQuoteCharacter);
                            }
                            stringBuilder.Append(ch);
                            break;
                        default:
                            if (cellNeedEscape)
                                stringBuilder.Append(ch);
                            else
                            {
                                //Judge whether it is new line.
                                bool isNewLine = false;
                                for (int separatorsIndex = 0; separatorsIndex < NewLineSeparatorsLength; separatorsIndex++)
                                {
                                    string separator = NewLineSeparators[separatorsIndex];
                                    int separatorLen = separator.Length;
                                    if (ch == separator[0] && cellIndex + separatorLen <= len)
                                    {
                                        isNewLine = true;
                                        for (int separatorIndex = 1; separatorIndex < separatorLen; separatorIndex++)
                                        {
                                            if (cell[cellIndex + separatorIndex] == separator[separatorIndex]) continue;
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
                                            for (int separatorIndex = 0; separatorIndex < separatorLen; separatorIndex++)
                                                stringBuilder.Append(cell[cellIndex + separatorIndex]);
                                            //Skip new line string.
                                            cellIndex += separatorLen - 1;
                                            break;
                                        }
                                    }
                                }
                                if(!isNewLine)
                                    stringBuilder.Append(ch);
                            }
                            break;
                    }
                }
                doubleQuoteInsertIndices.Clear();
                if(cellNeedEscape)
                    stringBuilder.Append(DoubleQuoteCharacter);//Add \" to the ending.
                if(cellsIndex != count - 1)
                    stringBuilder.Append(CommaCharacter);//Not the last cell, then add comma separator. 
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
                throw new CSVException(e.Message, e);
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