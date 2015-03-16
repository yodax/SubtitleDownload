namespace Subtitle.Common
{
    public interface IDownload
    {
        string From(string url, string referer = null);
    }
}