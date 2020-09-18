using System;

namespace TinyCSV
{
    public static class NewLineHelper
    {
        public static readonly string EnvironmentNewLine = Environment.NewLine;
        public const string UnixNewLine = "\n";
        public const string NonUnixNewLine = "\r\n";

        public static string GetNewLine(this NewLineStyle newLineStyle)
        {
            switch (newLineStyle)
            {
                case NewLineStyle.Environment:
                    return EnvironmentNewLine;
                case NewLineStyle.Unix:
                    return UnixNewLine;
                case NewLineStyle.NonUnix:
                    return NonUnixNewLine;
                default:
                    return EnvironmentNewLine;
            }
        }
    }
}