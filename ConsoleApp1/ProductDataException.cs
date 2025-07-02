using System;

namespace ConsoleApp1
{
    // Thrown when some product data (price, weight, volume, etc.) is invalid
    public class ProductDataException : Exception
    {
        public ProductDataException() { }
        public ProductDataException(string message) : base(message) { }
        public ProductDataException(string message, Exception inner) : base(message, inner) { }
    }
}