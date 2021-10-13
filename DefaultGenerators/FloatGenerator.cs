using Faker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultGenerators
{
    [Generate(typeof(float))]
    public class FloatGenerator : IGenerator
    {
        public object Generate()
        {
            return (float)2.0;
        }
    }
}
