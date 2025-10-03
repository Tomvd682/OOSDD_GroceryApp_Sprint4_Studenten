using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            // 1) Haal alle list-items op en vul bijbehorende Product-info
            var allItems = _groceriesRepository.GetAll();
            FillService(allItems); // zet g.Product via _productRepository

            // 2) Groepeer per product en tel VERKOOP (sum van Amount)
            var aggregated =
                allItems
                .GroupBy(i => new
                {
                    i.ProductId,
                    Name = i.Product!.Name,
                    Stock = i.Product!.Stock
                })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.Name,
                    g.Key.Stock,
                    NrOfSells = g.Sum(i => i.Amount) 
                })
                .OrderByDescending(x => x.NrOfSells)
                .ThenBy(x => x.Name)
                .ToList();

            // 3) De producten ranken
            var result = new List<BestSellingProducts>();
            for (int index = 0; index < aggregated.Count; index++)
            {
                var x = aggregated[index];
                result.Add(new BestSellingProducts(
                    productId: x.ProductId,
                    name: x.Name,
                    stock: x.Stock,
                    nrOfSells: x.NrOfSells,
                    ranking: index + 1
                ));
            }

            // 4) Alleen top X teruggeven
            return result.Take(topX > 0 ? topX : 5).ToList();
        }



        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
