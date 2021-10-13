using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Faker
{
    public class FakerConfig
    {
        private Dictionary<Type, Dictionary<string, IGenerator>> _generators;
        public FakerConfig()
        {
            _generators = new Dictionary<Type, Dictionary<string, IGenerator>>();
        }

        public void Add<TClass, TType, TGenerator>(Expression<Func<TClass, TType>> expression)
            where TGenerator : IGenerator
        {
            var member = expression.Body as MemberExpression;
            if (member == null) throw new ArgumentException("No member");
            Type classType = typeof(TClass);
            var name = member.Member.Name.ToLower();
            if (!_generators.ContainsKey(classType))
            {
                _generators.Add(classType, new Dictionary<string, IGenerator>());
            }
            if (!_generators[classType].ContainsKey(name))
            {
                _generators[classType].Add(name, (IGenerator)Activator.CreateInstance(typeof(TGenerator)));
            } else
            {
                _generators[classType][name] = (IGenerator)Activator.CreateInstance(typeof(TGenerator));
            }
        }

        internal IGenerator GetGenerator(Type type, String name)
        {
            if (_generators.TryGetValue(type, out var classGenerators))
            {
                if (classGenerators.TryGetValue(name.ToLower(), out var generator))
                    return generator;
                return null;
            }
            return null;
        }
    }
}
