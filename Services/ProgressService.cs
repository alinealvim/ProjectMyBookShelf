using Microsoft.EntityFrameworkCore;
using MyBookShelf.Data;

namespace MyBookShelf.Services
{
    public class ProgressService : IProgressService
    {        
        private readonly IDbContextFactory<AppDbContext> _dbFactory;        
        public ProgressService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        //context per operation
        public async Task<List<ReadingStatusDTO>> GetReadingStatus(int userId, DateTime startDateTime, DateTime endDateTime) 
        {
            using var context = _dbFactory.CreateDbContext();
            var query = context.UserBooks.AsQueryable();
            query = query.Where(x => x.UserID == userId);
            // Filtro de StartDate (deve ser maior ou igual ao início do intervalo)
            query = query.Where(x => !x.StartDate.HasValue || x.StartDate.Value.Date >= startDateTime.Date);
            // Filtro de EndDate:
            // - EndDate NULL só será incluído se StartDate <= data final do intervalo
            // - EndDate não nulo deve estar dentro do intervalo e não ser menor que StartDate
            //query = query.Where(x =>
            //    (!x.EndDate.HasValue && x.StartDate.HasValue && x.StartDate.Value.Date <= endDateTime.Date) ||
            //    (x.EndDate.HasValue && x.EndDate.Value.Date <= endDateTime.Date && (!x.StartDate.HasValue || x.EndDate.Value.Date >= x.StartDate.Value.Date))
            //);
            query = query.Where(x =>
        (!x.EndDate.HasValue && (!x.StartDate.HasValue || x.StartDate.Value.Date <= endDateTime.Date)) ||
        (x.EndDate.HasValue && x.EndDate.Value.Date <= endDateTime.Date && (!x.StartDate.HasValue || x.EndDate.Value.Date >= x.StartDate.Value.Date))
    );
            var groupedData = await query
                .GroupBy(ub => ub.Status)
                .Select(g => new ReadingStatusDTO
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                }).ToListAsync();
            return groupedData;
        }

        //context per operation
        public async Task<List<PageStatusDTO>> GetPageStatus(int userId, DateTime startDateTime, DateTime endDateTime)
        {
            using var context = _dbFactory.CreateDbContext();
            var query = context.ReadingHistories.AsQueryable();
            query = query.Where(x => x.UserBook!.UserID == userId);
            query = query.Where(x => !x.Date.HasValue || x.Date.Value.Date >= startDateTime.Date);
            query = query.Where(x => !x.Date.HasValue || // Inclui nulos
                (x.Date.Value.Date >= startDateTime.Date && x.Date.Value.Date <= endDateTime.Date) // Dentro do intervalo
            );
            var groupedData = await query
                .GroupBy(r => r.Date!.Value.Date)
                .Select(g => new PageStatusDTO
                {
                    Date = g.Key.ToString("dd-MM-yyyy"),
                    PagesRead = g.Sum(r => r.PagesRead)
                })
                .ToListAsync();            
            return groupedData;
        }
    }


    public interface IProgressService
    {
        Task<List<ReadingStatusDTO>> GetReadingStatus(int userId, DateTime startDateTime, DateTime endDateTime);
        Task<List<PageStatusDTO>> GetPageStatus(int userId, DateTime startDateTime, DateTime endDateTime);
    }

    public class ReadingStatusDTO
    {
        public required string Status { get; set; }
        public int Count { get; set; }
    }

    public class PageStatusDTO
    {
        public required string Date { get; set; }
        public int PagesRead { get; set; }
    }
}
