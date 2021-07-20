﻿using RefactorThis.Models;
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
            //var conn = Helpers.NewConnection();
            //conn.Open();
            //var cmd = conn.CreateCommand();

            //cmd.CommandText = $"insert into Products (id, name, description, price, deliveryprice) values ('{product.Id}', '{product.Name}', '{product.Description}', {product.Price}, {product.DeliveryPrice})"; 

            //conn.Open();
            //cmd.ExecuteNonQuery();
        }

        public async Task<Product> GetById(Guid id) 
        {
            using (var connection = Helpers.NewConnection())
            {
                // if the pass in id is not a Guid, return an err msg BadRequest()
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
                var options = await connection.QueryAsync<ProductOption>("select Id, ProductId, Name, Description from ProductOptions");
                return options.ToList();
            }

        }

        //public ProductOptions()
        //{
        //    LoadProductOptions(null);
        //}

        //public ProductOptions(Guid productId)
        //{
        //    LoadProductOptions($"where productid = '{productId}' collate nocase");
        //}

        //private void LoadProductOptions(string where)
        //{
        //    Items = new List<ProductOption>();
        //    var conn = Helpers.NewConnection();
        //    conn.Open();
        //    var cmd = conn.CreateCommand();

        //    cmd.CommandText = $"select id from productoptions {where}";

        //    var rdr = cmd.ExecuteReader();
        //    while (rdr.Read())
        //    {
        //        var id = Guid.Parse(rdr.GetString(0));
        //        Items.Add(new ProductOption(id));
        //    }
        //}

    }
}
