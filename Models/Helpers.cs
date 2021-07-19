using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace RefactorThis.Models
{
    public class Helpers
    {
        //private const string ConnectionString = "Data Source=App_Data/products.db";
        private const string ConnectionString = "Server=.;Database=product;Trusted_Connection=True;"; // https://www.connectionstrings.com/sql-server/

        //public static SqliteConnection NewConnection()
        //{
        //    return new SqliteConnection(ConnectionString);
        //}

        public static SqlConnection NewConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
