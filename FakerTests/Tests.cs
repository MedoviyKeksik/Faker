using Faker;
using Faker.FakerContext;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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

    class C
    {
        
    }

    class D
    {
        private D() { }
    }

    class E
    {
        public E(int value)
        {
            Value = value + 5;
        }
        public int Value;
    }

    [Generate(typeof(int))]
    class CustomIntGenerator : IGenerator
    {
        public object Generate(IFakerContext context)
        {
            return 5;
        }
    }
    public class Tests
    {
        private FakerConfig _config;
        private Faker.Faker _faker;
        [SetUp]
        public void Setup()
        {
            _faker = new Faker.Faker();
            _config = new FakerConfig();
            _config.Add<E, int, CustomIntGenerator>(obj => obj.Value);
        }

        [Test]
        public void CyclicDependency()
        {
            var tmp = _faker.Create<A>();
            Assert.IsNull(tmp.b.a);
        }

        [Test]
        public void IntGeneration()
        {
            var tmp = _faker.Create<int>();
            Assert.IsTrue(tmp.GetType() == typeof(int), "Type error");
        }

        [Test] 
        public void ListGeneration()
        {
            var tmp = _faker.Create<List<double>>();
            Assert.IsNotNull(tmp);
            Assert.IsTrue(tmp.GetType() == typeof(List<double>), "Type error");
        }

        [Test]
        public void PrivateConstructor()
        {
            var tmp = _faker.Create<D>();
            Assert.IsNull(tmp, "Object created, but expected not");
        }

        public void CustomConstructorField()
        {
            var tmp = _faker.Create<E>();
            Assert.AreEqual(10, tmp);
        }
    }
}