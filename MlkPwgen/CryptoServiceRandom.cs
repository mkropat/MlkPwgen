using System;
using System.Security.Cryptography;

namespace MlkPwgen
{
    public sealed class CryptoServiceRandom : Random, IDisposable
    {
        byte[] _buf = new byte[sizeof(uint) * 64];
        int _i;
        readonly RandomNumberGenerator _rng;

        public CryptoServiceRandom() : this(new RNGCryptoServiceProvider()) { }

        public CryptoServiceRandom(RandomNumberGenerator rng)
        {
            if (rng == null)
                throw new ArgumentNullException("rng");

            _rng = rng;
            _i = _buf.Length;
        }

        public override uint GetNum()
        {
            if (_i >= _buf.Length)
            {
                _rng.GetBytes(_buf);
                _i = 0;
            }

            var result = BitConverter.ToUInt32(_buf, _i);
            _i += sizeof(uint);
            return result;
        }

        public void Dispose()
        {
            _rng.Dispose();
        }
    }
}
