using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];

                if (nodesByCharacter.TryGetValue(c, out var existingCharacterNode))
                {
                    existingCharacterNode.Weight++;
                }
                else
                {
                    var internalNode = new Node { Type = NodeType.Internal };
                    var newCharacterNode = new Node { Type = NodeType.Character, Character = c, Weight = 1 };
                    var oldNytParent = nyt.Parent;

                    if (oldNytParent != null)
                    {
                        oldNytParent.SetChildren(internalNode, oldNytParent.Right);
                    }
                    else
                    {
                        root = internalNode;
                    }

                    internalNode.SetChildren(nyt, newCharacterNode);

                    nodesByCharacter.Add(newCharacterNode.Character.Value, newCharacterNode);
                }

                treeDrawer.DrawNodesToFile($"{ResultsDirectory}/tree{i}.png", NodeUtils.PrepareDrawNodes(root).ToDictionary(dn => dn.OrderNumber));

                var characterNodesWithOrderNumberByCharacter = NodeUtils.FlattenNodes(new[] { root })
                    .Reverse()
                    .Select((node, index) => (node, index))
                    .Where(t => t.node.Type == NodeType.Character)
                    .ToDictionary(t => t.node.Character);

                var rows = root.BuildDataTable(string.Empty).Select(t => new DataTableRow(
                    characterNodesWithOrderNumberByCharacter[t.character].index,
                    t.character,
                    t.occurrences,
                    t.code)).OrderByDescending(r => r.OrderNumber)
                    .ToArray();

                var averageCodeWordLength = CalculateAverageCodewordLength(rows);
                var entropy = CalculateEntropy(rows);

                statsTable.Add(new StatsTableRow(c, averageCodeWordLength, entropy));

                dataWriter.WriteToFile(
                    $"{ResultsDirectory}/tree{i}.csv",
                    new[] { "Order number", "Character", "Occurrences", "Codeword" },
                    rows);

                Console.WriteLine($"Odczytano znak [{c}], pomyślnie zapisano grafikę oraz tabelę danych.");
            }

            dataWriter.WriteToFile(
                $"{ResultsDirectory}/_stats.csv",
                new[] { "Character", "Average Codeword Length", "Entropy" },
                statsTable);

            Console.WriteLine("Kodowanie zakończone");
            Console.ReadKey();
        }

        private static double CalculateAverageCodewordLength(DataTableRow[] rows)
        {
            var allCharsOccurrences = rows.Sum(r => r.Occurrences);

            return rows.Sum(row => (row.Occurrences / (double)allCharsOccurrences) * row.CodeWord.Length);
        }

        private static double CalculateEntropy(DataTableRow[] rows)
        {
            var allCharsOccurrences = rows.Sum(r => r.Occurrences);

            return rows.Sum(row =>
            {
                var p = row.Occurrences / (double)allCharsOccurrences;

                return p * Math.Log2(1.0 / p);
            });
        }

        private static void ClearImagesDirectory()
        {
            if (!Directory.Exists(ResultsDirectory))
                Directory.CreateDirectory(ResultsDirectory);

            foreach (var filePath in Directory.GetFiles(ResultsDirectory))
                File.Delete(filePath);
        }

        private const string ResultsDirectory = "results";
    }
}
