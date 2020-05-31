using System;
using System.Collections.Generic;
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
            var dataWriter = new DataTableWriter();

            var nodesByCharacter = new Dictionary<char, Node>();

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

                treeDrawer.DrawNodesToFile($"{ImagesDirectory}/tree{i}.png", NodeUtils.PrepareDrawNodes(root).ToDictionary(dn => dn.OrderNumber));

                var characterNodesWithOrderNumberByCharacter = NodeUtils.FlattenNodes(new[] { root })
                    .Reverse()
                    .Select((node, index) => (node, index))
                    .Where(t => t.node.Type == NodeType.Character)
                    .ToDictionary(t => t.node.Character);

                var rows = root.BuildDataTable(string.Empty).Select(t => new DataTableRow(
                    characterNodesWithOrderNumberByCharacter[t.character].index,
                    t.character,
                    t.occurrences,
                    t.code)).OrderByDescending(r => r.OrderNumber);

                dataWriter.WriteToFile($"{ImagesDirectory}/tree{i}.csv", rows);
            }

            Console.ReadKey();
        }

        private static void ClearImagesDirectory()
        {
            if (!Directory.Exists(ImagesDirectory))
                Directory.CreateDirectory(ImagesDirectory);

            foreach (var filePath in Directory.GetFiles(ImagesDirectory))
                File.Delete(filePath);
        }

        private const string ImagesDirectory = "images";
    }
}
