using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConsoleApp1
{
    public class LinkedListContainer<T> : IEnumerable<T>, INotifyTotalPriceChanged where T : IName<T>, IBinarySerializable
    {
        private class Node
        {
            public T Data;
            public Node Next;
            public Node Prev;

            public Node(T data) => Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        private class LinkedListContainerIterator : IEnumerator<T>
        {
            private readonly LinkedListContainer<T> container;
            private Node currentNode;
            private bool isStarted;

            public LinkedListContainerIterator(LinkedListContainer<T> container)
            {
                this.container = container;
                Reset();
            }

            public T Current => currentNode != null ? currentNode.Data : default;
            object IEnumerator.Current => Current;
            public void Dispose() { }

            public bool MoveNext()
            {
                if (!isStarted)
                {
                    currentNode = container.head;
                    isStarted = true;
                }
                else if (currentNode != null)
                {
                    currentNode = currentNode.Next;
                }
                return currentNode != null;
            }

            public void Reset()
            {
                currentNode = null;
                isStarted = false;
            }
        }

        private Node head;
        private Node tail;
        private int count;

        private decimal _totalPrice;
        public event Action<decimal> TotalPriceChanged;
        public decimal TotalPrice => _totalPrice;

        public int Count => count;

        public LinkedListContainer()
        {
            _totalPrice = 0;
            // head, tail, count are already 0/null by default
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
            var n = new Node(item);
            if (head == null)
                head = tail = n;
            else
            {
                tail.Next = n;
                n.Prev = tail;
                tail = n;
            }
            count++;

            SubscribeToItemEvents(item);
            if (item is IPrice priceItem)
            {
                _totalPrice += priceItem.Price;
            }
            TotalPriceChanged?.Invoke(_totalPrice);
        }

        public void Insert(T item, int index)
        {
            if (index < 0 || index > count) throw new InvalidIndexException($"Invalid index: {index}. Allowed: 0..{count}");
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (index == count) { Add(item); return; } // Add will handle price and event

            Node newNode = new Node(item);
            if (index == 0)
            {
                newNode.Next = head;
                if (head != null) head.Prev = newNode;
                head = newNode;
                if (tail == null) tail = head;
            }
            else
            {
                Node current = GetNodeAtInternal(index);
                newNode.Next = current;
                newNode.Prev = current.Prev;
                if (current.Prev != null) current.Prev.Next = newNode;
                current.Prev = newNode;
                // If current was head, newNode.Prev is null, head remains head (no, head becomes newNode if index=0)
                // This 'else' block is for index > 0. So current.Prev is not null unless GetNodeAtInternal is wrong, or list struct issues.
                // Recheck: if index=0, head becomes newNode. If index > 0, head is unchanged.
            }
            count++;

            SubscribeToItemEvents(item);
            if (item is IPrice priceItem)
            {
                _totalPrice += priceItem.Price;
            }
            TotalPriceChanged?.Invoke(_totalPrice);
        }

        private Node GetNodeAtInternal(int index) // Assumes index is valid
        {
            Node current;
            if (index < count / 2)
            {
                current = head;
                for (int i = 0; i < index; i++)
                    current = current.Next;
            }
            else
            {
                current = tail;
                for (int i = count - 1; i > index; i--)
                    current = current.Prev;
            }
            return current;
        }


        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count) throw new InvalidIndexException($"Invalid index: {index}. Allowed: 0..{count - 1}");
            Node cur = GetNodeAtInternal(index);
            T removedItem = cur.Data;

            if (cur.Prev != null) cur.Prev.Next = cur.Next; else head = cur.Next;
            if (cur.Next != null) cur.Next.Prev = cur.Prev; else tail = cur.Prev;

            // After unlinking, ensure head and tail's links to outside are null if they became new head/tail
            if (head != null) head.Prev = null;
            if (tail != null) tail.Next = null;

            count--;
            if (count == 0) head = tail = null;

            UnsubscribeFromItemEvents(removedItem);
            if (removedItem is IPrice priceItem)
            {
                _totalPrice -= priceItem.Price;
            }
            TotalPriceChanged?.Invoke(_totalPrice);
        }

        public T this[int index]
        {
            get => GetAt(index);
            set
            {
                if (index < 0 || index >= count) throw new InvalidIndexException($"Invalid index: {index}. Allowed: 0..{count - 1}");
                if (value == null) throw new ArgumentNullException(nameof(value));

                Node cur = GetNodeAtInternal(index);
                T oldItem = cur.Data;

                UnsubscribeFromItemEvents(oldItem);
                if (oldItem is IPrice oldPriceItem)
                {
                    _totalPrice -= oldPriceItem.Price;
                }

                cur.Data = value;
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
                for (Node n = head; n != null; n = n.Next)
                    if (n.Data.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) return n.Data;
                throw new ProductNotFoundException($"Item with name '{name}' not found.");
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                for (Node n = head; n != null; n = n.Next)
                {
                    if (n.Data.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        T oldItem = n.Data;
                        UnsubscribeFromItemEvents(oldItem);
                        if (oldItem is IPrice oldPriceItem)
                        {
                            _totalPrice -= oldPriceItem.Price;
                        }

                        n.Data = value;
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
            if (index < 0 || index >= count) throw new InvalidIndexException($"Invalid index: {index}. Allowed: 0..{count - 1}");
            return GetNodeAtInternal(index).Data;
        }

        public List<T> FindAllByName(string name)
        {
            var list = new List<T>();
            for (Node n = head; n != null; n = n.Next)
                if (n.Data.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) list.Add(n.Data);
            return list;
        }

        private Node GetMiddleNode(Node h)
        {
            if (h == null) return h;
            Node slow = h, fast = h;
            while (fast.Next != null && fast.Next.Next != null)
            {
                slow = slow.Next;
                fast = fast.Next.Next;
            }
            return slow;
        }

        private Node Merge(Node a, Node b, MyComparison<T> comparison)
        {
            if (a == null) return b;
            if (b == null) return a;

            Node result;
            if (comparison(a.Data, b.Data) <= 0)
            {
                result = a;
                result.Next = Merge(a.Next, b, comparison);
                if (result.Next != null) result.Next.Prev = result;
            }
            else
            {
                result = b;
                result.Next = Merge(a, b.Next, comparison);
                if (result.Next != null) result.Next.Prev = result;
            }
            result.Prev = null;
            return result;
        }

        private Node MergeSortRecursive(Node h, MyComparison<T> comparison)
        {
            if (h == null || h.Next == null) return h;

            Node middle = GetMiddleNode(h);
            Node nextOfMiddle = middle.Next;

            middle.Next = null;
            if (nextOfMiddle != null) nextOfMiddle.Prev = null;


            Node left = MergeSortRecursive(h, comparison);
            Node right = MergeSortRecursive(nextOfMiddle, comparison);

            Node sortedList = Merge(left, right, comparison);
            return sortedList;
        }


        public void Sort(IComparer<T> comparer, bool ascending = true)
        {
            if (count < 2) return;

            MyComparison<T> actualComparison;
            if (comparer != null)
            {
                actualComparison = (x, y) => comparer.Compare(x, y);
            }
            else if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
            {
                actualComparison = (x, y) => ((IComparable<T>)x).CompareTo(y);
            }
            else if (typeof(IComparable).IsAssignableFrom(typeof(T)))
            {
                actualComparison = (x, y) => ((IComparable)x).CompareTo(y);
            }
            else
            {
                throw new InvalidOperationException($"Cannot sort type {typeof(T).Name} without an IComparer or IComparable implementation.");
            }

            MyComparison<T> finalComparison = actualComparison;
            if (!ascending)
            {
                finalComparison = (x, y) => actualComparison(y, x);
            }

            this.head = MergeSortRecursive(this.head, finalComparison);

            if (this.head == null)
            {
                this.tail = null;
            }
            else
            {
                this.head.Prev = null;
                Node current = this.head;
                while (current.Next != null)
                {
                    current.Next.Prev = current;
                    current = current.Next;
                }
                this.tail = current;
            }
        }


        public void Sort(MyComparison<T> comparison, bool ascending = true)
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));
            if (count < 2) return;

            MyComparison<T> finalComparison = comparison;
            if (!ascending)
            {
                finalComparison = (x, y) => comparison(y, x);
            }

            this.head = MergeSortRecursive(this.head, finalComparison);

            if (this.head == null)
            {
                this.tail = null;
            }
            else
            {
                this.head.Prev = null;
                Node current = this.head;
                while (current.Next != null)
                {
                    current.Next.Prev = current;
                    current = current.Next;
                }
                this.tail = current;
            }
        }

        public void Sort(bool ascending = true) => Sort((IComparer<T>)null, ascending);


        public void SortByPrice(bool ascending = true)
        {
            if (typeof(IPrice).IsAssignableFrom(typeof(T)))
            {
                Sort(new PriceComparerInternal(), ascending);
            }
            else
            {
                Console.WriteLine("Cannot sort by price: items do not implement IPrice or T is not assignable from IPrice.");
            }
        }

        private class PriceComparerInternal : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                if (x is IPrice priceX && y is IPrice priceY)
                {
                    return priceX.Price.CompareTo(priceY.Price);
                }
                return 0;
            }
        }

        public List<T> FindAllByPrice(decimal price)
        {
            var list = new List<T>();
            for (Node n = head; n != null; n = n.Next)
                if (n.Data is IPrice p && p.Price == price) list.Add(n.Data);
            return list;
        }

        public T Find(MyPredicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            for (Node n = head; n != null; n = n.Next)
            {
                if (match(n.Data))
                    return n.Data;
            }
            return default(T);
        }

        public List<T> FindAll(MyPredicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            var list = new List<T>();
            for (Node n = head; n != null; n = n.Next)
            {
                if (match(n.Data))
                    list.Add(n.Data);
            }
            return list;
        }

        public void Clear()
        {
            Node current = head;
            while (current != null)
            {
                UnsubscribeFromItemEvents(current.Data);
                current = current.Next;
            }

            head = tail = null;
            count = 0;
            _totalPrice = 0;
            TotalPriceChanged?.Invoke(_totalPrice);
        }

        public override string ToString()
        {
            if (count == 0) return "Container is empty.";
            var sb = new System.Text.StringBuilder("Container contents:\n");
            int i = 0; for (Node n = head; n != null; n = n.Next) sb.AppendLine($"[{i++}] {n.Data}");
            return sb.ToString();
        }

        private void WriteNodeRecursive(BinaryWriter writer, Node currentNode)
        {
            if (currentNode == null) return;

            writer.Write(currentNode.Data.GetType().AssemblyQualifiedName);
            currentNode.Data.Write(writer);

            WriteNodeRecursive(writer, currentNode.Next);
        }

        public void SaveToBinary(string filePath)
        {
            using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                writer.Write(count);
                WriteNodeRecursive(writer, head);
            }
        }

        private void ReadAndAddRecursive(BinaryReader reader, LinkedListContainer<T> container, int itemsToRead, int currentIndex)
        {
            if (currentIndex >= itemsToRead) return;

            string typeName = reader.ReadString();
            Type itemType = Type.GetType(typeName, throwOnError: true);
            T item = (T)Activator.CreateInstance(itemType);
            item.Read(reader);
            container.Add(item); // Add method handles _totalPrice and subscriptions

            ReadAndAddRecursive(reader, container, itemsToRead, currentIndex + 1);
        }

        public static LinkedListContainer<T> LoadFromBinary(string filePath)
        {
            var container = new LinkedListContainer<T>(); // _totalPrice is 0
            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int itemCount = reader.ReadInt32();
                container.ReadAndAddRecursive(reader, container, itemCount, 0);
            }
            return container;
        }

        private record JsonEntry(string Type, string Json);

        public void SaveToJson(string filePath)
        {
            var entries = new List<JsonEntry>();
            var options = new JsonSerializerOptions { WriteIndented = true };
            Node current = head;
            while (current != null)
            {
                T item = current.Data;
                string json = JsonSerializer.Serialize(item, item.GetType(), options);
                entries.Add(new JsonEntry(item.GetType().AssemblyQualifiedName, json));
                current = current.Next;
            }
            File.WriteAllText(filePath, JsonSerializer.Serialize(entries, options));
        }

        public static LinkedListContainer<T> LoadFromJson(string filePath)
        {
            var container = new LinkedListContainer<T>(); // _totalPrice is 0
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
            return new LinkedListContainerIterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerable<T> IterateReverse()
        {
            for (Node n = tail; n != null; n = n.Prev)
                yield return n.Data;
        }

        public IEnumerable<T> IterateWhereNameContains(string substr,
            StringComparison cmp = StringComparison.OrdinalIgnoreCase)
        {
            if (substr == null) throw new ArgumentNullException(nameof(substr));
            for (Node n = head; n != null; n = n.Next)
                if (n.Data.Name.IndexOf(substr, cmp) >= 0)
                    yield return n.Data;
        }

        public IEnumerable<T> IterateOrdered(IComparer<T> comparer = null, bool ascending = true)
        {
            var buf = new List<T>(count);
            for (Node n = head; n != null; n = n.Next) buf.Add(n.Data);

            if (comparer != null) buf.Sort(comparer);
            else if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)) || typeof(IComparable).IsAssignableFrom(typeof(T)))
                buf.Sort();

            if (!ascending) buf.Reverse();
            foreach (var el in buf) yield return el;
        }
        public IEnumerable<decimal> PricesReverse()
        {
            for (Node n = tail; n != null; n = n.Prev)
                if (n.Data is IPrice pr)
                    yield return pr.Price;
        }

        public IEnumerable<decimal> PricesOrdered(bool ascending = true)
        {
            var list = new List<decimal>(count);
            for (Node n = head; n != null; n = n.Next)
                if (n.Data is IPrice pr)
                    list.Add(pr.Price);

            list.Sort();
            if (!ascending) list.Reverse();
            foreach (var price in list) yield return price;
        }
    }
}