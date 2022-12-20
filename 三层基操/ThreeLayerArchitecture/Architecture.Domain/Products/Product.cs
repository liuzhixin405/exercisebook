using Architecture.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Products
{
    public class Product:IAggregateRoot
    {
        protected Product()
        {
        }
        public Product(int id, string description, Money unitPrice, int stockLevel)
        {
            Id = id;
            Description = description;
            UnitPrice = unitPrice;
            StockLevel = stockLevel;
        }
        public int Id { get; private set; }

        public string Description { get; private set; }
        public Money UnitPrice { get; private set; }
        public int StockLevel { get; private set; }
        public bool Featured { get; private set; }

        #region Behavior
        /// <summary>
        /// Applies a bit of biz logic to determine how many items left should
        /// be displayed when the product is featured. By default half the real stock.
        /// </summary>
        /// <returns></returns>
        public int GetStockForDisplay()
        {
            return StockLevel / 2;
        }

        #endregion

        #region Identity Management
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null || GetType() != obj.GetType())
                return false;
            var other = (Product)obj;

            // Your identity logic goes here.  
            // You may refactor this code to the method of an entity interface 
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }
}
