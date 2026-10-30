# ERD — Library Management API

Діаграма предметної області (Entity-Relationship Diagram). Джерела: [TZ.md](TZ.md), REST-контракти S2 (#7, #9, #11, #13, #15), зв'язки S4 (#31–#33, #45), бізнес-логіка S5 (#50, #52), авторизація S7 (#66).
Діаграму оновлюють у кожній задачі, що змінює модель даних.

```mermaid
erDiagram
    Author ||--o{ BookAuthor : "пише"
    Book ||--o{ BookAuthor : "має авторів"
    Genre ||--o{ BookGenre : "класифікує"
    Book ||--o{ BookGenre : "має жанри"
    Book ||--o{ BookCopy : "має екземпляри (1:N, Restrict)"
    Reader ||--o{ Loan : "отримує (1:N)"
    BookCopy ||--o{ Loan : "видається (1:N, активна ≤ 1)"
    Loan ||--o| Fine : "штраф (1:0..1)"
    Reader ||--o{ Reservation : "резервує (1:N)"
    Book ||--o{ Reservation : "черга (1:N)"
    BookCopy |o--o{ Reservation : "утримується для (0..1:N)"
    User |o--o| Reader : "профіль (1:0..1)"

    Author {
        int Id PK
        string FirstName "NOT NULL, ≤ 100"
        string LastName "NOT NULL, ≤ 100"
        int BirthYear "NULL, ≤ поточний рік"
        string Country "NULL, ≤ 100"
        DateTimeOffset CreatedAt "аудит, UTC"
        DateTimeOffset UpdatedAt "аудит, UTC"
    }

    Genre {
        int Id PK
        string Name UK "NOT NULL, ≤ 100"
    }

    Book {
        int Id PK
        string Title "NOT NULL, ≤ 300"
        string Isbn UK "NOT NULL, char(13), ISBN-13 з контрольною цифрою"
        int PublishYear "CHECK >= 1450; ≤ поточний рік перевіряє застосунок"
        int Pages "CHECK 1..10000"
        string Language "NOT NULL, char(2), ISO 639-1"
        string CoverPath "NULL, шлях до обкладинки (S8)"
        uint xmin "row version, оптимістична конкурентність"
        DateTimeOffset CreatedAt "аудит, UTC"
        DateTimeOffset UpdatedAt "аудит, UTC"
    }

    BookAuthor {
        int BookId PK, FK "→ Book, Cascade"
        int AuthorId PK, FK "→ Author, Cascade"
    }

    BookGenre {
        int BookId PK, FK "→ Book, Cascade"
        int GenreId PK, FK "→ Genre, Cascade"
    }

    BookCopy {
        int Id PK
        int BookId FK "NOT NULL → Book, Restrict"
        string InventoryNumber UK "NOT NULL, INV-000123, унікальний серед усіх (і списаних)"
        string Condition "enum як рядок: New, Good, Worn, Damaged"
        string Location "NOT NULL, ≤ 50, шафа/полиця"
        bool IsDeleted "soft delete"
        DateTimeOffset DeletedAt "NULL"
        DateTimeOffset CreatedAt "аудит, UTC"
        DateTimeOffset UpdatedAt "аудит, UTC"
    }

    Reader {
        int Id PK
        string FirstName "NOT NULL, ≤ 100"
        string LastName "NOT NULL, ≤ 100"
        string Email UK "NOT NULL, ≤ 256"
        string Phone "NOT NULL, E.164, ≤ 16"
        string CardNumber UK "NOT NULL, рівно 10 цифр"
        DateOnly RegisteredAt "встановлює сервер"
        string UserId FK, UK "NULL → User (S7)"
        DateTimeOffset CreatedAt "аудит, UTC"
        DateTimeOffset UpdatedAt "аудит, UTC"
    }

    Loan {
        int Id PK
        int ReaderId FK "NOT NULL → Reader, Restrict"
        int BookCopyId FK "NOT NULL → BookCopy, Restrict"
        DateTimeOffset IssuedAt "NOT NULL"
        DateTimeOffset DueAt "IssuedAt + LoanDays"
        DateTimeOffset ReturnedAt "NULL = активна видача"
        int RenewCount "0..MaxRenewals"
    }

    Fine {
        int Id PK
        int LoanId FK, UK "NOT NULL → Loan, Restrict"
        decimal Amount "numeric(10,2), CHECK > 0"
        DateTimeOffset CreatedAt "NOT NULL"
        DateTimeOffset PaidAt "NULL = не оплачено"
    }

    Reservation {
        int Id PK
        int ReaderId FK "NOT NULL → Reader, Restrict"
        int BookId FK "NOT NULL → Book, Restrict"
        DateTimeOffset CreatedAt "порядок у черзі"
        string Status "enum як рядок: Waiting, ReadyForPickup, Fulfilled, Expired, Cancelled"
        DateTimeOffset HoldUntil "NULL, лише для ReadyForPickup"
        int HeldBookCopyId FK "NULL → BookCopy, утримуваний екземпляр"
    }

    User {
        string Id PK "ASP.NET Core Identity (S7)"
        string Email UK
        string Role "Reader, Librarian, Admin"
    }
```

## Кардинальність

| Зв'язок | Тип | Видалення батька |
|---|---|---|
| Book — Author | M:N через `BookAuthor` | Cascade (зникає лише запис зв'язку) |
| Book — Genre | M:N через `BookGenre` | Cascade (зникає лише запис зв'язку) |
| Book — BookCopy | 1:N | **Restrict**: не можна видалити книгу з екземплярами |
| Reader — Loan | 1:N | Restrict |
| BookCopy — Loan | 1:N (історія), активна — не більше 1 | Restrict |
| Loan — Fine | 1:0..1 | Restrict |
| Reader — Reservation | 1:N | Restrict |
| Book — Reservation | 1:N (черга на книгу) | Restrict |
| BookCopy — Reservation | 0..1:N (утримання) | SetNull |
| User — Reader | 0..1:0..1 (у читача може не бути облікового запису) | Restrict |

## Неочевидні рішення

- **Book і BookCopy — різні сутності.** Каталог описує видання (назва, ISBN), а видаються конкретні фізичні екземпляри з власним інвентарним номером і станом. Тому `Loan` посилається на `BookCopy`, а не на `Book`.
- **M:N без додаткових даних (`BookAuthor`, `BookGenre`)** — це join-таблиці зі складеним PK `(BookId, AuthorId)` / `(BookId, GenreId)`, тому одна пара не може повторитися. Книга без авторів допустима (немає жодного рядка в `BookAuthor`).
- **Loan — окрема сутність, а не M:N Reader–BookCopy**, бо несе власні дані: дати видачі, терміну, повернення й кількість продовжень. Один екземпляр має багато видач в історії, але **активна (ReturnedAt IS NULL) — не більше однієї**. Це гарантує частковий унікальний індекс `UNIQUE (BookCopyId) WHERE ReturnedAt IS NULL`.
- **Fine — 1:0..1 до Loan.** Штраф створюється лише при поверненні після `DueAt`, тому `LoanId` унікальний. Сума — `decimal` (`numeric(10,2)`), а не `double`, щоб не було похибок округлення грошей. Ставка (`FinePerDay`) береться з конфігурації і в БД не зберігається.
- **Reservation посилається на Book, а не на BookCopy**: читачу підходить будь-який вільний екземпляр. Коли екземпляр повертають, перший резерв у черзі переходить у `ReadyForPickup`, а в `HeldBookCopyId` записується утримуваний екземпляр. Так можна заборонити видачу цього екземпляра іншому читачу. Позиція в черзі обчислюється за `CreatedAt` серед резервів у статусі `Waiting` і не зберігається.
- **Один активний резерв на книгу для читача**: частковий унікальний індекс `UNIQUE (ReaderId, BookId) WHERE Status IN ('Waiting', 'ReadyForPickup')`.
- **Soft delete для BookCopy.** Списаний екземпляр лишається в БД (на нього посилаються історичні видачі). Глобальний query filter ховає записи з `IsDeleted = true`. Унікальність `InventoryNumber` діє серед **усіх** екземплярів, включно зі списаними: звичайний `UNIQUE` без фільтра. Інакше номер списаного екземпляра можна було б видати новому, і в історії видач два різні екземпляри мали б однаковий номер.
- **Конкурентність Book** — через системний стовпець PostgreSQL `xmin` (`IsRowVersion`), тому окремий стовпець версії не потрібен.
- **Enum-и** (`Condition`, `Status`) зберігаються як рядки (`HasConversion<string>()`), щоб дані в БД були читабельні й не ламалися при зміні порядку значень enum.
- **Сурогатні ключі `int Id`** використано всюди, навіть де є природний унікальний ключ (ISBN, CardNumber). Такий ключ стабільний, коли природне значення виправляють, і робить JOIN дешевшими.
- **Час** зберігається як `DateTimeOffset` в UTC (`timestamptz`) і береться з `TimeProvider`, щоб у тестах можна було підставити `FakeTimeProvider`. `RegisteredAt` — `DateOnly` (`date`), бо час доби там не потрібен.
- **User** з'являється в S7 (ASP.NET Core Identity). Ролі зберігаються в таблицях Identity, на діаграмі вони спрощено показані полем `Role`.

## Обмеження (індекси й CHECK)

| Таблиця | Обмеження |
|---|---|
| Genre | `UNIQUE (Name)` |
| Book | `UNIQUE (Isbn)`, `CHECK (Pages BETWEEN 1 AND 10000)`, `CHECK (PublishYear >= 1450)` |
| BookCopy | `UNIQUE (InventoryNumber)` (без фільтра, включно зі списаними) |
| Reader | `UNIQUE (Email)`, `UNIQUE (CardNumber)`, `UNIQUE (UserId)` |
| Loan | `UNIQUE (BookCopyId) WHERE ReturnedAt IS NULL`, `CHECK (DueAt > IssuedAt)` |
| Fine | `UNIQUE (LoanId)`, `CHECK (Amount > 0)` |
| Reservation | `UNIQUE (ReaderId, BookId) WHERE Status IN ('Waiting','ReadyForPickup')` |
