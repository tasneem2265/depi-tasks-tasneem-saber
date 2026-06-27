using BookStore.Data;
using BookStore.Events;
using BookStore.Extensions;
using BookStore.Interfaces;
using BookStore.Models;
using BookStore.Services;

var bookService     = new BookService();
var customerService = new CustomerService();
var orderService    = new OrderService(bookService, customerService);

BookStoreEvents.BookStockDepleted += (_, e) =>
    ConsoleHelper.PrintInfo($"STOCK ALERT: '{e.Book.Title}' is now OUT OF STOCK!");

await LoadDataAsync();
SeedDemoData();
await RunMenuAsync();

async Task RunMenuAsync()
{
    while (true)
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("📚  BookStore Management System");
        Console.WriteLine("  [1]  Book Management");
        Console.WriteLine("  [2]  Customer Management");
        Console.WriteLine("  [3]  Orders & Purchases");
        Console.WriteLine("  [4]  Reports & Analytics");
        Console.WriteLine("  [5]  Apply Book Rules");
        Console.WriteLine("  [6]  Save Data");
        Console.WriteLine("  [0]  Exit");
        Console.Write("\n  Choose: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1": await BookMenuAsync(); break;
            case "2": CustomerMenu(); break;
            case "3": OrderMenu(); break;
            case "4": ReportsMenu(); break;
            case "5": RulesMenu(); break;
            case "6": await SaveDataAsync(); break;
            case "0":
                await SaveDataAsync();
                ConsoleHelper.PrintSuccess("Goodbye!");
                return;
            default:
                ConsoleHelper.PrintError("Invalid option.");
                Pause();
                break;
        }
    }
}

async Task BookMenuAsync()
{
    Console.Clear();
    ConsoleHelper.PrintHeader("📖  Book Management");
    Console.WriteLine("  [1]  Add Book");
    Console.WriteLine("  [2]  Remove Book");
    Console.WriteLine("  [3]  List All Books");
    Console.WriteLine("  [4]  Search Books");
    Console.WriteLine("  [5]  Filter by Category");
    Console.WriteLine("  [6]  Filter by Author");
    Console.WriteLine("  [7]  Filter by Price Range");
    Console.WriteLine("  [0]  Back");
    Console.Write("\n  Choose: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": AddBook(); break;
        case "2": RemoveBook(); break;
        case "3": ListBooks(bookService.GetAll()); break;
        case "4": SearchBooks(); break;
        case "5": FilterByCategory(); break;
        case "6": FilterByAuthor(); break;
        case "7": FilterByPrice(); break;
        case "0": return;
        default: ConsoleHelper.PrintError("Invalid option."); break;
    }
    Pause();
}

void AddBook()
{
    ConsoleHelper.PrintHeader("➕  Add New Book");
    Console.WriteLine("  Format: [1] Paperback  [2] Ebook  [3] Audiobook");
    Console.Write("  Choose format: ");
    var fmt = Console.ReadLine()?.Trim();

    try
    {
        BookBase book = fmt switch
        {
            "2" => new EbookBook { FileFormat = ConsoleHelper.ReadRequired("File format (PDF/EPUB)") },
            "3" => new AudiobookBook { DurationHours = (double)ConsoleHelper.ReadDecimal("Duration (hours)") },
            _   => new PaperbackBook()
        };

        book.Title    = ConsoleHelper.ReadRequired("Title");
        book.Author   = ConsoleHelper.ReadRequired("Author");
        book.Category = ConsoleHelper.ReadRequired("Category");
        book.Price    = ConsoleHelper.ReadDecimal("Price");
        book.Stock    = ConsoleHelper.ReadInt("Stock");

        bookService.AddBook(book);
        ConsoleHelper.PrintSuccess($"Book '{book.Title}' added successfully (ID: {book.Id}).");
    }
    catch (Exception ex) { ConsoleHelper.PrintError(ex.Message); }
}

void RemoveBook()
{
    ConsoleHelper.PrintHeader("🗑️  Remove Book");
    ListBooks(bookService.GetAll());
    try
    {
        var id = ConsoleHelper.ReadInt("Enter Book ID to remove");
        bookService.RemoveBook(id);
        ConsoleHelper.PrintSuccess($"Book ID {id} removed.");
    }
    catch (Exception ex) { ConsoleHelper.PrintError(ex.Message); }
}

void ListBooks(IEnumerable<IBook> books)
{
    var list = books.ToList();
    if (!list.Any()) { ConsoleHelper.PrintInfo("No books found."); return; }
    Console.WriteLine();
    foreach (var b in list) Console.WriteLine($"  {b}");
}

void SearchBooks()
{
    var term = ConsoleHelper.ReadRequired("Search term");
    ListBooks(bookService.Search(term));
}

void FilterByCategory()
{
    var cat = ConsoleHelper.ReadRequired("Category name");
    ListBooks(bookService.FilterByCategory(cat));
}

void FilterByAuthor()
{
    var author = ConsoleHelper.ReadRequired("Author name");
    ListBooks(bookService.FilterByAuthor(author));
}

void FilterByPrice()
{
    var min = ConsoleHelper.ReadDecimal("Min price");
    var max = ConsoleHelper.ReadDecimal("Max price");
    ListBooks(bookService.FilterByPriceRange(min, max));
}

// ─── CUSTOMER MENU ───────────────────────────────────────────
void CustomerMenu()
{
    Console.Clear();
    ConsoleHelper.PrintHeader("👤  Customer Management");
    Console.WriteLine("  [1]  Register Customer");
    Console.WriteLine("  [2]  List All Customers");
    Console.WriteLine("  [0]  Back");
    Console.Write("\n  Choose: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": RegisterCustomer(); break;
        case "2": ListCustomers(); break;
        case "0": return;
        default: ConsoleHelper.PrintError("Invalid option."); break;
    }
    Pause();
}

void RegisterCustomer()
{
    ConsoleHelper.PrintHeader("➕  Register Customer");
    try
    {
        var c = new Customer
        {
            FirstName = ConsoleHelper.ReadRequired("First Name"),
            LastName  = ConsoleHelper.ReadRequired("Last Name"),
            Email     = ConsoleHelper.ReadRequired("Email")
        };
        customerService.RegisterCustomer(c);
        ConsoleHelper.PrintSuccess($"Customer '{c.FullName}' registered (ID: {c.Id}).");
    }
    catch (Exception ex) { ConsoleHelper.PrintError(ex.Message); }
}

void ListCustomers()
{
    var list = customerService.GetAll().ToList();
    if (!list.Any()) { ConsoleHelper.PrintInfo("No customers registered."); return; }
    Console.WriteLine();
    foreach (var c in list) Console.WriteLine($"  {c}");
}

// ─── ORDER MENU ──────────────────────────────────────────────
void OrderMenu()
{
    Console.Clear();
    ConsoleHelper.PrintHeader("🛒  Orders & Purchases");
    Console.WriteLine("  [1]  Place New Order");
    Console.WriteLine("  [2]  List All Orders");
    Console.WriteLine("  [3]  Orders by Customer");
    Console.WriteLine("  [0]  Back");
    Console.Write("\n  Choose: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": PlaceOrder(); break;
        case "2": ListOrders(orderService.GetAll()); break;
        case "3": OrdersByCustomer(); break;
        case "0": return;
        default: ConsoleHelper.PrintError("Invalid option."); break;
    }
    Pause();
}

void PlaceOrder()
{
    ConsoleHelper.PrintHeader("🛒  Place New Order");
    ListCustomers();
    try
    {
        var customerId = ConsoleHelper.ReadInt("Customer ID");
        var items = new List<(int, int)>();

        while (true)
        {
            ListBooks(bookService.GetAll());
            var bookId = ConsoleHelper.ReadInt("Book ID (0 to finish)");
            if (bookId == 0) break;
            var qty = ConsoleHelper.ReadInt("Quantity");
            items.Add((bookId, qty));
            Console.WriteLine("  Add another book? [y/n]");
            if (Console.ReadLine()?.Trim().ToLower() != "y") break;
        }

        var order = orderService.PlaceOrder(customerId, items);
        ConsoleHelper.PrintSuccess($"Order #{order.Id} placed! Total: {order.Total.ToCurrency()}");
        foreach (var item in order.Items)
            Console.WriteLine($"    - {item.BookTitle} x{item.Quantity} @ {item.PriceAtSale.ToCurrency()} = {item.LineTotal.ToCurrency()}");
    }
    catch (Exception ex) { ConsoleHelper.PrintError(ex.Message); }
}

void ListOrders(IEnumerable<Order> orders)
{
    var list = orders.ToList();
    if (!list.Any()) { ConsoleHelper.PrintInfo("No orders found."); return; }
    Console.WriteLine();
    foreach (var o in list)
    {
        Console.WriteLine($"  Order #{o.Id} | {o.CustomerName} | {o.OrderDate:yyyy-MM-dd} | Total: {o.Total.ToCurrency()}");
        foreach (var i in o.Items)
            Console.WriteLine($"      - {i.BookTitle} x{i.Quantity} @ {i.PriceAtSale.ToCurrency()}");
    }
}

void OrdersByCustomer()
{
    ListCustomers();
    var id = ConsoleHelper.ReadInt("Customer ID");
    ListOrders(orderService.GetByCustomer(id));
}

// ─── REPORTS MENU ────────────────────────────────────────────
void ReportsMenu()
{
    Console.Clear();
    ConsoleHelper.PrintHeader("📊  Reports & Analytics");
    var revenue = orderService.GetTotalRevenue();
    var (bestBook, sold) = orderService.GetBestSellingBook();
    var (topCustomer, spent) = orderService.GetTopCustomer();

    Console.WriteLine($"    Total Revenue    : {revenue.ToCurrency()}");
    Console.WriteLine($"    Best-Selling Book: {bestBook} ({sold} sold)");
    Console.WriteLine($"    Top Customer     : {topCustomer} ({spent.ToCurrency()} spent)");
    Pause();
}


void RulesMenu()
{
    Console.Clear();
    ConsoleHelper.PrintHeader("⚙️  Apply Book Rule");
    Console.WriteLine("  [1]  Apply Discount % to all books");
    Console.WriteLine("  [2]  Apply Price Adjustment to all books");
    Console.WriteLine("  [0]  Back");
    Console.Write("\n  Choose: ");

    try
    {
        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                var pct = ConsoleHelper.ReadDecimal("Discount percentage (e.g. 10)");
                var disc = new DiscountRule(pct);
                bookService.ApplyRule(disc, bookService.GetAll().ToList());
                ConsoleHelper.PrintSuccess($"Applied: {disc.RuleName} to all books.");
                break;
            case "2":
                var amt = ConsoleHelper.ReadDecimal("Adjustment amount (negative to reduce)");
                var adj = new PriceAdjustmentRule(amt);
                bookService.ApplyRule(adj, bookService.GetAll().ToList());
                ConsoleHelper.PrintSuccess($"Applied: {adj.RuleName} to all books.");
                break;
            case "0": return;
            default: ConsoleHelper.PrintError("Invalid option."); break;
        }
    }
    catch (Exception ex) { ConsoleHelper.PrintError(ex.Message); }
    Pause();
}


async Task SaveDataAsync()
{
    try
    {
        await DataStore.SaveAsync(bookService.GetAll(), customerService.GetAll(), orderService.GetAll());
        ConsoleHelper.PrintSuccess("Data saved successfully.");
    }
    catch (Exception ex) { ConsoleHelper.PrintError($"Save failed: {ex.Message}"); }
    Pause();
}

async Task LoadDataAsync()
{
    try
    {
        var data = await DataStore.LoadAsync();
        if (data is null) return;

        foreach (var b in data.Paperbacks.Cast<IBook>()
            .Concat(data.Ebooks).Concat(data.Audiobooks))
            bookService.AddBook(b);

        foreach (var c in data.Customers)
            customerService.RegisterCustomer(c);

        ConsoleHelper.PrintSuccess("Previous session data loaded.");
    }
    catch { }
}

void SeedDemoData()
{
    if (bookService.GetAll().Any()) return;

    bookService.AddBook(new PaperbackBook { Title = "Clean Code", Author = "Robert Martin", Category = "Software Engineering", Price = 39.99m, Stock = 10 });
    bookService.AddBook(new EbookBook     { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Category = "Software Engineering", Price = 34.99m, Stock = 5, FileFormat = "PDF" });
    bookService.AddBook(new AudiobookBook { Title = "Sapiens", Author = "Yuval Harari", Category = "History", Price = 18.99m, Stock = 8, DurationHours = 15.5 });
    bookService.AddBook(new PaperbackBook { Title = "Dune", Author = "Frank Herbert", Category = "Science Fiction", Price = 14.99m, Stock = 15 });
    bookService.AddBook(new EbookBook     { Title = "Design Patterns", Author = "Erich Gamma", Category = "Software Engineering", Price = 49.99m, Stock = 3, FileFormat = "EPUB" });

    customerService.RegisterCustomer(new Customer { FirstName = "Ahmed", LastName = "Hassan", Email = "ahmed@email.com" });
    customerService.RegisterCustomer(new Customer { FirstName = "Sara",  LastName = "Mohamed", Email = "sara@email.com" });
}

void Pause()
{
    Console.WriteLine();
    Console.Write("  Press Enter to continue...");
    Console.ReadLine();
}
