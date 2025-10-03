using System;
using System.Collections.Generic;
using System.Linq;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Xunit;
using Assert = Xunit.Assert;

public class UC13_BoughtProductsServiceTests
{
    [Fact]
    public void Get_Geeft_Unieke_ClientPlusList_Rijen_Terug_En_Sorteert()
    {
        // Arrange: products, clients, lijsten, items
        var products = new List<Product>
        {
            new Product(1, "Kaas", 100),
            new Product(2, "Brood", 50)
        };

        var clients = new List<Client>
        {
            new Client(1, "M.J. Curie", "user1@mail.com", "hash"),
            new Client(2, "A.J. Kwak",  "user2@mail.com", "hash")
        };

        // Let op: jouw GroceryList ctor verwacht (int id, string name, DateOnly date, string color, int clientId)
        var today = DateOnly.FromDateTime(DateTime.Today);
        var color = "#C49B33";

        var lists = new List<GroceryList>
        {
            new GroceryList(1, "Weekboodschappen",   today, color, 1),
            new GroceryList(2, "Kerstboodschappen",  today, color, 1),
            new GroceryList(3, "Vrijdagmarkt",       today, color, 2)
        };

        var items = new List<GroceryListItem>
        {
            // Kaas in twee verschillende lijsten van dezelfde client
            new GroceryListItem(1, 1, 1, 2) { Name = "Kaas" },  // list 1
            new GroceryListItem(2, 2, 1, 1) { Name = "Kaas" },  // list 2

            // Brood in lijst van andere client (irrelevant voor Get(1))
            new GroceryListItem(3, 3, 2, 3) { Name = "Brood" },
        };

        var sut = new BoughtProductsService(
            new FakeItemsRepo(items),
            new FakeListsRepo(lists),
            new FakeClientsRepo(clients),
            new FakeProductsRepo(products)
        );

        // Act
        var result = sut.Get(1); // productId 1 = Kaas

        // Assert
        Assert.Equal(2, result.Count); // twee rijen: (Curie, Week) en (Curie, Kerst)
        // gesorteerd op Client.Name, dan GroceryList.Name -> Kerst..., Week...
        Assert.Equal("M.J. Curie", result[0].Client.Name);
        Assert.Equal("Kerstboodschappen", result[0].GroceryList.Name);
        Assert.Equal("M.J. Curie", result[1].Client.Name);
        Assert.Equal("Weekboodschappen", result[1].GroceryList.Name);
        // en altijd het juiste product
        Assert.All(result, r => Assert.Equal("Kaas", r.Product.Name));
    }

    // ---------- Mini fakes (alleen wat de service nodig heeft) ----------

    private sealed class FakeItemsRepo : IGroceryListItemsRepository
    {
        private readonly List<GroceryListItem> _items;
        public FakeItemsRepo(IEnumerable<GroceryListItem> items) => _items = items.ToList();
        public List<GroceryListItem> GetAll() => _items.ToList();

        // interface-leden die we niet nodig hebben in deze test:
        public GroceryListItem Add(GroceryListItem item) => throw new NotImplementedException();
        public GroceryListItem? Get(int id) => throw new NotImplementedException();
        public GroceryListItem? Update(GroceryListItem item) => throw new NotImplementedException();
        public GroceryListItem? Delete(GroceryListItem item) => throw new NotImplementedException();

        // Handig voor volledigheid; je interface heeft deze waarschijnlijk ook
        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
            => _items.Where(i => i.GroceryListId == groceryListId).ToList();
    }

    private sealed class FakeProductsRepo : IProductRepository
    {
        private readonly Dictionary<int, Product> _byId;
        public FakeProductsRepo(IEnumerable<Product> products) => _byId = products.ToDictionary(p => p.Id);
        public Product? Get(int id) => _byId.TryGetValue(id, out var p) ? p : null;
        public List<Product> GetAll() => _byId.Values.ToList();

        // niet gebruikt in deze test:
        public Product Add(Product product) => throw new NotImplementedException();
        public Product? Update(Product product) => throw new NotImplementedException();
        public Product? Delete(Product product) => throw new NotImplementedException();
    }

    private sealed class FakeListsRepo : IGroceryListRepository
    {
        private readonly Dictionary<int, GroceryList> _byId;
        public FakeListsRepo(IEnumerable<GroceryList> lists) => _byId = lists.ToDictionary(l => l.Id);

        public GroceryList? Get(int id) => _byId.TryGetValue(id, out var l) ? l : null;
        public List<GroceryList> GetAll() => _byId.Values.ToList();

        public GroceryList Add(GroceryList list)
        {
            _byId[list.Id] = list;
            return list;
        }

        public GroceryList? Update(GroceryList list)
        {
            _byId[list.Id] = list;
            return list;
        }

        public GroceryList? Delete(GroceryList list)
        {
            return _byId.Remove(list.Id) ? list : null;
        }

        // Als jouw interface deze ook heeft:
        // public List<GroceryList> GetAllOnClientId(int clientId)
        //     => _byId.Values.Where(l => l.ClientId == clientId).ToList();
    }

    private sealed class FakeClientsRepo : IClientRepository
    {
        private readonly Dictionary<int, Client> _byId;
        public FakeClientsRepo(IEnumerable<Client> clients) => _byId = clients.ToDictionary(c => c.Id);

        public Client? Get(int id) => _byId.TryGetValue(id, out var c) ? c : null;
        public Client? Get(string email) => _byId.Values.FirstOrDefault(c => c.EmailAddress == email);
        public List<Client> GetAll() => _byId.Values.ToList();
    }
}
