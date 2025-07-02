using System;
using System.IO; 

namespace ConsoleApp1
{
    public class FreshProduct : Product 
    {
        private DateTime expirationDate;

        public DateTime ExpirationDate
        {
            get => expirationDate;
            set
            {
                // Validation during manual/random creation is in the constructor
                expirationDate = value;
            }
        }

        public FreshProduct() : base()
        {
            ExpirationDate = DateTime.Now.AddDays(1);
        }

        public FreshProduct(string name, decimal price, DateTime expirationDate)
            : base(name, price)
        {
            if (expirationDate < DateTime.Now.Date) // Stricter check for new items
            {
                throw new InvalidDateException($"Expiration date cannot be in the past: {expirationDate.ToShortDateString()}");
            }
            ExpirationDate = expirationDate; 
        }

        public override string ToString()
        {
            return $"Product: {Name}, Price: {Price:C}, Expiration Date: {ExpirationDate.ToShortDateString()}";
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(ExpirationDate.ToBinary());
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            ExpirationDate = DateTime.FromBinary(reader.ReadInt64());
        }
    }
}