using System;

namespace ConsoleApp1
{
    public interface INotifyTotalPriceChanged
    {
        event Action<decimal> TotalPriceChanged;
        decimal TotalPrice { get; }
    }
}