using ON.Fragments.Content;
using ON.Fragments.Settings;
using System.Collections.Generic;
using System.Linq;

namespace SubverseWeb.Models.CMS
{
    public class CategoryIndexViewModel
    {
        public const int NUM_COLS = 4;

        public List<CategoryRecord> Categories { get; set; }
        public int RowSize => (Categories.Count + NUM_COLS - 1) / NUM_COLS;
        public IEnumerable<CategoryRecord> Column1 => Categories.Take(RowSize);
        public IEnumerable<CategoryRecord> Column2 => Categories.Skip(RowSize * 1).Take(RowSize);
        public IEnumerable<CategoryRecord> Column3 => Categories.Skip(RowSize * 2).Take(RowSize);
        public IEnumerable<CategoryRecord> Column4 => Categories.Skip(RowSize * 3);
    }
}
