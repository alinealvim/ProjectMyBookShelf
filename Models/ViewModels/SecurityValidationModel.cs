namespace MyBookShelf.Models.ViewModels
{
    public class SecurityValidationModel
    {
        public string? Username { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }

    public class TokenResponse
    {
        public required string Token { get; set; }
    }
}
