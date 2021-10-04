using ORM;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSubject
{
    public class Address : Entity
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
