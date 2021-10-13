using Faker;
using Faker.FakerContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultGenerators
{
    [Generate(typeof(List<>))]
    public class ListGenerator : IGenerator
    {
        const int MaxLength = 100;
        public object Generate(IFakerContext context)
        {
            var target = context.Target;
            var result = Activator.CreateInstance(target);
            var length = context.GetInt() % MaxLength;
            var type = context.Target.GetGenericArguments()[0];
            var parameters = new object[1];
            for (int i = 0; i < length; i++)
            {
                parameters[0] = context.GetObject(type);
                target.GetMethod("Add")?.Invoke(result, parameters);
            }
            return result;
        }
    }
}
