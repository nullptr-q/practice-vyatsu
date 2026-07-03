using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

public class AdoProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public AdoProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Create(Product product)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        const string query = "INSERT INTO Products (Name, Price, Stock) VALUES (@Name, @Price, @Stock)";
        
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Stock", product.Stock);
        
        command.ExecuteNonQuery();
    }

    public Product GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        const string query = "SELECT Id, Name, Price, Stock FROM Products WHERE Id = @Id";
        
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Stock = reader.GetInt32(3)
            };
        }
        return null;
    }

    public List<Product> GetAll()
    {
        var products = new List<Product>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        const string query = "SELECT Id, Name, Price, Stock FROM Products";
        
        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Stock = reader.GetInt32(3)
            });
        }
        return products;
    }

    public void Update(Product product)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        const string query = "UPDATE Products SET Name = @Name, Price = @Price, Stock = @Stock WHERE Id = @Id";
        
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Stock", product.Stock);
        
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        const string query = "DELETE FROM Products WHERE Id = @Id";
        
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        
        command.ExecuteNonQuery();
    }
}