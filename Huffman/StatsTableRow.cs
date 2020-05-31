namespace Huffman
{
    public class StatsTableRow : ITableRow
    {
        public StatsTableRow(
            char character,
            double averageCodewordLength,
            double entropy)
        {
            Character = character;
            AverageCodewordLength = averageCodewordLength;
            Entropy = entropy;
        }

        public char Character { get; }

        public double AverageCodewordLength { get; }

        public double Entropy { get; }

        public string CreateCsvRow(char separator)
        {
            var parts = new[] { Character.ToString(), AverageCodewordLength.ToString(), Entropy.ToString() };

            return string.Join(separator, parts);
        }
    }
}
