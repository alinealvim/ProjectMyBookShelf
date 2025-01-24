using System.ComponentModel.DataAnnotations;

namespace MyBookShelf.Models.ViewModels
{
    public class PasswordViewModel
    {
        public int UserId { get; set; }  // Identificador do usuário (caso necessário para a requisição)

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.", MinimumLength = 5)]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "A confirmação da senha é obrigatória.")]
        [Compare("NewPassword", ErrorMessage = "A confirmação da senha não coincide com a nova senha.")]
        public string? ConfirmPassword { get; set; }
    }
}
