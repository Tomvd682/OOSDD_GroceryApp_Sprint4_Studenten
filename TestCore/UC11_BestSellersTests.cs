using System.Collections.Generic;
using System.Linq;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Xunit;
using Assert = Xunit.Assert; // voorkom conflict met NUnit.Assert

public class UC11_BestSellersTests
{
    [Fact]
    public void GetBestSellingProducts_SortsByAmountDesc_And_AssignsRanking()
    {
        // Arrange: simpele, duidelijke testdata
        var productRepo = new FakeProductRepository(new List<Product>
        {
            new Product(1, "Kaas",   stock: 100),
            new Product(2, "Brood",  stock:  50),
            new Product(3, "Appels", stock: 200),
        });

        var listRepo = new FakeGroceryListItemsRepository(new List<GroceryListItem>
{
    // Kaas = 3 + 3 = 6
    new GroceryListItem(1, 1, 1, 3) { Name = "Kaas" },
    new GroceryListItem(2, 1, 1, 3) { Name = "Kaas" },

    // Brood = 1 + 3 = 4
    new GroceryListItem(3, 1, 2, 1) { Name = "Brood" },
    new GroceryListItem(4, 2, 2, 3) { Name = "Brood" },

    // Appels = 2
    new GroceryListItem(5, 2, 3, 2) { Name = "Appels" },
});


        var sut = new GroceryListItemsService(listRepo, productRepo);

        // Act
        var result = sut.GetBestSellingProducts(topX: 3);

        // Assert: 3 rijen, aflopend op NrOfSells, ranking 1..3 en juiste volgorde
        Assert.Equal(3, result.Count);
        Assert.True(result.Zip(result.Skip(1), (a, b) => a.NrOfSells >= b.NrOfSells).All(x => x));
        Assert.Equal(new[] { 1, 2, 3 }, result.Select(r => r.Ranking).ToArray());
        Assert.Equal(new[] { "Kaas", "Brood", "Appels" }, result.Select(r => r.Name).ToArray());
    }

    // ------ FAKES (minimaal, maar volledig t.o.v. je interfaces) ------

    private sealed class FakeGroceryListItemsRepository : IGroceryListItemsRepository
    {
        private readonly List<GroceryListItem> _items;
        public FakeGroceryListItemsRepository(List<GroceryListItem> items) => _items = items;

        public List<GroceryListItem> GetAll() => _items.ToList();
        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
            => _items.Where(i => i.GroceryListId == groceryListId).ToList();

        public GroceryListItem Add(GroceryListItem item) { _items.Add(item); return item; }
        public GroceryListItem? Get(int id) => _items.FirstOrDefault(i => i.Id == id);
        public GroceryListItem? Update(GroceryListItem item)
        {
            var idx = _items.FindIndex(i => i.Id == item.Id);
            if (idx < 0) return null;
            _items[idx] = item;
            return item;
        }
        public GroceryListItem? Delete(GroceryListItem item)
        {
            var ok = _items.Remove(item);
            return ok ? item : null;
        }
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Dictionary<int, Product> _byId;
        public FakeProductRepository(IEnumerable<Product> products) => _byId = products.ToDictionary(p => p.Id);

        public List<Product> GetAll() => _byId.Values.ToList();
        public Product? Get(int id) => _byId.TryGetValue(id, out var p) ? p : null;
        public Product Add(Product product) { _byId[product.Id] = product; return product; }
        public Product? Update(Product product) { _byId[product.Id] = product; return product; }
        public Product? Delete(Product product)
        {
            // simpele delete: haal op Id weg als het bestaat
            return _byId.Remove(product.Id) ? product : null;
        }
    }
}
