using StoreCatalogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using StoreCatalogue.Models;


namespace StoreCatalogue.Controllers
{
    public class CategoryController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                List<Category> categories = await DocumentDbRepository.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromUri]string categoryName)
        {
            try
            {
                await DocumentDbRepository.CreateCategoryIfNotExists(categoryName);
                return Ok("Category created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody]Category updatedCategory)
        {
            if (updatedCategory != null)
            {
                try
                {
                    await DocumentDbRepository.ReplaceCategory(updatedCategory);
                    return Ok("Category updated successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Please provide valid category.");
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody]Category category)
        {
            if (category != null)
            {
                try
                {
                    await DocumentDbRepository.DeleteCategory(category);
                    return Ok("Category deleted successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Please provide valid category.");
            }
        }
    }
}
