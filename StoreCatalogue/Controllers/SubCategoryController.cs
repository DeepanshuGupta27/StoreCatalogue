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
    public class SubCategoryController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                List<SubCategory> subCategories = await DocumentDbRepository.GetAllSubCategories();
                return Ok(subCategories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromUri]string subCategoryName, [FromBody]Category category)
        {
            if (category != null)
            {
                try
                {
                    await DocumentDbRepository.CreateSubCategoryIfNotExists(subCategoryName, category);
                    return Ok("SubCategory created successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
                return BadRequest("Please select a category");
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody]SubCategory updatedSubCategory)
        {
            if (updatedSubCategory != null)
            {
                try
                {
                    await DocumentDbRepository.ReplaceSubCategory(updatedSubCategory);
                    return Ok("SubCategory updated successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Please provide valid subcategory.");
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody]SubCategory subCategory)
        {
            if (subCategory != null)
            {
                try
                {
                    await DocumentDbRepository.DeleteSubCategory(subCategory);
                    return Ok("SubCategory deleted successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Please provide valid subcategory.");
            }
        }
    }
}
