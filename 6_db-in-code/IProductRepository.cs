using System.Collections.Generic;

public interface IProductRepository
{
    void Create(Product product);
    Product GetById(int id);
    List<Product> GetAll();
    void Update(Product product);
    void Delete(int id);
}