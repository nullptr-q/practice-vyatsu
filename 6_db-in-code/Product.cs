public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public override string ToString() => $"[ID: {Id}] {Name} - {Price:F2} руб. (В наличии: {Stock} шт.)";
}