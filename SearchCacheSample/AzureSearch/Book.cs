using Microsoft.Azure.Search;
using System.ComponentModel.DataAnnotations;

namespace AzureSearch
{
    class Book
    {
        [Key]
        [IsFilterable, IsSortable]
        public string BookId { get; set; }

        [IsFilterable,IsSearchable, IsSortable]
        public string Name { get; set; }

        [IsFilterable,IsSearchable,IsFacetable]
        public string Genre { get; set; }

        [IsFilterable, IsSearchable, IsFacetable]
        public string Author { get; set; }

        [IsFilterable, IsSortable]
        public int Copies { get; set; }

        [IsFilterable]
        public bool IsIssued { get; set; }

        [IsFilterable]
        public bool IsDeleted { get; set; }
    }
}
