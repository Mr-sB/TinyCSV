# TinyCSV
Tiny CSV toolkit for .NET. Easy to read and write CSV table.

## Feature
* Easy to read and write csv table.
* First line is headers.
* Second line is descriptions.
* Support cell's form:

| commons | double quotes | multiline |
| :-----: | :-----------: | :-------: |
|    √    |       √       |     ×     |

## Usage
More examples can be found in the TinyCSV.Example.
### Reading a CSV file
```c#
var csv = File.ReadAllText("sample.csv");
//Create csv reader form csv content.
CSVTableReader csvTableReader = new CSVTableReader(csv);
```
### Writing a CSV file
```c#
//Create a empty csv writer.
CSVTableWriter csvTableWriter = new CSVTableWriter();
//Add headers.
csvTableWriter.AddHeader("Data1");
//Add descriptions.
csvTableWriter.AddDescription("string");
//Add records.
var record1 = new CSVRecordWriter();
record1.AddCell("\"string with double quote");
csvTableWriter.AddRecord(record1);
//Get csv form string.
string csv = csvTableWriter.ToString();

//Create csv writer from csv content.
CSVTableWriter csvTableWriter2 = new CSVTableWriter(csv);
```

## Requirements
TinyCSV currently targets and supports
* .NET Framework 3.5 and above.
