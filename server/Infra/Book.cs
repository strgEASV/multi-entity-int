
using LinqToDB.Mapping;

namespace Infra;

// Entita (model) knihy = jeden řádek v databázové tabulce "Book".
// Leží ve vrstvě Infra, protože popisuje, jak vypadají data v databázi.
public class Book
{
    // [PrimaryKey] = primární klíč tabulky, unikátní identifikátor knihy.
    [PrimaryKey] public string BookId { get; set; }

    // [Column] = obyčejný sloupec tabulky, sem se ukládá název knihy.
    [Column]public string BookTitle { get; set; }
    
    [Column]public string AuthorId { get; set; }
    
    [Association(ThisKey = nameof(AuthorId), OtherKey = nameof(Author.AuthorId))]
    public Author Author { get; set; }
}

public class Author
{
    [PrimaryKey] public string AuthorId { get; set; }
    [Column] public string AuthorName { get; set; }
    
}
