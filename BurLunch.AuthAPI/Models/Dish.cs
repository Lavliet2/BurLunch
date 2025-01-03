﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BurLunch.AuthAPI.Models
{
    public class Dish
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int DishTypeId { get; set; }

        [ForeignKey("DishTypeId")]
        public DishType? DishType { get; set; }
        public ICollection<WeeklyMenuCard> WeeklyMenuCards { get; set; } = new List<WeeklyMenuCard>();
    }
}