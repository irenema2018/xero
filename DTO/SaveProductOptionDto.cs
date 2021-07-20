using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RefactorThis.DTO
{
    public class SaveProductOptionDto
    {

        public Guid ProductId { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }

    }
}
