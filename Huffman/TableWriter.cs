using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Huffman
{
    public class TableWriter
    {
        public void WriteToFile(
            string filename,
            string[] headers,
            IEnumerable<ITableRow> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join('\t', headers));

            foreach (var row in rows)
            {
                sb.AppendLine(row.CreateCsvRow('\t'));
            }

            File.WriteAllText(filename, sb.ToString());
        }
    }
}
