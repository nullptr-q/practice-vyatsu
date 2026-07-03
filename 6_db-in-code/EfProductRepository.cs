using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

// Контекст БД для Entity Framework
public class AppDbContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<Product> Products { get; set; }

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}

// Репозиторий EF Core
public class EfProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public EfProductRepository(string connectionString)
    {
        _connectionString = connectionString;
        
        // Автоматически создаем БД и таблицы, если их нет
        using var context = new AppDbContext(_connectionString);
        context.Database.EnsureCreated();
    }

    public void Create(Product product)
    {
        using var context = new AppDbContext(_connectionString);
        context.Products.Add(product);
        context.SaveChanges();
    }

    public Product GetById(int id)
    {
        using var context = new AppDbContext(_connectionString);
        return context.Products.Find(id);
    }

    public List<Product> GetAll()
    {
        using var context = new AppDbContext(_connectionString);
        return context.Products.ToList();
    }

    public void Update(Product product)
    {
        using var context = new AppDbContext(_connectionString);
        context.Products.Update(product);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        using var context = new AppDbContext(_connectionString);
        var product = context.Products.Find(id);
        if (product != null)
        {
            context.Products.Remove(product);
            context.SaveChanges();
        }
    }
}