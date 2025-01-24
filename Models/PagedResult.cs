namespace MyBookShelf.Models
{
    public class PagedResult<T>
    {        
        public int TotalPages { get; set; }        
        public List<T>? Data { get; set; }
    }
}
