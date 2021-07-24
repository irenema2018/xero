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
    public class ProductsControllerTests
    {
        [Fact]
        public async Task GetProducts_ReturnsOk_WhenNamePassedIn()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            
            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.GetProducts("Sumsung");

            productRepositoryMock.Verify(p => p.GetProductsByName("Sumsung"), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WhenGetProductsByNameFromRepositoryReturnsNull()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(p => p.GetProductsByName("Sumsung")).ReturnsAsync((List<Product>)null);

            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.GetProducts("Sumsung");

            Assert.IsType<ActionResult<List<GetProductDto>>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<GetProductDto>>(actionResult.Value);
            Assert.Empty(list);
        }
        [Fact]
        public async Task GetProducts_ReturnsOk_WhenNamePassedInIsEmptyString()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            // 2. Act
            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.GetProducts(""); 
            // 3. Assert
            productRepositoryMock.Verify(p => p.GetProducts(), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WhenNamePassedInIsNull()
        {
            var productRepositoryMock = new Mock<IProductRepository>();

            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.GetProducts(null);

            productRepositoryMock.Verify(p => p.GetProducts(), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WhenNamePassedInIsWhiteSpace()
        {
            var productRepositoryMock = new Mock<IProductRepository>();

            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.GetProducts("   ");

            productRepositoryMock.Verify(p => p.GetProducts(), Times.Once);
        }

        [Fact]
        public async Task GetProduct_ReturnsAProduct()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var product = new Product()
            {
                Id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")
            };
            productRepositoryMock.Setup(p => p.GetProductById(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"))).ReturnsAsync(product);
            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.GetProduct(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));

            Assert.IsType<ActionResult<GetProductDto>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var getProductDto = Assert.IsType<GetProductDto>(actionResult.Value);
            Assert.Equal(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"), getProductDto.Id);

            //Assert.Equal(productExeResult.Result.DeliveryPrice, getProductDto.DeliveryPrice);
            //Assert.Equal(productExeResult.Result.Description, getProductDto.Description);
            //Assert.Equal(productExeResult.Result.Name, getProductDto.Name);
            //Assert.Equal(productExeResult.Result.Price, getProductDto.Price);
        }

        [Fact]
        public async Task GetProduct_ReturnsBadRequest_WhenProductDoesNotExist()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var product = new Product();
            product = null;
            productRepositoryMock.Setup(p => p.GetProductById(It.IsAny<Guid>())).ReturnsAsync(product);

            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.GetProduct(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));
            Assert.IsType<ActionResult<GetProductDto>>(result);
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var errorMsg = Assert.IsType<string>(actionResult.Value);

            Assert.Equal($"The product with the id [{new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")}] does not exist.", errorMsg);
        }

        [Fact]
        public async Task CreateProduct_ReturnsOKAndProductId()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(p => p.CreateProduct(It.IsAny<Product>())).ReturnsAsync(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));

            var sut = new ProductsController(productRepositoryMock.Object);
            var productDto = new SaveProductDto();
            productDto.Name = "One Plus";
            productDto.Description = "New looking";
            productDto.Price = 100.50M;
            productDto.DeliveryPrice = 10.95M;
            var result = await sut.CreateProduct(productDto);

            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var id = Assert.IsType<Guid>(actionResult.Value);
            Assert.Equal(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"), id);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenNameLengthIsGreaterThanSeventeen()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var sut = new ProductsController(productRepositoryMock.Object);

            var productDto = new SaveProductDto();
            productDto.Name = "My length is greater than 17";
            productDto.Description = "New looking";
            productDto.Price = 888.95M;
            productDto.DeliveryPrice = 10.95M;
            var result = await sut.CreateProduct(productDto);

            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Name [My length is greater than 17] can not be longer than 17 characters.", errorMsg);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenDescriptionLengthIsGreaterThanThirtyFive()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var sut = new ProductsController(productRepositoryMock.Object);

            var productDto = new SaveProductDto();
            productDto.Name = "One Plus";
            productDto.Description = "Trying to write a description with a length longer than 35. Is this enough long?";
            productDto.Price = 888.95M;
            productDto.DeliveryPrice = 10.95M;
            var result = await sut.CreateProduct(productDto);

            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Description [Trying to write a description with a length longer than 35. Is this enough long?] can not be longer than 23 characters.", errorMsg);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenPriceIsZero()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var sut = new ProductsController(productRepositoryMock.Object);

            var productDto = new SaveProductDto();
            productDto.Name = "One Plus";
            productDto.Description = "New looking";
            productDto.Price = 0M;
            productDto.DeliveryPrice = 10.95M;
            var result = await sut.CreateProduct(productDto);

            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result); // test if the result is ok
            var errorMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("The price must be greater than $0.", errorMsg);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenDeliveryPriceIsLessThanZero()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var productDto = new SaveProductDto();
            productDto.Name = "One Plus";
            productDto.Description = "New looking";
            productDto.Price = 988.95M;
            productDto.DeliveryPrice = -0.95M;

            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.CreateProduct(productDto);
            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("The delivery price must be greater than or equal to $0", errorMsg);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsOkAndAOkMsg()
        {
            // 1. Arrange   
            var productRepositoryMock = new Mock<IProductRepository>();

            // 2. Act
            var productDto = new SaveProductDto()
            {
                Name = "Good Name",
                Description = "Good Description",
                Price = 998M,
                DeliveryPrice = 10.5M,
            };
            var id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.UpdateProduct(id, productDto);

            // 3. Assert
            productRepositoryMock.Verify(p => p.UpdateProduct(It.IsAny<Product>()),Times.Once);
            Assert.IsType<ActionResult<string>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var okMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal($"The product with the id [{id}] has been updated.", okMsg);
        }

        // other 4 validation unit cases (name, description, price, delivery price)

        [Fact]
        public async Task DeleteProduct_ReturnsOkAndAOkMsg()
        {
            // Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var product = new Product()
            {
                Id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")
            };

            productRepositoryMock.Setup(p => p.GetProductById(product.Id)).ReturnsAsync(product);
            // Act
            var sut = new ProductsController(productRepositoryMock.Object);
            await sut.DeleteProduct(product.Id);
            // Assert
            productRepositoryMock.Verify(p => p.DeleteOptions(product.Id),Times.Once);
            productRepositoryMock.Verify(p => p.DeleteProduct(product.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFoundAndAMsg_WhenProductDoesNotExist()
        {
            //Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var product = new Product();
            product = null;
            productRepositoryMock.Setup(p => p.GetProductById(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"))).ReturnsAsync(product);
            // Act
            var sut = new ProductsController(productRepositoryMock.Object);
            var result = await sut.DeleteProduct(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));
            Assert.IsType<ActionResult<string>>(result);
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var notFoundMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal($"The product with the id [{new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")}] does not exist.", notFoundMsg);
        }

        //[Fact]
        //public async Task GetOptions_ReturnsOptions_WhenProductIdIsPassedIn()
        //{
        //    var productRepositoryMock = new Mock<IProductRepository>();
        //    var product = new Product();
        //    //{
        //    //    Id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")
        //    //};

        //    productRepositoryMock.Setup(p => p.GetProductById(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")).ReturnsAsync(product);
        //}

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