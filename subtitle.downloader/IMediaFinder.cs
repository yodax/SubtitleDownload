namespace Subtitle.Downloader
{
    using System.Collections.Generic;

    public interface IMediaFinder
    {
        IEnumerable<Media> LookFor(string showname, string episode);
    }
}