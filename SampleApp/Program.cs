using System;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Faker.Faker faker = new Faker.Faker();
            Console.WriteLine(faker.Create<int>());
            Console.WriteLine(faker.Create<float>());
            Console.WriteLine("Fuck you!");
        }
    }
}
