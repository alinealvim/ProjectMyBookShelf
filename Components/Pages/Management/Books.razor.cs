using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyBookShelf.Models.Entities;
using Microsoft.EntityFrameworkCore;
using MyBookShelf.Models;
using MyBookShelf.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MyBookShelf.Data;

namespace MyBookShelf.Components.Pages.Management
{
    public partial class Books
    {
        [Inject] protected ToastService ToastService { get; set; }
        private void ShowMessage(ToastType toastType, string message) => ToastService.Notify(new(toastType, message));

        private List<BookViewModel> BooksLst = new();
        private bool IsLoading = true;


        private bool IsAddModalOpen = false;
        private bool IsDetailsModalOpen = false;
        private bool IsEditModalOpen = false;
        private BookViewModel? SelectedBook = null;
        private BookViewModel NewBook = new();

        private int PageSize = 5;  // Quantidade de itens por página
        private int CurrentPage = 1;  // Página atual
        private int TotalPages;  // Total de páginas
        private readonly List<int> PageSizeOptions = [5, 10, 15, 20];  // Opções de número de itens por página

        private async Task OnPageChangedAsync(int page)
        {
            CurrentPage = page;
            await LoadBooks();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadBooks();
        }

        private async Task OnPageSizeChanged(ChangeEventArgs e)
        {
            PageSize = int.Parse(e.Value!.ToString()!);  // Atualiza o tamanho da página
            CurrentPage = 1;  // Voltar para a primeira página quando o número de itens mudar
            await LoadBooks();
        }

        private async Task LoadBooks()
        {
            try
            {
                IsLoading = true;
                StateHasChanged(); // Atualiza a interface enquanto carrega

                var response = await Http.GetAsync($"api/books/paginated?page={CurrentPage}&pageSize={PageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var pagedBooks = await response.Content.ReadFromJsonAsync<PagedResult<BookViewModel>>();
                    BooksLst = pagedBooks!.Data!;
                    TotalPages = pagedBooks.TotalPages;
                    
                }
                else
                {
                    ShowMessage(ToastType.Danger, "Erro ao carregar os livros.");
                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Erro inesperado.");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged(); // Garante que a interface seja atualizada
            }
        }

  
        private void ShowAddModal()
        {
            NewBook = new BookViewModel(); // Reseta os dados do formulário
            IsAddModalOpen = true;
            StateHasChanged(); // Atualiza o estado
        }

        private void CloseAddModal()
        {
            IsAddModalOpen = false;
            StateHasChanged(); // Atualiza o estado
        }

        private async Task AddBook()
        {
            try
            {
                // Verificar se o livro já existe
                var isBookTaken = await IsBookTaken(NewBook.Title, NewBook.Author);
                if (isBookTaken)
                {
                    ShowMessage(ToastType.Warning, "O livro já existe em nosso acervo. Por favor, escolha outro.");
                    CloseAddModal();
                    StateHasChanged();
                    return;
                }

                var response = await Http.PostAsJsonAsync("api/books", NewBook);

                if (response.IsSuccessStatusCode)
                {
                  
                    BooksLst.Add(NewBook); // Atualiza a lista local
                    ShowMessage(ToastType.Success, "Livro adicionado com sucesso.");
                    CloseAddModal();
                    StateHasChanged(); // Garante que a UI seja atualizada

                }
                else
                {
                    ShowMessage(ToastType.Danger, "Erro ao adicionar o livro.");
                    StateHasChanged(); // Atualiza a interface para mostrar o erro
                    CloseAddModal();
                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Ocorreu um erro inesperado.");
                StateHasChanged(); // Atualiza a interface para mostrar o erro
                CloseAddModal();
            }
        }

        private async Task<bool> IsBookTaken(string title, string author)
        {
            try
            {
                var response = await Http.GetAsync($"api/books/exists?title={Uri.EscapeDataString(title)}&author={Uri.EscapeDataString(author)}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>(); // Retorna 'true' ou 'false'
                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Erro ao verificar a existência do livro.");
            }
            return false;
        }


        private void ShowDetailsModal(int id)
        {
            SelectedBook = BooksLst.FirstOrDefault(b => b.Id == id);
            if (SelectedBook is not null)
            {
                IsDetailsModalOpen = true;
                StateHasChanged();
            }
        }

        private void CloseDetailsModal()
        {
            IsDetailsModalOpen = false;
            SelectedBook = null;
            StateHasChanged();
        }

        private void ShowEditModal(int id)
        {
            SelectedBook = BooksLst.FirstOrDefault(b => b.Id == id);
            if (SelectedBook is not null)
            {
                IsEditModalOpen = true;
                StateHasChanged();
            }
        }

        private void CloseEditModal()
        {
            IsEditModalOpen = false;
            SelectedBook = null;
            StateHasChanged();
        }

        private async Task SaveBookEdit()
        {
            if (SelectedBook is null) return;

            try
            {
                var response = await Http.PutAsJsonAsync($"api/books/{SelectedBook.Id}", SelectedBook);

                if (response.IsSuccessStatusCode)
                {
                    IsEditModalOpen = false;
                    await LoadBooks(); // Atualiza a lista completa
                    ShowMessage(ToastType.Success, "Sucesso ao atualizar o livro.");
                }
                else
                {
                    ShowMessage(ToastType.Danger, "Erro ao atualizar o livro.");

                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Erro inesperado.");
            }
            finally
            {
                StateHasChanged();                
            }
        }

        private async Task DeleteBook(int id)
        {
            try
            {
                var response = await Http.DeleteAsync($"api/books/{id}");

                if (response.IsSuccessStatusCode)
                {
                    BooksLst = BooksLst.Where(b => b.Id != id).ToList();
                    StateHasChanged(); // Atualiza a interface para refletir a exclusão
                }
                else
                {
                    ShowMessage(ToastType.Danger, "Erro ao excluir o livro.");
                    StateHasChanged(); // Atualiza a interface para mostrar o erro
                }
            }
            catch (Exception)
            {
                ShowMessage(ToastType.Danger, "Erro inesperado.");
                StateHasChanged(); // Atualiza a interface para mostrar o erro
            }
        }

        private ConfirmDialog? confirmDialog;

        private async Task ShowConfirmDialog(BookViewModel book)
        {
            var confirmation = await confirmDialog!.ShowAsync(
                title: "Confirmação",
                message1: $"Você tem certeza que deseja excluir o livro {book.Title}?",
                confirmDialogOptions: new ConfirmDialogOptions { NoButtonText = "Não", YesButtonText = "Sim" });
            if (confirmation)
            {
                await DeleteBook(book.Id);
                ShowMessage(ToastType.Success, "Sucesso ao excluir o livro.");
            }
            else
            {
                ShowMessage(ToastType.Secondary, "Ação cancelada.");
            }
        }
    }
} 