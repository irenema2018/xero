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
    }

    public class ProductRepository : IProductRepository // repository is reponsible for database opertations -- crud
    {
        public async Task Add(Product product)
        {
            using (var connection = Helpers.NewConnection())
            {
                if (string.IsNullOrWhiteSpace(product.Id))
                {
                    product.Id = Guid.NewGuid().ToString();
                }

                var parameters = new { product.Id, product.Name, product.Price, product.Description, product.DeliveryPrice };
                await connection.ExecuteAsync($"insert into Products (id, name, description, price, deliveryprice) values (@Id, @Name, @Description, @Price, @DeliveryPrice)", parameters);
            }
            //var conn = Helpers.NewConnection();
            //conn.Open();
            //var cmd = conn.CreateCommand();

            //cmd.CommandText = $"insert into Products (id, name, description, price, deliveryprice) values ('{product.Id}', '{product.Name}', '{product.Description}', {product.Price}, {product.DeliveryPrice})"; 

            //conn.Open();
            //cmd.ExecuteNonQuery();
        }

        public void GetById(Guid productId) { }

        public async Task<List<Product>> GetProducts()
        {

            using (var connection = Helpers.NewConnection())
            {
                var products = await connection.QueryAsync<Product>("select id, name, description, CAST(price AS REAL) as price, CAST(deliveryprice AS REAL) as deliveryprice from products");
                return products.ToList();
            }

            //var conn = Helpers.NewConnection();
            //conn.Open();
            //var cmd = conn.CreateCommand();

            //cmd.CommandText = $"select id from Products {where}";

            //var rdr = cmd.ExecuteReader();
            //while (rdr.Read())
            //{
            //    var id = Guid.Parse(rdr.GetString(0));
            //    products.Add(new Product(id));
            //}

        }

        public void Update(Product product) { }

        public void Delete(Guid productId) { }
    }
}
