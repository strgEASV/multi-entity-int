using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;
using Service;

// Servisní vrstva = business logika aplikace.
// Controller (API) sem deleguje práci, tahle třída pak mluví s databází (Infra).
// Díky tomu je logika oddělená od HTTP a jde ji testovat samostatně.
//
// MyDatabaseConnection se předává v konstruktoru (primary constructor) přes DI.
public class LibraryService(MyDatabaseConnection dbConnection)
{
    // Vrátí všechny knihy z databáze.
    // .ToList() teprve spustí SQL dotaz a načte výsledky do paměti.
    public List<Book> GetBooks()
    {
        return dbConnection.Books.LoadWith(b => b.Author).ToList();
    }

    // Založí novou knihu. ID generujeme na serveru přes Guid,
    // aby bylo zaručeně unikátní a klient ho nemusel posílat.
    public void CreateBook(CreateBookRequestDto dto)
    {
        dbConnection.Insert(new Book()
        {
            BookId = Guid.NewGuid().ToString(),
            BookTitle = dto.Title,
            AuthorId = dto.AuthorId
        });
    }

    // Smaže knihu podle ID.
    public void DeleteBook(string bookId)
    {
        // Nejdřív knihu najdeme. FirstOrDefault vrátí null, pokud nic nenajde,
        // a operátor ?? v tom případě vyhodí výjimku místo pádu na null.
        var book = dbConnection.Books
                       .FirstOrDefault(b => b.BookId == bookId) ??
                   throw new ValidationException("Book not found!");
        dbConnection.Delete(book);
    }

    public void DeleteAuthor(string authorId)
    {
        var hasBooks = dbConnection.Books.Any(b => b.AuthorId == authorId);
        if (hasBooks)
            throw new ValidationException("Cannot delete author with existing books!");

        var author = dbConnection.Authors.FirstOrDefault(a => a.AuthorId == authorId)
                     ?? throw new ValidationException("Author not found!");
        dbConnection.Delete(author);
    }
}
