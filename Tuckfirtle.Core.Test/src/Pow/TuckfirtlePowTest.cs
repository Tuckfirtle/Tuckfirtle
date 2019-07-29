using System;
using System.Collections.Generic;
using Tuckfirtle.Core.Pow;
using Xunit;

namespace Tuckfirtle.Core.Test.Pow
{
    public class TuckfirtlePowTest : IClassFixture<TuckfirtlePow>
    {
        public TuckfirtlePowTest(TuckfirtlePow tuckfirtlePow)
        {
            TuckfirtlePow = tuckfirtlePow;
        }

        private TuckfirtlePow TuckfirtlePow { get; }

        [Theory]
        [InlineData("", new byte[] { 227, 176, 196, 66, 152, 252, 28, 20, 154, 251, 244, 200, 153, 111, 185, 36, 39, 174, 65, 228, 100, 155, 147, 76, 164, 149, 153, 27, 120, 82, 184, 85, 56, 176, 96, 167, 81, 172, 150, 56, 76, 217, 50, 126, 177, 177, 227, 106, 33, 253, 183, 17, 20, 190, 7, 67, 76, 12, 199, 191, 99, 246, 225, 218, 39, 78, 222, 191, 231, 111, 101, 251, 213, 26, 210, 241, 72, 152, 185, 91, 207, 131, 225, 53, 126, 239, 184, 189, 241, 84, 40, 80, 214, 109, 128, 7, 214, 32, 228, 5, 11, 87, 21, 220, 131, 244, 169, 33, 211, 108, 233, 206, 71, 208, 209, 60, 93, 133, 242, 176, 255, 131, 24, 210, 135, 126, 236, 47, 99, 185, 49, 189, 71, 65, 122, 129, 165, 56, 50, 122, 249, 39, 218, 62 })]
        public void GetPowDataTest(string jsonData, byte[] expectedResult)
        {
            var powData = TuckfirtlePow.GetPowData(jsonData);
            Assert.Equal(powData, expectedResult);
        }

        [Theory]
        [InlineData("", new byte[] { 219, 0, 164, 229, 201, 80, 138, 44, 214, 34, 198, 182, 40, 222, 90, 78, 31, 30, 33, 67, 53, 55, 5, 116, 232, 76, 171, 101, 201, 227, 91, 63 })]
        public void GetAesKeyTest(string jsonData, byte[] expectedResult)
        {
            var powData = TuckfirtlePow.GetPowData(jsonData);
            var aesKey = TuckfirtlePow.GetAesKey(powData);
            Assert.Equal(aesKey, expectedResult);
        }

        [Theory]
        [InlineData("", new byte[] { 238, 126, 86, 36, 106, 81, 191, 254, 189, 88, 239, 239, 181, 155, 97, 221, 232, 205, 63, 138, 153, 128, 221, 70, 36, 78, 250, 161, 158, 245, 57, 92 })]
        public void GetXorKeyTest(string jsonData, byte[] expectedResult)
        {
            var powData = TuckfirtlePow.GetPowData(jsonData);
            var xorKey = TuckfirtlePow.GetXorKey(powData);
            Assert.Equal(xorKey, expectedResult);
        }

        [Theory]
        [InlineData("", new byte[] { 181, 153, 213, 184, 76, 22, 111, 93, 38, 204, 155, 91, 42, 75, 51, 240, 36, 105, 224, 129, 26, 196, 136, 49, 90, 187, 42, 168, 126, 89, 54, 17 })]
        public void GetAesDataTest(string jsonData, byte[] expectedResult)
        {
            var powData = TuckfirtlePow.GetPowData(jsonData);
            var aesData = TuckfirtlePow.GetAesData(powData);
            Assert.Equal(aesData, expectedResult);
        }

        [Theory]
        [InlineData("", new byte[] { 215, 132, 240, 118, 172, 200, 40, 32, 174, 207, 192, 252, 173, 91, 141, 16, 18, 155, 116, 209, 81, 174, 166, 121, 145, 160, 172, 46, 77, 103, 141, 96, 185, 49, 225, 38, 208, 45, 23, 185, 205, 88, 179, 255, 48, 48, 98, 235, 82, 142, 196, 98, 103, 205, 116, 48, 63, 127, 180, 204, 16, 133, 146, 169, 197, 172, 60, 93, 5, 141, 135, 25, 55, 248, 48, 19, 170, 122, 91, 185, 136, 196, 166, 114, 57, 168, 255, 250, 182, 19, 111, 23, 145, 42, 199, 64, 29, 235, 47, 206, 192, 156, 222, 23, 72, 63, 98, 234, 24, 167, 34, 5, 82, 197, 196, 41, 72, 144, 231, 165, 234, 150, 13, 199, 146, 107, 249, 58, 181, 111, 231, 107, 145, 151, 172, 87, 115, 238, 228, 172, 47, 241, 12, 232 })]
        public void GetPowValueTest(string jsonData, byte[] expectedResult)
        {
            var powValue = TuckfirtlePow.GetPowValue(jsonData);
            Assert.Equal(powValue, expectedResult);
        }

        [Fact]
        public void SimpleCollusionTest()
        {
            var result = new List<string>();
            var fail = false;

            for (var i = 0; i < 9999; i++)
            {
                var powValue = TuckfirtlePow.GetPowValue(i.ToString());
                var powResult = BitConverter.ToString(powValue);

                if (result.Contains(powResult))
                {
                    fail = true;
                    break;
                }

                result.Add(powResult);
            }

            Assert.False(fail);
        }
    }
}