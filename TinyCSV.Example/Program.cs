using System;
using System.IO;

namespace TinyCSV.Example
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Write.
            //Create a empty csv writer.
            CSVTableWriter csvTableWriter = new CSVTableWriter()
                //Add headers.
                .AddHeader("Data1")
                .AddHeader("Data2")
                .AddHeader("Data3")
                .AddHeader("Data4")
                .AddHeader("Data5")
                .AddHeader("Data6")
                //Add descriptions.
                .AddDescription("int")
                .AddDescription("Color")
                .AddDescription("int")
                .AddDescription("string")
                .AddDescription("string")
                .AddDescription("int[]")
                //Add records.
                .AddRecord(new CSVRecordWriter()
                    .AddCell("1")
                    .AddCell("#aaaaaa")
                    .AddCell("2")
                    .AddCell("normal string")
                    .AddCell("\"string with double quote")
                    .AddCell("1;2;3|4;5;6")
                )
                .AddRecord(new CSVRecordWriter()
                    .AddCell("2")
                    .AddCell("#bbbbbb")
                    .AddCell("4")
                    .AddCell("string with, comma")
                    .AddCell("string\" with\", comma and \"double quote")
                    .AddCell("7;8;9|10;11;12|7;7;7"))
                .AddRecord(new CSVRecordWriter()
                    .AddCell("3")
                    .AddCell("#ccc")
                    .AddCell("5")
                    .AddCell("string with \n \\n and \r\n \\r\\n")
                    .AddCell("string\" with \n \\n and \", comma and \"double quote and \r\n \\r\\n")
                    .AddCell("7;8;9|10;11;12|7;7;7"));
            //Get csv form string.
            string csv1 = csvTableWriter.GetEncodeTable();
            Console.WriteLine("Write to csv form:\n" + csv1);
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "CSVExample1.csv")))
                streamWriter.Write(csv1);
            
            //Read
            //Create csv reader form csv content.
            CSVTableReader csvTableReader = new CSVTableReader(csv1);
            Console.WriteLine("Read from csv form:\n" + csvTableReader);
            
            //Write
            //Create csv writer from csv reader, custom cell separator.
            char cellSeparator = ';';
            CSVTableWriter csvTableWriter2 = new CSVTableWriter(csvTableReader, cellSeparator)
                //Remove some row and column.
                .RemoveHeader(5)
                .RemoveDescription(5)
                .RemoveRecord(0);
            foreach (var record in csvTableWriter2.Records)
                record.RemoveCell(5);
            //Add new record.
            csvTableWriter2.AddRecord(new CSVRecordWriter(cellSeparator)
                .AddCell("100")
                .AddCell("#cccccc")
                .AddCell("5")
                .AddCell("string with; semicolon")
                .AddCell("string\" with\"; semicolon and \"double quote"));
            //Get csv form string, choose new line style.
            string csv2 = csvTableWriter2.GetEncodeTable(NewLineStyle.NonUnix);
            Console.WriteLine("Write to csv form:\n" + csv2);
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "CSVExample2.csv")))
                streamWriter.Write(csv2);

            Console.ReadKey();
        }
    }
}