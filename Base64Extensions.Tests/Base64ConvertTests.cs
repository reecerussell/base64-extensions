using FluentAssertions;
using System.Text;
using Xunit;

namespace Base64Extensions.Tests
{
    public class Base64ConvertTests
    {
        [Theory]
        [InlineData("Hello World", "SGVsbG8gV29ybGQ=")]
        [InlineData("Hello World ", "SGVsbG8gV29ybGQg")]
        public void Encode_GivenPlainTextValue_ReturnsEncodedBase64String(string value, string expected)
        {
            Base64Convert.Encode(value).Should().Be(expected);
        }
        
        [Theory]
        [InlineData("Hello World", "SGVsbG8gV29ybGQ=", 16)]
        [InlineData("Hello World ", "SGVsbG8gV29ybGQg", 16)]
        public void Encode_GivenPlainTextValue_ReturnsEncodedBase64StringWithBytesWritten(
            string value, string expected, int expectedBytesWritten)
        {
            Base64Convert.Encode(value, out var bytesWritten).Should().Be(expected);
            bytesWritten.Should().Be(expectedBytesWritten);
        }

        [Theory]
        [InlineData("Hello World", "SGVsbG8gV29ybGQ")]
        public void Encode_GivenPlainTextAndUrlSafeFlag_ReturnsUrlSafeBase64String(string value, string expected)
        {
            Base64Convert.Encode(value, true).Should().Be(expected);
        }
        
        [Theory]
        [InlineData("Hello World", "SGVsbG8gV29ybGQ", 15)]
        public void Encode_GivenPlainTextAndUrlSafeFlag_ReturnsUrlSafeBase64StringWithBytesWritten(
            string value, string expected, int expectedBytesWritten)
        {
            var encoded = Base64Convert.Encode(value, true, out var bytesWritten);
            encoded.Should().Be(expected);
            bytesWritten.Should().Be(expectedBytesWritten);
        }

        [Fact]
        public void Encode_NotGivenUrlSafeFlag_ReturnsStandardBase64()
        {
            var bytes = new byte[]
            {
                161, 203, 187, 6,
                69, 54, 110, 237,
                102, 171, 236, 129,
                217, 210, 255, 224
            };

            var encoded = Base64Convert.Encode(bytes);
            var encString = Encoding.UTF8.GetString(encoded);

            encString.Should().Be("ocu7BkU2bu1mq+yB2dL/4A==");
        }
        
        [Fact]
        public void Encode_NotGivenUrlSafeFlag_ReturnsStandardBase64WithBytesWritten()
        {
            var bytes = new byte[]
            {
                161, 203, 187, 6,
                69, 54, 110, 237,
                102, 171, 236, 129,
                217, 210, 255, 224
            };

            var encoded = Base64Convert.Encode(bytes, out var bytesWritten);
            
            var encString = Encoding.UTF8.GetString(encoded);
            encString.Should().Be("ocu7BkU2bu1mq+yB2dL/4A==");

            bytesWritten.Should().Be(24);
        }

        [Fact]
        public void Encode_GivenUrlSafeFlag_ReturnsUrlSafeBase64()
        {
            var bytes = new byte[]
            {
                161, 203, 187, 6,
                69, 54, 110, 237,
                102, 171, 236, 129,
                217, 210, 255, 224
            };

            var encoded = Base64Convert.Encode(bytes, true);
            var encString = Encoding.UTF8.GetString(encoded);

            encString.Should().Be("ocu7BkU2bu1mq-yB2dL_4A");
        }
        
        [Fact]
        public void Encode_GivenUrlSafeFlag_ReturnsUrlSafeBase64WithBytesWritten()
        {
            var bytes = new byte[]
            {
                161, 203, 187, 6,
                69, 54, 110, 237,
                102, 171, 236, 129,
                217, 210, 255, 224
            };

            var encoded = Base64Convert.Encode(bytes, true, out var bytesWritten);
            
            var encString = Encoding.UTF8.GetString(encoded);
            encString.Should().Be("ocu7BkU2bu1mq-yB2dL_4A");

            bytesWritten.Should().Be(22);
        }

        [Theory]
        [InlineData("SGVsbG8gV29ybGQ=", "Hello World")]
        [InlineData("SGVsbG8gV29ybGQg", "Hello World ")]
        public void DecodeString_GivenEncodedString_ReturnsPlainText(string base64, string expected)
        {
            Base64Convert.Decode(base64).Should().Be(expected);
        }

        [Theory]
        [InlineData("SGVsbG8gV29ybGQ", "Hello World")]
        [InlineData("SGVsbG8gV29ybGQg", "Hello World ")]
        public void DecodeString_GivenEncodedStringAndUrlSafeFlag_ReturnsPlainText(string base64, string expected)
        {
            Base64Convert.Decode(base64).Should().Be(expected);
        }

        [Fact]
        public void Decode_NotGivenUrlSafeFlag_ReturnsPlainText()
        {
            var encoded = Encoding.UTF8.GetBytes("ocu7BkU2bu1mq+yB2dL/4A==");
            var bytes = Base64Convert.Decode(encoded);

            bytes.Should().BeEquivalentTo(new byte[]
            {
                161, 203, 187, 6,
                69, 54, 110, 237,
                102, 171, 236, 129,
                217, 210, 255, 224
            });
        }

        [Fact]
        public void Decode_GivenUrlSafeFlag_ReturnsPlainText()
        {
            var encoded = Encoding.UTF8.GetBytes("ocu7BkU2bu1mq-yB2dL_4A");
            var bytes = Base64Convert.Decode(encoded);

            bytes.Should().BeEquivalentTo(new byte[]
            {
                161, 203, 187, 6,
                69, 54, 110, 237,
                102, 171, 236, 129,
                217, 210, 255, 224
            });
        }
    }
}
