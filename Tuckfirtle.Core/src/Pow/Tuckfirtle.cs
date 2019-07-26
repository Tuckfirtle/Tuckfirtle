namespace Tuckfirtle.Core.Pow
{
    public class Tuckfirtle
    {
        /*
         * Tuckfirtle is a POW algorithm that uses the pre-existing cryptography library available in microsoft.
         * Note that this is not proven to be relatively safe for production use. Use with caution.
         * We will implement SHA3 variants when it is available in microsoft library for performance reason.
         *
         * Disclaimer: This is not finished and may change overtime when I got new ideas.
         *
         * Input: (Based on the block data with random nonce) These will be in json and converted to UTF-8 bytes.
         * Output: The final POW value.
         *
         * START OF ALGORITHM:
         * Input will be hashed with SHA 512 to produce a 64 bytes of data.
         * SHA 512 output is 512 bits == 64 bytes.
         *
         * SHA 512 data will be split into groups of 16 bytes.
         * 1st and 2nd group of 16 bytes will be XOR to produce the first set of 16 bytes for AES Key.
         * 3rd and 4th group of 16 bytes will be XOR to produce the second set of 16 bytes for XOR Key.
         *
         * 1st and 4th group of 16 bytes will be XOR to produce the 5th set of 16 bytes data.
         * 2nd and 3rd group of 16 bytes will be XOR to produce the 6th set of 16 bytes data.
         *
         *                              XOR >===================================
         *                               ^                                     |
         *         ==============================================              |
         *         |                                            |              |
         *         |                    XOR >===================|==============|==============|
         *         |                     ^                      |              |              |
         *         |              ================              |              |              |
         *         v              v              v              v              v              v      
         * | 00..15 bytes | 16..31 bytes | 32..47 bytes | 48..63 bytes | 64..79 bytes | 80..95 bytes |
         *         ^              ^              ^              ^      
         *         ================              ================      
         *                v                             v              
         *               XOR                           XOR             
         *      00..15 bytes (AES Key)        16..31 bytes (XOR Key)   
         *
         * Now we have 96 bytes of data to play with, lets do some AES.
         *
         * For each 16 bytes of data, encrypt them with AES with the given AES key.
         */
    }
}