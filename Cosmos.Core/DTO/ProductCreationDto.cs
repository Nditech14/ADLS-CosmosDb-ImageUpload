using Cosmos.Application.Entities;
using Cosmos.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Core.DTO
{
     public  class ProductCreationDto
    {
        [Required(ErrorMessage = "ProductName is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "ProductName must be between 3 and 50 characters")]
        public string ProductName { get; set; } = string.Empty;
        

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 200 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<FileEntity> ?ImageUrl { get; set; } = new List<FileEntity>();


    }
}
