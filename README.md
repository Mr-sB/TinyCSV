# TinyCSV
<p align="left">
  <a href="https://github.com/Mr-sB/TinyCSV/releases"><img src="https://img.shields.io/badge/version-1.3.1-blue" alt="Version 1.3.0"></a>
</p>

Tiny CSV toolkit for .NET. Easy to read and write CSV table.

## Feature
* Easy to read and write csv table.
* Custom header lines.
* Support choose new line style.

|     Environment     | Unix | NonUnix  |
| :-----------------: | :--: | :------: |
| Environment.NewLine |  \n  |   \r\n   |
* Support cell's form:

| contain cells separators | contain double quotes | custom cells separator | multiline |
| :----------------------: | :-------------------: | :--------------------: | :-------: |
|             √            |           √           |           √            |     √     |
* You can custom cells separator. Default is comma.
* You can choose whether to support multiline cells. Default is support.

## Usage
More examples can be found in the TinyCSV.Example.
### Reading a CSV file
```c#
var csv = File.ReadAllText("sample.csv");
//Create csv reader form csv content, custom cell separator.
CSVTableReader csvTableReader = new CSVTableReader(csv, 2, ',');
```
### Writing a CSV file
```c#
//Create a empty csv writer.
CSVTableWriter csvTableWriter = new CSVTableWriter()
    //Add headers.
    .AddHeader(new CSVRecordWriter {"Data1", "Data2"})
    .AddHeader(new CSVRecordWriter {"string", "int"})
    //Add records.
    .AddRecord(new CSVRecordWriter()
        .Add("string with double quote\" and comma, and \n\\n and \r\n\\r\\n")
        .Add("1"));
//Get csv form string, custom cell separator and choose new line style.
string csv = csvTableWriter.GetEncodeTable(',', NewLineStyle.NonUnix);
//Create csv writer from csv content.
CSVTableWriter csvTableWriter2 = new CSVTableWriter(csv, 2);
```

## Requirements
TinyCSV currently targets and supports
* .NET Framework 4.0 and above.
