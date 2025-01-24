using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.Entities;
using MyBookShelf.Models.ViewModels;
using System.Security.Claims;

namespace MyBookShelf.Components.Pages.Account
{
    public partial class Register
    {
        [SupplyParameterFromForm]
        public RegisterViewModel Model { get; set; } = new();
        private string? errorMessage;
        private async Task RegisterUser()
        {
            var userAccount = new User
            {
                Username = Model.Username!,
                Password = PasswordService.EncryptPassword(Model.Password!),
                SecurityQuestion = Model.SecurityQuestion!,
                SecurityAnswer = PasswordService.EncryptPassword(Model.SecurityAnswer!),
                Role = "User"
            };
            await appDbContext.Users.AddAsync(userAccount);
            await appDbContext.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, Model.Username!),
                new(ClaimTypes.NameIdentifier, userAccount.UserID.ToString()),
                new(ClaimTypes.Role, userAccount.Role ),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                navigationManager.NavigateTo("/");
            }
            else
            {
                errorMessage = "Não foi possível acessar o HttpContext.";
            }
        }
    }
}