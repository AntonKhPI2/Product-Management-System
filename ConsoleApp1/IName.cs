using System;

namespace ConsoleApp1
{
    public interface IName<T> where T : IName<T>
    {
        string Name { get; }
    }

    public interface IName : IComparable<IName>, IName<IName>
    {
    }

    public interface IPrice : IComparable<IPrice>
    {
        decimal Price { get; }
    }
}