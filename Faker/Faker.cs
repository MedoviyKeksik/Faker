using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Faker
{
    public class Faker
    {
        private const string dllDirectory = "plugins";
        private static Dictionary<Type, Object> _generators = new Dictionary<Type, object>(); 
        static Faker()
        {
            try
            {
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dllDirectory);
                foreach (var dll in Directory.GetFiles(path, "*.dll"))
                {
                    var assembly = Assembly.LoadFrom(dll);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGenerator<>)))
                        {
                            var generator = Activator.CreateInstance(type);
                            var genericInterfaces = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGenerator<>));
                            foreach (var i in genericInterfaces)
                            {
                                _generators.TryAdd(i.GetGenericArguments()[0], generator);
                            }
                        }
                    }
                }
            } catch (DirectoryNotFoundException)
            {
                // Directory not found => all ok
            }
        }

        private readonly FakerConfig _fakerConfig;
        public Faker()
        {
            _fakerConfig = new FakerConfig();
        }

        public Faker(FakerConfig fakerConfig)
        {
            _fakerConfig = fakerConfig;
        }

        public T Create<T>()
        {
            var generated = Generator<T>().Generate();
            return generated;
        }
        private IGenerator<T> Generator<T>() {
            if (_generators.TryGetValue(typeof(T), out var generator))
            {
                return (IGenerator<T>) generator;
            }
            throw new NotImplementedException();
        }
    }
}
