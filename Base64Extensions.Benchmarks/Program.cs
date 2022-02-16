using BenchmarkDotNet.Running;
using System;
using System.Security.Cryptography;

namespace Base64Extensions.Benchmarks
{
    class DeterministicRandomGenerator : RandomNumberGenerator
    {
        Random r = new Random(0);
        public override void GetBytes(byte[] data)
        {
            r.NextBytes(data);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<EncodeBenchmark>();
            BenchmarkRunner.Run<DecodeBenchmark>();
        }
    }
}
