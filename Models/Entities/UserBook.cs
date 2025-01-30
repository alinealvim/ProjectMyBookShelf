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

        // Calcula o tempo da leitura
        public int? GetDaysSinceFirstReading()
        {
            var firstDate = ReadingHistories
                .Where(r => r.Date.HasValue)
                .OrderBy(r => r.Date)
                .Select(r => r.Date)
                .FirstOrDefault();

            var endDate = ReadingHistories
                .Where(r => r.Date.HasValue)
                .OrderByDescending(r => r.Date)
                .Select(r => r.Date)
                .FirstOrDefault();

            if ( EndDate.HasValue && firstDate.HasValue && endDate.HasValue) 
            {
                var dateTarget = (endDate - firstDate).Value.Days;
                return dateTarget;
            }
            else
            {
                return firstDate.HasValue ? (DateTime.UtcNow.Date - firstDate.Value.Date).Days : null;
            }

            
           
        }

    }

    public enum BookStatus
    {
        ParaLer,
        ALer,
        Lido
    }
}
