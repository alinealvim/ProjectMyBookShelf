using BlazorBootstrap;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyBookShelf.Data;
using MyBookShelf.Models.Entities;
using MyBookShelf.Models.ViewModels;
using MyBookShelf.Services;
using System.Security.Claims;

namespace MyBookShelf.Components.Pages.Account
{
    public partial class Register
    {
       
        private string? errorMessage;
       

        [SupplyParameterFromForm]
        public RegisterViewModel Model { get; set; } = new();
       
        
        private async Task RegisterUser()
        {
            // Verifica se o username já existe
            var existingUser = await appDbContext.Users
                .FirstOrDefaultAsync(u => u.Username == Model.Username);

            if (existingUser != null)
            {
                errorMessage = "Por favor, escolha outro nome de utilizador.";
                return; // Interrompe o registro se o username já existir
            }

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

            //var claims = new List<Claim>
            //{
            //    new(ClaimTypes.Name, Model.Username!),
            //    new(ClaimTypes.NameIdentifier, userAccount.UserID.ToString()),
            //    new(ClaimTypes.Role, userAccount.Role ),
            //};

            //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //var principal = new ClaimsPrincipal(identity);
            //var httpContext = HttpContextAccessor.HttpContext;
            //if (httpContext != null)
            //{
                //await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                navigationManager.NavigateTo("/login");
            //}
            //else
            //{
            //    errorMessage = "Não foi possível acessar o HttpContext.";
                
            //}
        }
    }
}