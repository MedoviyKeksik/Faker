using Faker;
using System;

namespace CustomGenerators
{
    [Generate(typeof(int))]
    public class IntGenerator : IGenerator
    {
        public object Generate()
        {
            return 1;
        }
    }
}
