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
        private static Dictionary<string, IGenerator> _generators = new Dictionary<string, IGenerator>(); 
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
                            try
                            {
                                _generators.Add(generateAttribute.Type.Assembly + generateAttribute.Type.Namespace + generateAttribute.Type.Name, (IGenerator)Activator.CreateInstance(type));
                            }
                            catch (ArgumentException)
                            {
                                // this type already exist => Ignore
                            }
                        }
                    }
                }
            } catch (DirectoryNotFoundException)
            {
                // Directory not found => Ignore
            }
        }

        private readonly FakerConfig _fakerConfig;
        private IFakerContext context;
        private Stack<Type> _generationStack;
        public Faker()
        {
            _fakerConfig = new FakerConfig();
            _generationStack = new Stack<Type>();
            context = new FakerContext.FakerContext(this);
        }

        public Faker(FakerConfig fakerConfig)
        {
            _fakerConfig = fakerConfig;
            _generationStack = new Stack<Type>();
            context = new FakerContext.FakerContext(this);
        }

        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        internal object Create(Type type)
        {
            context.Target = type;
            var generator = GetGenerator(type);
            if (generator != null)
            {
                return generator.Generate(context);
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
                    context.Target = parameterType;
                    var customGenerator = _fakerConfig.GetGenerator(type, parameters[i].Name);
                    if (customGenerator != null && customGenerator.GetType().GetCustomAttribute<GenerateAttribute>().Type == parameterType)
                    {
                        blacklist.Add(customGenerator);
                        generatedParams[i] = customGenerator.Generate(context);
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
            } catch (TargetInvocationException)
            {
                return null;
            }
            if (instance == null) return null;
            foreach (var field in type.GetFields())
            {
                if (!field.IsPublic) continue;
                context.Target = field.FieldType;
                var customGenerator = _fakerConfig.GetGenerator(type, field.Name);
                if (customGenerator != null)
                {
                    if (blacklist.Contains(customGenerator)) continue;
                    field.SetValue(instance, customGenerator.Generate(context));
                }
                else field.SetValue(instance, Create(field.FieldType));
            }

            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanWrite) continue;
                context.Target = prop.PropertyType;
                var customGenerator = _fakerConfig.GetGenerator(type, prop.Name);
                if (customGenerator != null)
                {
                    if (blacklist.Contains(customGenerator)) continue;
                    prop.SetValue(instance, customGenerator.Generate(context));
                }
                else prop.SetValue(instance, Create(prop.PropertyType));
            }
            _generationStack.Pop();
            return instance;
        }
        private IGenerator GetGenerator(Type type) {
            if (_generators.TryGetValue(type.Assembly + type.Namespace + type.Name, out var generator))
            {
                return generator;
            }
            return null;
        }
    }
}
