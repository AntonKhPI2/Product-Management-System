namespace ConsoleApp1
{
    // Thrown when an index is out of the valid range
    public class InvalidIndexException : Exception
    {
        public InvalidIndexException() { }
        public InvalidIndexException(string message) : base(message) { }
        public InvalidIndexException(string message, Exception inner) : base(message, inner) { }
    }
}