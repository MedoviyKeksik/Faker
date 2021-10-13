using Faker;
using Faker.FakerContext;
using System;

namespace CustomGenerators
{
    [Generate(typeof(int))]
    public class IntGenerator : IGenerator
    {
        public object Generate(IFakerContext randomProvider)
        {
            return randomProvider.GetInt();       
        }
    }
}
