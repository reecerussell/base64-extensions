using BenchmarkDotNet.Attributes;
using System;
using System.Security.Cryptography;

namespace Base64Extensions.Benchmarks
{
    [MemoryDiagnoser]
    public class EncodeBenchmark
    {
        public const int DataLength = 256;

        private readonly byte[] _data;

        public EncodeBenchmark()
        {
            _data = new byte[DataLength];

            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(_data);
        }

        [Benchmark]
        public byte[] EncodeUsingBase64Convert()
        {
            return Base64Convert.Encode(_data);
        }

        [Benchmark]
        public string EncodeUsingSystemConvert()
        {
            return Convert.ToBase64String(_data);
        }
    }
}
