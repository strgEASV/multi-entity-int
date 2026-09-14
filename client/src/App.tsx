import { APITester } from "./APITester";
import "./index.css";

import logo from "./logo.svg";
import reactLogo from "./react.svg";
import {useEffect, useState} from "react";
// Api.ts je VYGENEROVANÝ soubor - vznikne z OpenAPI dokumentace backendu
// příkazem `bun run gen:api` (backend musí běžet na http://localhost:5000).
// Nikdy ho needitujeme ručně, přepsal by se.
import {Api, type Book} from "@/api/Api.ts";

// Jedna sdílená instance klienta k backendu pro celou appku.
export const MyBackendAlwaysUseThisOneVeryImportant = new Api();

export function App() {

    // books = seznam knih vykreslený na stránce
    const [books, setBooks] = useState<Book[]>([])
    // newTitle = text, který uživatel právě píše do inputu
    const [newTitle, setNewTitle] = useState("")

    // useEffect s prázdným polem [] = spustí se jednou po prvním vykreslení.
    // Tady si natáhneme knihy z backendu.
    useEffect(() => {
        MyBackendAlwaysUseThisOneVeryImportant.getBooks.libraryGetBooks().then(r => {
            setBooks(r)
        })
    }, []);

  return (
    <div className="app">

        {
            // .map() = pro každou knihu vyrobíme jeden řádek s tlačítkem na smazání
            books.map(b => {
                return <div>Book title: {b.bookTitle}<button onClick={() => {
                    // Nejdřív smažeme knihu na backendu...
                    MyBackendAlwaysUseThisOneVeryImportant.deleteBook.libraryDeleteBook({bookId: b.bookId}).then(r => {
                        // ...a až potom si znovu načteme seznam, aby UI sedělo s databází.
                        MyBackendAlwaysUseThisOneVeryImportant.getBooks.libraryGetBooks().then(r => {
                            setBooks(r)
                        })
                    })
                }}>click to delete this book</button></div>
            })
        }
        {/* Controlled input: hodnotu drží React ve state (value + onChange). */}
        <input placeholder={"make title new a new book"} onChange={e => setNewTitle(e.target.value)} value={newTitle}  />
        <button onClick={() => {
            MyBackendAlwaysUseThisOneVeryImportant.createBook.libraryCreateBook({title: newTitle}).then(r => {
                MyBackendAlwaysUseThisOneVeryImportant.getBooks.libraryGetBooks().then(r => {
                    setBooks(r)
                })
                //if success (meaning if 200-something status code response from the backend)
                // = sem se dostaneme, když backend vrátí status 2xx
            }).catch(e => {
                //if failure (meaning if 400 or 500-something status codes get you into this block
                // = sem se dostaneme při chybě (4xx/5xx), tady se typicky zobrazí hláška uživateli
            })
        }}>Click to create new book</button>

    </div>
  );
}

export default App;
