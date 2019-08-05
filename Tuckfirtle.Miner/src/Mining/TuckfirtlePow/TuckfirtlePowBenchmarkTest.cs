// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuckfirtle.Core.Utility;

namespace Tuckfirtle.Miner.Mining.TuckfirtlePow
{
    internal sealed class TuckfirtlePowBenchmarkTest
    {
        public event Action<TuckfirtlePowBenchmarkTest, TuckfirtlePowInformation> NewJob;

        public event Action<TuckfirtlePowBenchmarkTest, bool, string> ShareResult;

        public int TotalAcceptedShare { get; private set; }

        public int TotalRejectedShare { get; private set; }

        public TuckfirtlePowInformation PowInformation { get; } = new TuckfirtlePowInformation();

        private TuckfirtlePowBenchmarkTestBlock LastBlock { get; }

        private List<TuckfirtlePowBenchmarkTestBlock> FoundBlocks { get; } = new List<TuckfirtlePowBenchmarkTestBlock>();

        private Task BlockRenewTask { get; set; }

        private int RenewIndex { get; set; }

        public TuckfirtlePowBenchmarkTest()
        {
            LastBlock = new TuckfirtlePowBenchmarkTestBlock
            {
                Version = 1,
                Height = 0,
                DateTime = DateTimeOffset.Now,
                Nonce = 0,
                TargetPowValue = DifficultyUtility.GetTargetPowValue(1000)
            };
        }

        public void StartTest()
        {
            PowInformation.Height = 1;
            PowInformation.TargetPowValue = DifficultyUtility.GetTargetPowValue(1000);
            PowInformation.BlockHeaderTemplate = JsonConvert.SerializeObject(LastBlock, Formatting.None);

            NewJob?.Invoke(this, PowInformation);

            BlockRenewTask = Task.Run(async () =>
            {
                var powInformation = PowInformation;

                while (true)
                {
                    if (FoundBlocks.Count != 0)
                    {
                        if (DateTimeOffset.Now > FoundBlocks[FoundBlocks.Count - 1].DateTime.AddMinutes(1 + RenewIndex))
                        {
                            RenewIndex++;
                            powInformation.TargetPowValue = FoundBlocks.Count - 1 - RenewIndex < 0 ? DifficultyUtility.GetTargetPowValue(1000) : FoundBlocks.Select(a => a.TargetPowValue).OrderBy(a => a).SkipWhile(a => a <= powInformation.TargetPowValue).FirstOrDefault();
                            NewJob?.Invoke(this, powInformation);
                        }
                    }

                    await Task.Delay(1).ConfigureAwait(false);
                }
            });
        }

        public void SubmitTestResult(TuckfirtlePowSubmitInformation submitInformation)
        {
            var powInformation = PowInformation;
            var powValue = Core.Pow.TuckfirtlePow.GetPowValueUnsafe($"{submitInformation.BlockHeaderTemplate}{submitInformation.Nonce}");

            if (powValue == submitInformation.PowValue && powValue < powInformation.TargetPowValue)
            {
                // Test Success. Move to next block.
                LastBlock.Height++;
                LastBlock.DateTime = DateTimeOffset.Now;
                LastBlock.Nonce = submitInformation.Nonce;
                LastBlock.TargetPowValue = submitInformation.PowValue;

                FoundBlocks.Add(new TuckfirtlePowBenchmarkTestBlock
                {
                    Version = LastBlock.Version,
                    Height = LastBlock.Height,
                    DateTime = LastBlock.DateTime,
                    Nonce = LastBlock.Nonce,
                    TargetPowValue = LastBlock.TargetPowValue
                });

                RenewIndex = 0;

                powInformation.Height++;
                powInformation.TargetPowValue = powValue;
                powInformation.BlockHeaderTemplate = JsonConvert.SerializeObject(LastBlock, Formatting.None);

                TotalAcceptedShare++;

                ShareResult?.Invoke(this, true, "Accepted");
                NewJob?.Invoke(this, powInformation);
            }
            else
            {
                TotalRejectedShare++;
                ShareResult?.Invoke(this, false, "Invalid share.");
            }
        }
    }
}