# Tuckfirtle
This is the main tuckfirtle cryptocurrency repository.

#### Table of contents:
- [About](#About)
- [Basic Information](#Basic-Information)
- [Donations](#Donations)
- [Developers](#Developers)

## About
Tuckfirtle is an open source cryptocurrency coin written from scratch in C#.
This coin is an educational project aim to be simple and do not have the means to offer privacy like Monero do.
As the coin follows the bitcoin specification to a certain extend, there may be chunks of codes that look similar to bitcoin.
Rest assured that every code written here are original and not a result of copy paste from C++ to C#.

The reason not to fork an existing coin and use their code as base is due to complexity and C++ languge can be very demanding for entry level developers to understand.
The dependencies for C++ can be huge and take a very long time to compile. This is not ideal for my goal of being simple.
Forking existing coin tend to not do well in long run and thus the reason to write from scratch.

Since this is written from scratch, there are tons of test needed before this project can be lanched as a real coin. Do not expect the coin to be released any time soon.

This coin will aim to be fully decentralized with a minimum of 1 second block time and indefinate amount of supply to create incentive to mine.
The block will be created as soon as the block is found. The miner rewards is capped to 1 TF per minute. The longer the time to find a block, the more reward the person gets.
There will be no transaction limit per block on the chain. If the node can handle more, it will include more transactions in one go.
There will not be much privacy equipped like Monero did with ring signature and mixins. These are reserved for future forks of the coin.

More information about this coin will be written when we have iron out the idea.

## Basic Information
- Coin Name: Tuckfirtle (TF)
- Github: https://github.com/Tuckfirtle/Tuckfirtle
- Discord: http://discord.gg/eEXTB3T
- Currency Mining Type: Proof of work (POW)
- POW Mining Algorithm: RandomX
- Minimum Block Time: 1 second (Difficulty will drop every minute of mining if it is not found)
- Block Reward: Capped at 1 TF/Minute.
- Total Supply: Infinity. (Based on the chain limitation, it would most likely be 184,467,440,737 TF aprox 350965.450412861 years to reach)
- Premine: 0%

## Donations
- BTC: `3Dc5jpiyuts136YhamcRbAeue7mi44gW8d`
- LTC: `LUU9Avuanafmq1vMp53AWS1mr3GCCc2X42`
- XMR: `42oj7eV68BK8Z8wcGzLMFEJgAQG22Z3ajGdtpmJx5p7iDqEgG91wNybWbwaVe4vUMveKAzAiA4j8xgUi29TpKXpm3zwfwWN`

## Developers
- Yong Jian Ming
