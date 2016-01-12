using System.Runtime.Serialization;

namespace MlkPwgen
{
    public static class WeightedItem
    {
        public static WeightedItem<T> Create<T>(uint weight, T item)
        {
            return new WeightedItem<T>
            {
                Item = item,
                Weight = weight,
            };
        }
    }

    [DataContract]
    public class WeightedItem<T>
    {
        [DataMember]
        public uint Weight { get; set; }

        [DataMember]
        public T Item { get; set; }
    }
}
