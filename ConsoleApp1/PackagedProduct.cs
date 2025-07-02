using System;
using System.IO; 

namespace ConsoleApp1
{
    public class PackagedProduct : Product
    {
        private DateTime productionDate;
        private DateTime bestBeforeDate;

        public DateTime ProductionDate
        {
            get => productionDate;
            set
            {
                productionDate = value;
            }
        }

        public DateTime BestBeforeDate
        {
            get => bestBeforeDate;
            set
            {
                bestBeforeDate = value;
            }
        }

        public PackagedProduct() : base()
        {
            DateTime now = DateTime.Now;
            productionDate = now.AddDays(-1); 
            bestBeforeDate = now.AddMonths(6);
        }

        public PackagedProduct(string name, decimal price, DateTime prodDate, DateTime bestBeforeDate)
            : base(name, price)
        {
            if (prodDate > DateTime.Now.AddDays(1)) // Allow a bit of leeway for current day
            {
                 throw new InvalidDateException($"Production date cannot be significantly in the future: {prodDate.ToShortDateString()}");
            }
            this.productionDate = prodDate; // Set field directly first
            
            if (bestBeforeDate < this.productionDate)
            {
                throw new InvalidDateException($"Best-before date ({bestBeforeDate.ToShortDateString()}) cannot be earlier than production date ({this.productionDate.ToShortDateString()}).");
            }
            this.bestBeforeDate = bestBeforeDate;
        }

        public override string ToString()
        {
            return $"Product: {Name}, Price: {Price:C}, Production Date: {ProductionDate.ToShortDateString()}, " +
                   $"Best Before: {BestBeforeDate.ToShortDateString()}";
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(ProductionDate.ToBinary());
            writer.Write(BestBeforeDate.ToBinary());
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            ProductionDate = DateTime.FromBinary(reader.ReadInt64());
            BestBeforeDate = DateTime.FromBinary(reader.ReadInt64());
        }
    }
}