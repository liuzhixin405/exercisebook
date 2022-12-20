using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Customers
{
    public sealed class Address
    {
        public static Address Create(string street="", string number = "", string city = "", string zip = "", string country = "")
        {

            var address = new Address { Street = street, Number = number, City = city, Zip = zip, Country = country };
            return address;
        }

        private Address()
        {

        }
        public string Street { get; private set; }
        public string Number { get; private set; }
        public string City { get; private set; }
        public string Zip { get; private set; }
        public string Country { get; private set; }

        public static bool operator ==(Address a1, Address a2)
        {
            if (object.Equals(a1,null))
            {
                if (object.Equals(a2,null))
                    return true;
                else
                    return false;
            }
            return a1.Equals(a2);
        
        }
        public static bool operator !=(Address a1,Address a2)
        {
            return !(a1 == a2);
        }

        public override bool Equals(object? obj)
        {
            if (this == (Address)obj)
            
                return true;
            
            if (obj == null || GetType() != obj.GetType())
                return false;
            var other = (Address)obj;
            return string.Equals(Street, other.Street, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(Number, other.Number, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(City, other.City, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(Zip, other.Zip, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            const int hashIndex = 307;
            var result = (Street != null ? Street.GetHashCode() : 0);
            result = (result * hashIndex) ^ (Number != null ? Number.GetHashCode() : 0);
            result = (result * hashIndex) ^ (City != null ? City.GetHashCode() : 0);
            result = (result * hashIndex) ^ (Zip != null ? Zip.GetHashCode() : 0);
            return result;
        }
    }
}
