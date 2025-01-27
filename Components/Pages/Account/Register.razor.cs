using BlazorBootstrap;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MyBookShelf.Data;
using MyBookShelf.Models.Entities;
using MyBookShelf.Models.ViewModels;
using MyBookShelf.Services;
using System.Formats.Asn1;
using System.Security.Claims;


namespace MyBookShelf.Components.Pages.Account
{
    public partial class Register
    {

        private string? errorMessage;

        [SupplyParameterFromForm]
        public RegisterViewModel Model { get; set; } = new();

        Alert successAlert = default!;
        private bool isSuccessAlertVisible = false;
        private bool isErrorAlertVisible = false;

        private async Task RegisterUser()
        {
            try
            {
                // Verifica se o username já existe
                var existingUser = await appDbContext.Users
                    .FirstOrDefaultAsync(u => u.Username == Model.Username);

                if (existingUser != null)
                {
                    errorMessage = "Por favor, escolha outro nome de utilizador.";                    
                    return; // Interrompe o registo se o username já existir
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
                ShowAlert();
            }
            catch (Exception ex)
            {
                ShowAlert(false);
            }
        }

        private void ShowAlert(bool isSucess = true)
        {
            isSuccessAlertVisible = isSucess;
            isErrorAlertVisible = !isSuccessAlertVisible;
        }

        private void LoginSuccessAlert()
        {
            navigationManager.NavigateTo("/login");
        }
    }

}
