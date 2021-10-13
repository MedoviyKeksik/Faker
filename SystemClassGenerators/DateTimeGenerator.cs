using Faker;
using Faker.FakerContext;
using System;

namespace SystemClassGenerators
{
    [Generate(typeof(DateTime))]
    public class DateTimeGenerator : IGenerator
    {
        public object Generate(IFakerContext context)
        {
            return new DateTime(((long)context.GetInt() * (long)context.GetInt()) % DateTime.MaxValue.Ticks);
        }
    }
}
