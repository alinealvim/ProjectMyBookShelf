using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBookShelf.Data;
using MyBookShelf.Models.Entities;
using MyBookShelf.Models.Response;
using MyBookShelf.Models.ViewModels;

namespace MyBookShelf.Controllers
{

    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // Verificar se o livro já existe (GET /api/books/exists)
        [HttpGet("exists")]
        public async Task<IActionResult> CheckBookExists([FromQuery] string title, [FromQuery] string author)
        {
            var bookExists = await _context.Books
                .AnyAsync(b => b.Title == title && b.Author == author);

            return Ok(bookExists); // Retorna 'true' se já existir um livro com o mesmo título e autor
        }


        // 1. Listar todos os livros (GET /api/books)
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _context.Books
                .Select(b => new BookViewModel
                {
                    Id = b.BookID,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    Pages = b.Pages
                })
                .ToListAsync();

            return Ok(books);
        }

        // 2. Obter detalhes de um livro (GET /api/books/{id})
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _context.Books
                .Where(b => b.BookID == id)
                .Select(b => new BookViewModel
                {
                    Id = b.BookID,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    Pages = b.Pages
                })
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound(new { Message = "Livro não encontrado." });
            }

            return Ok(book);
        }

        // 3. Adicionar um novo livro (POST /api/books)
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dados inválidos.");
            }

            var newBook = new Book
            {
                Title = model.Title!,
                Genre = model.Genre!,
                Author = model.Author!,
                Pages = model.Pages
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            var response = new BookResponse { Message = "Livro adicionado com sucesso!", BookId = newBook.BookID };

            return Ok(response);
        }

        // 4. Atualizar um livro (PUT /api/books/{id})
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dados inválidos.");
            }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null) 
            {
                return NotFound(new { Message = "Livro não encontrado." });
            }

            existingBook.Title = model.Title!;
            existingBook.Author = model.Author!;
            existingBook.Genre = model.Genre!;
            existingBook.Pages = model.Pages;


            _context.Books.Update(existingBook);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livro atualizado com sucesso!" });
        }

        // 5. Deletar um livro (DELETE /api/books/{id})
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { Message = "Livro não encontrado." });
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livro excluído com sucesso!" });
        }


        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedBooks(int page = 1, int pageSize = 10)
        {
            // Garantir que o número da página e o tamanho da página sejam válidos
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var booksQuery = _context.Books
                .Select(u => new BookViewModel
                {
                    Id = u.BookID,
                    Title = u.Title,
                    Genre = u.Genre,
                    Author = u.Author,
                    Pages = u.Pages
                });

            // Contar o número total de registos
            var totalBooks = await booksQuery.CountAsync();

            // Obter a lista paginada
            var books = await booksQuery
                .Skip((page - 1) * pageSize)  // Pular os itens anteriores
                .Take(pageSize)  // Pegar o número de itens da página
                .ToListAsync();

            // Retornar os dados paginados junto com informações de total de páginas
            var result = new
            {
                TotalItems = totalBooks,
                TotalPages = (int)Math.Ceiling(totalBooks / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Data = books
            };

            return Ok(result);
        }

        // Buscar livros por termo (GET /api/books/search)
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new { Message = "O termo de pesquisa não pode estar vazio." });
            }

            // Busca nos campos relevantes (Título, Autor, Gênero)
            var filteredBooks = await _context.Books
                .Where(b => EF.Functions.Like(b.Title, $"%{term}%") ||
                            EF.Functions.Like(b.Author, $"%{term}%") ||
                            EF.Functions.Like(b.Genre, $"%{term}%"))
                .Select(b => new BookViewModel
                {
                    Id = b.BookID,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    Pages = b.Pages
                })
                .ToListAsync();

            if (!filteredBooks.Any())
            {
                return NotFound(new { Message = "Nenhum livro encontrado com o termo fornecido." });
            }

            return Ok(filteredBooks);
        }


    }
}
