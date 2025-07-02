using System.IO; 
using System;

namespace ConsoleApp1
{

    public class Snack : PackagedProduct
    {
        public bool IsSalty { get; set; }

        public Snack() : base()
        {
            IsSalty = false; 
        }

        public Snack(string name, decimal price, DateTime productionDate, DateTime bestBeforeDate, bool isSalty)
            : base(name, price, productionDate, bestBeforeDate)
        {
            IsSalty = isSalty;
        }

        public override string ToString()
        {
            return base.ToString() + $", Salty: {IsSalty}";
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(IsSalty);
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            IsSalty = reader.ReadBoolean();
        }
    }
}