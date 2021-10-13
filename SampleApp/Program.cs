using System;

namespace SampleApp
{
    class Sample
    {
        public Sample() { }
        public double value;
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
