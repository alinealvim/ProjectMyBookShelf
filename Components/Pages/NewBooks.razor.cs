using Microsoft.AspNetCore.Components;
using MyBookShelf.Models.Response;
using MyBookShelf.Models.ViewModels;

namespace MyBookShelf.Components.Pages
{
    public partial class NewBooks
    {
        [Parameter] public bool IsAddModalOpen { get; set; }  // Controla a visibilidade do modal
        [Parameter] public EventCallback<int> IsAddModalOpenChanged { get; set; }  // Notifica o fechamento do modal        

        public void CloseModal() => IsAddModalOpen = false;  // Fecha o modal//IsAddModalOpenChanged.InvokeAsync(false);
        private BookViewModel NewBook = new();
        

        private async Task AddBook()
        {
            var response = await Http.PostAsJsonAsync("api/books", NewBook);

            if (response.IsSuccessStatusCode)
            {
                //Books.Add(NewBook); // Atualiza a lista local
                var responseObject = await response.Content.ReadFromJsonAsync<BookResponse>();
                int bookId = responseObject!.BookId;
                await IsAddModalOpenChanged.InvokeAsync(bookId);
                NewBook = new();
                CloseModal();
                StateHasChanged(); // Garante que a UI seja atualizada
            }            
        }

    }
}