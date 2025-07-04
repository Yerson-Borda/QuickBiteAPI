﻿using Microsoft.AspNetCore.Identity;
using DTO.Enums;

namespace DAL.Data
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
        public Gender Gender { get; set; }
        public string? Address { get; set; }

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Basket> BasketList { get; set; } = new List<Basket>();
        public ICollection<Order> OrderList { get; set; } = new List<Order>();
    }
}
