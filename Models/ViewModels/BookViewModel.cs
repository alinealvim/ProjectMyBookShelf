using System.ComponentModel.DataAnnotations;

namespace MyBookShelf.Models.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, insira um título.")]
        public string? Title { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, insira um género.")]
        public string? Genre { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, insira um autor.")]
        public string? Author { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "O número de páginas deve ser maior que zero.")]
        public int Pages { get; set; }
    }
}
