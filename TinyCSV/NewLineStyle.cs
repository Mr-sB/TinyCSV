namespace TinyCSV
{
    public enum NewLineStyle
    {
        /// <summary>
        /// \r\n for non-Unix platforms, or \n for Unix platforms.
        /// </summary>
        Environment,
        /// <summary>
        /// \n
        /// </summary>
        Unix,
        /// <summary>
        /// \r\n
        /// </summary>
        NonUnix
    }
}