namespace Subtitle.Provider.Addic7ed
{
    using System;

    [Serializable]
    public class ParsingException : Exception
    {
        public ParsingException(string message)
            : base(message)
        {
        }
    }
}