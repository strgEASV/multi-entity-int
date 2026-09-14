using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;

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
        return dbConnection.Books.ToList();
    }

    // Založí novou knihu. ID generujeme na serveru přes Guid,
    // aby bylo zaručeně unikátní a klient ho nemusel posílat.
    public void CreateBook(string title)
    {
        dbConnection.Insert(new Book()
        {
            BookId = Guid.NewGuid().ToString(),
            BookTitle = title
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
}
