using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace 内容相似度比较.Tools
{
    public class SimpleSimHash
    {
        private readonly int hashBits = 64;

        // 简单中文按字分词，也可替换为 jieba.NET
        public List<string> Tokenize(string text)
        {
            return text.ToCharArray()
                       .Where(c => !char.IsWhiteSpace(c))
                       .Select(c => c.ToString())
                       .ToList();
        }

        public ulong ComputeSimHash(string text)
        {
            int[] bitVector = new int[hashBits];
            var tokens = Tokenize(text);

            var freq = tokens.GroupBy(t => t)
                             .ToDictionary(g => g.Key, g => g.Count());

            foreach (var pair in freq)
            {
                ulong hash = Hash64(pair.Key);
                int weight = pair.Value;

                for (int i = 0; i < hashBits; i++)
                {
                    int bit = ((hash >> i) & 1) == 1 ? 1 : -1;
                    bitVector[i] += bit * weight;
                }
            }

            ulong simhash = 0;
            for (int i = 0; i < hashBits; i++)
            {
                if (bitVector[i] > 0)
                    simhash |= (1UL << i);
            }
            return simhash;
        }

        private ulong Hash64(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToUInt64(hash, 0); // 截断前8字节
            }
        }

        public int HammingDistance(ulong h1, ulong h2)
        {
            ulong x = h1 ^ h2;
            int count = 0;
            while (x != 0)
            {
                x &= (x - 1);
                count++;
            }
            return count;
        }
    }
}
