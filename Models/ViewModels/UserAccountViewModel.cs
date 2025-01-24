using System.ComponentModel.DataAnnotations;

namespace MyBookShelf.Models.ViewModels
{
    public class UserAccountViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O username é obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O username deve ter entre 3 e 50 caracteres.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "A senha deve ter pelo menos 5 caracteres.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "A função é obrigatória.")]
        [RegularExpression(@"^(Administrator|User)$", ErrorMessage = "A função deve ser Administrator ou User.")]
        public string? Role { get; set; }
        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswer { get; set; }
        
    }
}
