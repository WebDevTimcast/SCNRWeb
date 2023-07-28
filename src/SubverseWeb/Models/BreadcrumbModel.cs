using System.Collections.Generic;

namespace SubverseWeb.Models
{
    public class BreadcrumbModel
    {
        public string Title { get; set; }
        public List<Path> Paths { get; set; } = new();

        public class Path
        {
            public bool Active { get; set; } = false;
            public string Label { get; set; } = "";
            public string Link { get; set; } = "";
        }
    }
}
