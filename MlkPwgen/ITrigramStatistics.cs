using System;
using System.Collections.Generic;

namespace MlkPwgen
{
    public interface ITrigramStatistics
    {
        IReadOnlyCollection<WeightedItem<Tuple<char, char>>> GetPrefixWeights();
        IReadOnlyCollection<WeightedItem<char>> GetTrigramWeights(Tuple<char, char> prefix);
        bool Exists(Tuple<char, char> prefix);
    }
}
