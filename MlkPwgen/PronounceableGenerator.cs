using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MlkPwgen
{
    /// <summary>
    /// Random pronounceable password generator
    /// </summary>
    public static class PronounceableGenerator
    {
        public static string Generate(int length=10, IEnumerable<string> requiredSets=null, Func<string, bool> predicate=null)
        {
            if (requiredSets == null)
                requiredSets = Enumerable.Empty<string>();

            if (predicate == null)
                predicate = _ => true;

            var asSets = requiredSets.Select(s => new HashSet<char>(s));
            return Generate(length, asSets.ToList(), predicate);
        }

        public static string Generate(int length, IReadOnlyCollection<ISet<char>> requiredSets, Func<string, bool> predicate)
        {
            using (var random = new CryptoServiceRandom())
            {
                return Generate(length, requiredSets, predicate, random, EmbeddedTrigramStatistics.Instance);
            }
        }

        public static string Generate(int length, IReadOnlyCollection<ISet<char>> requiredSets, Func<string, bool> predicate, Random random, ITrigramStatistics stats)
        {
            if (length < requiredSets.Count + 2)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than the number of requiredSets + 2.");

            string result;
            do
                result = Generate(length, requiredSets, random, stats);
            while (!predicate(result));

            return result;
        }

        static string Generate(int length, IReadOnlyCollection<ISet<char>> requiredSets, Random random, ITrigramStatistics stats)
        {
            var parts = new List<string>();

            var all = requiredSets.Select(s => new RandomSetChooser(s, random))
                .Cast<IChooser<string>>()
                .Concat(new IChooser<string>[] { new RandomWordChooser(stats, random) });

            foreach (var chooser in all)
                parts.Add(chooser.GetRandom(length - GetLengthOfParts(parts)));

            while (GetLengthOfParts(parts) < length)
                parts.Add(random.Choose(all).GetRandom(length - GetLengthOfParts(parts)));

            return string.Concat(random.Shuffle(parts));
        }

        static int GetLengthOfParts(IEnumerable<string> items)
        {
            return items.Select(i => i.Length).Sum();
        }
    }

    internal interface IChooser<T>
    {
        T GetRandom(int maxLength);
    }

    internal class RandomSetChooser : IChooser<string>
    {
        ISet<char> _set;
        Random _random;

        public RandomSetChooser(ISet<char> set, Random random)
        {
            _set = set;
            _random = random;
        }

        public string GetRandom(int maxLength)
        {
            return char.ToString(_random.Choose(_set));
        }
    }

    internal class RandomWordChooser : IChooser<string>
    {
        readonly Random _random;
        readonly ITrigramStatistics _stats;

        public RandomWordChooser(ITrigramStatistics stats, Random random)
        {
            _random = random;
            _stats = stats;
        }

        public string GetRandom(int maxLength)
        {
            var word = new StringBuilder();

            var prev = ChooseRandomly(_stats.GetPrefixWeights(), _random);

            if (word.Length < maxLength)
                word.Append(char.ToUpperInvariant(prev.Item1));

            if (word.Length < maxLength)
                word.Append(prev.Item2);

            while (word.Length < maxLength && _stats.Exists(prev))
            {
                var next = ChooseRandomly(_stats.GetTrigramWeights(prev), _random);

                if (next == '$')
                    break;

                word.Append(next);

                prev = Tuple.Create(prev.Item2, next);
            }

            return word.ToString();
        }

        static T ChooseRandomly<T>(IReadOnlyCollection<WeightedItem<T>> cumulativeList, Random random)
        {
            var choice = random.GetNum();
            return cumulativeList.First(i => choice <= i.Weight).Item;
        }
    }
}
