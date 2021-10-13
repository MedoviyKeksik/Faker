using Faker;
using Faker.FakerContext;
using System;
using System.Collections.Generic;

namespace SampleApp
{
    class Sample
    {
        public Sample(DateTime date) {
            this.date = date.AddYears(1);
        }
        public List<int> list;
        public double someDouble { get; set; }
        public Uri uri;
        internal DateTime date;
        public DateTime GetDate()
        {
            return date;
        }
    }

    [Generate(typeof(DateTime))]
    class CustomDatetimeGenerator : IGenerator
    {
        public object Generate(IFakerContext randomProvider)
        {
            return DateTime.Parse("2021-10-13 18:00");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var config = new FakerConfig();
            config.Add<Sample, DateTime, CustomDatetimeGenerator>(obj => obj.date);
            Faker.Faker faker = new Faker.Faker(config);
            var tmp = faker.Create<Sample>();
            Console.WriteLine(tmp.GetDate());
            Console.WriteLine(tmp.list.Count);
            for (int i = 0; i < tmp.list.Count; i++) {
                Console.WriteLine(tmp.list[i]);
            }
            Console.WriteLine("Double: " + tmp.someDouble);
            Console.WriteLine("URI: " + tmp.uri);
        }
    }
}
