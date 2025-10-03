# GroceryApp sprint4 Studentversie  

## UC10 Productaantal in boodschappenlijst
Aanpassingen zijn compleet.

## UC11 Meest verkochte producten
Vereist aanvulling:  
- Werk in GroceryListItemsService de methode GetBestSellingProducts uit.  
- In BestSellingProductsView de kop van de tabel aanvullen met de gewenste kopregel boven de tabel. Daarnaast de inhoud van de tabel uitwerken.

## UC13 Klanten tonen per product  
Deze UC toont de klanten die een bepaald product hebben gekocht:  
- Maak enum Role met als waarden None en Admin.  
- Geef de Client class een property Role metb als type de enum Role. De default waarde is None.  
- In Client Repo koppel je de rol Role.Admin aan user3 (= admin).
- In BoughtProductsService werk je de Get(productid) functie uit zodat alle Clients die product met productid hebben gekocht met client, boodschappenlijst en product in de lijst staan die wordt geretourneerd.  
- In BoughtProductsView moet de naam van de Client ewn de naam van de Boodschappenlijst worden getoond in de CollectionView.  
- In BoughtProductsViewModel de OnSelectedProductChanged uitwerken zodat bij een ander product de lijst correct wordt gevuld.  
- In GroceryListViewModel maak je de methode ShowBoughtProducts(). Als de Client de rol admin heeft dan navigeer je naar BoughtProductsView. Anders doe je niets.  
- In GroceryListView voeg je een ToolbarItem toe met als binding Client.Name en als Command ShowBoughtProducts.  



## Architectuur & Mappenstructuur

We hanteren een gelaagde opzet:

- **Grocery.Core**  
  Domeinmodellen (entities/DTO’s), business-interfaces (services), value objects, enums.  
  *Geen* UI- of data-access code.

- **Grocery.Core.Data**  
  Implementaties van repositories en seed-/in-memory-data (of DB later).  
  Kent **Core**, maar niet de UI.

- **Grocery.App** (MAUI)  
  Views, ViewModels, dependency injection, app-startup en navigatie.  
  Praat via services/repositories (interfaces) met **Core/Core.Data**.

- **TestCore**  
  Unit/integration tests over services/repositories/VM-logica.  
  Seed testdata per test (AAA: Arrange-Act-Assert).

**Dependency-richting:**  
`Grocery.App → Grocery.Core (+ Grocery.Core.Data)`  
`TestCore → (Core / Core.Data / App onderdelen indien nodig)`

**Richtlijnen:**
- **SRP:** 1 verantwoordelijkheid per klasse/methode (services zonder UI-kennis).
- **Interfaces in Core**, implementaties in Core.Data.
- **ViewModel** bevat UI-logica (binding/commands), **Service** bevat domeinlogica.
- **Async waar passend**, geen blocking calls in UI.

## Ontwerprichtlijnen (kort)
- Naming volgt domein (bijv. *GroceryList*, *BoughtProduct*, *Client*).
- Publieke API’s gebruiken interface-typen (`IEnumerable<T>` i.p.v. `List<T>`).
- Exceptions/documentatie waar een methode kan falen (precondities/validatie).

## Keuzes (kort beargumenteerd)
- **Gelaagde bouw** voor testbaarheid en onderhoud (UI en data wisselbaar).  
- **Interfaces** om afhankelijkheden te mocken.  
- **ViewModel+Command** voor heldere UI-binding; **Service** voor domeinregels.

  
