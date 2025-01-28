using System.ComponentModel.DataAnnotations;

namespace MyBookShelf.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome de utilizador é obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome de utilizador deve ter entre 3 e 50 caracteres.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "A palvra-passe deve ter pelo menos 5 caracteres.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "A função é obrigatória.")]
        [RegularExpression(@"^(Administrator|User)$", ErrorMessage = "A função deve ser Administrator ou User.")]
        public string? Role { get; set; }
        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswer { get; set; }
        
    }
}
