using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.ViewModels;
using System.Security.Claims;

namespace MyBookShelf.Components.Pages.Account
{

    public partial class Login
    {
        private const string LoginForm = "login-form"; // Nome do formulário utilizado para o login.

        // Propriedade que representa o modelo do formulário de login.
        // Atributo [SupplyParameterFromForm] vincula o modelo ao formulário com o nome especificado.
        [SupplyParameterFromForm(FormName = LoginForm)]
        public LoginViewModel Model { get; set; } = new();

        private string? errorMessage;

        // Propriedade que captura a URL de retorno (redirecionamento após o login).
        // Atributo [SupplyParameterFromQuery] indica que a URL será obtida da query string.
        [SupplyParameterFromQuery]
        public string ReturnUrl { get; set; } = "/";

        // Método assíncrono que realiza o processo de login do utilizador.
        private async Task LoginUserAsync()
        {
            // Busca o utilizador na base de dados a partir nome de utilizador fornecido no formulário.
            var userAccount = appDbContext.Users.Where(x => x.Username == Model.Username).FirstOrDefault();

            // Verifica se o utilizador foi encontrado e se a palavra-passe fornecida é válida.
            if ((userAccount == null) || (!PasswordService.CheckPassword(Model.Password!, userAccount?.Password!)))
            {
                errorMessage = "Utilizador e/ou palavra-passe inválido(s)."; // Define mensagem de erro.
                return; // Interrompe o processo de login.
            }

            // Cria uma lista de claims (declarações) para identificar o utilizador autenticado.
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userAccount!.UserID.ToString()), // Identificador único do utilizador.
                new(ClaimTypes.Name, userAccount.Username), // Nome do utilizador.
                new(ClaimTypes.Role, userAccount.Role) // Papel do utilizador (ex.: Admin, user).
            };

            // Cria uma identidade baseada nos claims e o esquema de autenticação por cookies.
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Cria o principal (representação do utilizador autenticado) com base na identidade.
            var principal = new ClaimsPrincipal(identity);

            // Obtém o contexto HTTP atual para realizar a autenticação.
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Realiza o login do utilizador utilizando o esquema de autenticação baseado em cookies.
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redireciona o utilizador para a URL de retorno especificada.
                navigationManager.NavigateTo(ReturnUrl);
            }
            else
            {
                errorMessage = "Não foi possível acessar o HttpContext.";
            }
        }
    }
}
