using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConsoleApp1
{
    public class ArrayContainer<T> : IEnumerable<T>, INotifyTotalPriceChanged where T : IName<T>, IBinarySerializable
    {
        private class ArrayContainerIterator : IEnumerator<T>
        {
            private readonly ArrayContainer<T> container;
            private int currentIndex;

            public ArrayContainerIterator(ArrayContainer<T> container)
            {
                this.container = container;
                Reset();
            }

            public T Current => (currentIndex >= 0 && currentIndex < container.count)
                ? container.items[currentIndex]
                : default;

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (currentIndex < container.count - 1)
                {
                    currentIndex++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                currentIndex = -1;
            }
        }

        private T[] items;
        private int count;
        private int capacity;

        private decimal _totalPrice;
        public event Action<decimal> TotalPriceChanged;
        public decimal TotalPrice => _totalPrice;

        public int Count => count;
        public int Capacity => capacity;

        public ArrayContainer(int initialCapacity = 10)
        {
            capacity = initialCapacity > 0 ? initialCapacity : 10;
            items = new T[capacity];
            count = 0;
            _totalPrice = 0;
        }

        private void SubscribeToItemEvents(T item)
        {
            if (item is Product product)
            {
                product.PriceChanged += OnItemPriceChanged;
            }
        }

        private void UnsubscribeFromItemEvents(T item)
        {
            if (item is Product product)
            {
                product.PriceChanged -= OnItemPriceChanged;
            }
        }

        private void OnItemPriceChanged(Product sender, decimal oldPrice, decimal newPrice)
        {
            _totalPrice -= oldPrice;
            _totalPrice += newPrice;
            TotalPriceChanged?.Invoke(_totalPrice);
        }

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (count == capacity)
            {
                ResizeArray(capacity == 0 ? 10 : capacity * 2);
            }
            items[count++] = item;

            SubscribeToItemEvents(item);
            if (item is IPrice priceItem)
            {
                _totalPrice += priceItem.Price;
            }
            TotalPriceChanged?.Invoke(_totalPrice);
        }

        private void ResizeArray(int newCapacity)
        {
            capacity = newCapacity;
            T[] newArray = new T[capacity];
            Array.Copy(items, newArray, count);
            items = newArray;
        }

        public void Insert(T item, int index)
        {
            if (index < 0 || index > count)
                throw new InvalidIndexException($"Invalid index: {index}. Allowed: 0..{count}");
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (count == capacity)
            {
                ResizeArray(capacity == 0 ? 10 : capacity * 2);
            }
            if (index < count)
            {
                Array.Copy(items, index, items, index + 1, count - index);
            }
            items[index] = item;
            count++;

            SubscribeToItemEvents(item);
            if (item is IPrice priceItem)
            {
                _totalPrice += priceItem.Price;
            }
            TotalPriceChanged?.Invoke(_totalPrice);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
                throw new InvalidIndexException($"Invalid index: {index}. Allowed: 0..{count - 1}");

            T removedItem = items[index];

            if (index < count - 1)
            {
                Array.Copy(items, index + 1, items, index, count - index - 1);
            }
            items[--count] = default(T);

            UnsubscribeFromItemEvents(removedItem);
            if (removedItem is IPrice priceItem)
            {
                _totalPrice -= priceItem.Price;
            }

            if (count > 0 && count < capacity / 4 && capacity > 10)
            {
                ResizeArray(Math.Max(10, capacity / 2));
            }
            else if (count == 0 && capacity > 10)
            {
                ResizeArray(10);
            }
            TotalPriceChanged?.Invoke(_totalPrice);
        }

        public T this[int index]
        {
            get => GetAt(index);
            set
            {
                if (index < 0 || index >= count)
                    throw new InvalidIndexException($"Invalid index: {index}. Allowed: 0..{count - 1}");
                if (value == null) throw new ArgumentNullException(nameof(value));

                T oldItem = items[index];
                UnsubscribeFromItemEvents(oldItem);
                if (oldItem is IPrice oldPriceItem)
                {
                    _totalPrice -= oldPriceItem.Price;
                }

                items[index] = value;
                SubscribeToItemEvents(value);
                if (value is IPrice newPriceItem)
                {
                    _totalPrice += newPriceItem.Price;
                }
                TotalPriceChanged?.Invoke(_totalPrice);
            }
        }

        public T this[string name]
        {
            get
            {
                for (int i = 0; i < count; i++)
                {
                    if (items[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return items[i];
                }
                throw new ProductNotFoundException($"Item with name '{name}' not found.");
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                for (int i = 0; i < count; i++)
                {
                    if (items[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        T oldItem = items[i];
                        UnsubscribeFromItemEvents(oldItem);
                        if (oldItem is IPrice oldPriceItem)
                        {
                            _totalPrice -= oldPriceItem.Price;
                        }

                        items[i] = value;
                        SubscribeToItemEvents(value);
                        if (value is IPrice newPriceItem)
                        {
                            _totalPrice += newPriceItem.Price;
                        }
                        TotalPriceChanged?.Invoke(_totalPrice);
                        return;
                    }
                }
                throw new ProductNotFoundException($"Item with name '{name}' not found.");
            }
        }

        public T GetAt(int index)
        {
            if (index < 0 || index >= count)
                throw new InvalidIndexException($"Invalid index: {index}. Allowed: 0..{count - 1}");
            return items[index];
        }

        public List<T> FindAllByName(string name)
        {
            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                if (items[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    list.Add(items[i]);
            }
            return list;
        }

        public void Sort(IComparer<T> comparer = null, bool ascending = true)
        {
            if (count < 2) return;

            Array.Sort(this.items, 0, this.count, comparer);

            if (!ascending)
            {
                Array.Reverse(this.items, 0, this.count);
            }
        }

        public void Sort(MyComparison<T> comparison, bool ascending = true)
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));
            if (count < 2) return;

            IComparer<T> comparerAdapter = Comparer<T>.Create((x, y) => comparison(x, y));
            Array.Sort(this.items, 0, this.count, comparerAdapter);

            if (!ascending)
            {
                Array.Reverse(this.items, 0, this.count);
            }
        }


        public T Find(MyPredicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            for (int i = 0; i < count; i++)
            {
                if (match(items[i]))
                    return items[i];
            }
            return default(T);
        }

        public List<T> FindAll(MyPredicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            var list = new List<T>();
            for (int i = 0; i < count; i++)
            {
                if (match(items[i]))
                    list.Add(items[i]);
            }
            return list;
        }

        public void Clear()
        {
            for (int i = 0; i < count; i++)
            {
                UnsubscribeFromItemEvents(items[i]);
            }
            Array.Clear(items, 0, count);
            count = 0;
            _totalPrice = 0;
            TotalPriceChanged?.Invoke(_totalPrice);
            if (capacity > 10) ResizeArray(10);
        }

        public override string ToString()
        {
            if (count == 0) return "Container is empty.";
            var sb = new System.Text.StringBuilder("Container contents:\n");
            for (int i = 0; i < count; i++)
            {
                sb.AppendLine($"[{i}] {items[i]}");
            }
            return sb.ToString();
        }

        private void WriteItemRecursive(BinaryWriter writer, int index)
        {
            if (index >= count) return;

            writer.Write(items[index].GetType().AssemblyQualifiedName);
            items[index].Write(writer);

            WriteItemRecursive(writer, index + 1);
        }

        public void SaveToBinary(string filePath)
        {
            using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                writer.Write(count);
                WriteItemRecursive(writer, 0);
            }
        }

        private void ReadItemRecursive(BinaryReader reader, ArrayContainer<T> container, int itemsToRead, int currentIndex)
        {
            if (currentIndex >= itemsToRead) return;

            string typeName = reader.ReadString();
            Type itemType = Type.GetType(typeName, throwOnError: true);

            T item = (T)Activator.CreateInstance(itemType);
            item.Read(reader);
            container.Add(item); // Add method will handle _totalPrice and subscriptions

            ReadItemRecursive(reader, container, itemsToRead, currentIndex + 1);
        }

        public static ArrayContainer<T> LoadFromBinary(string filePath)
        {
            var container = new ArrayContainer<T>(); // _totalPrice is 0, no subscriptions yet
            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int itemCount = reader.ReadInt32();
                // ReadItemRecursive will call container.Add for each item
                container.ReadItemRecursive(reader, container, itemCount, 0);
            }
            // At this point, all items are added, _totalPrice is up-to-date, and events are subscribed.
            return container;
        }

        private record JsonEntry(string Type, string Json);

        public void SaveToJson(string filePath)
        {
            var entries = new List<JsonEntry>();
            var options = new JsonSerializerOptions { WriteIndented = true };
            for (int i = 0; i < count; i++)
            {
                T item = items[i];
                string json = JsonSerializer.Serialize(item, item.GetType(), options);
                entries.Add(new JsonEntry(item.GetType().AssemblyQualifiedName, json));
            }
            File.WriteAllText(filePath, JsonSerializer.Serialize(entries, options));
        }

        public static ArrayContainer<T> LoadFromJson(string filePath)
        {
            var container = new ArrayContainer<T>(); // _totalPrice is 0
            var options = new JsonSerializerOptions();
            string fileContent = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(fileContent)) return container;

            var entries = JsonSerializer.Deserialize<List<JsonEntry>>(fileContent, options);
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    Type itemType = Type.GetType(entry.Type, throwOnError: true);
                    T item = (T)JsonSerializer.Deserialize(entry.Json, itemType, options);
                    if (item != null)
                    {
                        container.Add(item); // Add method handles _totalPrice and subscriptions
                    }
                }
            }
            return container;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ArrayContainerIterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerable<T> IterateReverse()
        {
            for (int i = count - 1; i >= 0; i--)
                yield return items[i];
        }

        public IEnumerable<T> IterateWhereNameContains(string substr,
            StringComparison cmp = StringComparison.OrdinalIgnoreCase)
        {
            if (substr == null) throw new ArgumentNullException(nameof(substr));
            for (int i = 0; i < count; i++)
                if (items[i].Name.IndexOf(substr, cmp) >= 0)
                    yield return items[i];
        }

        public IEnumerable<T> IterateOrdered(IComparer<T> comparer = null,
            bool ascending = true)
        {
            var list = new List<T>(count);
            for (int i = 0; i < count; i++) list.Add(items[i]);

            if (comparer != null) list.Sort(comparer);
            else if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)) || typeof(IComparable).IsAssignableFrom(typeof(T)))
                list.Sort();

            if (!ascending) list.Reverse();
            foreach (var el in list) yield return el;
        }
        public IEnumerable<decimal> PricesReverse()
        {
            for (int i = count - 1; i >= 0; i--)
                if (items[i] is IPrice pr)
                    yield return pr.Price;
        }

        public IEnumerable<decimal> PricesOrdered(bool ascending = true)
        {
            var list = new List<decimal>(count);
            for (int i = 0; i < count; i++)
                if (items[i] is IPrice pr)
                    list.Add(pr.Price);

            list.Sort();
            if (!ascending) list.Reverse();
            foreach (var price in list) yield return price;
        }
    }
}