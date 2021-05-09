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
        public static unsafe Span<byte> Encode(ReadOnlySpan<byte> value, bool urlSafe)
        {
            var encodedLen = Base64.GetMaxEncodedToUtf8Length(value.Length);
            Span<byte> encoded = stackalloc byte[encodedLen];

            Base64.EncodeToUtf8(value, encoded, out _, out _);

            if (urlSafe)
            {
                fixed (byte* bytes = encoded)
                {
                    Replace(bytes, encoded.Length, EncodingReplacements);
                }

                encoded = encoded.TrimEnd(Padding);
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
        public static unsafe Span<byte> Decode(ReadOnlySpan<byte> value)
        {
            var offset = 0;
            switch (value.Length % 4)
            {
                case 2: offset = 2; break;
                case 3: offset = 1; break;
            }

            Span<byte> encoded = stackalloc byte[value.Length + offset];
            value.CopyTo(encoded);

            fixed (byte* bytes = encoded)
            {
                Replace(bytes, encoded.Length, DecodingReplacements);
                PadEnd(bytes, encoded.Length, offset);
            }

            var decodedLen = Base64.GetMaxDecodedFromUtf8Length(encoded.Length);
            Span<byte> decoded = stackalloc byte[decodedLen];

            Base64.DecodeFromUtf8(encoded, decoded, out _, out var bytesWritten);
            
            return decoded.Slice(0, bytesWritten).ToArray();
        }

        /// <summary>
        /// Replaces characters in an byte-array, inline using <paramref name="replacements"/>.
        /// </summary>
        /// <param name="src">A pointer to the target byte-array.</param>
        /// <param name="srcLength">The length of <paramref name="src"/>.</param>
        /// <param name="replacements">The character replacements to use.</param>
        private static unsafe void Replace(byte* src, int srcLength, IDictionary<char, char> replacements)
        {
            for (var i = 0; i < srcLength; i++)
            {
                var c = (char)src[i];

                if (replacements.ContainsKey(c))
                {
                    src[i] = (byte)replacements[c];
                }
            }
        }

        /// <summary>
        /// Appends <paramref name="paddingLength"/> amount padding to <paramref name="src"/>.
        /// </summary>
        /// <param name="src">A pointer to the target byte-array.</param>
        /// <param name="srcLength">The length of <paramref name="src"/>.</param>
        /// <param name="paddingLength">The number of characters padding to append.</param>
        private static unsafe void PadEnd(byte* src, int srcLength, int paddingLength)
        {
            for (var i = 1; i <= paddingLength; i++)
            {
                src[srcLength - i] = Padding;
            }
        }
    }
}
