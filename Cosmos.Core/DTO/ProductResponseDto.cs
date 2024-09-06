using Cosmos.Application.Entities;
using Cosmos.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Core.DTO
{
     public  class ProductResponseDto
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string ProductName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<FileEntity> ImageUrl { get; set; }

    }
}
