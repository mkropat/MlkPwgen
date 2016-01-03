using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace MlkPwgen
{
    public static class PasswordGenerator
    {
        public const string Lower = "abcdefghijklmnopqrstuvwxyz";
        public const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Digits = "0123456789";
        public const string Alphanumerics = Lower + Upper + Digits;

        public const string Special = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~";
        public const string Alphanumericspecials = Alphanumerics + Special;

        public static string Generate(int length=10, string allowed = Alphanumerics)
        {
            return Generate(length, new HashSet<char>(allowed));
        }

        public static string Generate(int length, HashSet<char> allowed)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");

            if (allowed.Count == 0)
                throw new ArgumentException("allowed cannot be empty.");

            using (var rng = new RNGCryptoServiceProvider())
            {
                return new string(
                    ChooseRandomly(allowed, rng)
                        .Take(length)
                        .ToArray());
            }
        }

        public static string GenerateComplex(int length=10)
        {
            return GenerateComplex(length, new[] { Lower, Upper, Digits });
        }

        public static string GenerateComplex(int length, IEnumerable<string> requiredSets)
        {
            return GenerateComplex(length, requiredSets, _ => true);
        }

        public static string GenerateComplex(int length, IEnumerable<string> requiredSets, Func<string, bool> predicate)
        {
            var asSets = requiredSets.Select(s => new HashSet<char>(s));
            return GenerateComplex(length, asSets.ToList(), predicate);
        }

        public static string GenerateComplex(int length, IReadOnlyCollection<HashSet<char>> requiredSets, Func<string,bool> predicate)
        {
            if (length < requiredSets.Count)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than the number of requiredSets.");

            var allowed = UnionAll(requiredSets);

            while (true)
            {
                var password = Generate(length, allowed);
                var allRequiredMatch = requiredSets.All(s => s.Overlaps(password));
                if (allRequiredMatch && predicate(password))
                    return password;
            }
        }

        static IEnumerable<T> ChooseRandomly<T>(HashSet<T> choices, RandomNumberGenerator rng)
        {
            var size = Math.Pow(2, 8 * sizeof(uint));
            if (size < choices.Count)
                throw new ArgumentException("Too many items to choose from.");

            var cutoff = size - size % choices.Count;
            var choicesArray = choices.ToArray();
            foreach (var i in GenerateRandomNums(rng))
                if (i < cutoff) // avoid biasing
                    yield return choicesArray[i % choices.Count];
        }

        static IEnumerable<uint> GenerateRandomNums(RandomNumberGenerator rng)
        {
            var buf = new byte[sizeof(uint) * 64];
            while (true)
            {
                rng.GetBytes(buf);
                for (var i = 0; i < buf.Length; i += sizeof(uint))
                    yield return BitConverter.ToUInt32(buf, i);
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
}
