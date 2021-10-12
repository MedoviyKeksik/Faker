using Faker;
using System;

namespace CustomGenerators
{
    public class IntGenerator : IGenerator<int>
    {
        public int Generate()
        {
            return 1;
        }
    }
}
