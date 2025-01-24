using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyBookShelf.Models;
using MyBookShelf.Models.ViewModels;
using MyBookShelf.Services;

namespace MyBookShelf.Components.Pages.Management
{    
    public partial class User
    {
        [Inject] protected ToastService ToastService { get; set; }
        private void ShowMessage(ToastType toastType, string message) => ToastService.Notify(new(toastType, message));

        private List<UserAccountViewModel> Users = [];
        private UserAccountViewModel CurrentUser = new();
        private bool IsLoading = true;
        private bool IsModalOpen = false;
        private string ModalTitle = "";
        private readonly List<string> AvailableRoles = ["Administrator", "User"];

        private int PageSize = 5;  // Quantidade de itens por página
        private int CurrentPage = 1;  // Página atual
        private int TotalPages;  // Total de páginas
        private readonly List<int> PageSizeOptions = [5, 10, 15, 20];  // Opções de número de itens por página
        private bool IsReadOnly { get; set; } = false;

        private bool IsResetPasswordModalOpen = false;
        private PasswordViewModel PasswordModel = new();
        private async Task OnPageChangedAsync(int page)
        {
            CurrentPage = page;
            await LoadUsers();
        }

        protected override async Task OnInitializedAsync()
        {            
            await LoadUsers();
        }

        private async Task OnPageSizeChanged(ChangeEventArgs e)
        {            
            PageSize = int.Parse(e.Value!.ToString()!);  // Atualiza o tamanho da página
            CurrentPage = 1;  // Voltar para a primeira página quando o número de itens mudar
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                IsLoading = true;
                var response = await Http.GetAsync($"api/users/paginated?page={CurrentPage}&pageSize={PageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var pagedUsers = await response.Content.ReadFromJsonAsync<PagedResult<UserAccountViewModel>>();
                    Users = pagedUsers!.Data!;
                    TotalPages = pagedUsers.TotalPages;
                }
                else
                {
                    ShowMessage(ToastType.Danger, "Erro ao carregar os utilizadores.");
                   
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ToastType.Danger, "Erro inesperado.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowAddModal()
        {
            ModalTitle = "Adicionar Usuário";
            CurrentUser = new UserAccountViewModel();            
            IsModalOpen = true;
        }

        private void ShowEditModal(UserAccountViewModel user)
        {
            ModalTitle = "Editar Usuário";
            CurrentUser = new UserAccountViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Password = "#####",
                Role = user.Role
            };
            IsReadOnly = true;
            IsModalOpen = true;
        }

        private void CloseModal()
        {
            IsReadOnly = false;
            IsModalOpen = false;
        }

        private async Task SaveUser()
        {
            try
            {
                HttpResponseMessage response;

                if (CurrentUser.Id == 0)
                {
                    // Adicionar usuário
                    response = await Http.PostAsJsonAsync("api/users", CurrentUser);
                }
                else
                {
                    // Atualizar usuário
                    response = await Http.PutAsJsonAsync($"api/users/{CurrentUser.Id}", CurrentUser);
                }

                if (response.IsSuccessStatusCode)
                {
                    await LoadUsers();
                    CloseModal();
                }
                else
                {
                    ShowMessage(ToastType.Danger, "Erro ao salvar o utilizador.");
                    ErrorMessage = "Erro ao salvar o usuário.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro: {ex.Message}";
            }
        }

        private async Task DeleteUser(int id)
        {
            try
            {
                var response = await Http.DeleteAsync($"api/users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    await LoadUsers();
                }
                else
                {
                    ErrorMessage = "Erro ao excluir o usuário.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro: {ex.Message}";
            }
        }

        private ConfirmDialog? confirmDialog;

        private async Task ShowConfirmDialog(UserAccountViewModel user)
        {
            var confirmation = await confirmDialog!.ShowAsync(
                title: "Confirmação",
                message1: $"Você tem certeza que deseja excluir o usuário {user.Username}?",
                confirmDialogOptions: new ConfirmDialogOptions { NoButtonText = "Não", YesButtonText = "Sim" });
            if (confirmation) await DeleteUser(user.Id);               
        }

        private void OpenResetPasswordModal(UserAccountViewModel user)
        {
            PasswordModel = new PasswordViewModel { UserId = user.Id };
            IsResetPasswordModalOpen = true;
        }

        private void CloseResetPasswordModal()
        {
            IsResetPasswordModalOpen = false;
            PasswordModel = new PasswordViewModel();
        }

        private async Task ResetPassword()
        {
            // Lógica para redefinir a senha do usuário
            try
            {
                var response = await Http.PostAsJsonAsync("api/users/reset-password", PasswordModel);
                if (response.IsSuccessStatusCode)
                {                    
                    CloseResetPasswordModal();                    
                }
                else
                {                    
                    ErrorMessage = "Erro ao redefinir a senha.";                    
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erro ao comunicar com o servidor: " + ex.Message;
            }
        }        
    }
}