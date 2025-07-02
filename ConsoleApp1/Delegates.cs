using System;

namespace ConsoleApp1
{
    public delegate int MyComparison<T>(T x, T y); 
    
    public delegate bool MyPredicate<T>(T obj);
    
    public delegate void MyActionDelegate(string message);
}