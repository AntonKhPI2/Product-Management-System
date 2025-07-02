using System;
using System.IO; 

namespace ConsoleApp1
{
    public class Drink : PackagedProduct
    {
        private int volume;

        public int Volume
        {
            get => volume;
            set
            {
                if (value <= 0)
                {
                     throw new ProductDataException($"Volume cannot be zero or negative: {value}");
                }
                volume = value;
            }
        }

        public Drink() : base() { Volume = 250; } 

        public Drink(string name, decimal price, DateTime productionDate, DateTime bestBeforeDate, int volume)
            : base(name, price, productionDate, bestBeforeDate)
        {
            if (volume <= 0)
            {
                throw new ProductDataException($"Volume must be positive: {volume}");
            }
            Volume = volume; 
        }

        public override string ToString()
        {
            return base.ToString() + $", Volume: {Volume} ml";
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Volume);
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            Volume = reader.ReadInt32();
        }
    }
}