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
                .AddHeader(new CSVRecordWriter {"Data1", "Data2", "Data3", "Data4", "Data5", "Data6"})
                .AddHeader(new CSVRecordWriter {"int", "Color", "int", "string", "string", "int[]"})
                //Add records.
                .AddRecord(new CSVRecordWriter {"1", "#aaaaaa", "2", "normal string", "\"string with double quote", "1;2;3|4;5;6"})
                .AddRecord(new CSVRecordWriter {"2", "#bbbbbb", "4", "string with, comma", "string\" with\", comma and \"double quote", "7;8;9|10;11;12|7;7;7"})
                .AddRecord(new CSVRecordWriter{ "3", "#ccc", "5", "string with \n \\n and \r\n \\r\\n", "string\" with \n \\n and \", comma and \"double quote and \r\n \\r\\n", "7;8;9|10;11;12|7;7;7"});
            //Get csv form string.
            string csv1 = csvTableWriter.GetEncodeTable();
            Console.WriteLine("Write to csv form:\n" + csv1);
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "CSVExample1.csv")))
                streamWriter.Write(csv1);
            
            //Read
            //Create csv reader form csv content.
            CSVTableReader csvTableReader = new CSVTableReader(csv1, 2);
            Console.WriteLine("Read from csv form:\n" + csvTableReader);
            
            //Write
            //Create csv writer from csv reader, custom cell separator.
            CSVTableWriter csvTableWriter2 = new CSVTableWriter(csvTableReader, ';');
            //Remove some row and column.
            csvTableWriter2.RemoveRecord(0);
            foreach (var header in csvTableWriter2.Headers)
                header.RemoveAt(5);
            foreach (var record in csvTableWriter2.Records)
                record.RemoveAt(5);
            //Add new record. AddRecord method will set CSVRecordWriter.CellSeparator.
            csvTableWriter2.AddRecord(new CSVRecordWriter()
                .Add("100")
                .Add("#cccccc")
                .Add("5")
                .Add("string with; semicolon")
                .Add("string\" with\"; semicolon and \"double quote"));
            //Get csv form string, choose new line style.
            string csv2 = csvTableWriter2.GetEncodeTable(NewLineStyle.NonUnix);
            Console.WriteLine("Write to csv form:\n" + csv2);
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "CSVExample2.csv")))
                streamWriter.Write(csv2);

            Console.ReadKey();
        }
    }
}