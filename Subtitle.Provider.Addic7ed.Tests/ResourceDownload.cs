namespace Subtitle.Provider.Addic7ed.Tests
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common;

    public class ResourceDownload : IDownload
    {
        public string From(string url, string referer = null)
        {
            var patchedUrl = url;

            if (patchedUrl.StartsWith("http://www.addic7ed.com/"))
                patchedUrl = patchedUrl.Substring("http://www.addic7ed.com/".Length);

            var thisAssembly = Assembly.GetExecutingAssembly();
            var resources = thisAssembly.GetManifestResourceNames();
            var resource = resources.FirstOrDefault(r => r.EndsWith(patchedUrl));
            var resourceStream = thisAssembly.GetManifestResourceStream(resource);

            if (resourceStream == null)
                throw new FileNotFoundException("File not found in resource", patchedUrl);

            var streamReader = new StreamReader(resourceStream);
            var content = streamReader.ReadToEnd();
            streamReader.Close();
            return content;
        }
    }
}