namespace MyBookShelf.Models.ViewModels
{
    public class ResetPasswordModel
    {
        public required string Token { get; set; }
        public string? NewPassword { get; set; }
    }
}
