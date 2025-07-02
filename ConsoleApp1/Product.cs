using System;
using System.IO;

namespace ConsoleApp1
{
    public abstract class Product : IName, IPrice, IName<Product>, IBinarySerializable, IComparable<Product>
    {
        private decimal price;
        private string name;

        public event Action<Product, decimal, decimal> PriceChanged; // Sender, OldPrice, NewPrice

        public string Name
        {
            get => name;
            set
            {
                // Basic validation, can be expanded
                if (string.IsNullOrWhiteSpace(value))
                    throw new ProductDataException("Product name cannot be empty or whitespace.");
                name = value;
            }
        }
        public decimal Price
        {
            get => price;
            set
            {
                if (value < 0)
                    throw new ProductDataException($"Price cannot be negative (price = {value}).");

                if (price != value)
                {
                    decimal oldPrice = price;
                    price = value;
                    PriceChanged?.Invoke(this, oldPrice, price);
                }
            }
        }

        public bool IsModified { get; set; } = false;

        protected Product()
        {
            name = "Unknown product"; // Initialize directly to avoid exception in setter if default is null/empty
            Price = 0m;
        }

        protected Product(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name)) // Ensure name is valid during construction
                throw new ProductDataException("Product name cannot be empty or whitespace.");
            this.name = name;
            Price = price; // Use property setter for validation and event
        }

        public int CompareTo(IName other)
        {
            if (other == null) return 1;
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(IPrice other)
        {
            if (other == null) return 1;
            return Price.CompareTo(other.Price);
        }

        public int CompareTo(Product other)
        {
            if (other == null) return 1;
            int nameComparison = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            if (nameComparison != 0)
            {
                return nameComparison;
            }
            return Price.CompareTo(other.Price);
        }

        public abstract override string ToString();

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(Name ?? string.Empty); // Name should not be null due to constructor/setter validation
            writer.Write(Price);
        }

        public virtual void Read(BinaryReader reader)
        {
            Name = reader.ReadString(); // Use property for potential future validation if needed
            Price = reader.ReadDecimal(); // Use property for event mechanism consistency (though not strictly needed here)
        }
    }
}