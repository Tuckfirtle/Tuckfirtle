using System;
using System.Collections.Generic;
using System.Numerics;
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
         * Input: (Based on the block data with random nonce) These will be in json and converted to UTF-8 bytes.
         * Output: The final POW value.
         *
         * SOME USEFUL INFORMATION:
         * This algorithm use SHA2 variants to generate enough bytes to avoid possible collusion.
         *
         * SHA2 hashing function that will be used in this algorithm: 
         * SHA 256 = 256 bits == 32 bytes
         * SHA 384 = 384 bits == 48 bytes
         * SHA 512 = 512 bits == 64 bytes
         *
         * AES Configuration Used:
         * BlockSize = 128
         * FeedbackSize = 8
         * KeySize = 256
         * Mode = CBC
         * Padding = None
         *
         * START OF ALGORITHM:
         * For each of the SHA2 hashing function, the input will be hashed and produce 32, 48 and 64 bytes of data respectively.
         * Then each of the data will be split into groups of 16 bytes as shown below.
         *
         * Pow Data Result (144 bytes):
         * | 000..015 bytes | 016..031 bytes | 032..047 bytes | 048..063 bytes | 064..079 bytes | 080..095 bytes | 096..111 bytes | 112..127 bytes | 128..143 bytes |
         * | SHA 256 (1st)  | SHA 256 (2nd)  | SHA 384 (3rd)  | SHA 384 (4th)  | SHA 384 (5th)  | SHA 512 (6th)  | SHA 512 (7th)  | SHA 512 (8th)  | SHA 512 (9th)  |
         * | AES key                         | XOR data       | XOR key                         | XOR data       | AES data                        | XOR data       |
         * | AES iv         | XOR Data       |
         *
         * To get the AES key, combine the 1st and 2nd set of pow data and XOR with the 3rd set of pow data.
         * To get the AES iv, take the AES key and split into 2 groups of 16 bytes. Apply XOR with the given 2 groups of 16 bytes.
         * To get the XOR key, combine the 4th and 5th set of pow data and XOR with the 6th set of pow data.
         * To get the AES data, combine the 7th and 8th set of pow data and XOR with the 9th set of pow data.
         *
         * How to XOR the data for the steps above:
         * | 00..15 bytes | 16..31 bytes | 32..47 bytes |
         * | Data         | Data         | XOR Data     |
         *
         * For index 0 and 32, 0 (data) XOR 32 (XOR data)
         * For index 1 and 33, 1 (data) XOR 33 (XOR data)
         * ... continue with the rest of the index ...
         * For index 15 and 47, 15 (data) XOR 47 (XOR data)
         *
         * For index 16 and 32, 16 (data) XOR 32 (XOR data)
         * For index 17 and 33, 17 (data) XOR 33 (XOR data)
         * ... continue with the rest of the index ...
         * For index 31 and 47, 31 (data) XOR 47 (XOR data)
         *
         * After doing the above, you should have the following data:
         *
         * Pow Data (144 bytes)
         * | 000..015 bytes | 016..031 bytes | 032..047 bytes | 048..063 bytes | 064..079 bytes | 080..095 bytes | 096..111 bytes | 112..127 bytes | 128..143 bytes |
         * | SHA 256 (1st)  | SHA 256 (2nd)  | SHA 384 (3rd)  | SHA 384 (4th)  | SHA 384 (5th)  | SHA 512 (6th)  | SHA 512 (7th)  | SHA 512 (8th)  | SHA 512 (9th)  |
         *
         * AES key Result (32 bytes):
         * | 00..15 bytes | 16..31 bytes |
         *
         * AES iv Result (16 bytes):
         * | 00..15 bytes |
         *
         * XOR Key Result (32 bytes):
         * | 00..15 bytes | 16..31 bytes |
         *
         * AES Data (32 bytes):
         * | 00..15 bytes | 16..31 bytes |
         *
         * Scratchpad size: 64 kb == 65536‬ bytes
         *
         * To initialize the scratchpad, you will need to encrypt the AES data with the AES key.
         * Continue to use the previous AES result with the AES key until the scratchpad is filled.
         *
         * Scratchpad:
         * | 0..15 bytes | 16..31 bytes | 32..47 bytes | 48..63 bytes | 64..65535 bytes |
         * | First Encrypted Data       | Second Encrypted Data       | ...             |
         *
         * For each 32 bytes in the scratchpad, convert the number into 32 bytes unsigned big integer.
         * Modulo that number with the scratchpad size to get the starting scratchpad location to XOR with XOR key.
         * From the starting location to the next 31 bytes, XOR with the XOR key.
         * If the offset goes out of bound, rollover to index 0 and continue from there.
         * 
         * Finally, to get the new 144 bytes of Pow Data:
         * For each 16 bytes of old pow data, convert the number into 16 bytes unsigned big integer.
         * Modulo that number with the scratchpad size to get the scratchpad location which contain a single byte number.
         * For each byte in the 16 bytes of old pow data, XOR with the single byte number you found above.
         *
         * Finally, compress the final result into groups of 3.
         * You will need to combine the 1st and 2nd set of data and XOR with the 3rd set of data.
         * You will need to combine the 4th and 5th set of data and XOR with the 6th set of data.
         * You will need to combine the 7th and 8th set of data and XOR with the 9th set of data.
         *
         * Final State: (96 bytes)
         * | 000..015 bytes | 016..031 bytes | 032..047 bytes | 048..063 bytes | 064..079 bytes | 080..095 bytes |
         *
         * This is a huge unsigned integer number in which the blockchain will use to verify your work.
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

        public static byte[] GetAesKey(byte[] powData)
        {
            return CompressAndXorBytes(powData, 0, 32, 32, 16);
        }

        public static byte[] GetXorKey(byte[] powData)
        {
            return CompressAndXorBytes(powData, 48, 32, 80, 16);
        }

        public static byte[] GetAesData(byte[] powData)
        {
            return CompressAndXorBytes(powData, 96, 32, 128, 16);
        }

        private static byte[] CompressAndXorBytes(IReadOnlyList<byte> powData, int dataOffset, int dataLength, int xorKeyOffset, int xorKeyLength)
        {
            var result = new byte[dataLength];
            var xorIndex = 0;

            for (var i = 0; i < dataLength; i++)
            {
                result[i] = (byte) (powData[dataOffset + i] ^ powData[xorKeyOffset + xorIndex++]);

                if (xorIndex >= xorKeyLength)
                    xorIndex = 0;
            }

            return result;
        }

        public byte[] GetPowValue(string jsonData)
        {
            var powData = GetPowData(jsonData);
            var aesKey = GetAesKey(powData);
            var aesIv = CompressAndXorBytes(aesKey, 0, 16, 16, 16);
            var aesData = GetAesData(powData);
            var xorKey = GetXorKey(powData);

            var powDataLength = powData.Length;
            var aesDataLength = aesData.Length;
            var xorKeyLength = xorKey.Length;

            var scratchPad = new byte[65536];
            var scratchPadLength = scratchPad.Length;

            using (var aes = new AesManaged())
            {
                aes.IV = aesIv;
                aes.Key = aesKey;
                aes.Padding = PaddingMode.None;

                var aesEncryption = aes.CreateEncryptor();
                var currentAesData = aesData;
                var lengthAdded = 0;

                while (lengthAdded < scratchPadLength)
                {
                    currentAesData = aesEncryption.TransformFinalBlock(currentAesData, 0, aesDataLength);
                    Array.Copy(currentAesData, 0, scratchPad, lengthAdded, aesDataLength);
                    lengthAdded += aesDataLength;
                }
            }

            for (var j = 0; j < scratchPadLength; j += 32)
            {
                var addressLocationBytes = new byte[33];
                addressLocationBytes[32] = 0;

                Array.Copy(scratchPad, j, addressLocationBytes, 0, 32);

                var addressLocation = new BigInteger(addressLocationBytes) % scratchPadLength;
                var addressLocationValue = (int) addressLocation;

                for (var k = 0; k < xorKeyLength; k++)
                {
                    if (addressLocationValue + k >= scratchPadLength)
                    {
                        var newLocation = (addressLocationValue + k) % scratchPadLength;
                        scratchPad[newLocation] = (byte) (scratchPad[newLocation] ^ xorKey[k]);
                    }
                    else
                        scratchPad[addressLocationValue + k] = (byte) (scratchPad[addressLocationValue + k] ^ xorKey[k]);
                }
            }

            for (var i = 0; i < powDataLength; i += 16)
            {
                var addressLocationBytes = new byte[17];
                addressLocationBytes[16] = 0;

                Array.Copy(powData, i, addressLocationBytes, 0, 16);

                var addressLocation = new BigInteger(addressLocationBytes) % scratchPadLength;
                var addressLocationValue = (int) addressLocation;

                for (var j = 0; j < 16; j++)
                    powData[j + i] = (byte) (powData[j + i] ^ scratchPad[addressLocationValue]);
            }

            var compressedBytes = new byte[96];

            GetAesKey(powData).CopyTo(compressedBytes, 0);
            GetXorKey(powData).CopyTo(compressedBytes, 32);
            GetAesData(powData).CopyTo(compressedBytes, 64);

            return compressedBytes;
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

        public void Dispose()
        {
            Sha256?.Dispose();
            Sha384?.Dispose();
            Sha512?.Dispose();
        }
    }
}