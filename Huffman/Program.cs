using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Huffman.Drawing;

namespace Huffman
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ClearImagesDirectory();

            var input = File.ReadAllText(args[0]);

            var root = new Node { Type = NodeType.Null, Weight = 0 };
            var nyt = root;

            var treeDrawer = new TreeDrawer();
            var dataWriter = new TableWriter();

            var nodesByCharacter = new Dictionary<char, Node>();
            var statsTable = new List<StatsTableRow>();
            var rows = new DataTableRow[0];

            foreach (char c in input)
            {
                if (nodesByCharacter.TryGetValue(c, out var existingCharacterNode))
                    existingCharacterNode.Weight++;
                else
                {
                    var parentNode = Node.NewInternal;
                    var newCharacterNode = Node.NewCharacter(c);
                    nodesByCharacter.Add(c, newCharacterNode);
                    
                    var oldNytParent = nyt.Parent;

                    if (oldNytParent is null)
                        root = parentNode;
                    else
                        oldNytParent.SetChildren(parentNode, oldNytParent.Right);

                    parentNode.SetChildren(nyt, newCharacterNode);

                }

                //treeDrawer.DrawNodesToFile($"{ResultsDirectory}/tree{i}.png", NodeUtils.PrepareDrawNodes(root).ToDictionary(dn => dn.OrderNumber));

                var characterNodesWithOrderNumberByCharacter = NodeUtils.FlattenNodes(new[] { root })
                    .Reverse()
                    .Select((node, index) => (node, index))
                    .Where(t => t.node.Type == NodeType.Character)
                    .ToDictionary(t => t.node.Character);

                rows = root.BuildDataTable(string.Empty).Select(t => new DataTableRow(
                    characterNodesWithOrderNumberByCharacter[t.character].index,
                    t.character,
                    t.occurrences,
                    t.code)).OrderByDescending(r => r.OrderNumber)
                    .ToArray();

                var probabilityByChar = GetProbabilityByChar(rows);
                var averageCodeWordLength = CalculateAverageCodewordLength(rows, probabilityByChar);
                var entropy = CalculateEntropy(probabilityByChar.Values);

                statsTable.Add(new StatsTableRow(c, averageCodeWordLength, entropy));

                dataWriter.WriteToFile(
                    $"{ResultsDirectory}/tree{rows.Sum(r => r.Occurrences)}.csv",
                    new[] { "Order number", "Character", "Occurrences", "Codeword" },
                    rows);

                Console.WriteLine($"Odczytano znak [{c}], pomyślnie zapisano grafikę oraz tabelę danych.");
            }

            dataWriter.WriteToFile(
                $"{ResultsDirectory}/_stats.csv",
                new[] { "Character", "Average Codeword Length", "Entropy" },
                statsTable);

            var compressed = CompressText(
                rows.ToDictionary(r => r.Character, r => r.CodeWord),
                input);

            File.WriteAllBytes($"{ResultsDirectory}/compressed.huf", compressed);

            Console.WriteLine("Kodowanie zakończone");
            Console.ReadKey();
        }

        private static byte[] CompressText(IDictionary<char, string> codeByChar, string textToCompress)
        {
            var sb = new StringBuilder();
            foreach (var c in textToCompress)
                sb.Append(codeByChar[c]);
            var bytesStr = sb.ToString();

            var bytes = new List<byte>();
            while (bytesStr.Length > 8)
            {
                var byteStr = bytesStr.Substring(0, 8);

                bytes.Add(BitsStrToByte(byteStr));

                bytesStr = bytesStr.Substring(8);
            }

            bytes.Add(BitsStrToByte(bytesStr));

            return bytes.ToArray();
        }

        private static byte BitsStrToByte(string bitsStr)
        {
            bitsStr += string.Concat(Enumerable.Range(0, 8 - bitsStr.Length).Select(_ => "0"));

            byte b = 0;

            for (int i = 0; i < 8; i++)
            {
                if (bitsStr[7 - i] == '1')
                {
                    byte bit = (byte)(1 << i);

                    b |= bit;
                }
            }

            return b;
        }

        private static IDictionary<char, double> GetProbabilityByChar(DataTableRow[] rows)
        {
            var totalCharsOccurrences = rows.Sum(r => r.Occurrences);

            return rows.ToDictionary(
                r => r.Character,
                r => r.Occurrences / (double)totalCharsOccurrences);
        }

        private static double CalculateAverageCodewordLength(
            DataTableRow[] rows,
            IDictionary<char, double> probabilityByChar)
        {
            return rows.Sum(row => probabilityByChar[row.Character] * row.CodeWord.Length);
        }

        private static double CalculateEntropy(IEnumerable<double> probabilities)
        {
            return (-1.0) * probabilities.Sum(p => p * Math.Log2(p));
        }

        private static void ClearImagesDirectory()
        {
            if (!Directory.Exists(ResultsDirectory))
                Directory.CreateDirectory(ResultsDirectory);

            foreach (var filePath in Directory.GetFiles(ResultsDirectory))
                File.Delete(filePath);
        }

        private const string ResultsDirectory = "results2";
    }
}
