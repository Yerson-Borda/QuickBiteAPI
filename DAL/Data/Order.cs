using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Enums;

namespace DAL.Data
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime DeliveryTime { get; set; }
        public DateTime OrderTime { get; set; }
        public Status Status { get; set; } = Status.InProcess;
        public decimal Price { get; set; }


        public Guid UserId { get; set; }
        public ICollection<Basket> Baskets { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
