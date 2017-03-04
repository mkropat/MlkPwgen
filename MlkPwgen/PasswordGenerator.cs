using System;
using System.Collections.Generic;
using System.Linq;

namespace MlkPwgen
{
    /// <summary>
    /// Random password generator
    /// </summary>
    public static class PasswordGenerator
    {
        /// <param name="length"></param>
        /// <param name="allowed">The set of characters the password will be randomly made of</param>
        public static string Generate(int length=10, string allowed=Sets.Alphanumerics)
        {
            return Generate(length, new HashSet<char>(allowed));
        }

        /// <param name="length"></param>
        /// <param name="allowed">The set of characters the password will be randomly made of</param>
        public static string Generate(int length, HashSet<char> allowed)
        {
            using (var random = new CryptoServiceRandom())
            {
                return Generate(length, allowed, random);
            }
        }

        /// <param name="length"></param>
        /// <param name="allowed">The set of characters the password will be randomly made of</param>
        /// <param name="random"></param>
        public static string Generate(int length, HashSet<char> allowed, Random random)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");

            if (allowed.Count == 0)
                throw new ArgumentException("allowed cannot be empty.");

            return random.GetChoiceStream(allowed).Take(length).AsString();
        }

        /// <param name="length"></param>
        /// <param name="requiredSets">The sets characters that the password must contain</param>
        /// <param name="predicate">An arbitrary function that the password must match</param>
        public static string GenerateComplex(int length=10, IEnumerable<string> requiredSets=null, Func<string, bool> predicate=null)
        {
            if (requiredSets == null)
                requiredSets = Sets.AlphanumericGroups;

            if (predicate == null)
                predicate = _ => true;

            var asSets = requiredSets.Select(s => new HashSet<char>(s));
            return GenerateComplex(length, asSets.ToList(), predicate);
        }

        /// <param name="length"></param>
        /// <param name="requiredSets">The sets characters that the password must contain</param>
        /// <param name="predicate">An arbitrary function that the password must match</param>
        public static string GenerateComplex(int length, IReadOnlyCollection<HashSet<char>> requiredSets, Func<string, bool> predicate)
        {
            using (var random = new CryptoServiceRandom())
            {
                return GenerateComplex(length, requiredSets, predicate, random);
            }
        }

        /// <param name="length"></param>
        /// <param name="requiredSets">The sets characters that the password must contain</param>
        /// <param name="predicate">An arbitrary function that the password must match</param>
        /// <param name="random"></param>
        public static string GenerateComplex(int length, IReadOnlyCollection<HashSet<char>> requiredSets, Func<string,bool> predicate, Random random)
        {
            if (length < requiredSets.Count)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than the number of requiredSets.");

            var allowed = UnionAll(requiredSets);

            while (true)
            {
                var password = Generate(length, allowed, random);
                var allRequiredMatch = requiredSets.All(s => s.Overlaps(password));
                if (allRequiredMatch && predicate(password))
                    return password;
            }
        }

        static HashSet<T> UnionAll<T>(IEnumerable<HashSet<T>> sets)
        {
            var result = new HashSet<T>();
            foreach (var s in sets)
                result.UnionWith(s);
            return result;
        }
    }

    internal static class PasswordGeneratorExtensions
    {
        public static string AsString(this IEnumerable<char> cs)
        {
            return new string(cs.ToArray());
        }
    }
}
