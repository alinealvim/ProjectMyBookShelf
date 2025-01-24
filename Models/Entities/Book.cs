using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBookShelf.Models.Entities
{
    public class Book
    {
        public int BookID { get; set; }
        public required string Title { get; set; }
        public required string Genre { get; set; }
        public required string Author { get; set; }
        public int Pages { get; set; }
        public ICollection<UserBook> UserBooks { get; set; } = [];
    }
}