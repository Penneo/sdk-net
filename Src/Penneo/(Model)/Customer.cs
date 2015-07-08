namespace Penneo
{
    public class Customer : Entity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public bool Active { get; set; }
        public string VatIn { get; set; }
        public int Status { get; set; }
        public bool AccessControl { get; set; }
    }
}
