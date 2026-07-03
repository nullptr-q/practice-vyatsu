using Microsoft.Extensions.Configuration;
using System;
using System.IO;

public class Program
{
    private static IProductRepository _repository;
    private static string _connectionString;

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Инициализация конфигурации для чтения строки подключения из appsettings.json
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();
        _connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(_connectionString))
        {
            Console.WriteLine("Ошибка: Не найдена строка подключения в конфигурационном файле.");
            return;
        }

        // Выбор режима работы
        Console.WriteLine("Выберите технологию доступа к данным:");
        Console.WriteLine("1. ADO.NET");
        Console.WriteLine("2. Entity Framework Core");
        Console.Write("Выбор: ");
        string techChoice = Console.ReadLine();

        if (techChoice == "1")
        {
            _repository = new AdoProductRepository(_connectionString);
            Console.WriteLine("Выбран режим: ADO.NET\n");
        }
        else
        {
            _repository = new EfProductRepository(_connectionString);
            Console.WriteLine("Выбран режим: Entity Framework Core\n");
        }

        RunMenu();
    }

    private static void RunMenu()
    {
        while (true)
        {
            Console.WriteLine("--- МЕНЮ CRUD ---");
            Console.WriteLine("1. Просмотреть все товары");
            Console.WriteLine("2. Найти товар по ID");
            Console.WriteLine("3. Добавить товар");
            Console.WriteLine("4. Обновить товар");
            Console.WriteLine("5. Удалить товар");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        ShowAllProducts();
                        break;
                    case "2":
                        FindProduct();
                        break;
                    case "3":
                        AddProduct();
                        break;
                    case "4":
                        UpdateProduct();
                        break;
                    case "5":
                        DeleteProduct();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void ShowAllProducts()
    {
        var products = _repository.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("Список товаров пуст.");
            return;
        }

        foreach (var product in products)
        {
            Console.WriteLine(product);
        }
    }

    private static void FindProduct()
    {
        Console.Write("Введите ID товара: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var product = _repository.GetById(id);
            if (product != null)
                Console.WriteLine($"\nНайден товар:\n{product}");
            else
                Console.WriteLine("Товар с таким ID не найден.");
        }
        else
        {
            Console.WriteLine("Некорректный формат ID.");
        }
    }

    private static void AddProduct()
    {
        Console.Write("Наименование: ");
        string name = Console.ReadLine();

        Console.Write("Цена: ");
        decimal price = decimal.Parse(Console.ReadLine());

        Console.Write("Количество: ");
        int stock = int.Parse(Console.ReadLine());

        var product = new Product { Name = name, Price = price, Stock = stock };
        _repository.Create(product);
        Console.WriteLine("Товар успешно добавлен.");
    }

    private static void UpdateProduct()
    {
        Console.Write("Введите ID товара для обновления: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Некорректный ID.");
            return;
        }

        var product = _repository.GetById(id);
        if (product == null)
        {
            Console.WriteLine("Товар не найден.");
            return;
        }

        Console.WriteLine($"Текущие данные: {product}");
        Console.Write("Новое наименование (оставьте пустым для пропуска): ");
        string name = Console.ReadLine();
        if (!string.IsNullOrEmpty(name)) product.Name = name;

        Console.Write("Новая цена (оставьте пустым для пропуска): ");
        string priceInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(priceInput)) product.Price = decimal.Parse(priceInput);

        Console.Write("Новое количество (оставьте пустым для пропуска): ");
        string stockInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(stockInput)) product.Stock = int.Parse(stockInput);

        _repository.Update(product);
        Console.WriteLine("Данные товара успешно обновлены.");
    }

    private static void DeleteProduct()
    {
        Console.Write("Введите ID товара для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _repository.Delete(id);
            Console.WriteLine("Запрос на удаление выполнен.");
        }
        else
        {
            Console.WriteLine("Некорректный ID.");
        }
    }
}