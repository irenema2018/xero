using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;

namespace RefactorThis.Models
{
    [Table("Products")]
    public class Product
    {   
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal DeliveryPrice { get; set; }

        [NotMapped]// Excludes the property from all operations. Info from Dapper
        [JsonIgnore]
        public bool IsNew { get; }

        public Product()
        {
            IsNew = true;
        }

    }

}
