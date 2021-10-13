using Faker.FakerContext;
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
        private IFakerContext _random;
        private Stack<Type> _generationStack;
        public Faker()
        {
            _fakerConfig = new FakerConfig();
            _generationStack = new Stack<Type>();
            _random = new FakerContext.FakerContext();
        }

        public Faker(FakerConfig fakerConfig)
        {
            _fakerConfig = fakerConfig;
            _generationStack = new Stack<Type>();
            _random = new FakerContext.FakerContext();
        }

        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        internal object Create(Type type)
        {
            var generator = GetGenerator(type);
            if (generator != null)
            {
                return generator.Generate(_random);
            }
            object instance = null;
            var blacklist = new SortedSet<IGenerator>();
            try
            {
                var constructor = type.GetConstructors()
                .Where(c => c.GetParameters().All(p => p.ParameterType != type))
                .OrderByDescending(c => c.GetParameters().Length)
                .Take(1).Single();
                if (_generationStack.Contains(type)) throw new StackOverflowException("Cyclic dependency");
                _generationStack.Push(type);
                var parameters = constructor.GetParameters();
                var generatedParams = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameterType = parameters[i].ParameterType;
                    var customGenerator = _fakerConfig.GetGenerator(type, parameters[i].Name);
                    if (customGenerator != null && customGenerator.GetType().GetCustomAttribute<GenerateAttribute>().Type == parameterType)
                    {
                        blacklist.Add(customGenerator);
                        generatedParams[i] = customGenerator.Generate(_random);
                    }
                    else
                        generatedParams[i] = Create(parameterType);
                }
                instance = constructor.Invoke(generatedParams);
            } catch (InvalidOperationException)
            {
                // No public constructors => Ignore
            } catch (StackOverflowException)
            {
                // Cyclic dependency => Ignore
            }
            if (instance == null) return null;
            foreach (var field in type.GetFields())
            {
                if (!field.IsPublic) continue;
                var customGenerator = _fakerConfig.GetGenerator(type, field.Name);
                if (customGenerator != null)
                {
                    if (blacklist.Contains(customGenerator)) continue;
                    field.SetValue(instance, customGenerator.Generate(_random));
                }
                else field.SetValue(instance, Create(field.FieldType));
            }

            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanWrite) continue;
                var customGenerator = _fakerConfig.GetGenerator(type, prop.Name);
                if (customGenerator != null)
                {
                    if (blacklist.Contains(customGenerator)) continue;
                    prop.SetValue(instance, customGenerator.Generate(_random));
                }
                else prop.SetValue(instance, Create(prop.PropertyType));
            }
            _generationStack.Pop();
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
