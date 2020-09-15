using System;

namespace TinyCSV.Example
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Write.
            //Create a empty csv writer.
            CSVTableWriter csvTableWriter = new CSVTableWriter();
            //Add headers.
            csvTableWriter.AddHeader("Data1");
            csvTableWriter.AddHeader("Data2");
            csvTableWriter.AddHeader("Data3");
            csvTableWriter.AddHeader("Data4");
            csvTableWriter.AddHeader("Data5");
            csvTableWriter.AddHeader("Data6");
            //Add descriptions.
            csvTableWriter.AddDescription("int");
            csvTableWriter.AddDescription("Color");
            csvTableWriter.AddDescription("int");
            csvTableWriter.AddDescription("string");
            csvTableWriter.AddDescription("string");
            csvTableWriter.AddDescription("int[]");
            //Add records.
            var record1 = new CSVRecordWriter();
            record1.AddCell("1");
            record1.AddCell("#aaaaaa");
            record1.AddCell("2");
            record1.AddCell("normal string");
            record1.AddCell("\"string with double quote");
            record1.AddCell("1;2;3|4;5;6");
            csvTableWriter.AddRecord(record1);
            var record2 = new CSVRecordWriter();
            record2.AddCell("2");
            record2.AddCell("#bbbbbb");
            record2.AddCell("4");
            record2.AddCell("string with, comma");
            record2.AddCell("string\" with\", comma and \"double quote");
            record2.AddCell("7;8;9|10;11;12|7;7;7");
            csvTableWriter.AddRecord(record2);
            var record3 = new CSVRecordWriter();
            record3.AddCell("3");
            record3.AddCell("#ccc");
            record3.AddCell("5");
            record3.AddCell("string with \n \\n and \r\n \\r\\n");
            record3.AddCell("string\" with \n \\n and \", comma and \"double quote and \r\n \\r\\n");
            record3.AddCell("7;8;9|10;11;12|7;7;7");
            csvTableWriter.AddRecord(record3);
            //Get csv form string.
            string csv = csvTableWriter.ToString();
            Console.WriteLine("Write to csv form:\n" + csv);
            
            //Read
            //Create csv reader form csv content.
            CSVTableReader csvTableReader = new CSVTableReader(csv);
            Console.WriteLine("Read from csv form:\n" + csvTableReader);
            
            //Write
            //Create csv writer from csv reader.
            CSVTableWriter csvTableWriter2 = new CSVTableWriter(csvTableReader);
            csvTableWriter2.RemoveHeader(5);
            csvTableWriter2.RemoveDescription(5);
            csvTableWriter2.RemoveRecord(0);
            foreach (var record in csvTableWriter2.Records)
                record.RemoveCell(5);
            var record100 = new CSVRecordWriter();
            record100.AddCell("100");
            record100.AddCell("#cccccc");
            record100.AddCell("5");
            record100.AddCell("string with, comma");
            record100.AddCell("\"string with\", comma and \"double quote");
            csvTableWriter2.AddRecord(record100);
            Console.WriteLine("Write to csv form:\n" + csvTableWriter2);

            Console.ReadKey();
        }
    }
}