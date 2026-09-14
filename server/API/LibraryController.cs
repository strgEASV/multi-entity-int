using Infra;
using Microsoft.AspNetCore.Mvc;

namespace API;


// Controller = vstupní bod HTTP requestů. Sám nic nepočítá,
// jen předá práci LibraryService (ten se sem dostane přes DI).
public class LibraryController(LibraryService service) : ControllerBase
{
    // nameof(GetBooks) udělá z názvu metody cestu -> GET /GetBooks
    // (výhoda: při přejmenování metody se URL změní automaticky)
    [HttpGet(nameof(GetBooks))]
    public List<Book> GetBooks()
    {
        return service.GetBooks();
    }

    // POST /CreateBook?title=...
    // Parametr title se bere z query stringu.
    [HttpPost(nameof(CreateBook))]
    public void CreateBook(string title)
    {
        service.CreateBook(title);
    }

    // PUT /UpdateBook - zatím neimplementováno (záměrně, jako cvičení).
    [HttpPut(nameof(UpdateBook))]
    public void UpdateBook()
    {
        throw new NotImplementedException();
    }

    // DELETE /DeleteBook?bookId=...
    [HttpDelete(nameof(DeleteBook))]
    public void DeleteBook(string bookId)
    {
        service.DeleteBook(bookId);
    }
}
