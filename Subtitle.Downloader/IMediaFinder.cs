using System.Collections.Generic;

namespace Subtitle.Downloader
{
    public interface IMediaFinder
    {
        IEnumerable<Media> LookFor(string showname, string episode);
    }
}