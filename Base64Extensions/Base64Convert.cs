using System;
using System.Buffers.Text;
using System.Collections.Generic;

namespace Base64Extensions
{
    /// <summary>
    /// A collection of methods used to encode and decode base64.
    /// </summary>
    public static class Base64Convert
    {
        private const byte Padding = (byte)'=';

        private static readonly System.Text.Encoding Encoding = System.Text.Encoding.UTF8;

        private static readonly IDictionary<char, char> EncodingReplacements = new Dictionary<char, char>
        {
            ['+'] = '-',
            ['/'] = '_'
        };

        private static readonly IDictionary<char, char> DecodingReplacements = new Dictionary<char, char>
        {
            ['-'] = '+',
            ['_'] = '/'
        };

        /// <summary>
        /// Converts <paramref name="value"/> to a base64 string, using UTF8 encoding.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to encode.</param>
        /// <returns>A base64 representation of <paramref name="value"/>.</returns>
        public static string Encode(string value)
            => Encode(value, false);

        /// <summary>
        /// Converts <paramref name="value"/> to a base64 string, using UTF8 encoding.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to encode.</param>
        /// <param name="urlSafe">Determines whether the result will contain URL-safe characters.</param>
        /// <returns>A base64 representation of <paramref name="value"/>.</returns>
        public static string Encode(string value, bool urlSafe)
        {
            var bytes = Encoding.GetBytes(value);
            var encoded = Encode(bytes, urlSafe);

            return Encoding.GetString(encoded);
        }

        /// <summary>
        /// Converts <paramref name="value"/> to base64, using UTF8 encoding.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>A base64 representation of <paramref name="value"/>.</returns>
        public static byte[] Encode(byte[] value)
            => Encode(value, false);

        /// <summary>
        /// Converts <paramref name="value"/> to base64, using UTF8 encoding.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <param name="urlSafe">Determines whether the result will contain URL-safe characters.</param>
        /// <returns>A base64 representation of <paramref name="value"/>.</returns>
        public static byte[] Encode(byte[] value, bool urlSafe)
        {
            Span<byte> bytes = value;

            return Encode(bytes, urlSafe).ToArray();
        }

        /// <summary>
        /// Converts <paramref name="value"/> to base64, using UTF8 encoding.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <param name="urlSafe">Determines whether the result will contain URL-safe characters.</param>
        /// <returns>A base64 representation of <paramref name="value"/>.</returns>
        public static Span<byte> Encode(ReadOnlySpan<byte> value, bool urlSafe)
        {
            var encodedLen = Base64.GetMaxEncodedToUtf8Length(value.Length);
            Span<byte> encoded = stackalloc byte[encodedLen];

            Base64.EncodeToUtf8(value, encoded, out _, out _);

            if (urlSafe)
            {
                Replace(encoded, EncodingReplacements);
                TrimEndPadding(ref encoded);
            }

            return encoded.ToArray();
        }

        /// <summary>
        /// Converts a base64 string to plain text, using UTF8 encoding.
        /// </summary>
        /// <param name="value">The base64 value to decode.</param>
        /// <returns>A plain text representation of <paramref name="value"/>.</returns>
        public static string Decode(string value)
        {
            var bytes = Encoding.GetBytes(value);
            var decoded = Decode(bytes);
            var result = Encoding.GetString(decoded);

            return result;
        }

        /// <summary>
        /// Converts base64 to plain text, using UTF8 encoding.
        /// </summary>
        /// <param name="value">The base64 data to decode.</param>
        /// <returns>A plain text representation of <paramref name="value"/>.</returns>
        public static byte[] Decode(byte[] value)
        {
            Span<byte> bytes = value;

            return Decode(bytes).ToArray();
        }

        /// <summary>
        /// Converts base64 to plain text, using UTF8 encoding.
        /// </summary>
        /// <param name="value">The base64 data to decode.</param>
        /// <returns>A plain text representation of <paramref name="value"/>.</returns>
        public static Span<byte> Decode(ReadOnlySpan<byte> value)
        {
            var offset = 0;
            switch (value.Length % 4)
            {
                case 2: offset = 2; break;
                case 3: offset = 1; break;
            }

            Span<byte> encoded = stackalloc byte[value.Length + offset];
            value.CopyTo(encoded);

            Replace(encoded, DecodingReplacements);
            PadEnd(encoded, offset);

            Span<byte> decoded = stackalloc byte[Base64.GetMaxDecodedFromUtf8Length(encoded.Length)];

            Base64.DecodeFromUtf8(encoded, decoded, out _, out var bytesWritten);

            return decoded.Slice(0, bytesWritten).ToArray();
        }

        /// <summary>
        /// Replaces characters in <paramref name="src"/>, inline using <paramref name="replacements"/>.
        /// </summary>
        /// <param name="src">The <see cref="Span{T}"/> to do replacements on.</param>
        /// <param name="replacements">A dictionary of replacement characters.</param>
        private static void Replace(Span<byte> src, IDictionary<char, char> replacements)
        {
            for (var i = 0; i < src.Length; i++)
            {
                var c = (char)src[i];

                if (replacements.ContainsKey(c))
                {
                    src[i] = (byte)replacements[c];
                }
            }
        }

        /// <summary>
        /// Sets the <paramref name="paddingLength"/> number of bytes from the end of
        /// <paramref name="src"/> to <see cref="Padding"/>.
        /// </summary>
        /// <param name="src">The <see cref="Span{T}"/> to append to.</param>
        /// <param name="paddingLength">The amount of padding to add.</param>
        private static void PadEnd(Span<byte> src, int paddingLength)
        {
            for (var i = 1; i <= paddingLength; i++)
            {
                src[^i] = Padding;
            }
        }

        /// <summary>
        /// Trims all consecutive <see cref="Padding"/> characters from the end of <paramref name="src"/>.
        /// </summary>
        /// <param name="src">A reference to the <see cref="Span{T}"/> to trim.</param>
        private static void TrimEndPadding(ref Span<byte> src)
        {
            var count = 0;

            for (var i = src.Length - 1; i > 0; i--)
            {
                if (src[i] == Padding)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            src = src.Slice(0, src.Length - count);
        }
    }
}
