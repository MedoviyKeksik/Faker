using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faker.FakerContext
{
    class FakerContext : IFakerContext
    {
        private Faker _faker;
        private Random _random;
        public Type Target { get; set; }
        public FakerContext(Faker faker)
        {
            _faker = faker;
            _random = new Random();
        }
        public FakerContext(Faker faker, int seed)
        {
            _faker = faker;
            _random = new Random(seed);
        }

        public double GetDouble()
        {
            return _random.NextDouble();
        }

        public int GetInt()
        {
            return _random.Next();
        }

        public void GetBytes(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

        public object GetObject(Type type)
        {
            return _faker.Create(type);
        }
    }
}
