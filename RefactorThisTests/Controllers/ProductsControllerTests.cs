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
    public class FakeRepository : IProductRepository
    {
        public Task CreateOption(ProductOption productOption)
        {
            throw new NotImplementedException();
        }

        public Task CreateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOption(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOptions(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProduct(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductOption> GetOptionById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductOption>> GetOptions(Guid productId)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetProductById(Guid id)
        {
            var product = new Product();

            if (id == new Guid("11111111-1111-1111-1111-111111111111"))
            {
                product.Id = id;
                product.Name = "Good Product Name";
                product.Description = "Good Product Desc";
                product.Price = decimal.Parse("100.50");
                product.DeliveryPrice = decimal.Parse("19.95");
            }
            else if (id == new Guid("22222222-2222-2222-2222-222222222222"))
            {
                product = null;
            }

            return product;
        }

        public Task<List<Product>> GetProducts()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetProductsByName(string name)
        {
            var products = new List<Product>();
            var product = new Product();
            product.Id = new Guid("33333333-3333-3333-3333-333333333333");
            product.Name = $"New {name}";
            product.Description = "First Product Desc";
            product.Price = decimal.Parse("100.50");
            product.DeliveryPrice = decimal.Parse("19.95");

            products.Add(product);

            var product2 = new Product();
            product2.Id = new Guid("44444444-4444-4444-4444-444444444444");
            product2.Name = $"{name} v2.0";
            product2.Description = "Second Product Desc";
            product2.Price = decimal.Parse("266.50");
            product2.DeliveryPrice = decimal.Parse("13.65");

            products.Add(product2);
            return products;
        }

        public Task UpdateOption(ProductOption productOption)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductsControllerTests
    {
        //[Fact] // label for an unit test case
        //public async Task GetProduct_ReturnsAProduct()
        //{
        //    //var productRepository = new FakeRepository();
        //    // subject under testing
        //    var productRepositoryMock = new Mock<IProductRepository>();
        //    var product = new Product()
        //    {
        //        Id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")
        //    };
        //    productRepositoryMock.Setup(p => p.GetProductById(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"))).ReturnsAsync(product);
        //    var sut = new ProductsController(productRepositoryMock.Object);
        //    var result = await sut.GetProduct(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));
        //    Assert.NotNull(result);
        //    Assert.IsType<ActionResult<GetProductDto>>(result);

        //    var actionResult = Assert.IsType<OkObjectResult>(result.Result); //Result is a class member
        //    var getProductDto = Assert.IsType<GetProductDto>(actionResult.Value);
        //    Assert.Equal(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"), getProductDto.Id);

        //    //Assert.Equal(productExeResult.Result.DeliveryPrice, getProductDto.DeliveryPrice);
        //    //Assert.Equal(productExeResult.Result.Description, getProductDto.Description);
        //    //Assert.Equal(productExeResult.Result.Name, getProductDto.Name);
        //    //Assert.Equal(productExeResult.Result.Price, getProductDto.Price);
        //}

        [Fact]
        public async Task GetProducts_ReturnsOk_WhenNameIsPassedIn()
        {
            // Assemble
            var sut = new ProductsController(new FakeRepository());

            // Act
           
            var taskResult = await sut.GetProducts("Sumsung"); // Task<ActionResult<GetProductDto>>

            // Assert 
            Assert.NotNull(taskResult);
            var actionResult = Assert.IsType<OkObjectResult>(taskResult.Result);

            Assert.NotNull(actionResult.Value);
            var dtoList = Assert.IsType<List<GetProductDto>>(actionResult.Value);
                      
            Assert.Equal(2, dtoList.Count);
            //Assert.Equal("Good Product Name", dto.Name);
            //Assert.Equal("Good Product Desc", dto.Description);
            //Assert.Equal(decimal.Parse("100.50"), dto.Price);
            //Assert.Equal(decimal.Parse("19.95"), dto.DeliveryPrice);
        }

        [Fact]
        public async Task GetProduct_ReturnsOk()
        {
            // Assemble
            var sut = new ProductsController(new FakeRepository());

            // Act
            var id = new Guid("11111111-1111-1111-1111-111111111111");
            var taskResult = await sut.GetProduct(id); // Task<ActionResult<GetProductDto>>

            // Assert 
            Assert.NotNull(taskResult);
            var actionResult = Assert.IsType<OkObjectResult>(taskResult.Result);

            Assert.NotNull(actionResult.Value);
            var dto = Assert.IsType<GetProductDto>(actionResult.Value);

            Assert.Equal(id, dto.Id);
            Assert.Equal("Good Product Name", dto.Name);
            Assert.Equal("Good Product Desc", dto.Description);
            Assert.Equal(decimal.Parse("100.50"), dto.Price);
            Assert.Equal(decimal.Parse("19.95"), dto.DeliveryPrice);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductIdDoesNotExist()
        {
            var sut = new ProductsController(new FakeRepository());

            var id = new Guid("22222222-2222-2222-2222-222222222222");
            var taskResult = await sut.GetProduct(id); // Task<ActionResult<GetProductDto>>

            Assert.NotNull(taskResult);
            Assert.IsType<NotFoundObjectResult>(taskResult.Result);
        }
    }
}

/*
Unit Test (dev)
    -> Integration Test / System Testing (test team)
        -> User Acceptance Testing (Manual) (finish dev)
            -> Smoke Test / Kick the tyres (release)
                -> Business Verification Test (production)


                        ProductRepository --> Database
IProductRepository
                        FakeProductRepository

----------------------------------------------------------------

*/