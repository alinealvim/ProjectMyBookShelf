using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.ViewModels;

namespace MyBookShelf.Components.Pages.Account
{
    public partial class SecurityAnswerValidation
    {
        [Inject] protected ToastService ToastService { get; set; }
        private void ShowMessage(ToastType toastType, string message) => ToastService.Notify(new(toastType, message));

        private const string SecurityAnswerValidationForm = "security-answer-validation-form";
        [SupplyParameterFromForm(FormName = SecurityAnswerValidationForm)]
        public SecurityValidationModel SecurityValidationModel { get; set; } = new();

        private async Task ValidateSecurityQuestion()
        {
            var response = await Http.PostAsJsonAsync("api/users/password-reset/validate-security-question", SecurityValidationModel);
            if (response.IsSuccessStatusCode)
            {
                // Armazena o token para redefinir a senha
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                ShowMessage(ToastType.Success, "Token gerado com sucesso.");
                NavigationManager.NavigateTo($"/reset-password?Token={result?.Token}");
            }
            else 
            {
                ShowMessage(ToastType.Danger, "Falha ao gerar token.");
            }
        }
    }
}