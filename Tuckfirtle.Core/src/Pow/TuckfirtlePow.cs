using System;
using System.Security.Cryptography;
using System.Text;

namespace Tuckfirtle.Core.Pow
{
    public class TuckfirtlePow : IDisposable
    {
        /*
         * Tuckfirtle is a POW algorithm that uses the pre-existing cryptography library available in C#.
         * Note that this is not proven to be relatively safe for production use. Use with caution.
         * There will be SHA3 variants in a different POW fork in the future when C# have better implementation of them.
         *
         * Disclaimer: This is not finished and may change overtime when I got new ideas.
         *
         * Input: (Based on the block data with random nonce) These will be in json and converted to UTF-8 bytes.
         * Output: The final POW value in hexadecimal.
         *
         * SOME USEFUL INFORMATION:
         * This algorithm use SHA2 variants to generate enough bytes to avoid possible collusion.
         *
         * SHA2 hashing function that will be used in this algorithm: 
         * SHA 256 = 256 bits == 32 bytes
         * SHA 384 = 384 bits == 48 bytes
         * SHA 512 = 512 bits == 64 bytes
         *
         * START OF ALGORITHM:
         * For each of the SHA2 hashing function, the input will be hashed and produce 32, 48 and 64 bytes of data respectively.
         * Then each of the data will be split into groups of 16 bytes as shown below.
         *
         * The diagram below will show the result (144 bytes):
         * | 000..015 bytes | 016..031 bytes | 032..047 bytes | 048..063 bytes | 064..079 bytes | 080..095 bytes | 096..111 bytes | 112..127 bytes | 128..143 bytes |
         * | SHA 256 (1st)  | SHA 256 (2nd)  | SHA 384 (3rd)  | SHA 384 (4th)  | SHA 384 (5th)  | SHA 512 (6th)  | SHA 512 (7th)  | SHA 512 (8th)  | SHA 512 (9th)  |
         *
         * To get the AES Key, you will need to combine the 1st and 2nd set of data and XOR with the 3rd set of data.
         *
         * AES Key Result (32 bytes):
         * | 00..15 bytes | 16..31 bytes |
         *
         * To get the XOR key, you will need to combine the 4th and 5th set of data and XOR with the 6th set of data.
         *
         * XOR Key Result (32 bytes):
         * | 00..15 bytes | 16..31 bytes |
         *
         * How to XOR the data for the steps above:
         * | 00..15 bytes | 16..31 bytes | 32..47 bytes |
         * | Data         | Data         | XOR Data     |
         *
         * For index 0 and 32, 0 (data) XOR 32 (XOR data)
         * For index 1 and 33, 1 (data) XOR 33 (XOR data)
         * For index 2 and 34, 2 (data) XOR 34 (XOR data)
         * ... continue with the rest of the index ...
         * For index 15 and 47, 15 (data) XOR 47 (XOR data)
         *
         * For index 16 and 32, 16 (data) XOR 32 (XOR data)
         * For index 17 and 33, 17 (data) XOR 33 (XOR data)
         * For index 18 and 34, 18 (data) XOR 34 (XOR data)
         * ... continue with the rest of the index ...
         * For index 31 and 47, 31 (data) XOR 47 (XOR data)
         *
         * ====================================================================================================
         * After doing the above, you should have the following data:
         *
         * Pow Data (144 bytes)
         * | 000..015 bytes | 016..031 bytes | 032..047 bytes | 048..063 bytes | 064..079 bytes | 080..095 bytes | 096..111 bytes | 112..127 bytes | 128..143 bytes |
         * | SHA 256 (1st)  | SHA 256 (2nd)  | SHA 384 (3rd)  | SHA 384 (4th)  | SHA 384 (5th)  | SHA 512 (6th)  | SHA 512 (7th)  | SHA 512 (8th)  | SHA 512 (9th)  |
         *
         * AES Key Result (32 bytes):
         * | 00..15 bytes | 16..31 bytes |
         *
         * XOR Key Result (32 bytes):
         * | 00..15 bytes | 16..31 bytes |
         *
         * Unused data (48 bytes):
         * | 00..15 bytes | 16..31 bytes | 32..47 bytes |
         * | SHA 512 (7th)| SHA 512 (8th)| SHA 512 (9th)|
         *
         * Scratchpad size: 64 kb == 65536‬ bytes
         */

        private SHA256 Sha256 { get; }

        private SHA384 Sha384 { get; }

        private SHA512 Sha512 { get; }

        public TuckfirtlePow()
        {
            Sha256 = SHA256.Create();
            Sha384 = SHA384.Create();
            Sha512 = SHA512.Create();
        }

        public byte[] GetPowData(string jsonData)
        {
            var result = new byte[144];
            var dataBytes = Encoding.UTF8.GetBytes(jsonData);

            var sha256Result = Sha256.ComputeHash(dataBytes);
            var sha384Result = Sha384.ComputeHash(dataBytes);
            var sha512Result = Sha512.ComputeHash(dataBytes);

            sha256Result.CopyTo(result, 0);
            sha384Result.CopyTo(result, 32);
            sha512Result.CopyTo(result, 80);

            return result;
        }

        public byte[] GetAesKey(byte[] powData)
        {
            var result = new byte[32];
            var dataBytes = new byte[32];
            var xorBytes = new byte[16];

            Array.Copy(powData, 0, dataBytes, 0, 32);
            Array.Copy(powData, 32, xorBytes, 0, 16);

            var dataBytesLength = dataBytes.Length;
            var xorBytesLength = xorBytes.Length;
            var xorIndex = 0;

            for (var i = 0; i < dataBytesLength; i++)
            {
                result[i] = (byte) (dataBytes[i] ^ xorBytes[xorIndex]);
                xorIndex++;

                if (xorIndex == xorBytesLength)
                    xorIndex = 0;
            }

            return result;
        }

        public byte[] GetXorKey(byte[] powData)
        {
            var result = new byte[32];
            var dataBytes = new byte[32];
            var xorBytes = new byte[16];

            Array.Copy(powData, 48, dataBytes, 0, 32);
            Array.Copy(powData, 80, xorBytes, 0, 16);

            var dataBytesLength = dataBytes.Length;
            var xorBytesLength = xorBytes.Length;
            var xorIndex = 0;

            for (var i = 0; i < dataBytesLength; i++)
            {
                result[i] = (byte) (dataBytes[i] ^ xorBytes[xorIndex]);
                xorIndex++;

                if (xorIndex == xorBytesLength)
                    xorIndex = 0;
            }

            return result;
        }

        public byte[] GetUnusedData(byte[] powData)
        {
            var result = new byte[48];

            Array.Copy(powData, 96, result, 0, 48);

            return result;
        }

        public void Dispose()
        {
            Sha256?.Dispose();
            Sha384?.Dispose();
            Sha512?.Dispose();
        }
    }
}