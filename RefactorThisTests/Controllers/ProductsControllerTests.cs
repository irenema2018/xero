using Microsoft.AspNetCore.Mvc;
using Moq;
using RefactorThis.Controllers;
using RefactorThis.DTO;
using RefactorThis.Models;
using RefactorThis.Repositories;
using Serilog;
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
            //1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            //2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetProducts("Sumsung");
            //3. Assert
            productRepositoryMock.Verify(p => p.GetProductsByName("Sumsung"), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WhenGetProductsByNameFromRepositoryReturnsNull()
        {
            //1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(p => p.GetProductsByName("Sumsung")).ReturnsAsync((List<Product>)null);
            var logger = new Mock<ILogger>();
            //2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetProducts("Sumsung");
            //3. Assert
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
            var logger = new Mock<ILogger>();
            // 2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetProducts("");
            // 3. Assert
            productRepositoryMock.Verify(p => p.GetProducts(), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WhenNamePassedInIsNull()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            // 2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetProducts(null);
            // 3. Assert
            productRepositoryMock.Verify(p => p.GetProducts(), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WhenNamePassedInIsWhiteSpace()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            // 2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetProducts("   ");
            //3.Assert
            productRepositoryMock.Verify(p => p.GetProducts(), Times.Once);
        }

        [Fact]
        public async Task GetProduct_ReturnsAProduct()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            var id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var product = new Product()
            {
                Id = id,
            };
            productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            // 2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetProduct(id);
            //3.Assert
            Assert.IsType<ActionResult<GetProductDto>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var getProductDto = Assert.IsType<GetProductDto>(actionResult.Value);
            Assert.Equal(id, getProductDto.Id);
        }

        [Fact]
        public async Task GetProduct_ReturnsBadRequest_WhenProductDoesNotExist()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            var product = new Product();
            product = null;
            productRepositoryMock.Setup(p => p.GetProductById(It.IsAny<Guid>())).ReturnsAsync(product);
            // 2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetProduct(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));
            //3.Assert
            Assert.IsType<ActionResult<GetProductDto>>(result);
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var errorMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal($"The product with the id [{new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC")}] does not exist.", errorMsg);
        }

        [Fact]
        public async Task CreateProduct_ReturnsOKAndProductId()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            productRepositoryMock.Setup(p => p.CreateProduct(It.IsAny<Product>())).ReturnsAsync(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"));

            var productDto = new SaveProductDto();
            productDto.Name = "One Plus";
            productDto.Description = "New looking";
            productDto.Price = 100.50M;
            productDto.DeliveryPrice = 10.95M;
            // 2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.CreateProduct(productDto);
            //3.Assert
            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var id = Assert.IsType<Guid>(actionResult.Value);
            Assert.Equal(new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC"), id);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenNameLengthIsGreaterThanSeventeen()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();

            var productDto = new SaveProductDto();
            productDto.Name = "My length is greater than 17";
            productDto.Description = "New looking";
            productDto.Price = 888.95M;
            productDto.DeliveryPrice = 10.95M;
            //2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.CreateProduct(productDto);
            //3. Assert
            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Name [My length is greater than 17] can not be longer than 17 characters.", errorMsg);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenDescriptionLengthIsGreaterThanThirtyFive()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();

            var productDto = new SaveProductDto();
            productDto.Name = "One Plus";
            productDto.Description = "Trying to write a description longer than 35. Is this enough long?";
            productDto.Price = 888.95M;
            productDto.DeliveryPrice = 10.95M;
            //2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.CreateProduct(productDto);
            //3. Assert
            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Description [Trying to write a description longer than 35. Is this enough long?] can not be longer than 23 characters.", errorMsg);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenPriceIsZero()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();

            var productDto = new SaveProductDto();
            productDto.Name = "One Plus";
            productDto.Description = "New looking";
            productDto.Price = 0M;
            productDto.DeliveryPrice = 10.95M;
            //2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.CreateProduct(productDto);
            //3. Assert
            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result); 
            var errorMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("The price must be greater than $0.", errorMsg);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenDeliveryPriceIsLessThanZero()
        {
            //1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            var productDto = new SaveProductDto();
            productDto.Name = "One Plus";
            productDto.Description = "New looking";
            productDto.Price = 988.95M;
            productDto.DeliveryPrice = -0.95M;
            //2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.CreateProduct(productDto);
            //3. Assert
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
            var logger = new Mock<ILogger>();
            // 2. Act
            var productDto = new SaveProductDto()
            {
                Name = "Good Name",
                Description = "Good Description",
                Price = 998M,
                DeliveryPrice = 10.5M,
            };
            var id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.UpdateProduct(id, productDto);
            // 3. Assert
            productRepositoryMock.Verify(p => p.UpdateProduct(It.IsAny<Product>()), Times.Once);
            Assert.IsType<ActionResult<string>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var okMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal($"The product with the id [{id}] has been updated.", okMsg);
        }

        // other 4 validation unit cases (name, description, price, delivery price)

        [Fact]
        public async Task DeleteProduct_ReturnsOkAndAOkMsg()
        {
            // 1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();

            var id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var product = new Product()
            {
                Id = id,
            };
            productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            // 2.Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            await sut.DeleteProduct(id);
            // 3.Assert
            productRepositoryMock.Verify(p => p.DeleteOptions(id), Times.Once);
            productRepositoryMock.Verify(p => p.DeleteProduct(id), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFoundAndAMsg_WhenProductDoesNotExist()
        {
            //1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            var id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var product = new Product();
            product = null;
            productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            //2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.DeleteProduct(id);
            //3. Assert
            Assert.IsType<ActionResult<string>>(result);
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var notFoundMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal($"The product with the id [{id}] does not exist.", notFoundMsg);
        }

        [Fact]
        public async Task GetOptions_ReturnsOptions_WhenProductIdIsPassedIn()
        {
            //1. Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();
            var id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var product = new Product()
            {
                Id = id,
                Name = "Good Name",
                Description = "Good Description",
                Price = 998M,
                DeliveryPrice = 10.5M,
            };

            var productOptions = new List<ProductOption>();

            var productOption = new ProductOption()
            {
                ProductId = id,
            };
            productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);
            productRepositoryMock.Setup(p => p.GetOptions(id)).ReturnsAsync(productOptions);            
            //2. Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            await sut.GetOptions(id);
            //3. Assert
            productRepositoryMock.Verify(p => p.GetOptions(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetOptions_ReturnsNotFoundAndAMsg_WhenProductDoesNotExist()
        {
            // 1.Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();

            var id = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var product = new Product();
            product = null;
            productRepositoryMock.Setup(p => p.GetProductById(id)).ReturnsAsync(product);

            // 2.Act
            var sut = new ProductsController(productRepositoryMock.Object,logger.Object);
            var result = await sut.GetOptions(id);
            // 3.Assert
            Assert.IsType<ActionResult<List<GetProductOptionDto>>>(result);
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var notFoundMsg = Assert.IsType<string>(actionResult.Value);
            Assert.Equal($"The product with the id [{id}] does not exist.", notFoundMsg);
        }

        [Fact]
        public async Task GetOption_ReturnsAProductOptionDto()
        {
            //1.Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();

            var productId = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var id = new Guid("4C8BA093-CABD-4A8C-B4E6-19B251A67210");
            var product = new Product();
            productRepositoryMock.Setup(p => p.GetProductById(productId)).ReturnsAsync(product);

            var productOptionDto = new GetProductOptionDto();

            var productOption = new ProductOption();
            productRepositoryMock.Setup(p => p.GetOptionById(id)).ReturnsAsync(productOption);

            //2.Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetOption(productId, id);
            //3.Assert
            Assert.IsType<ActionResult<GetProductOptionDto>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<GetProductOptionDto>(actionResult.Value);
        }

        [Fact]
        public async Task GetOption_ReturnsNotFound_WhenProductDoesNotExist()
        {
            //1.Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();

            var productId = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var id = new Guid("4C8BA093-CABD-4A8C-B4E6-19B251A67210");
            var product = new Product();
            product = null;
            productRepositoryMock.Setup(p => p.GetProductById(productId)).ReturnsAsync(product);

            //2.Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetOption(productId, id);

            //3.Assert
            Assert.IsType<ActionResult<GetProductOptionDto>>(result);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetOption_ReturnsNotFound_WhenProductOptionDoesNotExist()
        {
            //1.Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var logger = new Mock<ILogger>();

            var productId = new Guid("37AF0817-161D-4EFA-94C5-FFEC90BD66FC");
            var id = new Guid("4C8BA093-CABD-4A8C-B4E6-19B251A67210");
            var product = new Product();
            productRepositoryMock.Setup(p => p.GetProductById(productId)).ReturnsAsync(product);


            var productOption = new ProductOption();
            productOption = null;
            productRepositoryMock.Setup(p => p.GetOptionById(id)).ReturnsAsync(productOption);

            //2.Act
            var sut = new ProductsController(productRepositoryMock.Object, logger.Object);
            var result = await sut.GetOption(productId, id);

            //3.Assert
            Assert.IsType<ActionResult<GetProductOptionDto>>(result);
            Assert.IsType<NotFoundObjectResult>(result.Result);


        }

    }

}


//1.Arrange
//2.Act
//3.Assert

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