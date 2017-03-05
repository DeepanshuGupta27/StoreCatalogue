using StoreCatalogue;
using StoreCatalogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace StoreCatalogue.Controllers
{
    public class ProductController : ApiController
    {

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                List<Product> products = await DocumentDbRepository.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromUri]string productName, [FromBody]SubCategory subCategory)
        {
            if (subCategory != null)
            {
                try
                {
                    await DocumentDbRepository.CreateProductIfNotExists(productName, subCategory);
                    return Ok("Product created successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
                return BadRequest("Please select a subcategory");
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody]Product updatedProduct)
        {
            if (updatedProduct != null)
            {
                try
                {
                    await DocumentDbRepository.ReplaceProduct(updatedProduct);
                    return Ok("Product updated successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Please provide valid product.");
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody]Product product)
        {
            if (product != null)
            {
                try
                {
                    await DocumentDbRepository.DeleteProduct(product);
                    return Ok("Product deleted successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Please provide valid product.");
            }
        }
    }
}
