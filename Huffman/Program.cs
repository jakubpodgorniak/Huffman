using System;
using System.IO;
using System.Linq;
using Huffman.Drawing;

namespace Huffman
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ClearImagesDirectory();

            var input = File.ReadAllText(args[0]);

            var root = new Node { Type = NodeType.Null, Weight = 0 };
            var nyt = root;

            var treeDrawer = new TreeDrawer();

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];

                treeDrawer.DrawNodesToFile($"{ImagesDirectory}/tree{i}.png", NodeUtils.PrepareDrawNodes(root).ToDictionary(dn => dn.OrderNumber));
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
