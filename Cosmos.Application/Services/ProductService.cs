using AutoMapper;
using Cosmos.Application.Entities;
using Cosmos.Core.DTO;
using Cosmos.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace Cosmos.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ICosmosDbService<Product> _productService;
        private readonly ICosmosDbService<Category> _categoryService;
        private readonly ICosmosDbService<SubCategory> _subCategoryService;
        private readonly IMapper _mapper;
        private readonly IFileService<FileEntity> _fileService;

        public ProductService(
            ICosmosDbService<Product> productService,
            ICosmosDbService<Category> categoryService,
            ICosmosDbService<SubCategory> subCategoryService,
            IFileService<FileEntity> fileService,
             IMapper mapper)
        {
            _productService = productService;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _mapper = mapper;
            _fileService = fileService;
        }

        //public async Task AddProductAsync(ProductCreationDto creationDto)
        //{
            
        //    //// Handle file uploads and save file metadata
        //    //var fileinfo = new List<FileEntity>();
        //    //if (creationDto.Files != null && creationDto.Files.Any())
        //    //{
        //    //    foreach (var file in creationDto.Files)
        //    //    {
        //    //        using (var fileStream = file.OpenReadStream())
        //    //        {
        //    //            var uploadedFile = await _fileService.UploadFileAsync(fileStream, file.FileName);
        //    //            fileinfo.Add(uploadedFile);
        //    //        }
        //    //    }
        //    //}
        //    var product = _mapper.Map<Product>(creationDto);

        //    //product.FileEntities = fileinfo;
        //    foreach (var category in product.Categories)
        //    {
        //        category.id = Guid.NewGuid().ToString();
        //        category.ProductId = product.id;


        //        foreach (var subCategory in category.subCategories)
        //        {
        //            subCategory.id = Guid.NewGuid().ToString();
        //            subCategory.CategoryId = category.id;
        //            await _subCategoryService.AddItemAsync(subCategory);
        //        }
        //        await _categoryService.AddItemAsync(category);
        //    }

        //    await _productService.AddItemAsync(product);

        //}
        ////    var product = _mapper.Map<Product>(productDto);

        ////    product.CreatedDate = DateTime.UtcNow;
        ////    product.UpdatedDate = DateTime.UtcNow;
        ////    foreach (var category in product.Categories)
        ////    {
        ////        category.id = Guid.NewGuid().ToString();
        ////        category.ProductId = product.id;


        ////        foreach (var subCategory in category.subCategories)
        ////        {
        ////            subCategory.id = Guid.NewGuid().ToString();
        ////            subCategory.CategoryId = category.id;
        ////            await _subCategoryService.AddItemAsync(subCategory);
        ////        }
        ////        await _categoryService.AddItemAsync(category);
        ////    }

        ////    // Handle file uploads and save file metadata
        ////    var fileprints = new List<FileEntity>();
        ////    if (productDto.Files != null && productDto.Files.Any())
        ////    {
        ////        foreach (var file in productDto.Files)
        ////        {
        ////            using (var fileStream = file.OpenReadStream())
        ////            {
        ////                var uploadedFile = await _fileService.UploadFileAsync(fileStream, file.FileName);
        ////                fileprints.Add(uploadedFile);
        ////            }
        ////        }
        ////    }

        ////    product.FileEntity = fileprints;
            

        ////    await _productService.AddItemAsync(product);
        ////}
        ///
            public async Task DeleteProductAsync(string id)
        {
            await _productService.DeleteItemAsync(id, new PartitionKey(id));
        }
        public async Task AddProductAsync(Product product)
        {


            foreach (var category in product.Categories)
            {
                category.id = Guid.NewGuid().ToString();
                category.ProductId = product.id;


                foreach (var subCategory in category.subCategories)
                {
                    subCategory.id = Guid.NewGuid().ToString();
                    subCategory.CategoryId = category.id;
                    await _subCategoryService.AddItemAsync(subCategory);
                }
                await _categoryService.AddItemAsync(category);
            }

            await _productService.AddItemAsync(product);
        }


        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productService.GetItemsAsync("SELECT * FROM c");
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            var product = await _productService.GetItemAsync(id, new PartitionKey(id));

            if (product.ImageUrl is string) // Handle string case if still present
            {
                product.ImageUrl = new List<FileEntity> { new FileEntity { Name = product.ImageUrl.ToString() } };
            }

            return product;
        }

        public async Task UpdateProductAsync(string id, UpdateProductDto updateDto)
        {
            try
            {
            
                var product = await _productService.GetItemAsync(id, new PartitionKey(id));

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with id {id} not found.");
                }

               
                if (!string.IsNullOrWhiteSpace(updateDto.ProductName))
                {
                    product.ProductName = updateDto.ProductName;
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Description))
                {
                    product.Description = updateDto.Description;
                }

                if (updateDto.Price.HasValue)
                {
                    product.Price = updateDto.Price.Value;
                }

              
                product.UpdatedDate = DateTime.UtcNow;

             
                await _productService.UpdateItemAsync(id, product);
            }
            catch (Exception ex)
            {
         
                throw new Exception("An error occurred while updating the product.", ex);
            }
        }


        //public async Task<(List<Product> Products, string ContinuationToken)> GetProductsAsync(string continuationToken = null)
        //{
        //    var query = new QueryDefinition("SELECT * FROM c");
        //    var queryRequestOptions = new QueryRequestOptions { MaxItemCount = 30 };

        //    var productIterator = _productService.GetItemQueryIterator(query, continuationToken, queryRequestOptions);

        //    var products = new List<Product>();
        //    string newContinuationToken = null;

        //    while (productIterator.HasMoreResults)
        //    {
        //        var response = await productIterator.ReadNextAsync();

        //        if (response == null || response.Count == 0)
        //        {
        //            break;
        //        }

        //        products.AddRange(response);
        //        newContinuationToken = response.ContinuationToken;

        //        if (products.Count >= 30)
        //        {
        //            break;
        //        }
        //    }

        //    return (products.Take(30).ToList(), newContinuationToken);
        //}

        public async Task<ResponseDto<(IEnumerable<ProductResponseDto> Products, string ContinuationToken)>> GetProductsAsync(string continuationToken, int pageSize)
        {
            try
            {
                var result = await _productService.GetItemsWithContinuationTokenAsync(continuationToken, pageSize);

                var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(result.Items);

                return ResponseDto<(IEnumerable<ProductResponseDto> Products, string ContinuationToken)>
                    .Success((productDtos, result.ContinuationToken), "Products retrieved successfully.");
            }
            catch (Exception ex)
            {
                var errors = new List<Error> { new Error("GetProductsError", ex.Message) };
                return ResponseDto<(IEnumerable<ProductResponseDto> Products, string ContinuationToken)>
                    .Failure(errors, (int)HttpStatusCode.InternalServerError);
            }
        }

        //public async Task UpdateProductImagesAsync(string id, List<IFormFile> images)
        //{
        //    var product = await _productService.GetItemAsync(id, new PartitionKey(id));

        //    if (product == null)
        //    {
        //        throw new KeyNotFoundException($"Product with id {id} not found.");
        //    }

        //    // Initialize ImageUrl if it's null
        //    if (product.ImageUrl == null)
        //    {
        //        product.ImageUrl = new List<FileEntity>();
        //    }

        //    // Upload images using existing _fileService
        //    foreach (var image in images)
        //    {
        //        using (var fileStream = image.OpenReadStream())
        //        {
        //            var uploadedFile = await _fileService.UploadFileAsync(fileStream, image.FileName);
        //            product.ImageUrl.Add(uploadedFile);
        //        }
        //    }

        //    await _productService.UpdateItemAsync(id, product);
        //}
        public async Task UpdateProductImagesAsync(string id, List<IFormFile> images)
        {
            var product = await _productService.GetItemAsync(id, new PartitionKey(id));

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }

            // Ensure ImageUrl is initialized as a list if it's null
            if (product.ImageUrl == null)
            {
                product.ImageUrl = new List<FileEntity>();
            }

            // Upload images using the existing _fileService
            foreach (var image in images)
            {
                using (var fileStream = image.OpenReadStream())
                {
                    var uploadedFile = await _fileService.UploadFileAsync(fileStream, image.FileName);
                    product.ImageUrl.Add(uploadedFile);
                }
            }

            await _productService.UpdateItemAsync(id, product);
        }

    }
}