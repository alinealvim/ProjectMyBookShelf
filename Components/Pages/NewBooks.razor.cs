using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.Response;
using MyBookShelf.Models.ViewModels;

namespace MyBookShelf.Components.Pages
{
    public partial class NewBooks
    {

        [Inject] protected ToastService ToastService { get; set; }
        private void ShowMessage(ToastType toastType, string message) => ToastService.Notify(new(toastType, message));

        [Parameter] public bool IsAddModalOpen { get; set; }  // Controla a visibilidade do modal
        [Parameter] public EventCallback<int> IsAddModalOpenChanged { get; set; }  // Notifica o fechamento do modal        

        public void CloseModal() => IsAddModalOpen = false;  // Fecha o modal
        private BookViewModel NewBook = new();


        private async Task AddBook()
        {
            var isBookTaken = await IsBookTaken(NewBook.Title, NewBook.Author);
            if (isBookTaken)
            {
                ShowMessage(ToastType.Warning, "O livro já existe em nosso acervo. Por favor, escolha outro.");
                NewBook = new();
                CloseModal();
                StateHasChanged();
                return;
            }

            var response = await Http.PostAsJsonAsync("api/books", NewBook);

            if (response.IsSuccessStatusCode)
            {
                
                var responseObject = await response.Content.ReadFromJsonAsync<BookResponse>();
                int bookId = responseObject!.BookId;
                await IsAddModalOpenChanged.InvokeAsync(bookId);
                NewBook = new();
                CloseModal();
                StateHasChanged(); // Garante que a UI seja atualizada
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
    }
}