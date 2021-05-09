using BenchmarkDotNet.Running;

namespace Base64Extensions.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<EncodeBenchmark>();
            BenchmarkRunner.Run<DecodeBenchmark>();
        }
    }
}
