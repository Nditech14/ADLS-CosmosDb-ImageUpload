using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Core.Entities
{
    public class FileEntity
    {
        public string Id { get; set; }
        public string Name { get; set; } 
        public string Url { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; } 

    }
}
