using ON.Fragments.Content;
using ON.Fragments.Settings;
using System.Collections.Generic;
using System.Linq;

namespace SubverseWeb.Models.CMS
{
    public class TagIndexViewModel
    {
        public const int NUM_COLS = 4;

        public List<string> Tags { get; set; }
        public int RowSize => (Tags.Count + NUM_COLS - 1) / NUM_COLS;
        public IEnumerable<string> Column1 => Tags.Take(RowSize);
        public IEnumerable<string> Column2 => Tags.Skip(RowSize * 1).Take(RowSize);
        public IEnumerable<string> Column3 => Tags.Skip(RowSize * 2).Take(RowSize);
        public IEnumerable<string> Column4 => Tags.Skip(RowSize * 3);
    }
}
