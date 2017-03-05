using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoreCatalogue.Controllers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Configuration;
using StoreCatalogue.Models;
using System.Web.Http.Results;

namespace StoreCatalogue.Tests
{
    [TestClass]
    public class StoreCatalogueTest
    {
        CategoryController categoryController;
        ProductController productController;
        SubCategoryController subCategoryController;
        private const string categoryName = "Chinese";
        private const string subCategoryName = "Starter";
        private const string productName = "Manchurian";
        private const string replaceCategoryName = "Vegetarian";
        private const string replaceSubCategoryName = "Main Course";
        private const string replaceProduct = "Paneer";

        [TestInitialize]
        public void TestInitialize()
        {
            ConfigurationManager.AppSettings["endpoint"] = "https://tillposstorecatalogue.documents.azure.com:443/";
            ConfigurationManager.AppSettings["authKey"] = "btdKjGOuBTkqAcpwlw7HF7CIe4pTrY9APkjIXuQbFvdR3PYOILeauVqMU2sYj0zgDjpLMpjmEFlfNG6fXKjhDQ==";
            ConfigurationManager.AppSettings["database"] = "StoreCatalogue";
            ConfigurationManager.AppSettings["categoryCollection"] = "Categories";
            ConfigurationManager.AppSettings["subCategoryCollection"] = "SubCategories";
            ConfigurationManager.AppSettings["productCollection"] = "Products";
            DocumentDbRepository.Initialize();
            categoryController = new CategoryController();
            productController = new ProductController();
            subCategoryController = new SubCategoryController();
        }

        [TestMethod]
        public void IntegrationTest()
        {
            Test_AddCategory();
            Test_AddSubCategory();
            Test_AddProduct();

            Test_UpdateCategory();
            Test_UpdateSubCategory();
            Test_UpdateProduct();

            Test_Delete();
        }

        public void Test_AddCategory()
        {
            Category category = DocumentDbRepository.GetCategory(categoryName);
            var result = categoryController.Post(categoryName).Result;
            Assert.IsNotNull(result);
            if (category == null)
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));
            else
                Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

        }

        public void Test_AddSubCategory()
        {
            Category category = null;
            var result = subCategoryController.Post(subCategoryName, category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            category = new Category { Id = Guid.NewGuid(), Name = categoryName };
            result = subCategoryController.Post(subCategoryName, category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            category = DocumentDbRepository.GetCategory(categoryName);
            SubCategory subCategory = DocumentDbRepository.GetSubCategory(subCategoryName, category);
            result = subCategoryController.Post(subCategoryName, category).Result;
            Assert.IsNotNull(result);

            if (subCategory == null)
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));
            else
                Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

        }

        public void Test_AddProduct()
        {
            SubCategory subCategory = null;
            var result = productController.Post(productName, subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            subCategory = new SubCategory { Id = Guid.NewGuid(), Name = subCategoryName, CategoryId = Guid.NewGuid() };
            result = productController.Post(productName, subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            Category category = DocumentDbRepository.GetCategory(categoryName);
            subCategory = DocumentDbRepository.GetSubCategory(subCategoryName, category);
            Product product = DocumentDbRepository.GetProduct(productName, subCategory);
            result = productController.Post(productName, subCategory).Result;
            Assert.IsNotNull(result);

            if (product == null)
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));
            else
                Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

        }

        public void Test_UpdateCategory()
        {
            Category category = null;
            var result = categoryController.Put(category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            category = new Category { Id = Guid.NewGuid(), Name = replaceCategoryName };
            result = categoryController.Put(category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            category = DocumentDbRepository.GetCategory(categoryName);
            result = categoryController.Put(category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            category.Name = replaceCategoryName;
            result = categoryController.Put(category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));
        }

        public void Test_UpdateSubCategory()
        {
            SubCategory subCategory = null;
            var result = subCategoryController.Put(subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            subCategory = new SubCategory { Id = Guid.NewGuid(), Name = subCategoryName, CategoryId = Guid.NewGuid() };
            result = subCategoryController.Put(subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            Category category = DocumentDbRepository.GetCategory(replaceCategoryName);
            subCategory = DocumentDbRepository.GetSubCategory(subCategoryName, category);
            result = subCategoryController.Put(subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            subCategory.Name = replaceSubCategoryName;
            result = subCategoryController.Put(subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));
        }

        public void Test_UpdateProduct()
        {
            Product product = null;
            var result = productController.Put(product).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            product = new Product { Id = Guid.NewGuid(), Name = productName, SubCategoryId = Guid.NewGuid() };
            result = productController.Put(product).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            Category category = DocumentDbRepository.GetCategory(replaceCategoryName);
            SubCategory subCategory = DocumentDbRepository.GetSubCategory(replaceSubCategoryName, category);
            product = DocumentDbRepository.GetProduct(productName, subCategory);
            result = productController.Put(product).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            product.Name = replaceProduct;
            result = productController.Put(product).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));
        }

        public void Test_Delete()
        {
            string productName = replaceProduct;
            string subCategoryName = replaceSubCategoryName;
            string categoryName = replaceCategoryName;

            Category category = null;
            SubCategory subCategory = null;
            Product product = null;
            var result = categoryController.Delete(category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            result = subCategoryController.Delete(subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            result = productController.Delete(product).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            category = DocumentDbRepository.GetCategory(categoryName);
            subCategory = DocumentDbRepository.GetSubCategory(subCategoryName, category);
            product = DocumentDbRepository.GetProduct(productName, subCategory);


            result = categoryController.Delete(category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            result = subCategoryController.Delete(subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            result = productController.Delete(product).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));

            result = subCategoryController.Delete(subCategory).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));

            result = categoryController.Delete(category).Result;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));

        }
        
    }
}
