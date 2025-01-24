using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using MyBookShelf.Data;
using MyBookShelf.Models.Entities;
using MyBookShelf.Services;

   


namespace MyBookShelf.Components.Pages
{
    public partial class Reading
    {
        [Inject] protected ToastService ToastService { get; set; }
        private void ShowMessage(ToastType toastType, string message) => ToastService.Notify(new(toastType, message));

        [Inject]
        IUserClaimService UserClaimService { get; set; } = default!;
        [Inject]
        AppDbContext AppDbContext { get; set; } = default!;

        private bool isAddModalOpen = false;

        private void AbrirModal()
        {
            isAddModalOpen = true;  // Abre o modal
        }

        private async Task OnModalStatusChanged(int bookId)
        {
            isAddModalOpen = false;
            StateHasChanged();
            var book = await AppDbContext.Books.FindAsync(bookId);
            await AddBookToUser(book!);
        }

        private string SearchTerm { get; set; } = string.Empty;
        private List<Book> SearchResults { get; set; } = [];

        private async Task SearchBooks()
        {
            // Realiza a busca na base de dados com base no termo de busca
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                SearchResults = await AppDbContext.Books
                    .Where(b => b.Title.Contains(SearchTerm)) // Busca no título do livro
                    .ToListAsync();
            }
            else
            {
                SearchResults.Clear(); // Limpa os resultados se a busca estiver vazia
            }
        }

    
        private async Task AddBookToUser(Book book)
        {
            // Verifica se o livro já está na lista de leitura do utiçizador
            var existingUserBook = await AppDbContext.UserBooks
                .FirstOrDefaultAsync(ub => ub.UserID == userInfo.UserId && ub.BookID == book.BookID);

            if (existingUserBook != null)
            {
                // O livro já está na lista de leitura
                ShowMessage(ToastType.Secondary, "O livro já está na sua lista.");              

                // Limpa os resultados de busca após a adição
                SearchResults.Clear();
                SearchTerm = string.Empty;

                // Não adiciona o livro novamente
                return; 
            }

            // Cria um novo UserBook para associar o livro ao usuário
            var userBook = new UserBook
            {
                UserID = userInfo.UserId,
                BookID = book.BookID, // Associa o livro com o ID do livro encontrado
                Status = BookStatus.ParaLer, // Define um status inicial para o livro
                StartDate = null, // Define uma data de início padrão
                CurrentPage = 0, // Define a página inicial como 0
            };

            // Adiciona a associação no banco de dados
            await AppDbContext.UserBooks.AddAsync(userBook);
            await AppDbContext.SaveChangesAsync();

            // Atualiza a lista de livros do usuário
            Books = await AppDbContext.UserBooks
                .Include(ub => ub.Book)
                .Where(x => x.UserID == userInfo.UserId)
                .ToListAsync();

            // Limpa os resultados de busca após a adição
            SearchResults.Clear();
            SearchTerm = string.Empty;

            // Exibe um alerta de sucesso
            ShowMessage(ToastType.Success, "O livro foi adicionado com sucesso à sua lista.");

        }

        private UserInfo userInfo = default!;
        private List<UserBook> Books = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            userInfo = UserClaimService.GetUserInfo();
            Books = AppDbContext.UserBooks.Include(ub => ub.Book).Where(x => x.UserID == userInfo.UserId).ToList();
        }

        private UserBook? BookToEdit;
        private UserBook? DraggingBook;

        private void OnDragStart(DragEventArgs e, UserBook book)
        {
            DraggingBook = book;
        }

        private void OpenEditModal(UserBook book)
        {
            //isAddModalOpen = false;
            BookToEdit = book;
        }

        private async Task DeleteBook(UserBook book)
        {
            var bookToDelete = await AppDbContext.UserBooks.FindAsync(book.UserBookID);
            if (bookToDelete is not null)
            {
                AppDbContext.UserBooks.Remove(bookToDelete);
                await AppDbContext.SaveChangesAsync();
            }
            
            Books = AppDbContext.UserBooks.Include(ub => ub.Book).Where(x => x.UserID == userInfo.UserId).ToList();
        }

        private async Task SaveChanges()
        {
            var bookToSave = new UserBook
            {
                BookID = BookToEdit!.BookID,
                UserID = userInfo.UserId,
                Status = BookToEdit.Status,
                StartDate = BookToEdit!.StartDate,
                EndDate = BookToEdit!.EndDate,
                CurrentPage = BookToEdit!.CurrentPage,
                Rating = BookToEdit.Rating // Salvar a classificação
            };
            var existingBook = await AppDbContext.UserBooks.FirstOrDefaultAsync(ub => ub.UserID == bookToSave.UserID && ub.BookID == bookToSave.BookID);
            if (existingBook != null)
            {
                // Se o livro já existir, atualiza os campos
                existingBook.Status = bookToSave.Status;
                existingBook.StartDate = bookToSave.StartDate;
                existingBook.EndDate = bookToSave.EndDate;
                existingBook.CurrentPage = bookToSave.CurrentPage;
                existingBook.Rating = bookToSave.Rating;

                if (bookToSave.CurrentPage > 0)
                {
                    var newReadingHistory = new ReadingHistory
                    {
                        UserBookID = existingBook.UserBookID,
                        Date = DateTime.Now
                    };

                    var readingHistory = await AppDbContext.ReadingHistories
                    .Where(x => x.UserBookID == existingBook.UserBookID)
                    .GroupBy(x => x.Date)
                        .Select(g => new
                        {
                            Date = g.Key,          // A data do grupo
                            TotalPagesRead = g.Sum(x => x.PagesRead) // Soma o total de páginas lidas no dia
                        })
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                    if (readingHistory.Count > 0)
                    {
                        var totalPagesRead = readingHistory.Sum(x => x.TotalPagesRead);
                        newReadingHistory.PagesRead = bookToSave.CurrentPage.Value - totalPagesRead;
                    }
                    else
                    {
                        newReadingHistory.PagesRead = bookToSave.CurrentPage.Value;
                    }

                    if (newReadingHistory.PagesRead > 0)
                    {
                        await AppDbContext.ReadingHistories.AddAsync(newReadingHistory);
                    }
                }
            }
            else
            {
                // Se o livro não existir, adiciona um novo registo
                await AppDbContext.UserBooks.AddAsync(bookToSave);
            }
            await AppDbContext.SaveChangesAsync();
            Books = AppDbContext.UserBooks.Include(ub => ub.Book).Where(x => x.UserID == userInfo.UserId).ToList();
            BookToEdit = null;
        }

        private async Task OnDrop(DragEventArgs e, BookStatus newStatus)
        {
            if (DraggingBook != null)
            {
                DraggingBook.Status = newStatus;
                var bookToUpdate = await AppDbContext.UserBooks.FindAsync(DraggingBook.UserBookID);
                if (bookToUpdate != null)
                {
                    bookToUpdate.Status = newStatus;
                    await AppDbContext.SaveChangesAsync();
                }
                DraggingBook = null;
                StateHasChanged();
            }
        }

        private ConfirmDialog? confirmDialog;        

        private async Task ShowConfirmDialog(UserBook userBook)
        {            
            var confirmation = await confirmDialog!.ShowAsync(
                title: "Confirmação",
                message1: $"Você tem certeza que deseja excluir o livro {userBook.Book!.Title}?",
                confirmDialogOptions: new ConfirmDialogOptions { NoButtonText = "Não", YesButtonText = "Sim" });
            if (confirmation)
            {
                await DeleteBook(userBook);
                // Exibe um alerta de sucesso
                ShowMessage(ToastType.Success, "O livro foi excluído com sucesso.");
            }
            else
            { // Exibe um alerta de sucesso
                ShowMessage(ToastType.Secondary, "Ação cancelada.");
            }

        }

        private void SetRating(int rating)
        {
            if (BookToEdit != null)
            {
                BookToEdit.Rating = rating;
            }
        }

    }
}