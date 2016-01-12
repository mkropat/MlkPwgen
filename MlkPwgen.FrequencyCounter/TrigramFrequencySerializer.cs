using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;

namespace MlkPwgen
{
    public static class TrigramFrequencySerializer
    {
        public static void SerializeToZip(SerializableTrigramStatistics stats, string filePath)
        {
            using (var fileStream = File.Create(filePath))
            using (var zipStream = new GZipStream(fileStream, CompressionLevel.Optimal))
            {
                Serialize(stats, zipStream);
            }
        }

        public static void Serialize(SerializableTrigramStatistics stats, Stream output)
        {
            var serializer = new DataContractJsonSerializer(typeof(SerializableTrigramStatistics));
            serializer.WriteObject(output, stats);
        }
    }
}
