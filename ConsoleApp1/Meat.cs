using System;
using System.IO; 

namespace ConsoleApp1
{
    public class Meat : FreshProduct
    {
        private double weight;

        public double Weight
        {
            get => weight;
            set
            {
                if (value <= 0) // Allow 0 for loading, but constructor enforces positive
                {
                     throw new ProductDataException($"Meat weight must be positive: {value}");
                }
                weight = value;
            }
        }

        public Meat() : base() { Weight = 0.1; } 

        public Meat(string name, decimal price, DateTime expirationDate, double weight)
            : base(name, price, expirationDate)
        {
            if (weight <= 0)
            {
                throw new ProductDataException($"Meat weight must be positive: {weight}");
            }
            Weight = weight; 
        }

        public override string ToString()
        {
            return base.ToString() + $", Weight: {Weight:F2} kg";
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Weight);
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            Weight = reader.ReadDouble();
        }
    }
}