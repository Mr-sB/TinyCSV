# TinyCSV
<p align="left">
  <a href="https://github.com/Mr-sB/TinyCSV/releases"><img src="https://img.shields.io/badge/version-1.1.0-blue" alt="Version 1.1.0"></a>
</p>

Tiny CSV toolkit for .NET. Easy to read and write CSV table.

## Feature
* Easy to read and write csv table.
* First line is headers.
* Second line is descriptions.
* Support choose new line style, `Environment` or `Unix` or `Non-Unix`.
* Support cell's form:

| commas  | double quotes | custom cells separator | multiline |
| :-----: | :-----------: | :--------------------: | :-------: |
|    √    |       √       |           √            |     √     |
* You can custom separators. Default is comma.
* You can choose whether to support multiline cells. Default is support.

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
record1.AddCell("string with double quote\" and comma, and \n\\n and \r\n\\r\\n");
csvTableWriter.AddRecord(record1);
//Get csv form string.
string csv = csvTableWriter.ToString();

//Create csv writer from csv content.
CSVTableWriter csvTableWriter2 = new CSVTableWriter(csv);
```

## Requirements
TinyCSV currently targets and supports
* .NET Framework 3.5 and above.
