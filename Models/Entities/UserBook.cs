using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBookShelf.Models.Entities
{
    [Table("UserBook")]
    public class UserBook
    {
        public int UserBookID { get; set; }
        public int UserID { get; set; }
        public User? User { get; set; }
        public int BookID { get; set; }
        public Book? Book { get; set; }
        public BookStatus Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CurrentPage { get; set; }
        public string? Notes { get; set; }
        public int? Rating { get; set; }
        public ICollection<ReadingHistory> ReadingHistories { get; set; } = [];

        // Calcula o progresso da leitura
        public int GetProgress()
        {
            if (Book!.Pages == 0) return 0;
            return (int)((double)CurrentPage! / Book!.Pages * 100);
        }
    }

    public enum BookStatus
    {
        ParaLer,
        ALer,
        Lido
    }
}
