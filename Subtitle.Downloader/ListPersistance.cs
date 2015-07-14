namespace Subtitle.Downloader
{
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Xml.Serialization;

    public static class ListPersistance
    {
        public static void Store<T>(this List<T> listToPersist, string storeName, IFileSystem fileSystem)
        {
            var ser = new XmlSerializer(typeof (List<T>));
            using (var streamWriter = fileSystem.File.CreateText(GenerateStorePath(storeName)))
            {
                ser.Serialize(streamWriter, listToPersist);
            }
        }

        private static string GenerateStorePath(string storeName)
        {
            return storeName + ".xml";
        }

        public static List<T> Load<T>(this List<T> listToLoad, string storeName, IFileSystem fileSystem)
        {
            if (!fileSystem.File.Exists(GenerateStorePath(storeName))) return listToLoad;

            var ser = new XmlSerializer(typeof (List<T>));
            using (var streamReader = fileSystem.File.OpenRead(GenerateStorePath(storeName)))
            {
                var list = (List<T>) ser.Deserialize(streamReader);
                listToLoad.Clear();
                listToLoad.AddRange(list);
            }

            return listToLoad;
        }
    }
}