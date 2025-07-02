using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            LinkedListContainer<Product> products = new LinkedListContainer<Product>();
            ArrayContainer<Product> arrayProducts = new ArrayContainer<Product>();
            
            //Start the menu
            Menu<Product> menu = new Menu<Product>(products, arrayProducts);
            menu.Show();
        }
    }
}