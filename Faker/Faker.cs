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
        private static Dictionary<Type, IGenerator> _generators = new Dictionary<Type, IGenerator>(); 
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
                        var generateAttribute = type.GetCustomAttribute<GenerateAttribute>();
                        if (generateAttribute != null)
                        {
                            _generators.Add(generateAttribute.Type, (IGenerator)Activator.CreateInstance(type));
                        }
                    }
                }
            } catch (DirectoryNotFoundException)
            {
                // Directory not found => Ignore
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
            return (T)Create(typeof(T));
        }

        private object Create(Type type)
        {
            var generator = GetGenerator(type);
            if (generator != null)
            {
                return generator.Generate();
            }
            object instance = null;
            try
            {
                var constructor = type.GetConstructors()
                .Where(c => c.GetParameters().All(p => p.ParameterType != type))
                .OrderByDescending(c => c.GetParameters().Length)
                .Take(1).Single();
                var parameters = constructor.GetParameters();
                var generatedParams = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterType = parameters[i].ParameterType;
                    generatedParams[i] = Convert.ChangeType(Create(parameterType), parameterType);
                }
                instance = constructor.Invoke(generatedParams);
            } catch (InvalidOperationException)
            {
                // No public constructors => Ignore
            }
            if (instance == null) return null;
            foreach (var field in type.GetFields())
            {
                if (!field.IsPublic) continue;
                field.SetValue(instance, Create(field.FieldType));
            }

            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanWrite) continue;
                prop.SetValue(instance, Create(prop.PropertyType));
            }
            return instance;
        }
        private IGenerator GetGenerator(Type type) {
            if (_generators.TryGetValue(type, out var generator))
            {
                return generator;
            }
            return null;
        }
    }
}
