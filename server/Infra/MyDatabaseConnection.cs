using LinqToDB;
using LinqToDB.Data;

namespace Infra;

// Naše připojení k databázi. Dědí z DataConnection (linq2db),
// takže umí Insert/Update/Delete a LINQ dotazy nad tabulkami.
//
// DataOptions<MyDatabaseConnection> se sem dostane přes dependency injection
// (nastavuje se v Program.cs) a nese v sobě connection string + typ databáze (SQLite).
public class MyDatabaseConnection(DataOptions<MyDatabaseConnection> options) 
    : DataConnection(options.Options)
{
    // Tabulka knih. Na tohle se pak píšou LINQ dotazy, např. dbConnection.Books.ToList().
    public ITable<Book> Books => this.GetTable<Book>();
}
