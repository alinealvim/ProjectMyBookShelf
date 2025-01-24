using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.ViewModels;
using System.Security.Claims;

namespace MyBookShelf.Components.Pages.Account
{
    public partial class Login
    {
        private const string LoginForm = "login-form";
        [SupplyParameterFromForm(FormName = LoginForm)]
        public LoginViewModel Model { get; set; } = new();
        private string? errorMessage;
        [SupplyParameterFromQuery]
        public string ReturnUrl { get; set; } = "/";

        private async Task LoginUserAsync()
        {
            var userAccount = appDbContext.Users.Where(x => x.Username == Model.Username).FirstOrDefault();

            if ((userAccount == null) || (!PasswordService.CheckPassword(Model.Password!, userAccount?.Password!)))
            {
                errorMessage = "Usuário e/ou Senha inválido(s).";
                return;
            }            

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userAccount!.UserID.ToString()),
                new(ClaimTypes.Name, userAccount.Username),
                new(ClaimTypes.Role, userAccount.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                navigationManager.NavigateTo(ReturnUrl);
            }
            else
            {
                errorMessage = "Não foi possível acessar o HttpContext.";
            }

        }
    }
}