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
        Task Add(Product product);
        Task<List<Product>> GetProducts();
        Task<Product> GetById(Guid id);
        Task<List<Product>> GetByName(string Name);
        Task Update(Product product);
        Task Delete(Guid id);
        Task DeleteOptions(Guid id);

        Task<List<ProductOption>> GetProductOptions(Guid productId);
        Task<ProductOption> GetProductOptionById(Guid id);
        Task AddProductOption(Guid productId, ProductOption productOption);
    }

    public class ProductRepository : IProductRepository // repository is reponsible for database opertations -- crud
    {
        public async Task Add(Product product)
        {
            using (var connection = Helpers.NewConnection())
            {
                if (product.Id == null || product.Id == Guid.Empty)
                {
                    product.Id = Guid.NewGuid();
                }

                var parameters = new { product.Id, product.Name, product.Price, product.Description, product.DeliveryPrice };
                await connection.ExecuteAsync($"insert into Products (id, name, description, price, deliveryprice) values (@Id, @Name, @Description, @Price, @DeliveryPrice)", parameters);
            }
        }

        public async Task<Product> GetById(Guid id) 
        {
            using (var connection = Helpers.NewConnection())
            {
               
                var parameters = new { id };
                var result = await connection.QueryAsync<Product>($"select Id, Name, Description, Price, DeliveryPrice from Products where Id = @id", parameters);

                return result.FirstOrDefault(); 
            }
        }

        public async Task<List<Product>> GetByName(string name)
        {
            using (var connection = Helpers.NewConnection())
            {
                // if the pass in id is not a Guid, return an err msg BadRequest()
                name = "%" + name + "%";
                var parameters = new { name };
                var result = await connection.QueryAsync<Product>($"select id, name, description, CAST(price AS REAL) as price, CAST(deliveryprice AS REAL) as deliveryprice from products where name like @name", parameters);

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

        public async Task Update(Product product) 
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new {product.Id, product.Name, product.Price, product.Description, product.DeliveryPrice };
                await connection.ExecuteAsync($"update Products set name = @Name, description = @Description, price = @Price, deliveryprice = @DeliveryPrice where id = @Id", parameters);
            }
        }

        public async Task Delete(Guid id) 
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { id };
                await connection.ExecuteAsync($"delete from products where id = @id",parameters);
            }
        }

        public async Task DeleteOptions(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { id};
                await connection.ExecuteAsync($"delete from productoptions where productid = @id", parameters);
            }
        }

        public async Task<List<ProductOption>> GetProductOptions(Guid productId)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { productId };
                var options = await connection.QueryAsync<ProductOption>("select Id, ProductId, Name, Description from ProductOptions where productId = @productId",parameters);
                return options.ToList();
            }

        }


        public async Task<ProductOption> GetProductOptionById(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {
                var parameters = new { id };
                var result = await connection.QueryAsync<ProductOption>("select Id, ProductId, Name, Description from ProductOptions where Id = @id",parameters);
                return result.FirstOrDefault();
            }
        }
       
        public async Task AddProductOption(Guid productId, ProductOption productOption)
        {
            using (var connection = Helpers.NewConnection())
            {
                if (productOption.Id == null || productOption.Id == Guid.Empty)
                {
                    productOption.Id = Guid.NewGuid();
                }

                var parameters = new { productOption.Id, productOption.ProductId, productOption.Name, productOption.Description};
                await connection.ExecuteAsync($"insert into ProductOptions (Id, ProductId, Name, Description) values (@Id, @ProductId, @Name, @Description)", parameters);
            }
        }

    }
}
