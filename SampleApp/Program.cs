using Faker;
using Faker.FakerContext;
using System;

namespace SampleApp
{
    class Sample
    {
        public Sample() { }
        public double value;
        public Sample sample;
    }

    class CustomIntGenerator : IGenerator
    {
        public object Generate(IFakerContext randomProvider)
        {
            return 5;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Faker.Faker faker = new Faker.Faker();
            Console.WriteLine(faker.Create<Sample>().value);
            Console.WriteLine("Fuck you!");
        }
    }
}
