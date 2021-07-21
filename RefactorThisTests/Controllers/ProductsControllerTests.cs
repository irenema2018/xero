using Microsoft.AspNetCore.Mvc;
using Moq;
using RefactorThis.Controllers;
using RefactorThis.DTO;
using RefactorThis.Models;
using RefactorThis.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RefactorThis.Tests.Controllers
{
    //public class FakeRepository : IProductRepository
    //{
    //    public Task Add(Product product)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task AddProductOption(Guid productId, ProductOption productOption)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task Delete(Guid id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task DeleteOptions(Guid id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<Product> GetById(Guid id)
    //    {
    //        return Task.FromResult(new Product());
    //    }

    //    public Task<List<Product>> GetByName(string Name)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<ProductOption> GetProductOptionById(Guid id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<List<ProductOption>> GetProductOptions(Guid productId)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<List<Product>> GetProducts()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task Update(Product product)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}




    public class ProductsControllerTests
    {
        [Fact] // label for an unit test case
        public async Task GetProduct_ReturnsAProduct()
        {
            //var productRepository = new FakeRepository();
            // subject under testing
            var productRepositoryMock = new Mock<IProductRepository>();
            var product = new Product()
            {
                Id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")
            };
            productRepositoryMock.Setup(p => p.GetProductById(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"))).ReturnsAsync(product);
            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.GetProduct(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));
            Assert.NotNull(result);
            Assert.IsType<ActionResult<GetProductDto>>(result);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result); //Result is a class member
            var getProductDto = Assert.IsType<GetProductDto>(actionResult.Value);
            Assert.Equal(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"), getProductDto.Id);
            //Assert.Equal(productExeResult.Result.DeliveryPrice, getProductDto.DeliveryPrice);
            //Assert.Equal(productExeResult.Result.Description, getProductDto.Description);
            //Assert.Equal(productExeResult.Result.Name, getProductDto.Name);
            //Assert.Equal(productExeResult.Result.Price, getProductDto.Price);
        }

        //[Fact]
        //public async Task GetProduct_ReturnsNotFound_WhenProductIdDoesNotExist()
        //{

        //}

    }
}
