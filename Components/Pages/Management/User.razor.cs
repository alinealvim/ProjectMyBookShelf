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
            catch (Exception)
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
            ModalTitle = "Adicionar Utilizador";
            CurrentUser = new UserAccountViewModel();            
            IsModalOpen = true;
        }

        private void ShowEditModal(UserAccountViewModel user)
        {
            ModalTitle = "Editar Utilizador";
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
            ShowMessage(ToastType.Secondary, "Ação cancelada.");
            
            
        }

        private async Task SaveUser()
        {
            try
            {
                HttpResponseMessage response;

                if (CurrentUser.Id == 0)
                {
                    // Verificar se o username já existe
                    var isUsernameTaken = await IsUsernameTaken(CurrentUser.Username);
                    if (isUsernameTaken)
                    {
                        ShowMessage(ToastType.Warning, "O nome de utilizador já existe. Por favor, escolha outro.");
                        return;
                    }

                    // Adicionar utilizador
                    response = await Http.PostAsJsonAsync("api/users", CurrentUser);
                    ShowMessage(ToastType.Success, "Sucesso ao criar novo utilizador.");
                }
                else 
                {
                    // Atualizar utilizador
                    response = await Http.PutAsJsonAsync($"api/users/{CurrentUser.Id}", CurrentUser);
                    ShowMessage(ToastType.Success, "Sucesso ao atualizar utilizador.");
                }
               

                if (response.IsSuccessStatusCode)
                {
                    await LoadUsers();
                    IsReadOnly = false;
                    IsModalOpen = false;
                }
                else
                {
                    ShowMessage(ToastType.Danger, "Erro ao salvar o utilizador.");
                    IsReadOnly = false;
                    IsModalOpen = false;
                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Erro inesperado.");
            }
        }

        private async Task<bool> IsUsernameTaken(string username)
        {
            try
            {
                var response = await Http.GetAsync($"api/users/exists?username={Uri.EscapeDataString(username)}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Erro ao verificar nome de utilizador.");
            }
            return false;
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
                    ShowMessage(ToastType.Danger, "Erro ao excluir o utilizador.");
                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Erro inesperado.");
            }
        }

        private ConfirmDialog? confirmDialog;

        private async Task ShowConfirmDialog(UserAccountViewModel user)
        {
            var confirmation = await confirmDialog!.ShowAsync(
                title: "Confirmação",
                message1: $"Tem certeza que deseja excluir o utilizador {user.Username}?",
                confirmDialogOptions: new ConfirmDialogOptions { NoButtonText = "Não", YesButtonText = "Sim" });
            if (confirmation)
            {
                await DeleteUser(user.Id);
                ShowMessage(ToastType.Success, "Sucesso ao excluir utilizador.");
            }
            else
            {
                ShowMessage(ToastType.Secondary, "Ação cancelada.");
            }
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
            ShowMessage(ToastType.Secondary, "Ação cancelada.");
        }

        private async Task ResetPassword()
        {
            // Lógica para redefinir a palavra-passe do utilizador
            try
            {
                var response = await Http.PostAsJsonAsync("api/users/reset-password", PasswordModel);
                if (response.IsSuccessStatusCode)
                {
                    ShowMessage(ToastType.Success, "Sucesso ao redefinir a palavra-passe.");
                    IsResetPasswordModalOpen = false;
                    PasswordModel = new PasswordViewModel();
                }
                else
                {
                    ShowMessage(ToastType.Danger, "Erro ao redefinir a palavra-passe.");
                    IsResetPasswordModalOpen = false;
                    PasswordModel = new PasswordViewModel();

                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Erro inesperado.");
            }
        }        
    }
}