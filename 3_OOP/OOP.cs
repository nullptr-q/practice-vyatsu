using System;

public class Product
{
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public decimal Price { get; set; }
    public TimeSpan ShelfLife { get; set; }
    public DateTime ProductionDate { get; set; }

    // Конструктор класса
    public Product(string name, string manufacturer, 
    decimal price, TimeSpan shelfLife, DateTime productionDate)
    {
        Name = name;
        Manufacturer = manufacturer;
        Price = price;
        ShelfLife = shelfLife;
        ProductionDate = productionDate;
    }

    // Свойство только для чтения.
    public DateTime ExpirationDate
    {
        get { return ProductionDate.Add(ShelfLife); }
    }

    // Переопределение метода ToString().
    public override string ToString()
    {
        return $"--- Информация о товаре ---\n" +
               $"Наименование: {Name}\n" +
               $"Производитель: {Manufacturer}\n" +
               $"Цена: {Price:C}\n" + 
               $"Срок годности: {ShelfLife.Days} дней\n" +
               $"Дата производства: {ProductionDate:d}\n" + 
               $"Годен до: {ExpirationDate:d}\n" +
               $"--------------------------";
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Введите информацию о товаре:");

        Console.Write("Наименование: ");
        string name = Console.ReadLine();

        Console.Write("Производитель: ");
        string manufacturer = Console.ReadLine();

        decimal price;
        while (true)
        {
            Console.Write("Цена: ");
            if (decimal.TryParse(Console.ReadLine(), out price) && price >= 0)
            {
                break;
            }
            Console.WriteLine(@"Некорректный ввод цены.
             Пожалуйста, введите положительное число.");
        }

        int shelfLifeDays;
        while (true)
        {
            Console.Write("Срок годности (в днях): ");
            if (int.TryParse(Console.ReadLine(), out shelfLifeDays)
             && shelfLifeDays >= 0)
            {
                break;
            }
            Console.WriteLine(@"Некорректный ввод срока годности. 
            Пожалуйста, введите положительное целое число.");
        }
        TimeSpan shelfLife = TimeSpan.FromDays(shelfLifeDays);

        DateTime productionDate;
        while (true)
        {
            Console.Write("Дата производства (формат дд.мм.гггг): ");
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", 
            null, System.Globalization.DateTimeStyles.None, out productionDate))
            {
                break;
            }
            Console.WriteLine(@"Некорректный формат даты. 
            Пожалуйста, используйте дд.мм.гггг.");
        }

        // Создание экземпляра класса Product.
        Product product = new Product(name, manufacturer, price, shelfLife, productionDate);

        Console.WriteLine("\nИнформация о введенном товаре:");
        Console.WriteLine(product);
    }
}