using System.ComponentModel.DataAnnotations;
namespace MyBookShelf.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "O nome de utilizador é obrigatório.")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [MinLength(5, ErrorMessage = "A palavra-passe deve ter pelo menos 5 caracteres.")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "A confirmação da palavra-passe é obrigatória.")]
        [Compare("Password", ErrorMessage = "As palavras-passes não coincidem.")]
        public string? ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "A pergunta é obrigatória.")]
        public string? SecurityQuestion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "A resposta é obrigatória.")]
        public string? SecurityAnswer { get; set; }
    }
}
