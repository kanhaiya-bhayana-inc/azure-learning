using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace sqlFunction
{
    public static class GetProducts
    {
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> RunProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            SqlConnection _conn = GetConnection();
            List<Product> _products = new List<Product>();
            string statement = "SELECT * FROM Products";

            _conn.Open();

            SqlCommand cmd = new SqlCommand(statement, _conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductID = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Quantity = reader.GetInt32(2).ToString(),
                    };

                    _products.Add(product);
                }
            }
            _conn.Close();
            return new OkObjectResult(JsonConvert.SerializeObject(_products));
        }

        private static SqlConnection GetConnection()
        {
            string connecitonString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SqlConnectionString");
            return new SqlConnection(connecitonString);
        }

        [FunctionName("GetProduct")]
        public static async Task<IActionResult> RunProduct(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            int productId = int.Parse(req.Query["id"]);
            string statement = String.Format("SELECT * FROM Products WHERE ProductID = {0}", productId);

            SqlConnection _conn = GetConnection();

            _conn.Open();
            SqlCommand sqlCommand = new SqlCommand(statement, _conn);
            Product product = new Product();
            try
            {


                using (SqlDataReader _reader = sqlCommand.ExecuteReader())
                {
                    _reader.Read();
                    product.ProductID = _reader.GetInt32(0);
                    product.ProductName = _reader.GetString(1);
                    product.Quantity = _reader.GetInt32(2).ToString();
                    var response = product;
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                var response = "No records found...";
                _conn.Close();
                return new OkObjectResult(response);
            }

        }
    }
}
