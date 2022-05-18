using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShopifyInventoryWebClient.Models;
using System.Text;

namespace ShopifyInventoryWebClient.Controllers
{
    public class ItemsController : Controller
    {
        public IConfiguration configuration;
        public static string baseUrl;

        public ItemsController(IConfiguration iConfig)
        {
            configuration = iConfig;
            baseUrl = configuration.GetSection("AppSettings").GetSection("BaseUrl").Value;
        }
        public async Task<IActionResult> Index()
        {
            List<Item> itemList = new List<Item>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{baseUrl}api/inventoryItems"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    itemList = JsonConvert.DeserializeObject<List<Item>>(apiResponse);
                }
            }
            return View(itemList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateProductsDropDownList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ProductId,Cost")] Item item)
        {
            var createObj = new
            {
                ProductId = item.ProductId,
                Cost = item.Cost,
            };
            var httpClient = new HttpClient();
            var newCreated = JsonConvert.SerializeObject(createObj);
            var requestContent = new StringContent(newCreated, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{baseUrl}api/inventoryItems", requestContent);
            if ((int)response.StatusCode == 201)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, string Sku, string Cost)
        {
            var createObj = new
            {
                ItemId = Id,    
                Sku = Sku,
                Cost = string.IsNullOrEmpty(Cost) ? null : Cost,
            };
            var httpClient = new HttpClient();
            var newCreated = JsonConvert.SerializeObject(createObj);
            var requestContent = new StringContent(newCreated, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"{baseUrl}api/inventoryItems", requestContent);
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
            var response = await httpClient.DeleteAsync($"{baseUrl}api/inventoryItems/" + id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateProductsDropDownList(object selectedProduct = null)
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

            ViewBag.ProductId = new SelectList(productList, "Id", "Name", selectedProduct);
        }
    }
}
