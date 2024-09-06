using Cosmos.Application.Entities;
using Cosmos.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Core.DTO
{
     public  class ProductDto
    {
        public string ProductName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

    }
}
