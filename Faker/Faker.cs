using System;

namespace Faker
{
    public class Faker
    {
        FakerConfig _fakerConfig;

        static Faker()
        {

        }

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
