using System;
using System.Numerics;
using System.Security.Cryptography;
using Tuckfirtle.Core.Pow;
using Xunit;

namespace Tuckfirtle.Core.Test.Pow
{
    public class TuckfirtlePowTest
    {
        [Fact]
        public void EmptyTest()
        {
            var pow = new TuckfirtlePow();
            var powData = pow.GetPowData("");
            var aesKey = pow.GetAesKey(powData);
            var xorKey = pow.GetXorKey(powData);
            var aesData = pow.GetAesData(powData);
            var scratchPad = new byte[65536];

            using (var aes = new AesManaged())
            {
                aes.Key = aesKey;
                aes.Padding = PaddingMode.None;

                var aesEncryptor = aes.CreateEncryptor();
                var currentAesData = aesData;
                var lengthAdded = 0;

                while (lengthAdded < scratchPad.Length)
                {
                    currentAesData = aesEncryptor.TransformFinalBlock(currentAesData, 0, currentAesData.Length);
                    Array.Copy(currentAesData, 0, scratchPad, lengthAdded, currentAesData.Length);
                    lengthAdded += currentAesData.Length;
                }
            }

            for (var i = 0; i < 256; i++)
            {
                for (var j = 0; j < scratchPad.Length; j += 32)
                {
                    var addressLocationBytes = new byte[32];
                    Array.Copy(scratchPad, j, addressLocationBytes, 0, addressLocationBytes.Length);
                    addressLocationBytes[31] = (byte) (addressLocationBytes[31] & 127);

                    var addressLocation = new BigInteger(addressLocationBytes) % scratchPad.Length;
                    var addressLocationValue = Convert.ToInt32(addressLocation.ToString());


                    foreach (var xor in xorKey)
                        scratchPad[addressLocationValue] = (byte) (scratchPad[addressLocationValue] ^ xor);
                }
            }

            var endOfTest = "";
        }
    }
}