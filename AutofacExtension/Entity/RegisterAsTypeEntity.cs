using System;
using System.Collections.Generic;

namespace AutofacExtension.Entity
{
    public class RegisterAsTypeEntity
    {
        public Type OriginType { get; set; }
        public IEnumerable<Type> RegisterType { get; set; }
    }
}