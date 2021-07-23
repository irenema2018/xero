using RefactorThis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace RefactorThis.Repositories
{
    public interface IProductRepository
    {
        Task<Guid> CreateProduct(Product product);
        Task<List<Product>> GetProducts();
        Task<Product> GetProductById(Guid id);
        Task<List<Product>> GetProductsByName(string Name);
        Task UpdateProduct(Product product);
        Task DeleteProduct(Guid id);
        Task DeleteOptions(Guid productId);

        Task<List<ProductOption>> GetOptions(Guid productId);
        Task<ProductOption> GetOptionById(Guid id);
        Task CreateOption(ProductOption productOption);
        Task UpdateOption(ProductOption productOption);
        Task DeleteOption(Guid id);
    }

    public class ProductRepository : IProductRepository 
    {
        public async Task<Guid> CreateProduct(Product product)
        {
            var productId = Guid.NewGuid();
            using (var connection = Helpers.NewConnection())
            {
                if (product.Id != null && product.Id != Guid.Empty)
                {
                     productId = product.Id;
                }

                var parameters = new { Id=productId, product.Name, product.Price, product.Description, product.DeliveryPrice };//without product, C# can still recognize Name/Price, etc.
                await connection.ExecuteAsync($"insert into Products (id, name, description, price, deliveryprice) values (@Id, @Name, @Description, @Price, @DeliveryPrice)", parameters);
                return productId;
            }
        }

        public async Task<Product> GetProductById(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {

                var parameters = new { id };
                var result = await connection.QueryAsync<Product>($"select Id, Name, Description, Price, DeliveryPrice from Products where Id = @id", parameters);

                return result.FirstOrDefault();
            }
        }

        public async Task<List<Product>> GetProductsByName(string name)
        {
            using (var connection = Helpers.NewConnection())
            {
                // if the pass in id is not a Guid, return an err msg BadRequest()
                name = "%" + name + "%";
                var parameters = new { name };
                var result = await connection.QueryAsync<Product>($"select Id, Name, Description, Price, DeliveryPrice from Products where Name like @name", parameters);

                return result.ToList();
            }
        }

        public async Task<List<Product>> GetProducts()
        {
            using (var connection = Helpers.NewConnection())
            {
                var products = await connection.QueryAsync<Product>("select Id, Name, Description, Price, DeliveryPrice from Products");

                return products.ToList();
            }
        }

        public async Task UpdateProduct(Product product)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { product.Id, product.Name, product.Price, product.Description, product.DeliveryPrice };
                await connection.ExecuteAsync($"update Products set Name = @Name, Description = @Description, Price = @Price, DeliveryPrice = @DeliveryPrice where Id = @Id", parameters);
            }
        }

        public async Task DeleteProduct(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { id };
                await connection.ExecuteAsync($"delete from Products where Id = @id", parameters);
            }
        }

        public async Task DeleteOptions(Guid productId)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { productId };
                await connection.ExecuteAsync($"delete from ProductOptions where ProductId = @productId", parameters);
            }
        }

        public async Task<List<ProductOption>> GetOptions(Guid productId)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { productId };
                var options = await connection.QueryAsync<ProductOption>("select Id, ProductId, Name, Description from ProductOptions where ProductId = @productId", parameters);
                return options.ToList();
            }
        }

        public async Task<ProductOption> GetOptionById(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { id };
                var result = await connection.QueryAsync<ProductOption>("select Id, ProductId, Name, Description from ProductOptions where Id = @id", parameters);
                return result.FirstOrDefault();
            }
        }

        public async Task CreateOption(ProductOption productOption)
        {
            using (var connection = Helpers.NewConnection())
            {
                if (productOption.Id == null || productOption.Id == Guid.Empty)
                {
                    productOption.Id = Guid.NewGuid();
                }

                var parameters = new { productOption.Id, productOption.ProductId, productOption.Name, productOption.Description };
                await connection.ExecuteAsync($"insert into ProductOptions (Id, ProductId, Name, Description) values (@Id, @ProductId, @Name, @Description)", parameters);
            }
        }

        public async Task UpdateOption(ProductOption productOption)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { productOption.Id, productOption.Name, productOption.Description };
                await connection.ExecuteAsync($"update ProductOptions set Name = @Name, Description = @Description where Id = @Id", parameters);
            }
        }

        public async Task DeleteOption(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { id };
                await connection.ExecuteAsync($"delete from ProductOptions where Id = @Id", parameters);
            }
        }
    }
}
