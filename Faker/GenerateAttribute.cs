using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker
{
    public class GenerateAttribute : Attribute
    {
        public GenerateAttribute(Type type)
        {
            Type = type;
        }
        public Type Type { get; set; }
    }
}
