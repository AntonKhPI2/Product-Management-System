using System;

namespace ConsoleApp1
{
    // Thrown when a provided date is invalid (e.g. expiration date is in the past)
    public class InvalidDateException : ProductDataException
    {
        public InvalidDateException() { }
        public InvalidDateException(string message) : base(message) { }
        public InvalidDateException(string message, Exception inner) : base(message, inner) { }
    }
}