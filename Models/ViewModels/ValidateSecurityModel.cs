namespace MyBookShelf.Models.ViewModels
{
    public class ValidateSecurityModel
    {
        public required string Username { get; set; }
        public required string Question { get; set; }
        public required string Answer { get; set; }
    }
}
