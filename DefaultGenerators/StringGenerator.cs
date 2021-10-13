using Faker;
using Faker.FakerContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultGenerators
{
    [Generate(typeof(string))]
    public class StringGenerator : IGenerator
    {
        private const int MaxLength = 100;
        public object Generate(IFakerContext context)
        {
            int len = context.GetInt() % MaxLength;
            var buffer = new byte[len];
            context.GetBytes(buffer);
            return Encoding.ASCII.GetString(buffer);
        }
    }
}
