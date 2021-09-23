using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBPizzaShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BBPizzaShop
{
    public class Shop
    {
        dbPizzasContext context;

        public Shop()
        {
            context = new dbPizzasContext();
        }

        public void Alina()
        {
            PrintAllPizzas();
        }

        public int ForPizzaOrders()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("Enter the Pizza of your choice");
            int pizzaId = 0;
            try
            {
                pizzaId = Convert.ToInt32(Console.ReadLine());
                if (pizzaId > 0)
                {
                    return pizzaId;
                }
                else
                {
                    Console.WriteLine("Pizza's Number should not be null");
                    ForPizzaOrders();
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            return pizzaId;
        }
        public int ForToppingsOrders()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("Enter the Topping of your choice");
            int toppingId = 0;
            try
            {
                toppingId = Convert.ToInt32(Console.ReadLine());
                if (toppingId > 0)
                {
                    return toppingId;
                }
                else
                {
                    Console.WriteLine("Toppings Number should not be null");
                    ForToppingsOrders();
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            return toppingId;
        }

        Pizza OrderPizza(int pizzaId) => context.Pizzas.Find(pizzaId);
        Topping OrderTopping(int toppingId) => context.Toppings.Find(toppingId);

        Order FindOrder(int orderId) => context.Orders.Find(orderId);

        int OrderDetailId() 
        {
            OrderDetail orderDetail = new OrderDetail();
            
            int orderDetailId = context.OrderDetails.Max(od => od.OdId); ;
            return orderDetailId;
        }

        public void ForOrderSummary(User user)
        {
            Order order = new Order();
            order.OrderUserId = user.UserId;
            order.OrderStatus = "Processing";
            order.OrderDelivery = 5;
            order.Ordertotal = 0;

            context.Orders.Add(order);
            context.SaveChanges();

            ForOrder();
            PrintOrder();
        }

        Order FindCurrentOrder()
        {
            int currentOrderId = context.Orders.Max(order => order.OrderId);
            return FindOrder(currentOrderId);
        }
        public void ForOrder()
        {

            Order currentOrder = new Order();
            OrderDetail orderDetail = new OrderDetail();
          

           
            
            currentOrder = FindCurrentOrder();
           

            PrintAllPizzas();
            int orderedPizzaId = ForPizzaOrders();
            Pizza orderedPizza = OrderPizza(orderedPizzaId);
            if (orderedPizza is not null)
            {
                currentOrder.Ordertotal += orderedPizza.PizzaPrice;
                Console.WriteLine("You have selected " + orderedPizza.PizzaName + " for $" + orderedPizza.PizzaPrice);
                orderDetail.PizzaId = orderedPizzaId;
                orderDetail.OrderId = context.Orders.Max(od => od.OrderId);
                context.OrderDetails.Add(orderDetail);
                context.Orders.Update(currentOrder);
                context.SaveChanges();

               
            }
            else {
                Console.WriteLine("Incorrect Input");
                ForOrder();
            }

            double? pizzaTotal = orderedPizza.PizzaPrice;

            char choise = 'n';
            do
            {
                OrderItemDetail orderItemDetail = new OrderItemDetail();


                Console.WriteLine("Do u want extra toppings?y/n");
                choise = Console.ReadLine()[0];
                if (choise == 'y')
                {
                    PrintAllToppings();
                    int orderedToppingId = ForToppingsOrders();

                    Topping orderedTopping = OrderTopping(orderedToppingId);

                    if (orderedTopping is not null)
                    {

                        currentOrder = FindCurrentOrder();
                        pizzaTotal += orderedTopping.ToppingsPrice;
                        currentOrder.Ordertotal += orderedTopping.ToppingsPrice;
                        Console.WriteLine("You have selected " + orderedTopping.ToppingsName + " for $" + orderedTopping.ToppingsPrice
                                             + " for total $" + pizzaTotal);
                        orderItemDetail.ToppingsId = orderedToppingId;
                        orderItemDetail.OdId = context.OrderDetails.Max(od => od.OdId);
                        context.OrderItemDetails.Add(orderItemDetail);
                        context.Orders.Update(currentOrder);
                        context.SaveChanges();


                    }
                    else 
                    {
                        Console.WriteLine("Incorrect Input");
                        ForOrder();
                    }
                }
                else if (choise != 'n')
                    Console.WriteLine("Please choose y/n");

            }
            while (choise != 'n');

            Console.WriteLine("Do you want to select another pizza for this order?y/n");
            choise = Console.ReadLine()[0];
            if (choise == 'y')
            {
                ForOrder();
            }

               


        }

       public void PrintOrder() 
        {
            
            Order currentOrder = new Order();
            OrderDetail orderDetail = new OrderDetail();
            OrderItemDetail orderItemDetail = new OrderItemDetail();
            char choise = 'n';

            User user = context.Users.Include(a => a.Orders).FirstOrDefault();
           //Pizza pizza = context.Pizzas.Include(a => a.OrderDetails).FirstOrDefault();

     

            currentOrder = FindCurrentOrder();
            List<Pizza> pizzas = context.Pizzas.Include(a => a.OrderDetails.Where(od => od.OrderId == 536)).ToList();

            List<OrderDetail> orderDetailsList = context.OrderDetails.Where(a => a.OrderId == currentOrder.OrderId).ToList();

            
            Console.WriteLine("Your order summary");

            int counter = 0;
           // List<Pizza> orderedPizzas ;
            foreach (var item in orderDetailsList)
            {
                counter++;
                Console.WriteLine("Pizza"+counter);
                List<Pizza> tmpPizzaList = context.Pizzas.Where(a => a.PizzaId == item.PizzaId).ToList();
                foreach (var pizzaitem in tmpPizzaList)
                {      
                    Console.WriteLine(pizzaitem);
                    List<Topping> tmpToppingList = context.Toppings.Include(a => a.OrderItemDetails
                                .Where(oid => oid.ToppingsId == pizzaitem.PizzaId)).ToList();
                    Console.WriteLine("Toppings");
                    if (tmpToppingList is not null)
                    {
                        foreach (var toppingItem in tmpToppingList)
                        {
                            Console.WriteLine(toppingItem);
                        }
                    }
                }
                //  if (tmpPizzaList is not null)
                // orderedPizzas.AddRange(tmpPizzaList);
                //  else Console.WriteLine(tmpPizzaList);


            }
           
       

          

         



            Console.WriteLine("Total price - $"+currentOrder.Ordertotal);
            if (currentOrder.Ordertotal >= 25)
            {
                Console.WriteLine("Delivery cost - $0");
                currentOrder.OrderDelivery = 0;
                context.Orders.Update(currentOrder);
                context.SaveChanges();
            }
            else
                Console.WriteLine("Delivery cost - $" + currentOrder.OrderDelivery);

            Console.WriteLine("Note- delivery cost of $5 will be added for order less than $25");
            Console.WriteLine("Please confirm your order y/n?");
            choise = Console.ReadLine()[0];
            if (choise == 'y')
            {
                Console.WriteLine("The order will be delivered to address");
                Console.WriteLine(user.UserAddress);
                Console.WriteLine("Please pay on delivery\nThank you");
                currentOrder.OrderStatus = "Confirmed";
                context.Orders.Update(currentOrder);
                context.SaveChanges();
            }
            else Console.WriteLine("Thank you for visiting us! Have a good day!");


        }

        public void PrintAllPizzas()
        {
            Console.WriteLine("The following are the pizza that are available for ordering");
            Console.WriteLine("Number\tName\t\tPrice\tType");
            foreach (var item in context.Pizzas)
            {
                Console.WriteLine(item);
            }
        }

        public void PrintAllToppings()
        {
            Console.WriteLine("The folowing are the toppings");
            Console.WriteLine("Number\tName\t\tPrice");
            foreach (var item in context.Toppings)
            {
                Console.WriteLine(item);
            }
        }

        public void UserRegistration()
        {
            User user = new User();

            Console.WriteLine("Please enter your Name:");
            string name = Console.ReadLine();
            Console.WriteLine("Please enter your e-mail:");
            string email = Console.ReadLine();
            Console.WriteLine("Please enter your password:");
            string pswd = Console.ReadLine();
            Console.WriteLine("Please enter your phone number:");
            string phone = Console.ReadLine();
            Console.WriteLine("Please enter your address:");
            string address = Console.ReadLine();

            if (name != "" && email != "" && phone != "" && address != "" && pswd != "")
            {
                User tmpUser = context.Users.Where(u => u.UserPhone == phone || u.UserEmail == email).FirstOrDefault();
                if (user is not null)
                {
                    Console.WriteLine("-- User already exists!");
                    UserLogin();
                }

                user.UserName = name;
                user.UserEmail = email;
                user.UserPhone = phone;
                user.UserAddress = address;
                user.UserPswd = pswd;
                Console.WriteLine(user);
                context.Users.Add(user);
                context.SaveChanges();
            }


        }

        public void UserLogin()
        {
        

            Console.WriteLine("Enter your username:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter your password:");
            string pswd = Console.ReadLine();

            if (name != "" && pswd != "" && name != " " && pswd != " ")
            {
                User user = context.Users.Where(u => u.UserName == name).FirstOrDefault();
                if (user is null)
                {
                    Console.WriteLine("Incorrect username");
                    UserLogin();
                }
                if (user.UserPswd == pswd)
                {
                    ForOrderSummary(user);
                }
                else
                {
                    Console.WriteLine("Incorrect password, please try again");
                    UserLogin();
                }
            }
        }

        public void Menu()
        {
            int iChoice = 5;

            do
            {
                Console.WriteLine("Pizza ordering system");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");

                try
                {
                    iChoice = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
                switch (iChoice)
                {
                    case 0:
                        Console.WriteLine("You have selected to exit. Bye...");
                        break;
                    case 1:
                        UserLogin();
                        break;
                    case 2:
                        UserRegistration();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again");
                        break;
                }
            } while (iChoice != 0);


        }
    }
}
