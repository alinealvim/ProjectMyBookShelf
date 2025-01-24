using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.ViewModels;

namespace MyBookShelf.Components.Pages.Account
{
    public partial class SecurityAnswerValidation
    {

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
                NavigationManager.NavigateTo($"/reset-password?Token={result?.Token}");
            }
        }
    }
}