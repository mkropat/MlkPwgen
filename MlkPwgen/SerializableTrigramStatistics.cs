using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MlkPwgen
{
    [DataContract]
    public class SerializableTrigramStatistics
    {
        [DataMember]
        public List<WeightedItem<Tuple<char, char>>> PrefixWeights { get; set; }

        [DataMember]
        public Dictionary<Tuple<char, char>, List<WeightedItem<char>>> TrigramWeights { get; set; }
    }
}
