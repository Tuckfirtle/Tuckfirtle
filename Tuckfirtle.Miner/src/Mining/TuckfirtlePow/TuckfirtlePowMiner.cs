using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsThread = Tuckfirtle.Miner.Threading.Windows.Thread;

namespace Tuckfirtle.Miner.Mining.TuckfirtlePow
{
    internal sealed class TuckfirtlePowMiner
    {
        private Thread MiningThread { get; }

        private bool IsRunning { get; set; }

        private TuckfirtlePowMinerInformation MinerInformation { get; }

        private TuckfirtlePowInformation PowInformation { get; }

        private Action<TuckfirtlePowSubmitInformation> SubmitAction { get; }

        public TuckfirtlePowMiner(TuckfirtlePowMinerInformation minerInformation, TuckfirtlePowInformation powInformation, Action<TuckfirtlePowSubmitInformation> submitAction)
        {
            MinerInformation = minerInformation;
            PowInformation = powInformation;
            SubmitAction = submitAction;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsThread = new WindowsThread(InternalStartMining) { ProcessorAffinity = minerInformation.ThreadAffinity };
                windowsThread.ManagedThread.IsBackground = true;

                MiningThread = windowsThread.ManagedThread;
            }
            else
                MiningThread = new Thread(InternalStartMining) { IsBackground = true };
        }

        public void StartMining()
        {
            MiningThread.Start();
            IsRunning = true;
        }

        public void StopMining()
        {
            IsRunning = false;
            MiningThread.Join();
        }

        private unsafe void InternalStartMining()
        {
            var powValueUnsignedBytes = new byte[33];
            var currentNonce = MinerInformation.StartingNonce;

            fixed (byte* powValueUnsignedBytesPtr = powValueUnsignedBytes)
            {
                while (IsRunning)
                {
                    var powInformation = PowInformation;
                    var powValueBytes = Core.Pow.TuckfirtlePow.GetPowValueUnsafe($"{powInformation.BlockHeaderTemplate}{currentNonce++}");
                    var powValueBytesLength = powValueBytes.Length;

                    fixed (byte* powValueBytesPtr = powValueBytes)
                    {
                        *(powValueUnsignedBytesPtr + powValueBytesLength) = 0;
                        Buffer.MemoryCopy(powValueBytesPtr, powValueUnsignedBytesPtr, 32, 32);

                        var powValue = new BigInteger(powValueUnsignedBytes);

                        if (powValue < powInformation.TargetPowValue)
                        {
                            // Found a block!
                            SubmitAction(new TuckfirtlePowSubmitInformation { Height = powInformation.Height, Nonce = currentNonce });
                        }
                    }
                }
            }
        }
    }
}