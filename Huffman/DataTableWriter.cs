using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman
{
    public class DataTableWriter
    {
        public void WriteToFile(string filename, IEnumerable<DataTableRow> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Order Number;Character;Occurrences;Code");

            foreach (var row in rows)
            {
                sb.AppendLine(row.CreateCsvRow(';'));
            }

            File.WriteAllText(filename, sb.ToString());
        }
    }
}
