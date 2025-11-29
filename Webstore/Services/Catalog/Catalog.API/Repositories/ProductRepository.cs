using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository: IProductRepository
{
    private readonly ICatalogContext _context;
    
    public ProductRepository(ICatalogContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context.Products
            .Find(p => true)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(string id)
    {
        return await _context.Products
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByCategoryAsync(string category)
    {
        return await _context.Products
            .Find(p => p.Category == category)
            .ToListAsync();
    }

    public async Task CreateProductAsync(Product product)
    {
        await _context.Products.InsertOneAsync(product);
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        var updateResult = await _context.Products
            .ReplaceOneAsync(p => p.Id == product.Id, product);
        return updateResult.ModifiedCount > 0; 
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        var deleteResult = await _context.Products
            .DeleteOneAsync(p => p.Id == id);
        return deleteResult.DeletedCount > 0;
    }
}