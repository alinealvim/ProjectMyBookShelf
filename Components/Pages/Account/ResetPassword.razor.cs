using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.ViewModels;

namespace MyBookShelf.Components.Pages.Account
{
    public partial class ResetPassword
    {

        
        [SupplyParameterFromQuery]
        public string? Token { get; set; }
        private const string ResetPasswordForm = "reset-password-form";
        [SupplyParameterFromForm(FormName = ResetPasswordForm)]
        private ResetPasswordModel? ResetModel { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Token) && ResetModel == null)
            {
                ResetModel = new ResetPasswordModel { Token = Token };
            }
            await base.OnParametersSetAsync();
        }

        private async Task SubmitNewPassword()
        {
            var response = await Http.PostAsJsonAsync("api/users/password-reset/confirm", ResetModel);
            if (response.IsSuccessStatusCode)
            {

                Console.WriteLine("Sucesso");
                NavigationManager.NavigateTo("/");
            }
            else
            {
                Console.WriteLine("Falha");
            }
            
        }
    }
}