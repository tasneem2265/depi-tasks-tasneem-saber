using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreEF.Models;

public class Book
{
    public int    Id         { get; set; }
    public string Title      { get; set; } = string.Empty;
    public string Format     { get; set; } = "Paperback";

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int AuthorId   { get; set; }
    public int CategoryId { get; set; }

    public Author   Author   { get; set; } = null!;
    public Category Category { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}