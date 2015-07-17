using System.IO;
using System.Linq;
using System.Reflection;

namespace Subtitle.Provider.Addic7ed.Tests
{
    internal static class ResourceManager
    {
        public static TextReader GetInputFile(string filename)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var resources = thisAssembly.GetManifestResourceNames();
            var resource = resources.FirstOrDefault(r => r.EndsWith(filename));
            var resourceStream = thisAssembly.GetManifestResourceStream(resource);

            if (resourceStream == null)
                throw new FileNotFoundException("File not found in resource", filename);

            return new StreamReader(resourceStream);
        }
    }
}