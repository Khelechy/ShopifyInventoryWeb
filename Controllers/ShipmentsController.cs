using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShopifyInventoryWebClient.Models;
using System.Text;

namespace ShopifyInventoryWebClient.Controllers
{
    public class ShipmentsController : Controller
    {
        public IConfiguration configuration;
        public static string baseUrl;

        public ShipmentsController(IConfiguration iConfig)
        {
            configuration = iConfig;
            baseUrl = configuration.GetSection("AppSettings").GetSection("BaseUrl").Value;
        }
        public async Task<IActionResult> Index()
        {
            List<Shipment> itemList = new List<Shipment>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{baseUrl}api/shipments"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    itemList = JsonConvert.DeserializeObject<List<Shipment>>(apiResponse);
                }
            }
            return View(itemList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateInventoryDropDownList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("InventoryId,Quantity,Location")] Shipment shipment)
        {
            var createObj = new
            {
                InventoryId = shipment.InventoryId,
                Quantity = shipment.Quantity,
                Location = shipment.Location
            };
            var httpClient = new HttpClient();
            var newCreated = JsonConvert.SerializeObject(createObj);
            var requestContent = new StringContent(newCreated, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{baseUrl}api/shipments", requestContent);
            if ((int)response.StatusCode == 201)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        private async Task PopulateInventoryDropDownList(object selectedInventory = null)
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

            ViewBag.InventoryBag = new SelectList(inventoryList, "Id", "Item.Product.Name", selectedInventory);
        }
    }
}
