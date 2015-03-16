namespace Subtitle.Provider.Addic7ed
{
    using System;

    public class EpisodePageIsEmtpyException : Exception
    {
        public EpisodePageIsEmtpyException() : base ("Page is empty")
        {
            
        }
    }
}