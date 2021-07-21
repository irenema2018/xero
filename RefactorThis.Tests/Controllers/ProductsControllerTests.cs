using RefactorThis.Controllers;
using RefactorThis.Models;
using RefactorThis.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RefactorThis.Tests.Controllers
{
    public class FakeRepository : IProductRepository
    {
        public Task Add(Product product)
        {
            throw new NotImplementedException();
        }

        public Task AddProductOption(Guid productId, ProductOption productOption)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOptions(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetById(Guid id)
        {
            return Task.FromResult(new Product());
        }

        public Task<List<Product>> GetByName(string Name)
        {
            throw new NotImplementedException();
        }

        public Task<ProductOption> GetProductOptionById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductOption>> GetProductOptions(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetProducts()
        {
            throw new NotImplementedException();
        }

        public Task Update(Product product)
        {
            throw new NotImplementedException();
        }
    }



    public class ProductsControllerTests 
    {
        [Fact] // label for an unit test case
        public async Task GetProduct_ReturnsAProduct()
        {
            var productRepository = new FakeRepository();
            // subject under testing
            var sut = new ProductsController(productRepository);
            var result = await sut.GetProduct(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));
            Assert.NotNull(result);
        }

        //[Fact]
        //public async Task GetProduct_ReturnsNotFound_WhenProductIdDoesNotExist()
        //{

        //}

    }
}
