using Infra;
using LinqToDB;

// Program.cs je start aplikace. Nejdřív se poskládá DI kontejner (builder.Services),
// pak se aplikace postaví (builder.Build()) a nakonec spustí (app.Run()).

var builder = WebApplication.CreateBuilder(args);

// ---------- Nastavení databáze ----------
// Používáme SQLite - celá databáze je jeden soubor db.db vedle aplikace.
var dataSource = "Data Source=db.db";
var options = new DataOptions().UseSQLite(dataSource);
var dataOptions = new DataOptions<MyDatabaseConnection>(options);

// ---------- Registrace služeb do DI kontejneru ----------
// AddScoped = nová instance pro každý HTTP request (a na konci requestu se uklidí).
builder.Services.AddScoped<MyDatabaseConnection>(_ => new MyDatabaseConnection(dataOptions));
builder.Services.AddControllers();          // zapne controllery (LibraryController)
builder.Services.AddOpenApiDocument();      // NSwag - generuje OpenAPI/Swagger dokumentaci
builder.Services.AddScoped<LibraryService>();
builder.Services.AddCors();                 // CORS - aby na backend směl volat frontend z jiného portu

var app = builder.Build();

// CORS povolíme úplně všechno. Pro výuku OK, do produkce by se to mělo zúžit.
app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(_ => true));

// ---------- Příprava databáze při startu ----------
// Vytvoříme si vlastní scope, protože MyDatabaseConnection je scoped
// a tady, mimo HTTP request, žádný scope zatím neexistuje.
using (var scope = app.Services.CreateScope())
{
    var connectionToDb = scope.ServiceProvider.GetRequiredService<MyDatabaseConnection>();

    // Vytvoří tabulku Book, jen pokud ještě neexistuje.
    connectionToDb.CreateTable<Book>(tableOptions:TableOptions.CreateIfNotExists);

    connectionToDb.CreateTable<Author>(tableOptions: TableOptions.CreateIfNotExists);

    if (connectionToDb.Authors.Count() == 0)
    {
        connectionToDb.Insert(new Author() { AuthorId = "1", AuthorName = "Bob" });
    }

    // Seed dat: když je tabulka prázdná, vložíme jednu ukázkovou knihu.
    if (connectionToDb.Books.Count() == 0)
    {
        connectionToDb.Insert(new Book()
        {
            BookId = "1",
            BookTitle = "Bobs book",
            AuthorId = "1"
        });
    }
}

// ---------- Middleware pipeline ----------
app.MapControllers();   // nasměruje requesty na metody controllerů
app.UseOpenApi();       // zpřístupní /swagger/v1/swagger.json (z něj se generuje klientské Api.ts)
app.UseSwaggerUi();     // webové UI na /swagger, kde se dají endpointy naklikat

app.Run();
