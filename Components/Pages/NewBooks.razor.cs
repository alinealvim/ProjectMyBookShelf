using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.Response;
using MyBookShelf.Models.ViewModels;
using static MyBookShelf.Components.Pages.Reading;

namespace MyBookShelf.Components.Pages
{
    public partial class NewBooks
    {

        [Inject] protected ToastService ToastService { get; set; }
        private void ShowMessage(ToastType toastType, string message) => ToastService.Notify(new(toastType, message));

        [Parameter] public ModalState ModalState { get; set; }  // Controla a visibilidade do modal
        [Parameter] public EventCallback<ModalState> IsAddModalOpenChanged { get; set; }  // Notifica o fechamento do modal        

        public void CloseModal()
        {
            ModalState = ModalState with { IsAddModalOpen = false };
            IsAddModalOpenChanged.InvokeAsync(ModalState);
        }

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
                ModalState = ModalState with { IsAddModalOpen = false, Id = bookId };
                await IsAddModalOpenChanged.InvokeAsync(ModalState);
                NewBook = new();
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