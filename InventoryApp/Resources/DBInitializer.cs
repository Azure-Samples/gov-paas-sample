using System.Linq;
using InventoryApp.Models;

namespace InventoryApp.Resources
{
    public class DBInitializer
    {
        public static void Initialize(MyDB context)
        {
            context.Database.EnsureCreated();

            // Look for any products
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            var products = new Product[]
            {
            new Product{Name="Chocolate",Description="Hershey bar",Quantity=3},

            };
            foreach (Product p in products)
            {
                context.Products.Add(p);
            }
            context.SaveChanges();
        }
    }
}
