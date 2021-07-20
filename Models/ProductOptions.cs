using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RefactorThis.Models
{
    public class ProductOptions
    {
        public List<ProductOption> Items { get; private set; }

        //public ProductOptions()
        //{
        //    LoadProductOptions(null);
        //}

        //public ProductOptions(Guid productId)
        //{
        //    LoadProductOptions($"where productid = '{productId}' collate nocase");
        //}

        //private void LoadProductOptions(string where)
        //{
        //    Items = new List<ProductOption>();
        //    var conn = Helpers.NewConnection();
        //    conn.Open();
        //    var cmd = conn.CreateCommand();

        //    cmd.CommandText = $"select id from productoptions {where}";

        //    var rdr = cmd.ExecuteReader();
        //    while (rdr.Read())
        //    {
        //        var id = Guid.Parse(rdr.GetString(0));
        //        Items.Add(new ProductOption(id));
        //    }
        //}
    }


}
