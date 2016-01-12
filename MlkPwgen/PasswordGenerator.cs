using System;
using System.Collections.Generic;
using System.Linq;

namespace MlkPwgen
{
    public static class PasswordGenerator
    {
        public static string Generate(int length=10, string allowed=Classes.Alphanumerics)
        {
            return Generate(length, new HashSet<char>(allowed));
        }

        public static string Generate(int length, HashSet<char> allowed)
        {
            using (var random = new CryptoServiceRandom())
            {
                return Generate(length, allowed, random);
            }
        }

        public static string Generate(int length, HashSet<char> allowed, Random random)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");

            if (allowed.Count == 0)
                throw new ArgumentException("allowed cannot be empty.");

            return random.GetChoiceStream(allowed).Take(length).AsString();
        }

        public static string GenerateComplex(int length=10, IEnumerable<string> requiredSets=null, Func<string, bool> predicate=null)
        {
            if (requiredSets == null)
                requiredSets = Classes.AlphanumericGroups;

            if (predicate == null)
                predicate = _ => true;

            var asSets = requiredSets.Select(s => new HashSet<char>(s));
            return GenerateComplex(length, asSets.ToList(), predicate);
        }

        public static string GenerateComplex(int length, IReadOnlyCollection<HashSet<char>> requiredSets, Func<string, bool> predicate)
        {
            using (var random = new CryptoServiceRandom())
            {
                return GenerateComplex(length, requiredSets, predicate, random);
            }
        }

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
