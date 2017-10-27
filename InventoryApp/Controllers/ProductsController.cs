using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApp.Models;
using InventoryApp.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Extensions.Caching.Distributed;

namespace InventoryApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {

        private readonly MyDB _context;
        private CloudQueueClient queueClient;
        private IDistributedCache cache;
        private static List<string> keys;

        public ProductsController(MyDB context, CloudQueueClient queueClient, IDistributedCache cache)
        {
            _context = context;
            this.queueClient = queueClient;
            this.cache = cache;
        }

            // GET: Products
            public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Quantity,Date")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                Restock(product);
                return RedirectToAction(nameof(Index));
            }
            
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Quantity")] Product product)
        {
            if (!this.ModelState.IsValid)
            {
                return View(product);
            }
            else
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                Restock(product);
                return this.RedirectToAction("Index");
            }
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .SingleOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5z
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        //Writes items needed to be restocked to a Queue as well as a Redis Cache
        public void Restock(Product product)
        {
            
            var restockQueue = this.queueClient.GetQueueReference("<name of queue>"); 
            var queueMsg = new CloudQueueMessage(product.Name + " : " + product.Description);
            string restock = product.Name + "," + product.Description;
            string id = product.Id.ToString();
            List<String> temp = new List<String>();


            if ((id != "" || restock != null) && product.Quantity == 0)
            {
                //writes to Queue
                restockQueue.AddMessageAsync(queueMsg);
                //writes to Cache
                this.cache.SetString(id, restock);

                temp.Add(id);
                keys = temp;
  
            }
        }

        // Reads the products that are currently in the cache and adds them to a list 
        // GET: Products/Restock
        [ValidateAntiForgeryToken]
        public List<RestockProducts> RestockList()
        {
            List<RestockProducts> items = new List<RestockProducts>();

            while (keys != null)
            {
                if (keys.Count > 0)
                {
                    foreach (string key in keys)
                    {
                        string item = this.cache.GetString(key);
                        List<String> substrings = item.Split(",").ToList();
                        string name = substrings[0];
                        string description = substrings[1];
                        RestockProducts rp = new RestockProducts();
                        rp.Id = Convert.ToInt32(key);
                        rp.Name = name;
                        rp.Description = description;
                        items.Add(rp);
                    }
                }

                return items;
            }
            return items;
        }

        //Displays items needed to be restocked
        [Route("Products/DisplayRestock")]
        public IActionResult DisplayRestock()
        {
            this.ViewBag.Active = false;
            List<RestockProducts> list = RestockList();
            return this.View("DisplayRestock", list);
        }

    }
}
