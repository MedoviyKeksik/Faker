using Faker;
using Faker.FakerContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultGenerators
{
    [Generate(typeof(double))]
    public class DoubleGenerator : IGenerator
    {
        public object Generate(IFakerContext context)
        {
            return context.GetDouble();
        }
    }
}
