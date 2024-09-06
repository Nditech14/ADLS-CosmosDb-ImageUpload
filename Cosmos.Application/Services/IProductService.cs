using Cosmos.Application.Entities;
using Cosmos.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace Cosmos.Application.Services
{
    public interface IProductService
    {
        Task AddProductAsync(Product product);
        Task DeleteProductAsync(string id);
        Task UpdateProductImagesAsync(string id, List<IFormFile> images);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(string id);
        // Task<(List<Product> Products, string ContinuationToken)> GetProductsAsync(string continuationToken = null);
        Task<ResponseDto<(IEnumerable<ProductResponseDto> Products, string ContinuationToken)>> GetProductsAsync(string continuationToken, int pageSize);
        Task UpdateProductAsync(string id, UpdateProductDto updateDto);
    }
}