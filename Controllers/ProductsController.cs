using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RefactorThis.DTO;
using RefactorThis.Models;
using RefactorThis.Repositories;

namespace RefactorThis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductRepository _productRepository; //declare (ProductRepository productRepository)class member at first, have changed to interfece.

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;

        }//constructor. every time you use productController, a new productRepository will be created.

        [HttpGet]
        public async Task<ActionResult<List<GetProductDto>>> GetProducts(string name)
        {
            List<Product> products;

            if (string.IsNullOrWhiteSpace(name))
                products = await _productRepository.GetProducts();
            else
                products = await _productRepository.GetProductsByName(name);

            var productsDto = new List<GetProductDto>();

            foreach (var product in products)
            {
                productsDto.Add(CopyProductToDto(product));
            }
            
            return Ok(productsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductDto>> GetProduct(Guid id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
                return NotFound($"The product with the id [{id}] does not exist.");

            return Ok(CopyProductToDto(product));
        }

        //The below function is for getting a product dto.
        private GetProductDto CopyProductToDto(Product product)
        {
            var productDto = new GetProductDto();
            productDto.Id            = product.Id;
            productDto.Price         = product.Price;
            productDto.DeliveryPrice = product.DeliveryPrice;
            productDto.Name          = product.Name;
            productDto.Description   = product.Description;

            return productDto;
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(SaveProductDto productDto)
        {
            // TODO: check length
            //Name varchar no  17
            //Description varchar no  35

            if (productDto.Price <= 0)
            {
                return BadRequest("Thre price must be greater than $0.");
            }

            if (productDto.DeliveryPrice < 0)
            {
                return BadRequest("The delivery price must be greater than or equal to $0");
            }

            var product = new Product();
            product.Price = productDto.Price;
            product.DeliveryPrice = productDto.DeliveryPrice;
            product.Name = productDto.Name;
            product.Description = productDto.Description;

            await _productRepository.CreateProduct(product);

            return Ok(product.Id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(Guid id, SaveProductDto productDto)
        {
            var product = new Product();
            product.Id = id;
            product.Price = productDto.Price;
            product.DeliveryPrice = productDto.DeliveryPrice;
            product.Name = productDto.Name;
            product.Description = productDto.Description;

            await _productRepository.UpdateProduct(product);

            return Ok($"The product with the id [{id}] has been updated.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
                return NotFound($"The product with the id [{id}] does not exist.");

            await _productRepository.DeleteOptions(id);
            await _productRepository.DeleteProduct(id);

            return Ok($"The product with the id [{id}] has been deleted.");
        }

        [HttpGet("{productId}/options")]
        public async Task<ActionResult> GetOptions(Guid productId)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
                return NotFound($"The product with the id [{productId}] does not exist.");
            
            var productOptions = await _productRepository.GetOptions(productId);
            // TODO: where is DTO?

            return Ok(productOptions); // Unit Test: Make sure no function can return the original data model.
        }

        //Get a specific product option
        [HttpGet("{productId}/options/{id}")]
        public async Task<ActionResult> GetOption(Guid productId, Guid id)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
                return NotFound($"The product with the id [{productId}] does not exist.)");

            var productOption = await _productRepository.GetOptionById(id);
            if (productOption == null)
                return NotFound($"The product option with the id [{id}] does not exist.");

            var productOptionDto = new GetProductOptionDto();
            productOptionDto.Id          = productOption.Id;
            productOptionDto.ProductId   = productOption.ProductId;
            productOptionDto.Name        = productOption.Name;
            productOptionDto.Description = productOption.Description;
            
            return Ok(productOptionDto);
        }

        [HttpPost("{productId}/options")]
        public async Task<ActionResult> CreateOption(Guid productId, SaveProductOptionDto productOptionDto)
        {
            if (productOptionDto.Name.Length > 9)
                return BadRequest($"Name [{productOptionDto.Name}] can not be longer than 9 characters.");

            if (productOptionDto.Description.Length > 23)
                return BadRequest($"Description [{productOptionDto.Description}] can not be longer than 23 characters.");

            var productOption = new ProductOption();
            productOption.ProductId     = productId;
            productOption.Name          = productOptionDto.Name;
            productOption.Description   = productOptionDto.Description;
            
            await _productRepository.CreateOption(productOption);
            
            return Ok(productOption.Id);
        }

        [HttpPut("{productId}/options/{id}")]
        public async Task<ActionResult> UpdateOption(Guid productId, Guid id, SaveProductOptionDto productOptionDto)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
                return NotFound($"The product with the id [{productId}] does not exist.)");

            var productOption = await _productRepository.GetOptionById(id);
            if (productOption == null)
                return NotFound($"The product option with the id [{id}] does not exist.");

            if (productId != productOption.ProductId)
                return BadRequest($"The product option does not belong to the product.");
         
            if (productOptionDto.Name.Length > 9)
                return BadRequest($"Name [{productOptionDto.Name}] can not be longer than 9 characters.");

            if (productOptionDto.Description.Length > 23)
                return BadRequest($"Description [{productOptionDto.Description}] can not be longer than 23 characters.");

            productOption.Name = productOptionDto.Name;
            productOption.Description = productOptionDto.Description;

            await _productRepository.UpdateOption(productOption);

            return Ok($"The product option with the id [{id}] has been updated.");
        }

        [HttpDelete("{productId}/options/{id}")]
        public async Task<ActionResult> DeleteOption(Guid productId, Guid id)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
                return NotFound($"The product with the id [{productId}] does not exist.)");

            var productOption = await _productRepository.GetOptionById(id);
            if (productOption == null)
                return NotFound($"The product option with the id [{id}] does not exist.");

            await _productRepository.DeleteOption(id);
            return Ok($"The product option with the id [{id}] has been deleted.");
        }
    }
}