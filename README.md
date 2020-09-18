# TinyCSV
<p align="left">
  <a href="https://github.com/Mr-sB/TinyCSV/releases"><img src="https://img.shields.io/badge/version-1.1.0-blue" alt="Version 1.1.0"></a>
</p>

Tiny CSV toolkit for .NET. Easy to read and write CSV table.

## Feature
* Easy to read and write csv table.
* First line is headers.
* Second line is descriptions.
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
CSVTableReader csvTableReader = new CSVTableReader(csv, ',');
```
### Writing a CSV file
```c#
//Create a empty csv writer.
CSVTableWriter csvTableWriter = new CSVTableWriter()
    //Add headers.
    .AddHeader("Data1")
    //Add descriptions.
    .AddDescription("string")
    //Add records.
    .AddRecord(new CSVRecordWriter()
        .AddCell("string with double quote\" and comma, and \n\\n and \r\n\\r\\n"));
//Get csv form string, custom cell separator and choose new line style.
string csv = csvTableWriter.GetEncodeTable(',', NewLineStyle.NonUnix);
//Create csv writer from csv content.
CSVTableWriter csvTableWriter2 = new CSVTableWriter(csv);
```

## Requirements
TinyCSV currently targets and supports
* .NET Framework 3.5 and above.
