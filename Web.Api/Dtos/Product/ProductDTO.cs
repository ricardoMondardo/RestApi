﻿using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}