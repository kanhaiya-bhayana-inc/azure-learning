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
using System.Data;

namespace sqlFunction
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(requestBody);

            SqlConnection _connection = GetConnection();
            _connection.Open();

            string statement = "INSERT INTO Products(ProductName,Quantity) VALUES (@param1,@param2)";
            using (SqlCommand cmd = new SqlCommand(statement, _connection)) 
            {
                cmd.Parameters.Add("@param1", System.Data.SqlDbType.VarChar, 100).Value = product.ProductName;
                cmd.Parameters.Add("@param2", System.Data.SqlDbType.Int, 100).Value = product.Quantity;

                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            return new OkObjectResult("Product has been added...");
        }
        private static SqlConnection GetConnection()
        {
            string connecitonString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SqlConnectionString");
            return new SqlConnection(connecitonString);
        }
    }
}
