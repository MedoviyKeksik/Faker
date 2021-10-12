using Faker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultGenerators
{
    public class FloatGenerator : IGenerator<float>
    {
        public float Generate()
        {
            return (float)2.0;
        }
    }
}
