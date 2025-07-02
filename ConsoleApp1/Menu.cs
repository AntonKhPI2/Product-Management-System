using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public enum ContainerType
    {
        LinkedList,
        Array
    }

    public class Menu<TContainer> where TContainer : IName<TContainer>, IBinarySerializable
    {
        private LinkedListContainer<TContainer> linkedListContainer;
        private ArrayContainer<TContainer> arrayContainer;
        private ContainerType activeContainerType;
        private decimal currentTotalPrice;

        private Random random = new Random();

        static string[] freshNames = { "Fresh Tomato", "Fresh Cucumber", "Fresh Lettuce", "Fresh Potato" };
        static string[] fruitNames = { "Fruit Apple", "Fruit Orange", "Fruit Peach", "Fruit Pear" };
        static string[] meatNames = { "Meat Beef", "Meat Pork", "Meat Chicken" };
        static string[] packNames = { "Packaged Cookies", "Packaged Canned Food", "Packaged Rusks" };
        static string[] snackNames = { "Snack Chips", "Snack Crackers", "Snack Chocolate" };
        static string[] drinkNames = { "Drink Coca-Cola", "Drink Pepsi", "Drink Sprite", "Drink Fanta" };

        static List<string[]> fruitCharacteristicSets = new List<string[]>
        {
            new [] {"Red", "Sweet"},
            new [] {"Sour", "Has seeds"},
            new [] {"Soft", "Fragrant"},
            new [] {"Juicy", "Seedless"}
        };

        public Menu(LinkedListContainer<TContainer> linkedListContainer, ArrayContainer<TContainer> arrayContainer)
        {
            this.linkedListContainer = linkedListContainer;
            this.arrayContainer = arrayContainer;
            this.activeContainerType = ContainerType.LinkedList; // Default

            // Subscribe to the initially active container
            if (this.linkedListContainer is INotifyTotalPriceChanged notifierLL)
            {
                notifierLL.TotalPriceChanged += OnContainerTotalPriceChanged;
                this.currentTotalPrice = notifierLL.TotalPrice;
            }
            else
            {
                this.currentTotalPrice = 0;
            }
        }

        private void OnContainerTotalPriceChanged(decimal newTotalPrice)
        {
            currentTotalPrice = newTotalPrice;
        }


        private int GetActiveContainerCount()
        {
            return activeContainerType == ContainerType.LinkedList
                ? linkedListContainer.Count
                : arrayContainer.Count;
        }
        
        private IEnumerable<TContainer> GetActiveContainerAsEnumerable()
        {
            return activeContainerType == ContainerType.LinkedList
                ? linkedListContainer
                : arrayContainer;
        }


        private TContainer GetItemFromActiveContainer(int index)
        {
            return activeContainerType == ContainerType.LinkedList
                ? linkedListContainer.GetAt(index)
                : arrayContainer.GetAt(index);
        }

        private void AddToActiveContainer(TContainer item)
        {
            if (activeContainerType == ContainerType.LinkedList)
                linkedListContainer.Add(item);
            else
                arrayContainer.Add(item);
        }

        private void InsertToActiveContainer(TContainer item, int index)
        {
            if (activeContainerType == ContainerType.LinkedList)
                linkedListContainer.Insert(item, index);
            else
                arrayContainer.Insert(item, index);
        }

        private void RemoveFromActiveContainer(int index)
        {
            if (activeContainerType == ContainerType.LinkedList)
                linkedListContainer.RemoveAt(index);
            else
                arrayContainer.RemoveAt(index);
        }

        private void SetInActiveContainer(int index, TContainer item)
        {
            if (activeContainerType == ContainerType.LinkedList)
                linkedListContainer[index] = item;
            else
                arrayContainer[index] = item;
        }

        private TContainer GetFromActiveContainerByName(string name)
        {
            return activeContainerType == ContainerType.LinkedList
                ? linkedListContainer[name]
                : arrayContainer[name];
        }

        private List<TContainer> FindAllByNameInActiveContainer(string name)
        {
            return activeContainerType == ContainerType.LinkedList
                ? linkedListContainer.FindAllByName(name)
                : arrayContainer.FindAllByName(name);
        }

        private void SortActiveContainer(IComparer<TContainer> comparer = null, bool ascending = true)
        {
            if (activeContainerType == ContainerType.LinkedList)
                linkedListContainer.Sort(comparer, ascending);
            else
                arrayContainer.Sort(comparer, ascending);
        }

        private void SortActiveContainer(MyComparison<TContainer> comparison, bool ascending = true)
        {
            if (activeContainerType == ContainerType.LinkedList)
                linkedListContainer.Sort(comparison, ascending);
            else
                arrayContainer.Sort(comparison, ascending);
        }


        private void ClearActiveContainer()
        {
            if (activeContainerType == ContainerType.LinkedList)
                linkedListContainer.Clear();
            else
                arrayContainer.Clear();
        }

        public void Show()
        {
            MyActionDelegate simpleMessageDelegate = msg => Console.WriteLine($"Delegate says: {msg}");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"\n========== PRODUCT MENU ({activeContainerType})==========");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("1) Add random products");
                Console.WriteLine("2) Add products manually");
                Console.WriteLine("3) Insert product by index");
                Console.WriteLine("4) Remove product by index");
                Console.WriteLine("5) Show all products");
                Console.WriteLine("6) Sort products");
                Console.WriteLine("7) Clear all products");
                Console.WriteLine("8) Edit product");
                Console.WriteLine("9) Find products (menu)");
                Console.WriteLine("10) Switch container type");
                Console.WriteLine("11) Show items using foreach");
                Console.WriteLine("12) Show items (reverse)");
                Console.WriteLine("13) Show items (ordered copy: Name/Price)");
                Console.WriteLine("14) Show items (name contains substring)");
                Console.WriteLine("15) Save to Binary");
                Console.WriteLine("16) Load from Binary");
                Console.WriteLine("17) Save to JSON");
                Console.WriteLine("18) Load from JSON");
                Console.WriteLine("19) Show Total Price of items in container");
                Console.WriteLine("20) Find Cheapest/Most Expensive Product (LINQ)"); // New LINQ option 1
                Console.WriteLine("21) Show Average Price per Category (LINQ)");   // New LINQ option 2
                Console.WriteLine("22) Exit");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("Your choice: ");
                Console.ResetColor();

                string choice = Console.ReadLine()?.ToUpper();

                switch (choice)
                {
                    case "1": AddRandomProducts(); break;
                    case "2": AddProductsManually(); break;
                    case "3": InsertProductByIndex(); break;
                    case "4": RemoveProductByIndex(); break;
                    case "5": ShowAllProducts(); break;
                    case "6": SortProductsMenu(); break;
                    case "7": DeleteAllProducts(); break;
                    case "8": EditProductViaIndexers(); break;
                    case "9": FindProductsMenu(); break;
                    case "10": SwitchContainerType(); break;
                    case "11": ShowItemsUsingForeach(); break;
                    case "12": ShowReverseGeneric(); break;
                    case "13": ShowOrderedGeneric(); break;
                    case "14": ShowItemsBySubstring(); break;
                    case "15": SaveContainerToBinary(); break;
                    case "16": LoadContainerFromBinary(); break;
                    case "17": SaveContainerToJson(); break;
                    case "18": LoadContainerFromJson(); break;
                    case "19": ShowCurrentTotalPrice(); break;
                    case "20": FindMinMaxPriceProductsLinq(); break; 
                    case "21": ShowAveragePricePerCategoryLinq(); break; 
                    case "22": return;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        private void FindMinMaxPriceProductsLinq()
        {
            if (!typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("This LINQ query is only available for Product containers.");
                Console.ResetColor();
                return;
            }

            var productContainer = GetActiveContainerAsEnumerable().Cast<Product>();

            if (!productContainer.Any())
            {
                WarnNoItems();
                return;
            }

            Product cheapestProduct = productContainer.MinBy(p => p.Price);
            Product expensiveProduct = productContainer.MaxBy(p => p.Price);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n--- Cheapest Product (LINQ MinBy) ---");
            Console.ResetColor();
            if (cheapestProduct != null)
            {
                PrintProductTable(new List<Product> { cheapestProduct }, "Price");
            }
            else
            {
                Console.WriteLine("Could not determine the cheapest product.");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n--- Most Expensive Product (LINQ MaxBy) ---");
            Console.ResetColor();
            if (expensiveProduct != null)
            {
                PrintProductTable(new List<Product> { expensiveProduct }, "Price");
            }
            else
            {
                 Console.WriteLine("Could not determine the most expensive product.");
            }
        }

        private void ShowAveragePricePerCategoryLinq()
        {
            if (!typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("This LINQ query is only available for Product containers.");
                Console.ResetColor();
                return;
            }
            
            var productContainer = GetActiveContainerAsEnumerable().Cast<Product>();

            if (!productContainer.Any())
            {
                WarnNoItems();
                return;
            }

            var averagePricesByCategory = productContainer
                .GroupBy(p => p.GetType().Name) // Group by concrete class name (e.g., "Fruit", "Meat")
                .Select(group => new
                {
                    Category = group.Key,
                    AveragePrice = group.Average(p => p.Price)
                })
                .OrderBy(g => g.Category);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n--- Average Price per Product Category (LINQ GroupBy & Average) ---");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("| Category         | Average Price     |");
            Console.WriteLine("------------------------------------------");

            foreach (var item in averagePricesByCategory)
            {
                Console.WriteLine($"| {item.Category,-16} | {item.AveragePrice,17:C2} |");
            }
            Console.WriteLine("------------------------------------------");
            Console.ResetColor();
        }


        private void ShowCurrentTotalPrice()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n >>> The current total price of items in the '{activeContainerType}' container is: {currentTotalPrice:F2} <<<");
            Console.ResetColor();
        }

        private string PromptForFileName(string extension)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"Enter file name (without .{extension}): ");
            Console.ResetColor();
            string fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("File name cannot be empty.");
                Console.ResetColor();
                return null;
            }
            return fileName + "." + extension;
        }

        private void UpdateSubscriptionsAndTotalPrice(INotifyTotalPriceChanged oldNotifier, INotifyTotalPriceChanged newNotifier)
        {
            if (oldNotifier != null)
            {
                oldNotifier.TotalPriceChanged -= OnContainerTotalPriceChanged;
            }

            if (newNotifier != null)
            {
                newNotifier.TotalPriceChanged += OnContainerTotalPriceChanged;
                currentTotalPrice = newNotifier.TotalPrice; 
            }
            else
            {
                currentTotalPrice = 0;
            }
        }


        private void SaveContainerToBinary()
        {
            string filePath = PromptForFileName("bin");
            if (filePath == null) return;

            try
            {
                if (activeContainerType == ContainerType.LinkedList)
                {
                    linkedListContainer.SaveToBinary(filePath);
                }
                else
                {
                    arrayContainer.SaveToBinary(filePath);
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Container saved to {filePath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error saving to binary: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void LoadContainerFromBinary()
        {
            string filePath = PromptForFileName("bin");
            if (filePath == null) return;
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"File not found: {filePath}");
                Console.ResetColor();
                return;
            }

            try
            {
                INotifyTotalPriceChanged oldNotifier = null;
                INotifyTotalPriceChanged newNotifier = null;

                if (activeContainerType == ContainerType.LinkedList)
                {
                    oldNotifier = linkedListContainer as INotifyTotalPriceChanged;
                    linkedListContainer = LinkedListContainer<TContainer>.LoadFromBinary(filePath);
                    newNotifier = linkedListContainer as INotifyTotalPriceChanged;
                }
                else 
                {
                    oldNotifier = arrayContainer as INotifyTotalPriceChanged;
                    arrayContainer = ArrayContainer<TContainer>.LoadFromBinary(filePath);
                    newNotifier = arrayContainer as INotifyTotalPriceChanged;
                }

                UpdateSubscriptionsAndTotalPrice(oldNotifier, newNotifier);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Container loaded from {filePath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error loading from binary: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void SaveContainerToJson()
        {
            string filePath = PromptForFileName("json");
            if (filePath == null) return;

            try
            {
                if (activeContainerType == ContainerType.LinkedList)
                {
                    linkedListContainer.SaveToJson(filePath);
                }
                else
                {
                    arrayContainer.SaveToJson(filePath);
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Container saved to {filePath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error saving to JSON: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void LoadContainerFromJson()
        {
            string filePath = PromptForFileName("json");
            if (filePath == null) return;
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"File not found: {filePath}");
                Console.ResetColor();
                return;
            }
            try
            {
                INotifyTotalPriceChanged oldNotifier = null;
                INotifyTotalPriceChanged newNotifier = null;

                if (activeContainerType == ContainerType.LinkedList)
                {
                    oldNotifier = linkedListContainer as INotifyTotalPriceChanged;
                    linkedListContainer = LinkedListContainer<TContainer>.LoadFromJson(filePath);
                    newNotifier = linkedListContainer as INotifyTotalPriceChanged;
                }
                else 
                {
                    oldNotifier = arrayContainer as INotifyTotalPriceChanged;
                    arrayContainer = ArrayContainer<TContainer>.LoadFromJson(filePath);
                    newNotifier = arrayContainer as INotifyTotalPriceChanged;
                }

                UpdateSubscriptionsAndTotalPrice(oldNotifier, newNotifier);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Container loaded from {filePath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error loading from JSON: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void ShowOrderedGeneric()
        {
            if (GetActiveContainerCount() == 0) { WarnNoItems(); return; }

            bool ascending = AskAscending();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Order by:\n1) Name (default)\n2) Price");
            Console.Write("Your choice: ");
            Console.ResetColor();
            bool byPrice = Console.ReadLine() == "2";

            IComparer<TContainer> cmp = null;
            if (byPrice && typeof(IPrice).IsAssignableFrom(typeof(TContainer)))
            {
                cmp = new PriceComparer();
            }

            IEnumerable<TContainer> seq = null;
            if (activeContainerType == ContainerType.LinkedList)
            {
                seq = linkedListContainer.IterateOrdered(cmp, ascending);
            }
            else
            {
                seq = arrayContainer.IterateOrdered(cmp, ascending);
            }

            string banner = byPrice && typeof(IPrice).IsAssignableFrom(typeof(TContainer))
                ? $"PRICE {(ascending ? "↑" : "↓")} ORDERED"
                : $"NAME  {(ascending ? "↑" : "↓")} ORDERED";

            if (typeof(Product).IsAssignableFrom(typeof(TContainer)))
                PrintProductTable(seq.Cast<Product>().ToList(), highlightColumn: (byPrice && typeof(IPrice).IsAssignableFrom(typeof(TContainer)) ? "Price" : "Name"));
            else
                DisplayItems(seq, banner);
        }

        private void ShowReverseGeneric()
        {
            if (GetActiveContainerCount() == 0) { WarnNoItems(); return; }

            IEnumerable<TContainer> seq =
                (activeContainerType == ContainerType.LinkedList)
                    ? linkedListContainer.IterateReverse()
                    : arrayContainer.IterateReverse();

            const string banner = "ORIGINAL ORDER ⏪ REVERSED";

            if (typeof(Product).IsAssignableFrom(typeof(TContainer)))
                PrintProductTable(seq.Cast<Product>().ToList(), highlightColumn: "Name");
            else
                DisplayItems(seq, banner);
        }


        private void DisplayItems(IEnumerable<TContainer> seq, string banner = "SHOW ITEMS", string highlightName = null)
        {
            if (seq == null) throw new ArgumentNullException(nameof(seq));

            var list = seq.ToList();
            if (list.Count == 0)
            {
                WarnNoItems();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"\n========== {banner} ({activeContainerType}) ==========");
            Console.WriteLine($"Container type: {activeContainerType}");
            Console.WriteLine($"Items count:    {list.Count}");
            Console.ResetColor();

            if (typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                var prodList = list.Cast<Product>().ToList();

                if (!string.IsNullOrWhiteSpace(highlightName))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Filter: name contains \"{highlightName}\"");
                    Console.ResetColor();
                }
                PrintProductTable(prodList);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\nItems:");
                Console.ResetColor();
                int idx = 1;
                foreach (var item in list)
                    Console.WriteLine($"[{idx++}] {item.Name}");
            }
        }

        private void ShowItemsBySubstring()
        {
            int cnt = GetActiveContainerCount();
            if (cnt == 0) { WarnNoItems(); return; }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Enter substring: ");
            Console.ResetColor();
            string substr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(substr)) return;

            var items = new List<TContainer>();
            if (activeContainerType == ContainerType.LinkedList)
                items.AddRange(linkedListContainer.IterateWhereNameContains(substr));
            else
                items.AddRange(arrayContainer.IterateWhereNameContains(substr));

            if (items.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("No matches.");
                Console.ResetColor();
                return;
            }
            DisplayItems(items, $"CONTAINS '{substr}'", highlightName: substr);
        }

        private void WarnNoItems()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("No items to show.");
            Console.ResetColor();
        }

        private bool AskAscending()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Sort order:\n1) Ascending ↑ (default)\n2) Descending ↓");
            Console.Write("Your choice: ");
            Console.ResetColor();
            return Console.ReadLine() != "2";
        }

        private void SwitchContainerType()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\nSelect container type:");
            Console.WriteLine("1) Linked List");
            Console.WriteLine("2) Array");
            Console.Write("Your choice: ");
            Console.ResetColor();

            string choice = Console.ReadLine();
            ContainerType newActiveType;

            if (choice == "1") newActiveType = ContainerType.LinkedList;
            else if (choice == "2") newActiveType = ContainerType.Array;
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid choice. Container type not changed.");
                Console.ResetColor();
                return;
            }

            if (newActiveType == activeContainerType)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Container type is already {activeContainerType}.");
                Console.ResetColor();
                return;
            }

            INotifyTotalPriceChanged oldNotifier = null;
            if (activeContainerType == ContainerType.LinkedList)
                oldNotifier = linkedListContainer as INotifyTotalPriceChanged;
            else
                oldNotifier = arrayContainer as INotifyTotalPriceChanged;

            activeContainerType = newActiveType; 

            INotifyTotalPriceChanged newNotifier = null;
            if (activeContainerType == ContainerType.LinkedList)
                newNotifier = linkedListContainer as INotifyTotalPriceChanged;
            else
                newNotifier = arrayContainer as INotifyTotalPriceChanged;

            UpdateSubscriptionsAndTotalPrice(oldNotifier, newNotifier);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Switched to {activeContainerType} container.");
            Console.ResetColor();
        }

        private void AddRandomProducts()
        {
            if (!typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Random product generation is only available for Product containers.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("How many products do you want to add randomly? ");
            Console.ResetColor();

            if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
            {
                int successfullyAdded = 0;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        Product p = GetRandomProduct();
                        AddToActiveContainer((TContainer)(object)p);
                        successfullyAdded++;
                    }
                    catch (InvalidDateException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"[Error Creating Random] Invalid date specified: {ex.Message} for product {ex.Source}");
                        Console.ResetColor();
                    }
                    catch (ProductDataException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"[Error Creating Random] Invalid product data: {ex.Message}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"[Error Creating Random] An unexpected error occurred: {ex.Message}");
                        Console.ResetColor();
                    }
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Attempted to add {count} random products. Successfully added: {successfullyAdded}.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid number entered.");
                Console.ResetColor();
            }
        }

        private Product GetRandomProduct()
        {
            int productType = random.Next(6);
            string name; decimal price; DateTime date1, date2;
            switch (productType)
            {
                case 0:
                    name = freshNames[random.Next(freshNames.Length)];
                    price = Math.Round(random.Next(1, 50) + (decimal)random.NextDouble(), 2);
                    date1 = DateTime.Now.AddDays(random.Next(1, 11));
                    return new FreshProduct(name, price, date1);
                case 1:
                    name = fruitNames[random.Next(fruitNames.Length)];
                    price = Math.Round(random.Next(1, 50) + (decimal)random.NextDouble(), 2);
                    date1 = DateTime.Now.AddDays(random.Next(1, 11));
                    var fruitChars = new List<string>(fruitCharacteristicSets[random.Next(fruitCharacteristicSets.Count)]);
                    return new Fruit(name, price, date1, fruitChars);
                case 2:
                    name = meatNames[random.Next(meatNames.Length)];
                    price = Math.Round(random.Next(5, 100) + (decimal)random.NextDouble(), 2);
                    date1 = DateTime.Now.AddDays(random.Next(1, 21));
                    double weight = Math.Round(random.NextDouble() * 4.9 + 0.1, 2);
                    return new Meat(name, price, date1, weight);
                case 3:
                    name = packNames[random.Next(packNames.Length)];
                    price = Math.Round(random.Next(1, 30) + (decimal)random.NextDouble(), 2);
                    date1 = DateTime.Now.AddDays(-random.Next(1, 31));
                    date2 = date1.AddDays(random.Next(30, 181));
                    return new PackagedProduct(name, price, date1, date2);
                case 4:
                    name = snackNames[random.Next(snackNames.Length)];
                    price = Math.Round(random.Next(1, 20) + (decimal)random.NextDouble(), 2);
                    date1 = DateTime.Now.AddDays(-random.Next(1, 31));
                    date2 = date1.AddDays(random.Next(60, 201));
                    bool isSalty = (random.Next(2) == 0);
                    return new Snack(name, price, date1, date2, isSalty);
                default:
                    name = drinkNames[random.Next(drinkNames.Length)];
                    price = Math.Round(random.Next(1, 15) + (decimal)random.NextDouble(), 2);
                    date1 = DateTime.Now.AddDays(-random.Next(1, 31));
                    date2 = date1.AddDays(random.Next(90, 366));
                    int volume = random.Next(100, 2001);
                     if (volume <= 0) volume = 250;
                    return new Drink(name, price, date1, date2, volume);
            }
        }

        private void AddProductsManually()
        {
            if (!typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Manual product creation is only available for Product containers.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("How many products do you want to add manually? ");
            Console.ResetColor();

            if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"\nAdding product #{i + 1}");
                    Console.ResetColor();
                    try
                    {
                        Product newProduct = CreateProductManually();
                        AddToActiveContainer((TContainer)(object)newProduct);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"Product successfully added: {newProduct}");
                        Console.ResetColor();
                    }
                    catch (InvalidDateException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"[Error] Invalid date: {ex.Message}");
                        Console.ResetColor();
                    }
                    catch (ProductDataException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"[Error] Invalid product data: {ex.Message}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"[Error] Unexpected error: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid number entered.");
                Console.ResetColor();
            }
        }

        private Product CreateProductManually()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\nSelect the product type:");
            Console.WriteLine("1) FreshProduct");
            Console.WriteLine("2) Fruit");
            Console.WriteLine("3) Meat");
            Console.WriteLine("4) PackagedProduct");
            Console.WriteLine("5) Snack");
            Console.WriteLine("6) Drink");
            Console.Write("Enter your choice (1-6): ");
            Console.ResetColor();

            string typeChoice = Console.ReadLine();
            string name;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("Enter product name: ");
                Console.ResetColor();
                name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name)) break;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Product name cannot be empty. Please try again.");
                Console.ResetColor();
            }


            decimal price = PromptForDecimal("Enter product price (decimal): ");
            DateTime date1, date2;

            switch (typeChoice)
            {
                case "1":
                    date1 = PromptForDate("Enter expiration date (yyyy-MM-dd): ");
                    return new FreshProduct(name, price, date1);
                case "2":
                    date1 = PromptForDate("Enter expiration date (yyyy-MM-dd): ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Enter characteristics (comma-separated): ");
                    Console.ResetColor();
                    string charsInput = Console.ReadLine() ?? "";
                    var listChars = new List<string>(charsInput.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));
                    return new Fruit(name, price, date1, listChars);
                case "3":
                    date1 = PromptForDate("Enter expiration date (yyyy-MM-dd): ");
                    double w = PromptForPositiveDouble("Enter weight in kg (e.g. 1.5): ");
                    return new Meat(name, price, date1, w);
                case "4":
                    date1 = PromptForDate("Enter production date (yyyy-MM-dd): ");
                    date2 = PromptForDate("Enter best before date (yyyy-MM-dd): ");
                    return new PackagedProduct(name, price, date1, date2);
                case "5":
                    date1 = PromptForDate("Enter production date (yyyy-MM-dd): ");
                    date2 = PromptForDate("Enter best before date (yyyy-MM-dd): ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Is it salty? (true/false): ");
                    Console.ResetColor();
                    bool isSalty = (Console.ReadLine()?.Trim().ToLower() == "true");
                    return new Snack(name, price, date1, date2, isSalty);
                case "6":
                    date1 = PromptForDate("Enter production date (yyyy-MM-dd): ");
                    date2 = PromptForDate("Enter best before date (yyyy-MM-dd): ");
                    int v = PromptForPositiveInt("Enter volume in ml (integer): ");
                    return new Drink(name, price, date1, date2, v);
                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Invalid product type. A default FreshProduct will be created.");
                    Console.ResetColor();
                    return new FreshProduct(name, price, DateTime.Now.AddDays(1));
            }
        }

        private double PromptForPositiveDouble(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(message);
            Console.ResetColor();
            while (true)
            {
                string input = Console.ReadLine();
                if (double.TryParse(input, out double value) && value > 0)
                    return value;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid input. Must be a positive number. Try again.");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(message);
                Console.ResetColor();
            }
        }

        private int PromptForPositiveInt(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(message);
            Console.ResetColor();
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value > 0)
                    return value;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid input. Must be a positive integer. Try again.");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(message);
                Console.ResetColor();
            }
        }


        private decimal PromptForDecimal(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(message);
            Console.ResetColor();
            while (true)
            {
                string input = Console.ReadLine();
                if (decimal.TryParse(input, out decimal value) && value >= 0)
                    return value;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid decimal. Must be non-negative. Try again.");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(message);
                Console.ResetColor();
            }
        }

        private DateTime PromptForDate(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(message);
            Console.ResetColor();
            while (true)
            {
                string input = Console.ReadLine();
                if (DateTime.TryParse(input, out DateTime dateValue))
                    return dateValue;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid date. Use format yyyy-MM-dd. Try again.");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(message);
                Console.ResetColor();
            }
        }

        private void InsertProductByIndex()
        {
            if (!typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Product insertion is only available for Product containers.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Enter the index (1-based) at which to insert the product: ");
            Console.ResetColor();

            if (!int.TryParse(Console.ReadLine(), out int userIdx) || userIdx <=0 )
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid index format or value. Must be a positive integer.");
                Console.ResetColor();
                return;
            }
            int idx = userIdx -1;

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Select insertion mode:");
            Console.WriteLine("1) Manual product creation");
            Console.WriteLine("2) Random product creation");
            Console.Write("Your choice: ");
            Console.ResetColor();

            string mode = Console.ReadLine();
            Product newProduct = null;

            try
            {
                if (mode == "1")
                {
                    newProduct = CreateProductManually();
                }
                else if (mode == "2")
                {
                    newProduct = GetRandomProduct();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Invalid mode. No product created.");
                    Console.ResetColor();
                    return;
                }
                InsertToActiveContainer((TContainer)(object)newProduct, idx);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{newProduct.Name} inserted at index {userIdx} successfully!");
                Console.ResetColor();
            }
            catch (InvalidDateException ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error Inserting] Invalid date: {ex.Message}");
                Console.ResetColor();
            }
            catch (InvalidIndexException ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error Inserting] Invalid index: {ex.Message}");
                Console.ResetColor();
            }
            catch (ProductDataException ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error Inserting] Invalid product data: {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Error inserting product: " + ex.Message);
                Console.ResetColor();
            }
        }

        private void RemoveProductByIndex()
        {
            int containerCount = GetActiveContainerCount();
            if (containerCount == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("There are no items to remove.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Enter the item index (1-based) to remove: ");
            Console.ResetColor();

            if (int.TryParse(Console.ReadLine(), out int userIdx) && userIdx > 0)
            {
                try
                {
                    RemoveFromActiveContainer(userIdx - 1);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Item at index {userIdx} has been removed.");
                    Console.ResetColor();
                }
                catch (InvalidIndexException ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"[Error] Invalid index: {ex.Message}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"[Error] Unexpected error: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid index format or value. Must be a positive integer.");
                Console.ResetColor();
            }
        }

        private void ShowAllProducts()
        {
            int count = GetActiveContainerCount();
            if (count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("No items to show.");
                Console.ResetColor();
                return;
            }

            if (typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                List<Product> all = new List<Product>();
                for (int i = 0; i < count; i++)
                {
                    all.Add((Product)(object)GetItemFromActiveContainer(i));
                }
                PrintProductTable(all);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Container type: {activeContainerType}");
                Console.WriteLine($"Items count: {count}");
                Console.WriteLine("\nItems:");
                Console.ResetColor();
                for (int i = 0; i < count; i++)
                {
                    var item = GetItemFromActiveContainer(i);
                    Console.WriteLine($"[{i + 1}] {item}");
                }
            }
        }

        private void PrintProductTable(IList<Product> listToPrint, string highlightColumn = null)
        {
            if (listToPrint == null || listToPrint.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("No products to show.");
                Console.ResetColor();
                return;
            }

            void PrintCell(string text, int width, string columnName = null)
            {
                Console.ResetColor();
                if (!string.IsNullOrEmpty(highlightColumn) && columnName != null && highlightColumn.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                }
                string val = text ?? "";
                if (val.Length > width) val = val.Substring(0, width - 3) + "...";
                else val = val.PadRight(width);
                Console.Write($"| {val} ");
            }

            int indexWidth = 4;
            int nameWidth = 20;
            int priceWidth = 10;
            int priceKgLWidth = 10;
            int dateWidth = 10;
            int charWidth = 25;
            int weightWidth = 8;
            int saltyWidth = 7;
            int volumeWidth = 8;

            int totalWidth = indexWidth + nameWidth + priceWidth + priceKgLWidth + priceKgLWidth + dateWidth + charWidth + weightWidth + dateWidth + dateWidth + saltyWidth + volumeWidth + (12*3);

            string separatorTop = new string('=', totalWidth);
            string separator = new string('-', totalWidth);


            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(separatorTop);
            Console.ResetColor();
            PrintCell("#", indexWidth, "Index");
            PrintCell("Name", nameWidth, "Name");
            PrintCell("Price", priceWidth, "Price");
            PrintCell("Price/kg", priceKgLWidth, "Price/kg");
            PrintCell("Price/l", priceKgLWidth, "Price/l");
            PrintCell("Exp. Date", dateWidth, "Exp. Date");
            PrintCell("Characteristics", charWidth, "Characteristics");
            PrintCell("Weight", weightWidth, "Weight");
            PrintCell("Prod. Date", dateWidth, "Prod. Date");
            PrintCell("BestBefore", dateWidth, "BestBefore");
            PrintCell("Salty", saltyWidth, "Salty");
            PrintCell("Volume", volumeWidth, "Volume");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("|");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(separatorTop);
            Console.ResetColor();

            for (int i = 0; i < listToPrint.Count; i++)
            {
                if (i > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(separator);
                    Console.ResetColor();
                }

                Product p = listToPrint[i];
                bool isModified = p.IsModified;
                if (isModified) Console.BackgroundColor = ConsoleColor.DarkGray;

                string rowIndex = (i + 1).ToString();
                string rowName = p.Name;
                string rowPrice = p.Price.ToString("C2");
                string rowPricePerKg = ""; string rowPricePerL = ""; string rowExpDate = "";
                string rowChars = ""; string rowWeight = ""; string rowProdDate = "";
                string rowBestBefore = ""; string rowSalty = ""; string rowVolume = "";

                if (p is FreshProduct fresh) rowExpDate = fresh.ExpirationDate.ToShortDateString();
                if (p is Fruit fruit && fruit.Characteristics != null) rowChars = string.Join(", ", fruit.Characteristics);
                if (p is Meat meat) { rowWeight = meat.Weight.ToString("F2"); if (meat.Weight > 0) rowPricePerKg = (meat.Price / (decimal)meat.Weight).ToString("F2"); }
                if (p is PackagedProduct packaged) { rowProdDate = packaged.ProductionDate.ToShortDateString(); rowBestBefore = packaged.BestBeforeDate.ToShortDateString(); }
                if (p is Snack snack) rowSalty = snack.IsSalty.ToString();
                if (p is Drink drink) { rowVolume = drink.Volume + "ml"; if (drink.Volume > 0) { decimal liters = drink.Volume / 1000m; if (liters > 0) rowPricePerL = (drink.Price / liters).ToString("F2"); } }

                PrintCell(rowIndex, indexWidth, "Index");
                PrintCell(rowName, nameWidth, "Name");
                PrintCell(rowPrice, priceWidth, "Price");
                PrintCell(rowPricePerKg, priceKgLWidth, "Price/kg");
                PrintCell(rowPricePerL, priceKgLWidth, "Price/l");
                PrintCell(rowExpDate, dateWidth, "Exp. Date");
                PrintCell(rowChars, charWidth, "Characteristics");
                PrintCell(rowWeight, weightWidth, "Weight");
                PrintCell(rowProdDate, dateWidth, "Prod. Date");
                PrintCell(rowBestBefore, dateWidth, "BestBefore");
                PrintCell(rowSalty, saltyWidth, "Salty");
                PrintCell(rowVolume, volumeWidth, "Volume");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("|");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(separatorTop);
            Console.ResetColor();
        }


        private void SortProductsMenu()
        {
            int containerCount = GetActiveContainerCount();
            if (containerCount == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("No items to sort.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\nSort by:");
            Console.WriteLine("1) Name (default IComparable)");
            if (typeof(IPrice).IsAssignableFrom(typeof(TContainer)))
            {
                Console.WriteLine("2) Price (using IComparer)");
                Console.WriteLine("4) Price (using MyComparison<T> delegate)");
            }
            Console.WriteLine("3) Name Length (using MyComparison<T> delegate)");
            Console.Write("Your choice: ");
            Console.ResetColor();

            string criterionChoice = Console.ReadLine();
            bool ascending = AskAscending();

            IComparer<TContainer> comparer = null;
            MyComparison<TContainer> customComparisonDelegate = null;

            switch (criterionChoice)
            {
                case "1":
                    comparer = null;
                    break;
                case "2":
                    if (typeof(IPrice).IsAssignableFrom(typeof(TContainer)))
                    {
                        comparer = new PriceComparer();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Price sort (IComparer) not applicable for this item type.");
                        Console.ResetColor();
                        return;
                    }
                    break;
                case "3":
                    customComparisonDelegate = (x, y) =>
                    {
                        if (x == null && y == null) return 0;
                        if (x == null) return -1;
                        if (y == null) return 1;
                        return x.Name.Length.CompareTo(y.Name.Length);
                    };
                    break;
                case "4": 
                    if (typeof(IPrice).IsAssignableFrom(typeof(TContainer)))
                    {
                        customComparisonDelegate = (x, y) =>
                        {
                            if (x == null && y == null) return 0;
                            if (x == null) return -1;
                            if (y == null) return 1;

                            IPrice priceX = x as IPrice;
                            IPrice priceY = y as IPrice;

                            if (priceX != null && priceY != null)
                            {
                                return priceX.Price.CompareTo(priceY.Price);
                            }
                            if (priceX == null && priceY == null) return 0;
                            if (priceX == null) return -1;
                            return 1;
                        };
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Price sort (MyComparison delegate) not applicable for this item type.");
                        Console.ResetColor();
                        return;
                    }
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Invalid sort criterion.");
                    Console.ResetColor();
                    return;
            }

            if (customComparisonDelegate != null)
            {
                SortActiveContainer(customComparisonDelegate, ascending);
            }
            else
            {
                SortActiveContainer(comparer, ascending);
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Items have been sorted!");
            Console.ResetColor();
        }


        private class PriceComparer : IComparer<TContainer>
        {
            public int Compare(TContainer x, TContainer y)
            {
                if (x is IPrice priceX && y is IPrice priceY)
                {
                    return priceX.Price.CompareTo(priceY.Price);
                }
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return 0;
            }
        }

        private void DeleteAllProducts()
        {
            ClearActiveContainer();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("All items have been deleted.");
            Console.ResetColor();
        }

        private void EditProductViaIndexers()
        {
            if (!typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Product editing is only available for Product containers.");
                Console.ResetColor();
                return;
            }

            int containerCount = GetActiveContainerCount();
            if (containerCount == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("No products to edit.");
                Console.ResetColor();
                return;
            }

            for (int i = 0; i < containerCount; i++)
            {
                if (GetItemFromActiveContainer(i) is Product p) p.IsModified = false;
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Choose how to find the product you want to edit:");
            Console.WriteLine("1) By ordinal index (1-based).");
            Console.WriteLine("2) By product name.");
            Console.ResetColor();

            string mode = Console.ReadLine();
            Product targetProduct = null;
            int targetIndex = -1;

            try
            {
                switch (mode)
                {
                    case "1":
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("Enter product index (1-based): ");
                        Console.ResetColor();
                        if (!int.TryParse(Console.ReadLine(), out int userIdx) || userIdx <= 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("Invalid index. Must be a positive integer.");
                            Console.ResetColor();
                            return;
                        }
                        targetIndex = userIdx - 1;
                        targetProduct = (Product)(object)GetItemFromActiveContainer(targetIndex);
                        break;
                    case "2":
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("Enter product name: ");
                        Console.ResetColor();
                        string name = Console.ReadLine();
                        targetProduct = (Product)(object)GetFromActiveContainerByName(name);
                        targetIndex = FindProductIndex(targetProduct);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Invalid choice.");
                        Console.ResetColor();
                        return;
                }
            }
            catch (InvalidIndexException iie)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error Finding Product] {iie.Message}");
                Console.ResetColor();
                return;
            }
            catch (ProductNotFoundException pnfe)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error Finding Product] {pnfe.Message}");
                Console.ResetColor();
                return;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error Finding Product] {ex.Message}");
                Console.ResetColor();
                return;
            }

            if (targetProduct == null || targetIndex == -1)
            {
                 Console.ForegroundColor = ConsoleColor.DarkRed;
                 Console.WriteLine("Product could not be identified for editing.");
                 Console.ResetColor();
                 return;
            }


            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("How do you want to change the product?");
            Console.WriteLine("1) Replace with a random product");
            Console.WriteLine("2) Edit properties manually");
            Console.Write("Your choice: ");
            Console.ResetColor();

            string editMode = Console.ReadLine();
            try
            {
                if (editMode == "1")
                {
                    Product newRandom = GetRandomProduct();
                    SetInActiveContainer(targetIndex, (TContainer)(object)newRandom); 
                    newRandom.IsModified = true;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Product has been replaced with a random one!");
                    Console.WriteLine("New product is: {0}", newRandom.ToString());
                    Console.ResetColor();
                }
                else if (editMode == "2")
                {
                    ChangeAllProperties(targetProduct);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Product has been updated!");
                    Console.WriteLine("New product is: {0}", targetProduct.ToString());
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("No changes made.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error] Failed to edit product: {ex.Message}");
                Console.ResetColor();
            }
        }

        private int FindProductIndex(Product p)
        {
            for (int i = 0; i < GetActiveContainerCount(); i++)
            {
                if (ReferenceEquals(GetItemFromActiveContainer(i), p)) return i;
            }
            return -1;
        }

        private void ChangeAllProperties(Product p)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Editing properties of this product.");
            Console.ResetColor();
            bool modified = false;

            Func<string, string, string> Prompt = (propName, currentValue) =>
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Current {propName}: {currentValue}");
                Console.Write($"Enter new {propName} (leave empty to keep current): ");
                Console.ResetColor();
                return Console.ReadLine();
            };

            string input;
            try
            {
                input = Prompt("name", p.Name); if (!string.IsNullOrWhiteSpace(input)) { p.Name = input; modified = true; }

                input = Prompt("price", p.Price.ToString("C")); if (!string.IsNullOrWhiteSpace(input) && decimal.TryParse(input, out decimal newPrice) && newPrice >= 0) { p.Price = newPrice; modified = true; } else if (!string.IsNullOrWhiteSpace(input) && (!decimal.TryParse(input, out _) || (decimal.TryParse(input, out newPrice) && newPrice < 0))) throw new ProductDataException("Invalid price format or value.");


                if (p is FreshProduct fresh) { input = Prompt("expiration date", fresh.ExpirationDate.ToString("yyyy-MM-dd")); if (!string.IsNullOrWhiteSpace(input) && DateTime.TryParse(input, out DateTime newExp)) { fresh.ExpirationDate = newExp; modified = true; } else if(!string.IsNullOrWhiteSpace(input)) throw new InvalidDateException("Invalid expiration date format.");}
                if (p is Fruit fruit) { input = Prompt("characteristics (comma-sep)", string.Join(", ", fruit.Characteristics ?? new List<string>())); if (!string.IsNullOrWhiteSpace(input)) { fruit.Characteristics = new List<string>(input.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s))); modified = true; } }
                if (p is Meat meat) { input = Prompt("weight (kg)", meat.Weight.ToString("F2")); if (!string.IsNullOrWhiteSpace(input) && double.TryParse(input, out double newW) && newW > 0) { meat.Weight = newW; modified = true; } else if(!string.IsNullOrWhiteSpace(input)) throw new ProductDataException("Invalid weight. Must be a positive number."); }
                if (p is PackagedProduct packaged)
                {
                    DateTime tempProdDate = packaged.ProductionDate;
                    DateTime tempBestBefore = packaged.BestBeforeDate;
                    bool prodDateChanged = false;
                    bool bestBeforeDateChanged = false;

                    input = Prompt("production date", packaged.ProductionDate.ToString("yyyy-MM-dd"));
                    if (!string.IsNullOrWhiteSpace(input) && DateTime.TryParse(input, out DateTime newProd))
                    {
                        tempProdDate = newProd; prodDateChanged = true; modified = true;
                    } else if(!string.IsNullOrWhiteSpace(input)) throw new InvalidDateException("Invalid production date format.");

                    input = Prompt("best-before date", packaged.BestBeforeDate.ToString("yyyy-MM-dd"));
                    if (!string.IsNullOrWhiteSpace(input) && DateTime.TryParse(input, out DateTime newBest))
                    {
                        tempBestBefore = newBest; bestBeforeDateChanged = true; modified = true;
                    } else if(!string.IsNullOrWhiteSpace(input)) throw new InvalidDateException("Invalid best-before date format.");

                    if(prodDateChanged) packaged.ProductionDate = tempProdDate;
                    if(bestBeforeDateChanged) packaged.BestBeforeDate = tempBestBefore;
                }
                if (p is Snack snack) { input = Prompt("'IsSalty'", snack.IsSalty.ToString()); if (!string.IsNullOrWhiteSpace(input) && bool.TryParse(input, out bool newSalty)) { snack.IsSalty = newSalty; modified = true; } else if (!string.IsNullOrWhiteSpace(input)) throw new ProductDataException("Invalid boolean value for 'IsSalty'."); }
                if (p is Drink drink) { input = Prompt("volume (ml)", drink.Volume.ToString()); if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int newVol) && newVol > 0) { drink.Volume = newVol; modified = true; } else if (!string.IsNullOrWhiteSpace(input)) throw new ProductDataException("Invalid volume. Must be a positive integer."); }

                if (modified) p.IsModified = true;
            }
            catch(InvalidDateException ide)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error updating date: {ide.Message}");
                Console.ResetColor();
            }
            catch(ProductDataException pde)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error updating property: {pde.Message}");
                Console.ResetColor();
            }
        }

        private void FindProductsMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\nFind items by:");
            Console.WriteLine("1) Index (1-based)");
            Console.WriteLine("2) Name");
            if (typeof(IPrice).IsAssignableFrom(typeof(TContainer)))
            {
                Console.WriteLine("3) Price");
            }
            Console.WriteLine("4) Find first product with price less than specified (MyPredicate<T> demo)");
            Console.WriteLine("5) Find all Drinks with volume greater than specified (MyPredicate<T> demo)");
            Console.WriteLine("6) Find all Fruits with a specific characteristic (MyPredicate<T> demo)");
            Console.Write("Your choice: ");
            Console.ResetColor();

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1": FindByIndex(); break;
                case "2": FindByName(); break;
                case "3":
                    if (typeof(IPrice).IsAssignableFrom(typeof(TContainer))) FindByPrice();
                    else { WarnNotPriceApplicable(); }
                    break;
                case "4": FindProductPriceLessThan(); break;
                case "5": FindAllDrinksWithVolumeGreaterThan(); break;
                case "6": FindAllFruitsWithCharacteristic(); break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Invalid choice.");
                    Console.ResetColor();
                    break;
            }
        }

        private void FindProductPriceLessThan()
        {
            if (!typeof(IPrice).IsAssignableFrom(typeof(TContainer)))
            {
                WarnNotPriceApplicable(); return;
            }
            if (GetActiveContainerCount() == 0) { WarnNoItems(); return; }

            decimal priceLimit = PromptForDecimal("Enter price limit: ");

            MyPredicate<TContainer> pricePredicateDelegate = item => item is IPrice pItem && pItem.Price < priceLimit;

            TContainer foundItem;
            if (activeContainerType == ContainerType.LinkedList)
                foundItem = linkedListContainer.Find(pricePredicateDelegate);
            else
                foundItem = arrayContainer.Find(pricePredicateDelegate);

            if (foundItem != null && !foundItem.Equals(default(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nFirst item found matching criteria:");
                if (foundItem is Product prod) PrintProductTable(new List<Product> {prod}, "Price");
                else Console.WriteLine(foundItem.ToString());
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("No item found matching the criteria.");
                Console.ResetColor();
            }
        }

        private void FindAllDrinksWithVolumeGreaterThan()
        {
            if (!typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("This search is specific to Product derived types (Drink).");
                Console.ResetColor();
                return;
            }
            if (GetActiveContainerCount() == 0) { WarnNoItems(); return; }

            int minVolume = PromptForPositiveInt("Enter minimum volume (ml): ");

            MyPredicate<TContainer> volumePredicateDelegate = item => item is Drink drinkItem && drinkItem.Volume > minVolume;

            List<TContainer> foundItems;
            if (activeContainerType == ContainerType.LinkedList)
                foundItems = linkedListContainer.FindAll(volumePredicateDelegate);
            else
                foundItems = arrayContainer.FindAll(volumePredicateDelegate);

            if (foundItems.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nFound {foundItems.Count} Drinks with volume > {minVolume}ml:");
                Console.ResetColor();
                PrintProductTable(foundItems.Cast<Product>().ToList(), "Volume");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("No Drinks found matching the criteria.");
                Console.ResetColor();
            }
        }

        private void FindAllFruitsWithCharacteristic()
        {
            if (!typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("This search is specific to Product derived types (Fruit).");
                Console.ResetColor();
                return;
            }
            if (GetActiveContainerCount() == 0) { WarnNoItems(); return; }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Enter characteristic to search for (e.g., Sweet, Red): ");
            Console.ResetColor();
            string charToFind = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(charToFind))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Characteristic cannot be empty.");
                Console.ResetColor();
                return;
            }

            MyPredicate<TContainer> characteristicPredicateDelegate = item =>
                item is Fruit fruitItem &&
                fruitItem.Characteristics != null &&
                fruitItem.Characteristics.Contains(charToFind, StringComparer.OrdinalIgnoreCase);

            List<TContainer> foundItems;
            if (activeContainerType == ContainerType.LinkedList)
                foundItems = linkedListContainer.FindAll(characteristicPredicateDelegate);
            else
                foundItems = arrayContainer.FindAll(characteristicPredicateDelegate);

            if (foundItems.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nFound {foundItems.Count} Fruits with characteristic '{charToFind}':");
                Console.ResetColor();
                PrintProductTable(foundItems.Cast<Product>().ToList(), "Characteristics");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"No Fruits found with characteristic '{charToFind}'.");
                Console.ResetColor();
            }
        }


        private void FindByIndex()
        {
            int containerCount = GetActiveContainerCount();
            if (containerCount == 0)
            {
                WarnNoItems();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Enter the item index (1-based): ");
            Console.ResetColor();

            string input = Console.ReadLine();
            if (!int.TryParse(input, out int userIdx) || userIdx <= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid index. Please enter a positive numeric value.");
                Console.ResetColor();
                return;
            }

            try
            {
                var item = GetItemFromActiveContainer(userIdx - 1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nItem found:");
                if (item is Product p)
                {
                     PrintProductTable(new List<Product>{p});
                }
                else
                {
                    Console.WriteLine($"\tDetails: {item}");
                }
                Console.ResetColor();
            }
            catch (InvalidIndexException ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error] {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Error] {ex.Message}");
                Console.ResetColor();
            }
        }

        private void FindByName()
        {
            int containerCount = GetActiveContainerCount();
            if (containerCount == 0)
            {
                WarnNoItems();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Enter name: ");
            Console.ResetColor();

            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) return;

            var all = FindAllByNameInActiveContainer(name);
            if (all.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("No items found with that name.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Found {all.Count} items:");
                Console.ResetColor();
                if (typeof(Product).IsAssignableFrom(typeof(TContainer)))
                {
                    PrintProductTable(all.Cast<Product>().ToList(), highlightColumn: "Name");
                }
                else
                {
                    for (int i = 0; i < all.Count; i++) Console.WriteLine($"[{i + 1}] {all[i]}");
                }
            }
        }

        private void FindByPrice()
        {
            if (!typeof(IPrice).IsAssignableFrom(typeof(TContainer)))
            {
                 WarnNotPriceApplicable(); return;
            }

            int containerCount = GetActiveContainerCount();
            if (containerCount == 0)
            {
                WarnNoItems();
                return;
            }

            decimal price = PromptForDecimal("Enter price (decimal): ");

            List<Product> results = new List<Product>();
            for (int i = 0; i < GetActiveContainerCount(); i++)
            {
                var item = GetItemFromActiveContainer(i);
                if (item is IPrice pItem && pItem.Price == price && item is Product prod)
                {
                    results.Add(prod);
                }
            }

            if (results.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("No products found with that price.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Found products:");
                Console.ResetColor();
                PrintProductTable(results, highlightColumn: "Price");
            }
        }
         private void WarnNotPriceApplicable() { Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine("This operation is only available for containers of IPrice items."); Console.ResetColor(); }


        private void ShowItemsUsingForeach()
        {
            int containerCount = GetActiveContainerCount();
            if (containerCount == 0)
            {
                WarnNoItems();
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"\n========== SHOWING ITEMS USING FOREACH ({activeContainerType}) ==========");
            Console.WriteLine($"Container type: {activeContainerType}");
            Console.WriteLine($"Items count: {containerCount}");
            Console.ResetColor();

            if (typeof(Product).IsAssignableFrom(typeof(TContainer)))
            {
                List<Product> products = new List<Product>();
                if (activeContainerType == ContainerType.LinkedList) foreach (var item in linkedListContainer) products.Add((Product)(object)item);
                else foreach (var item in arrayContainer) products.Add((Product)(object)item);
                PrintProductTable(products);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\nItems:");
                Console.ResetColor();
                int index = 1;
                if (activeContainerType == ContainerType.LinkedList) foreach (var item in linkedListContainer) Console.WriteLine($"[{index++}] {item.Name}");
                else foreach (var item in arrayContainer) Console.WriteLine($"[{index++}] {item.Name}");
            }
        }
    }
}