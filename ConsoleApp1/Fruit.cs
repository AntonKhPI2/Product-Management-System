using System;
using System.Collections.Generic;
using System.IO; 

namespace ConsoleApp1
{

    public class Fruit : FreshProduct
    {
        public List<string> Characteristics { get; set; }

        public Fruit() : base()
        {
            Characteristics = new List<string>();
        }

        public Fruit(string name, decimal price, DateTime expirationDate, List<string> characteristics)
            : base(name, price, expirationDate)
        {
            Characteristics = characteristics ?? new List<string>();
        }

        public override string ToString()
        {
            string chars = (Characteristics != null && Characteristics.Count > 0)
                ? string.Join(", ", Characteristics)
                : "No characteristics";

            return base.ToString() + $", Characteristics: {chars}";
        }
        
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Characteristics?.Count ?? 0);
            if (Characteristics != null)
            {
                foreach (string characteristic in Characteristics)
                {
                    writer.Write(characteristic ?? string.Empty);
                }
            }
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            int count = reader.ReadInt32();
            Characteristics = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                Characteristics.Add(reader.ReadString());
            }
        }
    }
}