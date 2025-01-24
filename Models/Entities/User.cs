namespace MyBookShelf.Models.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswer { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public ICollection<UserBook> UserBooks { get; set; } = [];
    }
}

