using ORM;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSubject
{
    public class Organization : Entity
    {
        public string Name { get; set; }
        public string WebAddress { get; set; }
        protected string RegistrationNumber { get; set; }
        public List<Branch> Branches { get; set; }
    }
}
