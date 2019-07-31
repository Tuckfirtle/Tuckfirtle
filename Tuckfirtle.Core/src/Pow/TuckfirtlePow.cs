using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Tuckfirtle.Core.Pow
{
    /*
     * Tuckfirtle is a POW algorithm that uses the pre-existing cryptography library available in C#.
     * Note that this is not proven to be relatively safe for production use. Use with caution.
     * There will be SHA3 variants in a different POW fork in the future when C# have better implementation of them.
     *
     * ====================================================================================================
     * 1. DEFINITION / INFORMATION
     * ====================================================================================================
     * Input: Block data appended with random nonce in json and converted to UTF-8 byte array.
     * Output: Final POW value of a big unsigned number.
     * Scratchpad: A huge byte array that is allocated in memory to do POW work.
     *
     * SHA 2 hashing function:
     * SHA 256 = 256 bits == 32 bytes
     * SHA 384 = 384 bits == 48 bytes
     * SHA 512 = 512 bits == 64 bytes
     *
     * AES encryption function:
     * BlockSize = 128 bits
     * FeedbackSize = 8
     * KeySize = 256 bits
     * Mode = CBC
     * Padding = None
     *
     * ====================================================================================================
     * 2. START OF ALGORITHM
     * ====================================================================================================
     * Using the input, perform SHA 256, SHA 384 and SHA 512 and combine the results together into 144 byte array as shown below.
     *
     * Pow data (144 bytes):
     * | 000..015 bytes | 016..031 bytes | 032..047 bytes | 048..063 bytes | 064..079 bytes | 080..095 bytes | 096..111 bytes | 112..127 bytes | 128..143 bytes |
     * | SHA 256 (1st)  | SHA 256 (2nd)  | SHA 384 (3rd)  | SHA 384 (4th)  | SHA 384 (5th)  | SHA 512 (6th)  | SHA 512 (7th)  | SHA 512 (8th)  | SHA 512 (9th)  |
     * | XOR key                         | XOR data       | AES key                         | XOR data       | AES data                        | XOR data       |
     *                                                    | AES iv         | XOR data       |
     *
     * XOR key:  Combine the 1st and 2nd set of pow data and perform XOR using the 3rd set of pow data.
     * AES key:  Combine the 4th and 5th set of pow data and perform XOR using the 6th set of pow data.
     * AES iv:   Use the first 16 bytes of AES key and perform XOR using the second 16 bytes of AES key.
     * AES data: Combine the 7th and 8th set of pow data and perform XOR using the 9th set of pow data.
     *
     * How to combine and perform XOR of pow data:
     * | 00..15 bytes | 16..31 bytes | 32..47 bytes |
     * | Data                        | XOR data     |
     *
     * Perform XOR using the byte value in index 0 and 32.
     * Perform XOR using the byte value in index 1 and 33.
     * ... continue with the rest of the index ...
     * Perform XOR using the byte value in index 15 and 47.
     * Perform XOR using the byte value in index 16 and 32.
     * Perform XOR using the byte value in index 17 and 33.
     * ... continue with the rest of the index ...
     * Perform XOR using the byte value in index 31 and 47.
     *
     * XOR data index rollover to index 0 when it goes out of range.
     *
     * After doing the above, you should have the following data:
     *
     * Pow data (144 bytes)
     * | 000..015 bytes | 016..031 bytes | 032..047 bytes | 048..063 bytes | 064..079 bytes | 080..095 bytes | 096..111 bytes | 112..127 bytes | 128..143 bytes |
     * | SHA 256 (1st)  | SHA 256 (2nd)  | SHA 384 (3rd)  | SHA 384 (4th)  | SHA 384 (5th)  | SHA 512 (6th)  | SHA 512 (7th)  | SHA 512 (8th)  | SHA 512 (9th)  |
     *
     * XOR key (32 bytes):
     * | 00..15 bytes | 16..31 bytes |
     *
     * AES key (32 bytes):
     * | 00..15 bytes | 16..31 bytes |
     *
     * AES iv (16 bytes):
     * | 00..15 bytes |
     *
     * AES data (32 bytes):
     * | 00..15 bytes | 16..31 bytes |
     *
     * ====================================================================================================
     * 2.1. MEMORY INITIALIZATION
     * ====================================================================================================
     * Scratchpad size: 64 kb == 65536‬ bytes
     *
     * Allocate a fixed 65536 byte array for the scratchpad.
     * Encrypt the AES data using the AES key and AES iv and insert into the first 32 bytes of scratchpad.
     * Encrypt the encrypted data you get from previous step using the AES key and AES iv and into the next 32 bytes of scratchpad.
     * Repeat the previous step until it is fully initialized.
     *
     * Scratchpad (65536 bytes):
     * | 0..15 bytes | 16..31 bytes | 32..47 bytes | 48..63 bytes | 64..65535 bytes |
     * | First Encrypted Data       | Second Encrypted Data       | ...             |
     *
     * ====================================================================================================
     * 2.2. MEMORY HARD LOOP (1 round)
     * ====================================================================================================
     * Every 16 bytes in the scratchpad correspond to a memory location.
     *
     * Take the first 16 bytes in the scratchpad and convert into a 16 bytes unsigned number.
     * Modulo that number with the scratchpad size to get the starting scratchpad location.
     * From the starting location to the next 31 bytes, perform XOR with the XOR key.
     * If the location goes out of bound, rollover the index to 0 and continue from there.
     *
     * Repeat the steps above by taking the next 16 bytes until the end of the scratchpad.
     *
     * How to perform XOR of scratchpad data:
     * From the starting location, perform XOR with the first index of XOR key.
     * Repeat using the next location and perform XOR with the next index of XOR key until all XOR key index is utilized.
     *
     * ====================================================================================================
     * 2.2. FINALIZATION
     * ====================================================================================================
     * Every 16 bytes in the pow data correspond to a memory location.
     *
     * Take the first 16 in the pow data and convert into a 16 bytes unsigned number.
     * Modulo that number with the scratchpad size to get the starting scratchpad location.
     * From the starting location to the next 16 bytes, perform XOR with the 16 bytes of pow data used on the previous step.
     * If the location goes out of bound, rollover the index to 0 and continue from there.
     *
     * Repeat the steps above by taking the next 16 bytes until the end of the pow data.
     *
     * How to perform XOR of each 16 bytes of pow data:
     * From the starting location, perform XOR with the first index of the starting scratchpad location found above.
     * Repeat using the next location and perform XOR with the next index of scratchpad location until the end of the 16 bytes of pow data.
     *
     * Finally, take the new pow data found and perform a SHA 256 to get the final POW value.
     */
    public class TuckfirtlePow
    {
        /// <summary>
        /// <para>The size of the scratchpad.</para>
        /// </summary>
        /// <remarks>The size of the scratchpad must be multiples of 32 bytes long.</remarks>
        private const int ScratchpadSize = 2 * 32 * 1024;

        /// <summary>
        /// <para>The amount of round needed for the memory loop.</para>
        /// </summary>
        private const int MemoryLoopRound = 1;

        public static unsafe byte[] GetPowValue(string jsonData)
        {
            var powData = new byte[144];
            var aesKey = new byte[32];
            var aesIv = new byte[16];
            var aesData = new byte[32];
            var addressLocation = new byte[17];

            var scratchpadIntPtr = Marshal.AllocHGlobal(ScratchpadSize);
            var scratchpadPtr = (byte*) scratchpadIntPtr;

            var xorKeyIntPtr = Marshal.AllocHGlobal(32);
            var xorKeyPtr = (byte*) xorKeyIntPtr;

            fixed (byte* powDataPtr = powData, aesKeyPtr = aesKey, aesIvPtr = aesIv, aesDataPtr = aesData, addressLocationPtr = addressLocation)
            {
                // Initialize powData 
                var dataBytes = Encoding.UTF8.GetBytes(jsonData);

                using (var sha = new SHA256Managed())
                {
                    fixed (byte* resultPtr = sha.ComputeHash(dataBytes))
                        Buffer.MemoryCopy(resultPtr, powDataPtr, 32, 32);
                }

                using (var sha = new SHA384Managed())
                {
                    fixed (byte* resultPtr = sha.ComputeHash(dataBytes))
                        Buffer.MemoryCopy(resultPtr, powDataPtr + 32, 48, 48);
                }

                using (var sha = new SHA512Managed())
                {
                    fixed (byte* resultPtr = sha.ComputeHash(dataBytes))
                        Buffer.MemoryCopy(resultPtr, powDataPtr + 80, 64, 64);
                }

                // Initialize aesKey, aesIv and aesData.
                XorBytesRollOver(powDataPtr, aesKeyPtr, 48, 32, 80, 16);
                XorBytes(aesKeyPtr, aesIvPtr, 0, 16, 16);
                XorBytesRollOver(powDataPtr, aesDataPtr, 96, 32, 128, 16);

                // Initialize addressLocation
                *(addressLocationPtr + 16) = 0;

                // Initialize xorKey.
                XorBytesRollOver(powDataPtr, xorKeyPtr, 0, 32, 32, 16);

                using (var aes = new AesManaged())
                {
                    aes.Key = aesKey;
                    aes.IV = aesIv;
                    aes.Padding = PaddingMode.None;

                    var aesEncryption = aes.CreateEncryptor();

                    for (var i = 0; i < ScratchpadSize; i += 32)
                    {
                        aesData = aesEncryption.TransformFinalBlock(aesData, 0, 32);

                        fixed (byte* newAesDataPtr = aesData)
                            Buffer.MemoryCopy(newAesDataPtr, scratchpadPtr + i, 32, 32);
                    }
                }

                for (var round = 0; round < MemoryLoopRound; round++)
                {
                    for (var i = 0; i < ScratchpadSize; i += 16)
                    {
                        Buffer.MemoryCopy(scratchpadPtr + i, addressLocationPtr, 16, 16);

                        var xorKeyByteArrayPtr = xorKeyPtr;
                        var scratchPadOffset = (int) (new BigInteger(addressLocation) % ScratchpadSize);

                        for (var j = 0; j < 32; j++)
                        {
                            var scratchpadByteArrayPtr = scratchpadPtr + scratchPadOffset;

                            *scratchpadByteArrayPtr = (byte) (*scratchpadByteArrayPtr ^ *xorKeyByteArrayPtr++);
                            scratchPadOffset++;

                            if (scratchPadOffset == ScratchpadSize)
                                scratchPadOffset = 0;
                        }
                    }
                }

                var powDataByteArrayPtr = powDataPtr;

                for (var i = 0; i < 144; i += 16)
                {
                    Buffer.MemoryCopy(powDataByteArrayPtr, addressLocationPtr, 16, 16);

                    var scratchPadOffset = (int) (new BigInteger(addressLocation) % ScratchpadSize);

                    for (var j = 0; j < 16; j++)
                    {
                        var scratchpadByteArrayPtr = scratchpadPtr + scratchPadOffset;

                        *powDataByteArrayPtr = (byte) (*scratchpadByteArrayPtr ^ *powDataByteArrayPtr);
                        powDataByteArrayPtr++;
                        scratchPadOffset++;

                        if (scratchPadOffset == ScratchpadSize)
                            scratchPadOffset = 0;
                    }
                }

                Marshal.FreeHGlobal(scratchpadIntPtr);
                Marshal.FreeHGlobal(xorKeyIntPtr);

                using (var sha256 = new SHA256Managed())
                    return sha256.ComputeHash(powData);
            }
        }

        private static unsafe void XorBytes(byte* srcArrayPtr, byte* destArrayPtr, int dataOffset, int dataLength, int xorKeyOffset)
        {
            var dataByteArrayPtr = srcArrayPtr + dataOffset;
            var xorKeyByteArrayPtr = srcArrayPtr + xorKeyOffset;
            var destByteArrayPtr = destArrayPtr;

            for (var i = 0; i < dataLength; i++)
                *destByteArrayPtr++ = (byte) (*dataByteArrayPtr++ ^ *xorKeyByteArrayPtr++);
        }

        private static unsafe void XorBytesRollOver(byte* srcArrayPtr, byte* destArrayPtr, int dataOffset, int dataLength, int xorKeyOffset, int xorKeyLength)
        {
            var dataByteArrayPtr = srcArrayPtr + dataOffset;
            var xorKeyByteArrayPtr = srcArrayPtr + xorKeyOffset;
            var destByteArrayPtr = destArrayPtr;
            var xorIndex = 0;

            for (var i = 0; i < dataLength; i++)
            {
                *destByteArrayPtr++ = (byte) (*dataByteArrayPtr++ ^ *(xorKeyByteArrayPtr + xorIndex++));

                if (xorIndex == xorKeyLength)
                    xorIndex = 0;
            }
        }
    }
}