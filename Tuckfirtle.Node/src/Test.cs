using System;
using System.Numerics;
using System.Threading.Tasks;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Core.Pow;

namespace Tuckfirtle.Node
{
    public class Test
    {
        private IConsoleLogger ConsoleLogger { get; }

        public Test(IConsoleLogger consoleLogger)
        {
            ConsoleLogger = consoleLogger;
        }

        public void Start()
        {
            var biggestNumber = new byte[33];

            for (var i = 0; i < biggestNumber.Length; i++)
            {
                if (i != 32)
                    biggestNumber[i] = byte.MaxValue;
                else
                    biggestNumber[i] = 0;
            }

            var bigNumber = new BigInteger(biggestNumber);
            var smallestNumber = bigNumber;

            ConsoleLogger.LogMessage($"Biggest Number: {bigNumber.ToString().PadLeft(bigNumber.ToString().Length, '0')}");

            Parallel.For(0, int.MaxValue, i =>
            {
                var powValue = TuckfirtlePow.GetPowValueUnsafe(i.ToString());
                var powValueLength = powValue.Length;

                var powNumber = new byte[powValueLength + 1];
                powNumber[powValueLength] = 0;

                Buffer.BlockCopy(powValue, 0, powNumber, 0, powValueLength);

                var bigInteger = new BigInteger(powNumber);

                if (bigInteger >= smallestNumber)
                    return;

                smallestNumber = bigInteger;
                ConsoleLogger.LogMessage($"NOnce {i}: {bigInteger.ToString().PadLeft(bigNumber.ToString().Length, '0')}");
            });
        }
    }
}