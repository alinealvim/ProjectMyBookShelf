using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyBookShelf.Models.Entities
{
    public class ReadingHistory
    {
        public int ReadingHistoryID { get; set; }
        public int UserBookID { get; set; }
        public UserBook? UserBook { get; set; }
        public int PagesRead { get; set; }
        public DateTime? Date { get; set; }
    }
}
