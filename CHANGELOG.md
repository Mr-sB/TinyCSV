# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- `CSVTableReader` and `CSVTableWriter` constructors add `readRecordCount` parameter.
- `CSVDataHelper.GetCSVRowArray` and `CSVDataHelper.GetCSVRowList` add `rowCount` parameter.
- `CSVDataHelper` add `HeaderInfoRowCount` constant.

## [1.2.0] - 2020-09-18
### Added
- Custom cells separator support.
- Choose new line style support, `Environment` or `Unix` or `Non-Unix`.
- `CSVDataHelper.GetCSVRowArray` method.
- `CSVRecordReader.GetDecodeRow`, `CSVRecordWriter.GetEncodeRow`, 
`CSVTableReader.GetDecodeTable` and `CSVTableWriter.GetEncodeTable` methods.

### Changed
- Use `string.Split` to replace `CSVDataHelper.StringSplit`.
- Rename `CSVDataHelper.GetCSVRows` to `CSVDataHelper.GetCSVRowList`.
- Writers' Add and Remove methods will return themselves.
- `CSVTableWriter.AddRecord` method will set `CSVRecordWriter.CellSeparator`.

### Security
- Change `CSVDataHelper.NewLineSeparators` and `CSVDataHelper.NewLineSeparatorsLength` to readonly.

## [1.1.0] - 2020-09-15
### Added
- Multiline cells support.

### Changed
- Split csv rows by \n or \r\n.


## [1.0.1] - 2020-09-14
### Added
- Readers add ToString methods. Writers add Remove methods.
- Example project.
- CSVException class.

## [1.0.0] - 2020-09-14
### Added
- Read and write csv form feature. Support table cells contain commas and double quotes.

[Unreleased]: https://github.com/Mr-sB/TinyCSV/compare/v1.2.0...HEAD
[1.2.0]: https://github.com/Mr-sB/TinyCSV/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Mr-sB/TinyCSV/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/Mr-sB/TinyCSV/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Mr-sB/TinyCSV/releases/tag/v1.0.0
