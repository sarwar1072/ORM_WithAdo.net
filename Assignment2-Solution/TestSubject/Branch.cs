using ORM;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSubject
{
    public class Branch : Entity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Address Location { get; set; }
    }
}
