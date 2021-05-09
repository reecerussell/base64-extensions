using BenchmarkDotNet.Attributes;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Base64Extensions.Benchmarks
{
    [MemoryDiagnoser]
    public class DecodeBenchmark
    {
        public const int DataLength = 256;

        private readonly string _data;

        public DecodeBenchmark()
        {
            var data = new byte[DataLength];

            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(data);

            _data = Convert.ToBase64String(data);
        }

        [Benchmark]
        public string DecodeUsingBase64Convert()
        {
            return Base64Convert.Decode(_data);
        }

        [Benchmark]
        public byte[] DecodeUsingSystemConvert()
        {
            return Convert.FromBase64String(_data);
        }
    }
}
