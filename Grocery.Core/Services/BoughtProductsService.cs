
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Linq;


namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            // Guard: geen productId => lege lijst terug
            if (productId is null) return new List<BoughtProducts>();

            // 1) Pak alle list-items met dit product (alleen Amount > 0 telt als “gekocht”)
            var items = _groceryListItemsRepository
                .GetAll()
                .Where(i => i.ProductId == productId.Value && i.Amount > 0)
                .ToList();

            if (items.Count == 0) return new List<BoughtProducts>();

            // 2) Haal het product op (voor naam/validatie)
            var product = _productRepository.Get(productId.Value);
            if (product is null) return new List<BoughtProducts>();

            // 3) Maak regels per (klant, lijst) die dit product bevat(ten)
            var rows = new List<BoughtProducts>();
            foreach (var it in items)
            {
                var list = _groceryListRepository.Get(it.GroceryListId);
                if (list is null) continue;

                var client = _clientRepository.Get(list.ClientId);
                if (client is null) continue;

                rows.Add(new BoughtProducts(client, list, product));
            }

            // 4) Uniek maken op combinatie (Client, GroceryList) en netjes sorteren
            return rows
                .GroupBy(bp => new { ClientId = bp.Client.Id, ListId = bp.GroceryList.Id })
                .Select(g => g.First())
                .OrderBy(bp => bp.Client.Name)
                .ThenBy(bp => bp.GroceryList.Name)
                .ToList();
        }

    }
}
