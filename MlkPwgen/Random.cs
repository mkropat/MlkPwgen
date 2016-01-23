using System;
using System.Collections.Generic;
using System.Linq;

namespace MlkPwgen
{
    /// <summary>
    /// Exposes a source of randomness
    /// </summary>
    public abstract class Random
    {
        /// <summary>
        /// Returns a random number between 0 and <c>uint.MaxValue</c> inclusive
        /// </summary>
        public abstract uint GetNum();

        /// <summary>
        /// Returns a random number between 0 and <paramref name="maxInclusive"/> inclusive
        /// </summary>
        public virtual uint GetNum(uint maxInclusive)
        {
            if (maxInclusive == 0)
                return 0;

            var size = (long)uint.MaxValue + 1;
            var maxExclusive = (long)maxInclusive + 1;
            var cutoff = size - size % maxExclusive;

            uint choice;
            do
                choice = GetNum();
            while (choice >= cutoff);

            return (uint)(choice % maxExclusive);
        }

        /// <summary>
        /// Returns a random number between 0 and <paramref name="maxInclusive"/> inclusive
        /// </summary>
        public virtual int GetNum(int maxInclusive)
        {
            if (maxInclusive < 0)
                throw new ArgumentOutOfRangeException("maxInclusive", "maxInclusive may not be negative.");

            return (int)GetNum((uint)maxInclusive);
        }

        /// <summary>
        /// Returns an infinite stream of random numbers between 0 and <paramref name="maxInclusive"/> inclusive
        /// </summary>
        public virtual IEnumerable<int> GetNumStream(int maxInclusive)
        {
            while (true)
                yield return GetNum(maxInclusive);
        }

        /// <summary>
        /// Returns one item from <paramref name="items"/> chosen randomly
        /// </summary>
        public virtual T Choose<T>(IEnumerable<T> items)
        {
            return Choose(new HashSet<T>(items));
        }

        /// <summary>
        /// Returns one item from <paramref name="set"/> chosen randomly
        /// </summary>
        public virtual T Choose<T>(ISet<T> set)
        {
            if (set.Count == 0)
                throw new ArgumentException("set is empty.");

            return set.ElementAt(GetNum(set.Count - 1));
        }

        /// <summary>
        /// Returns an infinite stream of randomly chosen items from <paramref name="set"/>
        /// </summary>
        public virtual IEnumerable<T> GetChoiceStream<T>(ISet<T> set)
        {
            if (set.Count == 0)
                throw new ArgumentException("set is empty.");

            return GetNumStream(set.Count - 1)
                .Select(n => set.ElementAt(n));
        }

        /// <summary>
        /// Returns <paramref name="items"/> in a random order
        /// </summary>
        public virtual IEnumerable<T> Shuffle<T>(IReadOnlyList<T> items)
        {
            // The Fisher-Yates shuffle algorithm
            // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm
            var result = items.ToArray();
            for (var i = result.Length - 1; i >= 1; i--)
                ArraySwap(result, i, GetNum(i));
            return result;
        }

        static void ArraySwap<T>(T[] items, int i, int j)
        {
            T val = items[i];
            items[i] = items[j];
            items[j] = val;
        }
    }
}
