using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBookShelf.Data;
using MyBookShelf.Models.Entities;
using MyBookShelf.Models.ViewModels;
using MyBookShelf.Services;

namespace MyBookShelf.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;

        public UsersController(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // 1. Listar todos os utilizadores
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserAccountViewModel
                {
                    Id = u.UserID,
                    Username = u.Username,
                    Role = u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        // 2. Adicionar novo utilizador
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var newUser = new User
            {
                Username = model.Username!,
                Password = _passwordService.EncryptPassword(model.Password!), 
                SecurityQuestion = model.SecurityQuestion!,                
                Role = model.Role!
            };

            if (!string.IsNullOrWhiteSpace(model.SecurityAnswer))
            {
                newUser.SecurityAnswer = _passwordService.EncryptPassword(model.SecurityAnswer);
            }

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Utilizador criado com sucesso!" });
        }

        // 3. Atualizar utilizador existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { Message = "Utilizador não encontrado." });

            user.Username = model.Username!;            
            user.Role = model.Role!;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Utilizador atualizado com sucesso!" });
        }

        // 4. Excluir utilizador
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { Message = "Utilizador não encontrado." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Utilizador excluído com sucesso!" });
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedUsers(int page = 1, int pageSize = 10)
        {
            // Garantir que o número da página e o tamanho da página sejam válidos
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var usersQuery = _context.Users
                .Select(u => new UserAccountViewModel
                {
                    Id = u.UserID,
                    Username = u.Username,
                    Role = u.Role
                });

            // Contar o número total de registos
            var totalUsers = await usersQuery.CountAsync();

            // Obter a lista paginada
            var users = await usersQuery
                .Skip((page - 1) * pageSize)  // Pular os itens anteriores
                .Take(pageSize)  // Pegar o número de itens da página
                .ToListAsync();

            // Retornar os dados paginados junto com informações de total de páginas
            var result = new
            {
                TotalItems = totalUsers,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Data = users
            };

            return Ok(result);
        }

        // Método para redefinir a senha
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordViewModel passwordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var user = await _context.Users.FindAsync(passwordModel.UserId);
            if (user == null)
                return NotFound(new { Message = "Utilizador não encontrado." });
            
            user.Password = _passwordService.EncryptPassword(passwordModel.NewPassword!);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Senha redefinida com sucesso." });
        }

        [HttpPost("password-reset/validate-security-question")]
        public async Task<IActionResult> ValidateSecurityQuestion([FromBody] ValidateSecurityModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
                return NotFound("Utilizador não encontrado.");

            if (user.SecurityQuestion != model.Question)
                return BadRequest("Pergunta de segurança inválida.");

            if (!_passwordService.CheckPassword(model.Answer, user.SecurityAnswer))
                return BadRequest("Resposta incorreta.");

            //Gere um token para permitir a redefinição de senha
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.TokenExpiry = DateTime.UtcNow.AddMinutes(30);
            await _context.SaveChangesAsync();

            return Ok(new { Token = user.PasswordResetToken });

        }        

        [HttpPost("password-reset/confirm")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] ResetPasswordModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token);

            if (user == null || user.TokenExpiry < DateTime.UtcNow)
                return BadRequest("Token inválido ou expirado.");

            user.Password = _passwordService.EncryptPassword(model.NewPassword!); // Atualiza a senha
            user.PasswordResetToken = null; // Limpa o token
            user.TokenExpiry = null;       // Remove a validade
            await _context.SaveChangesAsync();

            return Ok("Senha redefinida com sucesso.");
        }
    }
}

