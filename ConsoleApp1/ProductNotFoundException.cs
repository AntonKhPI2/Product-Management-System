using System;

namespace ConsoleApp1
{
    // Thrown when the requested product (by name or price) is not found
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException() { }
        public ProductNotFoundException(string message) : base(message) { }
        public ProductNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}