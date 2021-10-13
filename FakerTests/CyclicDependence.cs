using NUnit.Framework;
using System;

namespace FakerTests
{
    class A
    {
        public B b;
    }

    class B
    {
        public A a;
    }
    public class Tests
    {
        private Faker.Faker _faker;
        [SetUp]
        public void Setup()
        {
            _faker = new Faker.Faker();
        }

        [Test]
        public void Depth1()
        {
        }
    }
}