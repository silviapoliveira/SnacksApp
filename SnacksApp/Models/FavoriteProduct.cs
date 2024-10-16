using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnacksApp.Models
{
    public class FavoriteProduct
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string? Name { get; set; }

        public string? Details { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsFavorite { get; set; }
    }
}
