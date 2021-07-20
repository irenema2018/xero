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
            //productRepository = new ProductRepository();
        }//constructor. every time you use productController, a new productRepository will be created.

        [HttpGet]
        public async Task<ActionResult> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var productsDto = new List<GetProductDto>(); 
                var products = await _productRepository.GetProducts();
                foreach (var product in products) //todo don't repeat yourself
                {
                    var productDto = new GetProductDto();
                    //productDto.Id = Guid.Parse(product.Id);
                    productDto.Id = product.Id;
                    productDto.Price = product.Price;
                    productDto.DeliveryPrice = product.DeliveryPrice;
                    productDto.Name = product.Name;
                    productDto.Description = product.Description;

                    productsDto.Add(productDto);
                }
                return Ok(productsDto);
            }
            else
            {
                var productsDto = new List<GetProductDto>();
                var products = await _productRepository.GetByName(name);
                foreach (var product in products)
                {
                    var productDto = new GetProductDto();
                    productDto.Id = product.Id;
                    productDto.Price = product.Price;
                    productDto.DeliveryPrice = product.DeliveryPrice;
                    productDto.Name = product.Name;
                    productDto.Description = product.Description;
                    productsDto.Add(productDto);
                }
                    return Ok(productsDto);
                }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            //var product = new Product(id.ToString());
            var product = await _productRepository.GetById(id);

            if (product == null)
                return NotFound();

            var productDto = new GetProductDto();
            productDto.Id = product.Id;
            productDto.Price = product.Price;
            productDto.DeliveryPrice = product.DeliveryPrice;
            productDto.Name = product.Name;
            productDto.Description = product.Description;

            return Ok(productDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(SaveProductDto productDto)
        {
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

            await _productRepository.Add(product);
            return Ok(product.Id);
            // err handling: duplicate name is not allowed
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, SaveProductDto productDto)
        {
            //var orig = new Product(id.ToString())
            //{
            //    Name = product.Name,
            //    Description = product.Description,
            //    Price = product.Price,
            //    DeliveryPrice = product.DeliveryPrice
            //};

            //if (!orig.IsNew)
            //    orig.Save();

            // 1. parse guid id to string id
            // 2. check if the string id exists.loop products. not exists, return BadRequest err msg
            // 3.  declare a new product
            // 4. assign the pass in values from productdto to product
            // 5. return OK() and new value
            var product = new Product();
            product.Id = id;
            product.Price = productDto.Price;
            product.DeliveryPrice = productDto.DeliveryPrice;
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            
            await _productRepository.Update(product);
            return Ok($"The product with the id [{id}] has been update");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var product = new Product(id.ToString());
            if (product.IsNew)
            {
                return NotFound($"The product with the [{id}] does not exist.");
            }
            else
            {
                //product.Delete();
                await _productRepository.DeleteOptions(id);
                await _productRepository.Delete(id);
                return Ok($"The product with the id [{id}] has been deleted.");
            }

        }

        [HttpGet("{productId}/options")]
        public ProductOptions GetOptions(Guid productId)
        {
            return new ProductOptions(productId);
        }

        [HttpGet("{productId}/options/{id}")]
        //public ProductOption GetOption(Guid productId, Guid id)
        //{
        //    var option = new ProductOption(id);
        //    if (option.IsNew)
        //        throw new Exception();

        //    return option;
        //}
        public ActionResult GetOption(Guid productId, Guid id)
        {
            var product = new Product(productId.ToString());

            if (product.IsNew)
                return NotFound($"The product with the id [{id}] does not exist.");

            var productOption = new ProductOption(id);

            if (productOption.IsNew)
                return NotFound($"The product option with the id [{id}] does not exist.");

            var productOptionDto = new GetProductOptionsDto();
            productOptionDto.ProductId = productOption.ProductId;
            productOptionDto.Name = productOption.Name;
            productOptionDto.Description = productOption.Description;
            return Ok(productOptionDto);//successful
        }


        [HttpPost("{productId}/options")]
        //public void CreateOption(Guid productId, ProductOption option)
        //{
        //    option.ProductId = productId;
        //    option.Save();
        //}
        public ActionResult CreateOption(Guid productId, SaveProductOptionsDto productOptionsDto)
        {
            // if the product has existed by checking the productId, return with a status with an error msg.
            var product = new Product(productId.ToString());
            if (product.IsNew)
                return NotFound("$The product with the id [{productId}] does not exist.");

            var productOption = new ProductOption();
            productOption.ProductId = productOptionsDto.ProductId;
            //if (productOptionsDto.isNew)
            productOption.Name = productOptionsDto.Name;
            productOption.Description = productOptionsDto.Description;

            productOption.Save();
            return Ok(productOption.Id);

        }



        [HttpPut("{productId}/options/{id}")]
        public void UpdateOption(Guid id, ProductOption option)
        {
            var orig = new ProductOption(id)
            {
                Name = option.Name,
                Description = option.Description
            };

            if (!orig.IsNew)
                orig.Save();
        }

        [HttpDelete("{productId}/options/{id}")]
        //public void DeleteOption(Guid id)
        //{
        //    var opt = new ProductOption(id);
        //    opt.Delete();
        //}
        public ActionResult DeleteOption(Guid productId, Guid id)
        {
            var product = new Product(productId.ToString());
            if (product.IsNew)
                return NotFound($"The product with the id [{productId}] does not exist.");

            var productOption = new ProductOption(id);
            if (productOption.IsNew)
                return NotFound($"The product option with the id [{id}] does not exist.");

            productOption.Delete();
            return Ok($"The product option with the id [{id}] has been deleted.");//successful
        }
    }
}