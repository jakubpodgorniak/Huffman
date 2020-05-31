namespace Huffman
{
    public class DataTableRow : ITableRow
    {
        public DataTableRow(int orderNumber, char character, int occurrences, string code)
        {
            OrderNumber = orderNumber;
            Character = character;
            Occurrences = occurrences;
            CodeWord = code;
        }

        public int OrderNumber { get; set; }

        public char Character { get; set; }

        public int Occurrences { get; set; }

        public string CodeWord { get; set; }

        public string CreateCsvRow(char separator)
        {
            var parts = new[] { OrderNumber.ToString(), Character.ToString(), Occurrences.ToString(), CodeWord };

            return string.Join(separator, parts);
        }
    }
}
