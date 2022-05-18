using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifyInventoryWebClient.Models;
using System.Text;

namespace ShopifyInventoryWebClient.Controllers
{
    public class ProductsController : Controller
    {
        public IConfiguration configuration;
        public static string baseUrl;

        public ProductsController(IConfiguration iConfig)
        {
            configuration = iConfig;
            baseUrl = configuration.GetSection("AppSettings").GetSection("BaseUrl").Value;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> productList = new List<Product>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{baseUrl}api/products"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    productList = JsonConvert.DeserializeObject<List<Product>>(apiResponse);
                }
            }
            return View(productList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Description")] Product product)
        {
            var createObj = new
            {
                Name = product.Name,
                Description = product.Description,
            };
            var httpClient = new HttpClient();
            var newCreated = JsonConvert.SerializeObject(createObj);
            var requestContent = new StringContent(newCreated, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{baseUrl}api/products", requestContent);
            if((int)response.StatusCode == 201)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, string Name, string Description)
        {
            var createObj = new
            {
                ProductId = Id,
                Name = Name,
                Description = Description
            };
            var httpClient = new HttpClient();
            var newCreated = JsonConvert.SerializeObject(createObj);
            var requestContent = new StringContent(newCreated, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"{baseUrl}api/products", requestContent);
            if ((int)response.StatusCode == 200)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync($"{baseUrl}api/products/" + id);
            return RedirectToAction(nameof(Index));
        }
    }
}
