namespace Huffman
{
    public class DataTableRow
    {
        public DataTableRow(int orderNumber, char character, int occurrences, string code)
        {
            OrderNumber = orderNumber;
            Character = character;
            Occurrences = occurrences;
            Code = code;
        }

        public int OrderNumber { get; set; }

        public char Character { get; set; }

        public int Occurrences { get; set; }

        public string Code { get; set; }

        public string CreateCsvRow(char separator)
        {
            var parts = new[] { OrderNumber.ToString(), Character.ToString(), Occurrences.ToString(), Code };

            return string.Join(separator, parts);
        }
    }
}
