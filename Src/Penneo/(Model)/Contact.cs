namespace Penneo
{
    public class Contact : EntityWithIntId
    {
        public Contact()
        {
        }

        public Contact(string name)
        {
            Name = name;
        }

        public Contact(string name, string email)
            : this(name)
        {
            Email = email;
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string OnBehalfOf { get; set; }

    }
}
