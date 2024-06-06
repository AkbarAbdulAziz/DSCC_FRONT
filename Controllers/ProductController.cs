using CourseWork.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CourseWork.MVC.Controllers
{
    public class ProductController : Controller
    {

        Uri baseAddress = new Uri("http://95.130.227.225/api/10547/");
        HttpClient client;
      
        // GET: Product
        public ProductController()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }


        public async Task<ActionResult> Index()
        {
            List<Product> products = new List<Product>();
            HttpResponseMessage response = await client.GetAsync("products");

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<Product>>(responseString);
            }

            return View(products);

        }

        // GET: Product/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
        {
            //string Baseurl = "http://apifor10247.us-west-1.elasticbeanstalk.com/";
            Product productDetails = new Product();
            using (var client = new HttpClient())
            {
                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource using HttpClient
                HttpResponseMessage Res = await client.GetAsync("products/" + id);

                //Checking the response is successful or not which is sent HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details received from web api
                    var PrResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response received from web api and storing the Product 
                    productDetails = JsonConvert.DeserializeObject<Product>(PrResponse);


                }

                //returning the Product list to view
                return View(productDetails);
            }
        }

        //  GET: Product/Create
        public async Task<ActionResult> Create()
        {
            HttpResponseMessage response = await client.GetAsync("categories");

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                ViewBag.Categories = JsonConvert.DeserializeObject<List<Category>>(responseString);
            }

            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public async Task<ActionResult> Create(Product product)
        {
            // Assuming you have configured the base address and HttpClient elsewhere in your controller

            // Sending request to retrieve the category details from the API
            HttpResponseMessage categoryResponse = await client.GetAsync("categories/" + product.ProductCategory.Id);

            if (categoryResponse.IsSuccessStatusCode)
            {
                // Storing the response details received from the web API
                var categoryResponseBody = await categoryResponse.Content.ReadAsStringAsync();

                // Deserializing the response to get the category details
                var categoryProd = JsonConvert.DeserializeObject<Category>(categoryResponseBody);
                product.ProductCategory = categoryProd;
            }
            else
            {
                // Handle the case where retrieving category details fails
                // You might want to log the error or handle it appropriately
                return RedirectToAction("Index"); // Redirect to the index action if there's an error
            }

            // Serializing the product object to JSON
            string productData = JsonConvert.SerializeObject(product);

            // Creating StringContent to hold the JSON data
            StringContent content = new StringContent(productData, Encoding.UTF8, "application/json");

            // Sending a POST request to create a new product
            HttpResponseMessage response = await client.PostAsync("products", content);

            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the index action
                return RedirectToAction("Index");
            }
            else
            {
                // If the request fails, handle it appropriately
                // You might want to log the error or display an error message to the user
                return View(); // Return to the same view if there's an error
            }
        }

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Product product = new Product();
            HttpResponseMessage Res = client.GetAsync("products/" + id).Result;
            if (Res.IsSuccessStatusCode)
            {
                string data = Res.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(data);
            }

            HttpResponseMessage Resp = await client.GetAsync("categories");
            //Checking the response is successful or not which is sent HttpClient
            if (Resp.IsSuccessStatusCode)
            {
                //Storing the response details received from web api
                var PrResponse = Resp.Content.ReadAsStringAsync().Result;

                //Deserializing the response received from web api and storing the Product List
                var categoriesList = JsonConvert.DeserializeObject<List<Category>>(PrResponse);
                ViewBag.categories = categoriesList;

            }
            return View("Edit", product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Product product)
        {
            //get categories list for viewbag
            HttpResponseMessage Res = await client.GetAsync("ategories/" + product.ProductCategory.Id);
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details received from web api
                var PrResponse = Res.Content.ReadAsStringAsync().Result;

                //Deserializing the response 
                var categoryProd = JsonConvert.DeserializeObject<Category>(PrResponse);
                product.ProductCategory = categoryProd;
            }

            string data = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PutAsync(client.BaseAddress + "products/" + product.Id, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var deleteProduct = client.DeleteAsync("products/" + id.ToString());

                HttpResponseMessage response = deleteProduct.Result;
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

            return View();
        }
    }
}