using System.ComponentModel.DataAnnotations;

namespace MyBookShelf.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, insira seu username.")]
        public string? Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, insira sua password.")]
        public string? Password { get; set; }
    }
}

