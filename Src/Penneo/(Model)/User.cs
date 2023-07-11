using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Customer>> GetCustomers(PenneoConnector con)
        {
            return _customers ?? (_customers = (await GetLinkedEntitiesAsync<Customer>(con)).Objects.ToList());
        }
    }
}
