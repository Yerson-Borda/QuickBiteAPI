using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class Basket
    {
        public Guid Id { get; set; }

        public int Count { get; set; } // Amount of dishes-items in the basket DishInCart

        public Dish Dish { get; set; } 

        public Order? Order { get; set; } 

        public User User { get; set; } 
    }
}
