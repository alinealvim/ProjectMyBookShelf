using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.ViewModels;
using System.Security.Claims;

namespace MyBookShelf.Components.Pages.Account
{

    public partial class Login
    {
        private const string LoginForm = "login-form"; // Nome do formul�rio utilizado para o login.

        // Propriedade que representa o modelo do formul�rio de login.
        // Atributo [SupplyParameterFromForm] vincula o modelo ao formul�rio com o nome especificado.
        [SupplyParameterFromForm(FormName = LoginForm)]
        public LoginViewModel Model { get; set; } = new();

        private string? errorMessage;

        // Propriedade que captura a URL de retorno (redirecionamento ap�s o login).
        // Atributo [SupplyParameterFromQuery] indica que a URL ser� obtida da query string.
        [SupplyParameterFromQuery]
        public string ReturnUrl { get; set; } = "/";

        // M�todo ass�ncrono que realiza o processo de login do utilizador.
        private async Task LoginUserAsync()
        {
            // Busca o utilizador na base de dados a partir nome de utilizador fornecido no formul�rio.
            var userAccount = appDbContext.Users.Where(x => x.Username == Model.Username).FirstOrDefault();

            // Verifica se o utilizador foi encontrado e se a palavra-passe fornecida � v�lida.
            if ((userAccount == null) || (!PasswordService.CheckPassword(Model.Password!, userAccount?.Password!)))
            {
                errorMessage = "Utilizador e/ou palavra-passe inv�lido(s)."; // Define mensagem de erro.
                return; // Interrompe o processo de login.
            }

            // Cria uma lista de claims (declara��es) para identificar o utilizador autenticado.
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userAccount!.UserID.ToString()), // Identificador �nico do utilizador.
                new(ClaimTypes.Name, userAccount.Username), // Nome do utilizador.
                new(ClaimTypes.Role, userAccount.Role) // Papel do utilizador (ex.: Admin, user).
            };

            // Cria uma identidade baseada nos claims e o esquema de autentica��o por cookies.
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Cria o principal (representa��o do utilizador autenticado) com base na identidade.
            var principal = new ClaimsPrincipal(identity);

            // Obt�m o contexto HTTP atual para realizar a autentica��o.
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Realiza o login do utilizador utilizando o esquema de autentica��o baseado em cookies.
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redireciona o utilizador para a URL de retorno especificada.
                navigationManager.NavigateTo(ReturnUrl);
            }
            else
            {
                errorMessage = "N�o foi poss�vel acessar o HttpContext.";
            }
        }
    }
}
