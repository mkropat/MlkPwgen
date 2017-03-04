using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace MlkPwgen
{
    public sealed class EmbeddedTrigramStatistics : ITrigramStatistics
    {
        static EmbeddedTrigramStatistics _instance;

        public static EmbeddedTrigramStatistics Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EmbeddedTrigramStatistics();
                return _instance;
            }
        }

        SerializableTrigramStatistics _stats;

        private EmbeddedTrigramStatistics()
        {
            _stats = LoadStats();
        }

        public bool Exists(Tuple<char, char> prefix)
        {
            return _stats.TrigramWeights.ContainsKey(prefix);
        }

        public IReadOnlyCollection<WeightedItem<Tuple<char, char>>> GetPrefixWeights()
        {
            return _stats.PrefixWeights;
        }

        public IReadOnlyCollection<WeightedItem<char>> GetTrigramWeights(Tuple<char, char> prefix)
        {
            return _stats.TrigramWeights[prefix];
        }

        SerializableTrigramStatistics LoadStats()
        {
            var asm = typeof(EmbeddedTrigramStatistics).GetTypeInfo().Assembly;
            using (var rscStream = asm.GetManifestResourceStream("MlkPwgen.TrigramStatistics.json.gz"))
            using (var zipStream = new GZipStream(rscStream, CompressionMode.Decompress))
            {
                var serializer = new DataContractJsonSerializer(typeof(SerializableTrigramStatistics));
                return (SerializableTrigramStatistics)serializer.ReadObject(zipStream);
            }
        }
    }
}
