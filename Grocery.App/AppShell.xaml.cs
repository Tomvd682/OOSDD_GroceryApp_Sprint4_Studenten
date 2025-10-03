using System.Linq;
using Grocery.App.ViewModels;   // GlobalViewModel
using Grocery.App.Views;        // Views
using Grocery.Core.Models;      // Role

namespace Grocery.App
{
    public partial class AppShell : Shell
    {
        private readonly GlobalViewModel _global;

        public AppShell(GlobalViewModel global)
        {
            InitializeComponent();
            _global = global;

            // Routes die je al had
            Routing.RegisterRoute(nameof(GroceryListItemsView), typeof(GroceryListItemsView));
            Routing.RegisterRoute(nameof(ProductView), typeof(ProductView));
            Routing.RegisterRoute(nameof(ChangeColorView), typeof(ChangeColorView));
            Routing.RegisterRoute("Login", typeof(LoginView));
            Routing.RegisterRoute(nameof(BestSellingProductsView), typeof(BestSellingProductsView));
            Routing.RegisterRoute(nameof(BoughtProductsView), typeof(BoughtProductsView));

            // Eerste check (kan vóór login nog niks doen)
            EnsureAdminTab();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Cruciaal: na login nogmaals checken
            EnsureAdminTab();
        }

        private void EnsureAdminTab()
        {
            // Zoek de bestaande TabBar uit je XAML
            var tabBar = this.Items.OfType<TabBar>().FirstOrDefault();
            if (tabBar == null) return; // geen TabBar? dan niks te doen

            // Alleen als ingelogde user Admin is
            if (_global?.Client?.Role != Role.Admin) return;

            // Staat de tab er al?
            var exists = tabBar.Items.OfType<Tab>().Any(t => t.Title == "Klanten per product");
            if (exists) return;

            // Maak de admin-tab
            var adminTab = new Tab
            {
                Title = "Klanten per product",
                Icon = "users.png"
            };
            adminTab.Items.Add(new ShellContent
            {
                Title = "Klanten per product",
                ContentTemplate = new DataTemplate(typeof(BoughtProductsView))
            });

            // Voeg toe aan de bestaande TabBar
            tabBar.Items.Add(adminTab);
        }
    }
}
