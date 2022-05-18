using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShopifyInventoryWebClient.Models;
using System.Text;

namespace ShopifyInventoryWebClient.Controllers
{
    public class InventoryController : Controller
    {
        public IConfiguration configuration;
        public static string baseUrl;

        public InventoryController(IConfiguration iConfig)
        {
            configuration = iConfig;
            baseUrl = configuration.GetSection("AppSettings").GetSection("BaseUrl").Value;
        }
        public async Task<IActionResult> Index()
        {
            List<Inventory> inventoryList = new List<Inventory>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{baseUrl}api/inventories"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    inventoryList = JsonConvert.DeserializeObject<List<Inventory>>(apiResponse);
                }
            }
            return View(inventoryList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateItemsDropDownList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ItemId,Quantity")] Inventory inventory)
        {
            var createObj = new
            {
                ItemId = inventory.ItemId,
                Quantity = inventory.Quantity,
            };
            var httpClient = new HttpClient();
            var newCreated = JsonConvert.SerializeObject(createObj);
            var requestContent = new StringContent(newCreated, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{baseUrl}api/inventories", requestContent);
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
        public async Task<IActionResult> Edit(int Id, int Quantity)
        {
            var createObj = new
            {
                InventoryId = Id,
                Quantity = Quantity
            };
            var httpClient = new HttpClient();
            var newCreated = JsonConvert.SerializeObject(createObj);
            var requestContent = new StringContent(newCreated, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"{baseUrl}api/inventories", requestContent);
            if ((int)response.StatusCode == 200)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        private async Task PopulateItemsDropDownList(object selectedItem = null)
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

            ViewBag.ItemId = new SelectList(itemList, "Id", "Product.Name", selectedItem);
        }
    }
}
