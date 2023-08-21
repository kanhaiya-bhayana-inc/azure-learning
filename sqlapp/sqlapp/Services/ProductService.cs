using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using sqlapp.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace sqlapp.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IFeatureManager _featureManager;
        public ProductService(IConfiguration configuration, IFeatureManager featureManager)
        {
            _configuration = configuration;
            _featureManager = featureManager;
        }

        public async Task<bool> IsBeta()
        {
            if (await _featureManager.IsEnabledAsync("beta"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration["SqlConnection"]);
        }

        public async Task<List<Product>> GetProducts()
        {
            /*SqlConnection conn = GetConnection();
            List<Product> _products = new List<Product>();
            string statement = "SELECT * FROM Products";

            conn.Open();

            SqlCommand cmd = new SqlCommand(statement, conn);

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
            conn.Close();
            return _products;*/

            string FunctionURL = "https://firstazfunc.azurewebsites.net/api/GetProducts?code=sIBVGjdFSJUTi3b1vRNiJLo5LJyFAUqXsBJGvoGrfuOZAzFuinuyFA==";
            
            using(HttpClient client = new HttpClient()) 
            {
                HttpResponseMessage response = await client.GetAsync(FunctionURL);
                
                string content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Product>>(content);  
            }

        }
    }
}
