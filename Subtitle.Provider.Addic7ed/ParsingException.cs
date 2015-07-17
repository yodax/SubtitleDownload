using System;

namespace Subtitle.Provider.Addic7ed
{
    [Serializable]
    public class ParsingException : Exception
    {
        public ParsingException(string message)
            : base(message)
        {
        }
    }
}