using Microsoft.EntityFrameworkCore;
using MyBookShelf.Models.Entities;

namespace MyBookShelf.Data
{
    public class AppDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
        public DbSet<ReadingHistory> ReadingHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            

            // Configuração da entidade User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(u => u.UserID);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(50);
                entity.Property(u => u.SecurityQuestion).HasMaxLength(200);
                entity.Property(u => u.SecurityAnswer).HasMaxLength(200);
                entity.Property(u => u.PasswordResetToken).HasMaxLength(200);
            });

            // Configuração da entidade Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");
                entity.HasKey(b => b.BookID);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Genre).HasMaxLength(100);
                entity.Property(b => b.Author).HasMaxLength(100);
                entity.Property(b => b.Pages).IsRequired();
            });

            // Configuração da entidade UserBook
            modelBuilder.Entity<UserBook>(entity =>
            {
                entity.ToTable("UserBook");
                entity.HasKey(ub => ub.UserBookID);

                entity.HasOne(ub => ub.User)
                      .WithMany(u => u.UserBooks)
                      .HasForeignKey(ub => ub.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ub => ub.Book)
                      .WithMany(b => b.UserBooks)
                      .HasForeignKey(ub => ub.BookID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ub => ub.Status)
                .HasMaxLength(50)
                .HasConversion(
                    status => status.ToString(), // Enum para string ao salvar no banco
                    status => (BookStatus)Enum.Parse(typeof(BookStatus), status)); // String para enum ao carregar do banco
                entity.Property(ub => ub.Notes).HasMaxLength(500);
            });

            // Configuração da entidade ReadingHistory
            modelBuilder.Entity<ReadingHistory>(entity =>
            {
                entity.ToTable("ReadingHistory");
                entity.HasKey(rh => rh.ReadingHistoryID);

                entity.HasOne(rh => rh.UserBook)
                      .WithMany(ub => ub.ReadingHistories)
                      .HasForeignKey(rh => rh.UserBookID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rh => rh.PagesRead).IsRequired();
                entity.Property(rh => rh.Date).IsRequired();
            });
        }
    }
}
