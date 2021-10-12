using System;
using System.IO;
using System.Reflection;

namespace Faker
{
    public class Faker
    {
        const string dllDirectory = "plugins";
        static Faker()
        {
            // import assemblies
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + dllDirectory);
            foreach (var dll in Directory.GetFiles(path, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(dll);
                foreach (var type in assembly.GetTypes())
                {
                }
            }
            
        }

        FakerConfig _fakerConfig;
        Faker()
        {
            _fakerConfig = new FakerConfig();
        }

        Faker(FakerConfig fakerConfig) 
        {
            _fakerConfig = fakerConfig;
        }

        public T Create<T>() where T : class
        {
            return null;
        }
    }
}
