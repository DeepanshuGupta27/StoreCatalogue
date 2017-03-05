using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using StoreCatalogue.Models;
using Microsoft.Azure.Documents.Linq;

namespace StoreCatalogue
{
    public static class DocumentDbRepository
    {
        public static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        public static readonly string CategoryCollectionId = ConfigurationManager.AppSettings["categoryCollection"];
        public static readonly string ProductCollectionId = ConfigurationManager.AppSettings["productCollection"];
        public static readonly string SubCategoryCollectionId = ConfigurationManager.AppSettings["subCategoryCollection"];
        private static DocumentClient client;

        public static void Initialize()
        {
            client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync(CategoryCollectionId).Wait();
            CreateCollectionIfNotExistsAsync(ProductCollectionId).Wait();
            CreateCollectionIfNotExistsAsync(SubCategoryCollectionId).Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync(string collectionId)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = collectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }


        internal static async Task CreateCategoryIfNotExists(string categoryName)
        {
            Category temp_category = client.CreateDocumentQuery<Category>(
                                        UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoryCollectionId))
                                    .Where(f => f.Name == categoryName).AsEnumerable().FirstOrDefault();
            if (temp_category == null)
            {
                Category category = new Category { Id = Guid.NewGuid(), Name = categoryName };
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoryCollectionId), category);
            }
            else
            {
                throw new StoreCatalogueException("Category name already exist.");
            }

        }

        internal static async Task CreateSubCategoryIfNotExists(string subCategoryName, Category category)
        {

            Category temp_category = client.CreateDocumentQuery<Category>(
                                        UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoryCollectionId))
                                    .Where(f => f.Id == category.Id).AsEnumerable().FirstOrDefault();


            if (temp_category != null)
            {
                SubCategory temp_subCategory = client.CreateDocumentQuery<SubCategory>(
                                                UriFactory.CreateDocumentCollectionUri(DatabaseId, SubCategoryCollectionId))
                                                .Where(f => f.Name == subCategoryName && f.CategoryId == category.Id).AsEnumerable().FirstOrDefault();

                if (temp_subCategory == null)
                {

                    SubCategory subCategory = new SubCategory { Id = Guid.NewGuid(), Name = subCategoryName, CategoryId = category.Id };
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, SubCategoryCollectionId), subCategory);
                }
                else
                {
                    throw new StoreCatalogueException("Sub Category Name already exist.");
                }
            }
            else
            {
                throw new StoreCatalogueException("Category does not exist.");
            }

        }

        internal static async Task CreateProductIfNotExists(string productName, SubCategory subCategory)
        {
            SubCategory temp_subCategory = client.CreateDocumentQuery<SubCategory>(
                                           UriFactory.CreateDocumentCollectionUri(DatabaseId, SubCategoryCollectionId))
                                           .Where(f => f.Id == subCategory.Id).AsEnumerable().FirstOrDefault();


            if (temp_subCategory != null)
            {
                Product temp_product = client.CreateDocumentQuery<Product>(
                                                UriFactory.CreateDocumentCollectionUri(DatabaseId, ProductCollectionId))
                                                .Where(f => f.Name == productName && f.SubCategoryId == subCategory.Id).AsEnumerable().FirstOrDefault();

                if (temp_product == null)
                {

                    Product product = new Product { Id = Guid.NewGuid(), Name = productName, SubCategoryId = subCategory.Id };
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, ProductCollectionId), product);
                }
                else
                {
                    throw new StoreCatalogueException("Product already exist.");
                }
            }
            else
            {
                throw new StoreCatalogueException("SubCategory does not exist.");
            }
        }

        internal static async Task DeleteSubCategory(SubCategory subCategory)
        {
            Product product = client.CreateDocumentQuery<Product>(
                                          UriFactory.CreateDocumentCollectionUri(DatabaseId, ProductCollectionId))
                                          .Where(f => f.SubCategoryId == subCategory.Id).AsEnumerable().FirstOrDefault();

            if (product == null)
            {
                await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, SubCategoryCollectionId, subCategory.Id.ToString()));
            }
            else
            {
                throw new StoreCatalogueException("SubCategory cannot be deleted.It has products assosciated with it.");
            }
        }

        internal static async Task DeleteCategory(Category category)
        {
            SubCategory subCategory = client.CreateDocumentQuery<SubCategory>(
                                          UriFactory.CreateDocumentCollectionUri(DatabaseId, SubCategoryCollectionId))
                                          .Where(f => f.CategoryId == category.Id).AsEnumerable().FirstOrDefault();

            if (subCategory == null)
            {
                await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CategoryCollectionId, category.Id.ToString()));
            }
            else
            {
                throw new StoreCatalogueException("Category cannot be deleted.It has subCategories assosciated with it.");
            }
        }

        internal static async Task DeleteProduct(Product product)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, ProductCollectionId, product.Id.ToString()));
        }

        internal static async Task ReplaceCategory(Category updatedCategory)
        {
            Category temp_category = client.CreateDocumentQuery<Category>(
                                     UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoryCollectionId))
                                     .Where(f => f.Id == updatedCategory.Id).AsEnumerable().FirstOrDefault();

            if (temp_category != null)
            {
                if (temp_category.Name != updatedCategory.Name)
                    await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CategoryCollectionId, updatedCategory.Id.ToString()), updatedCategory);
                else
                    throw new StoreCatalogueException("There is nothing to update in category.");
            }
            else
            {
                throw new StoreCatalogueException("Category does not exist.");
            }
        }

        internal static async Task ReplaceSubCategory(SubCategory updatedSubCategory)
        {
            SubCategory temp_subCategory = client.CreateDocumentQuery<SubCategory>(
                                     UriFactory.CreateDocumentCollectionUri(DatabaseId, SubCategoryCollectionId))
                                     .Where(f => f.Id == updatedSubCategory.Id).AsEnumerable().FirstOrDefault();

            if (temp_subCategory != null)
            {
                if (temp_subCategory.Name != updatedSubCategory.Name)
                    await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, SubCategoryCollectionId, updatedSubCategory.Id.ToString()), updatedSubCategory);
                else
                    throw new StoreCatalogueException("There is nothing to update in subCategory.");
            }
            else
            {
                throw new StoreCatalogueException("subCategory does not exist.");
            }
        }

        internal static async Task ReplaceProduct(Product updatedProduct)
        {
            Product temp_product = client.CreateDocumentQuery<Product>(
                                     UriFactory.CreateDocumentCollectionUri(DatabaseId, ProductCollectionId))
                                     .Where(f => f.Id == updatedProduct.Id).AsEnumerable().FirstOrDefault();

            if (temp_product != null)
            {
                if (temp_product.Name != updatedProduct.Name)
                    await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, ProductCollectionId, updatedProduct.Id.ToString()), updatedProduct);
                else
                    throw new StoreCatalogueException("There is nothing to update in product.");
            }
            else
            {
                throw new StoreCatalogueException("product does not exist.");
            }
        }

        public static Category GetCategory(string categoryName)
        {

            Category category = client.CreateDocumentQuery<Category>(
                                     UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoryCollectionId))
                                     .Where(f => f.Name == categoryName).AsEnumerable().FirstOrDefault();
            return category;

        }

        public static Product GetProduct(string productName, SubCategory subCategory)
        {

            Product product = client.CreateDocumentQuery<Product>(
                                     UriFactory.CreateDocumentCollectionUri(DatabaseId, ProductCollectionId))
                                     .Where(f => f.Name == productName && f.SubCategoryId == subCategory.Id).AsEnumerable().FirstOrDefault();
            return product;

        }

        public static SubCategory GetSubCategory(string subCategoryName, Category category)
        {

            if (category != null)
            {
                SubCategory subCategory = client.CreateDocumentQuery<SubCategory>(
                                          UriFactory.CreateDocumentCollectionUri(DatabaseId, SubCategoryCollectionId))
                                         .Where(f => f.Name == subCategoryName && f.CategoryId == category.Id).AsEnumerable().FirstOrDefault();
                return subCategory;
            }
            else
            {
                return null;
            }

        }

        internal async static Task<List<Category>> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            IDocumentQuery<Category> query = client.CreateDocumentQuery<Category>(
                                      UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoryCollectionId))
                                     .AsDocumentQuery();
            
            while (query.HasMoreResults)
            {
                categories.AddRange(await query.ExecuteNextAsync<Category>());
            }

            return categories;
        }

        internal async static Task<List<SubCategory>> GetAllSubCategories()
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            IDocumentQuery<SubCategory> query = client.CreateDocumentQuery<SubCategory>(
                                      UriFactory.CreateDocumentCollectionUri(DatabaseId, SubCategoryCollectionId))
                                     .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                subCategories.AddRange(await query.ExecuteNextAsync<SubCategory>());
            }

            return subCategories;
        }

        internal async static Task<List<Product>> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            IDocumentQuery<Product> query = client.CreateDocumentQuery<Product>(
                                      UriFactory.CreateDocumentCollectionUri(DatabaseId, ProductCollectionId))
                                     .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                products.AddRange(await query.ExecuteNextAsync<Product>());
            }

            return products;
        }
    }

}