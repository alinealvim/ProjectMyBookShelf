using System.ComponentModel.DataAnnotations;
namespace MyBookShelf.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "O username é obrigatório.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(5, ErrorMessage = "A senha deve ter pelo menos 5 caracteres.")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "A confirmação da senha é obrigatória.")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
        public string? ConfirmPassword { get; set; }
        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswer { get; set; }
    }
}
