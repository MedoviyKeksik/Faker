using Faker;
using Faker.FakerContext;
using System;
using System.Collections.Generic;

namespace SampleApp
{
    class Sample
    {
        public Sample() { }
        public List<int> list;
        public DateTime date;
    }

    [Generate(typeof(string))]
    class CustomIntGenerator : IGenerator
    {
        public object Generate(IFakerContext randomProvider)
        {
            return "asd";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Faker.Faker faker = new Faker.Faker();
            Console.WriteLine(faker.Create<Sample>().date);
        }
    }
}
