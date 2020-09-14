using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCSV
{
    public static class CSVDataHelper
    {
        public const char DoubleQuoteCharacter = '\"';
        public const char CommaCharacter = ',';
        
        public static List<string> GetCSVRows(this string csvContent, bool enableCellMultiline)
        {
            if (string.IsNullOrEmpty(csvContent)) return new List<string>();
            //Split by new line.
            if (!enableCellMultiline)
                return csvContent.StringSplit(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            
            List<string> rows = new List<string>();
            string newLine = Environment.NewLine;
            int newLineLen = newLine.Length;

            StringBuilder stringBuilder = new StringBuilder();
            bool isCellBeginning = true;
            bool cellNeedEscape = false;//If true, all characters need to read, include new line.
            bool passEvenDoubleQuotes = false;
            for (int i = 0, len = csvContent.Length; i < len; i++)
            {
                char ch = csvContent[i];
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
                        //Do not need escape or pass even double quotes character means the cell is end.
                        if (ch == newLine[0] && (!cellNeedEscape || passEvenDoubleQuotes) && i + newLineLen <= len)
                        {
                            //Judge whether it is a new line.
                            bool isNewLine = true;
                            for (int j = 1; j < newLineLen; j++)
                            {
                                if (csvContent[i + j] == newLine[j]) continue;
                                isNewLine = false;
                                break;
                            }
                            if (isNewLine)
                            {
                                //Skip empty row.
                                if(stringBuilder.Length != 0)
                                    rows.Add(stringBuilder.ToString());
                                //Clear. After .NETFramework v4.0 can replace to Clear method;
                                stringBuilder.Length = 0;
                                //Skip new line string.
                                i += newLineLen - 1;
                                isCellBeginning = true;
                                cellNeedEscape = false;
                                passEvenDoubleQuotes = false;
                                continue;
                            }
                            stringBuilder.Append(ch);
                        }
                        else
                            stringBuilder.Append(ch);
                        break;
                }
                isCellBeginning = false;
            }
            //The last line maybe does not have new line separator.
            //Skip empty row.
            if(stringBuilder.Length != 0)
                rows.Add(stringBuilder.ToString());
            //Clear. After .NETFramework v4.0 can replace to Clear method;
            stringBuilder.Length = 0;
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
                            //Clear. After .NETFramework v4.0 can replace to Clear method;
                            cellValueBuilder.Length = 0;
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
            //Clear. After .NETFramework v4.0 can replace to Clear method;
            cellValueBuilder.Length = 0;
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
            for (int i = 0, count = cellList.Count; i < count; i++)
            {
                var cell = cellList[i];
                bool cellNeedParaphrase = false;
                int cellStartCharIndex = stringBuilder.Length;
                for (int j = 0, len = cell.Length; j < len; j++)
                {
                    char ch = cell[j];
                    switch (ch)
                    {
                        case DoubleQuoteCharacter:
                            if (j == 0)
                            {
                                cellNeedParaphrase = true;
                                stringBuilder.Append(DoubleQuoteCharacter);//Add \" to beginning.
                            }
                            stringBuilder.Append(ch);
                            if (cellNeedParaphrase)
                                stringBuilder.Append(ch);// \" change to \"\"
                            else//Record escape character \" insert index.
                                doubleQuoteInsertIndices.Enqueue(stringBuilder.Length + doubleQuoteInsertIndices.Count);
                            break;
                        case CommaCharacter:
                            if (!cellNeedParaphrase)
                            {
                                cellNeedParaphrase = true;
                                //The cell has comma character, so the preceding \" needs to be changed to \"\" and add \" in the cell's beginning.
                                while (doubleQuoteInsertIndices.Count > 0)
                                    stringBuilder.Insert(doubleQuoteInsertIndices.Dequeue(), DoubleQuoteCharacter);
                                stringBuilder.Insert(cellStartCharIndex, DoubleQuoteCharacter);
                            }
                            stringBuilder.Append(ch);
                            break;
                        default:
                            stringBuilder.Append(ch);
                            break;
                    }
                }
                doubleQuoteInsertIndices.Clear();
                if(cellNeedParaphrase)
                    stringBuilder.Append(DoubleQuoteCharacter);//Add \" to the ending.
                if(i != count - 1)
                    stringBuilder.Append(CommaCharacter);//Not the last cell, then add comma separator. 
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Split string by another string pattern.
        /// </summary>
        public static List<string> StringSplit(this string input, string pattern, StringSplitOptions options = StringSplitOptions.None)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern)) return new List<string> {input};
            List<string> outputs = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            int inputLen = input.Length;
            int patternLen = pattern.Length;
            for (int i = 0; i < inputLen; i++)
            {
                char ch = input[i];
                if (ch == pattern[0] && i + patternLen <= inputLen)
                {
                    //Judge whether it is pattern.
                    bool isPattern = true;
                    for (int j = 1; j < patternLen; j++)
                    {
                        if (input[i + j] == pattern[j]) continue;
                        isPattern = false;
                        break;
                    }
                    if (isPattern)
                    {
                        //Skip empty row if need.
                        if(options == StringSplitOptions.None || stringBuilder.Length != 0)
                            outputs.Add(stringBuilder.ToString());
                        //Clear. After .NETFramework v4.0 can replace to Clear method;
                        stringBuilder.Length = 0;
                        //Skip pattern.
                        i += patternLen - 1;
                        continue;
                    }
                    stringBuilder.Append(ch);
                }
                else
                    stringBuilder.Append(ch);
            }
            //Skip empty row if need.
            if(options == StringSplitOptions.None || stringBuilder.Length != 0)
                outputs.Add(stringBuilder.ToString());
            //Clear. After .NETFramework v4.0 can replace to Clear method;
            stringBuilder.Length = 0;
            return outputs;
        }
    }
}