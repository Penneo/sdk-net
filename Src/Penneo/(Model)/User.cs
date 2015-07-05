using System.Collections.Generic;
using System.Linq;

namespace Penneo
{
    public class User : Entity
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        private IEnumerable<Customer> _customers;

        public IEnumerable<Customer> Customers
        {
            get { return _customers; }
            set { _customers = value; }
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _customers ?? (_customers = GetLinkedEntities<Customer>().Objects.ToList());
        }
    }
}
