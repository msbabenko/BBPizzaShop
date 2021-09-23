using System;

namespace BBPizzaShop
{
    class Program
    {
        Shop shop;

        public Program()
        {
            shop = new Shop();
        }
        static void Main(string[] args)
        {
            Program prog = new Program();

            //prog.shop.PrintAllPizzas();
            //  prog.shop.PrintAllToppings();
            //prog.shop.ForOrder();
          //  prog.shop.PrintOrder();
           prog.shop.Menu();
            Console.ReadKey();
        }
    }
}
