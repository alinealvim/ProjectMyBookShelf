using System.ComponentModel.DataAnnotations;

namespace MyBookShelf.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, insira seu nome de utilizador.")]
        public string? Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, insira sua palavra-passe.")]
        public string? Password { get; set; }
    }
}

