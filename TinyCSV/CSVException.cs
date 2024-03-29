using System;

namespace TinyCSV
{
    public class CSVException : Exception
    {
        public CSVException()
        {
        }
        
        public CSVException(string message) : base(message)
        {
        }
        
        public CSVException(Exception innerException) : base(innerException.Message, innerException)
        {
        }

        public CSVException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}