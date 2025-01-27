using System.ComponentModel.DataAnnotations;

namespace MyBookShelf.Models.ViewModels
{
    public class PasswordViewModel
    {
        public int UserId { get; set; }  // Identificador do utilizador (caso necessário para a requisição)

        [Required(ErrorMessage = "A nova palavra-passe é obrigatória.")]
        [StringLength(100, ErrorMessage = "A palavra-passe deve ter pelo menos {2} caracteres.", MinimumLength = 5)]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "A confirmação da palavra-passe é obrigatória.")]
        [Compare("NewPassword", ErrorMessage = "A confirmação da palavra-passe não coincide com a nova palavra-passe.")]
        public string? ConfirmPassword { get; set; }
    }
}
