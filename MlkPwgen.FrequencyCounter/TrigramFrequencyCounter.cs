using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MlkPwgen
{
    public static class TrigramFrequencyCounter
    {
        public static void Count(string wordlistPath, string outputPath)
        {
            var stats = Count(File.ReadLines(wordlistPath));
            TrigramFrequencySerializer.SerializeToZip(stats, outputPath);
        }

        public static SerializableTrigramStatistics Count(IEnumerable<string> wordlist)
        {
            var count = GetTrigramCount(wordlist);

            var prefixWeights = count
                .GroupBy(kv => Tuple.Create(kv.Key.Item1, kv.Key.Item2), kv => kv.Value)
                .Select(g => WeightedItem.Create(Sum(g), g.Key))
                .OrderBy(i => i.Item.Item1).ThenBy(i => i.Item.Item2);

            return new SerializableTrigramStatistics
            {
                PrefixWeights = GetCumulativeWeightedList(prefixWeights),

                TrigramWeights = count
                    .GroupBy(
                        kv => Tuple.Create(kv.Key.Item1, kv.Key.Item2),
                        kv => Tuple.Create(kv.Key.Item3, kv.Value))
                    .ToDictionary(
                        g => g.Key,
                        g => GetCumulativeWeightedList(
                            g.Select(t => WeightedItem.Create(t.Item2, t.Item1)).OrderBy(i => i.Item))),
            };
        }

        static List<WeightedItem<T>> GetCumulativeWeightedList<T>(IEnumerable<WeightedItem<T>> items)
        {
            var result = new List<WeightedItem<T>>();

            var relevantItems = items
                .Where(i => i.Weight > 0)
                .ToArray();

            if (relevantItems.Length > 0)
            {
                var total = Sum(relevantItems.Select(i => i.Weight));

                uint threshold = 0;
                foreach (var i in relevantItems.Take(relevantItems.Length - 1))
                {
                    threshold += Scale(i.Weight, total);
                    result.Add(WeightedItem.Create(threshold, i.Item));
                }

                var lastItem = relevantItems[relevantItems.Length - 1].Item;
                result.Add(WeightedItem.Create(uint.MaxValue, lastItem));
            }

            return result;
        }

        static uint Sum(IEnumerable<uint> nums)
        {
            uint sum = 0;
            foreach (var n in nums)
                sum += n;
            return sum;
        }

        static uint Scale(uint val, uint max)
        {
            return (uint)Math.Round((double)val / max * uint.MaxValue);
        }

        static IDictionary<Tuple<char, char, char>, uint> GetTrigramCount(IEnumerable<string> wordlist)
        {
            var counts = new Dictionary<Tuple<char, char, char>, uint>();

            var allLetters = new Regex("^[A-Za-z]*$");
            var modifiedWordlist = wordlist
                .Where(w => allLetters.IsMatch(w))
                .Select(w => w.ToLowerInvariant() + '$');

            foreach (var word in modifiedWordlist)
            {
                var threewise = word.Zip(word.Skip(1), Tuple.Create)
                    .Zip(word.Skip(2), (t, c) => Tuple.Create(t.Item1, t.Item2, c));

                foreach (var t in threewise)
                {
                    uint count;
                    counts.TryGetValue(t, out count);
                    counts[t] = count + 1;
                }
            }

            return counts;
        }
    }
}
