using System;

namespace Subtitle.Provider.Addic7ed
{
    public class EpisodePageIsEmtpyException : Exception
    {
        public EpisodePageIsEmtpyException() : base("Page is empty")
        {
        }
    }
}