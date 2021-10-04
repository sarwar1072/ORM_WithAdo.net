using System;
using System.Collections.Generic;
using System.Text;

namespace ORM
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
    }
}
