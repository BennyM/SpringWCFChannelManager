using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WCFChannelManager;
using Rhino.Mocks;
using Spring.Pool;
using Spring.Pool.Support;

namespace WCFChannelManagerTests
{
    [TestFixture]
    public class SimplePoolFactoryTests
    {
        [Test]
        public void ConstructorWithNoArgs_WhenCalled_SetsDefaultPoolSize()
        {
            SimplePoolFactory factory = new SimplePoolFactory();

            Assert.AreEqual(SimplePoolFactory.DefaultPoolSize, factory.PoolSize);
        }

        [Test]
        public void ConstructorWithIntArg_WhenCalled_SetsPoolSize()
        {
            int poolSize = 8;
            SimplePoolFactory factory = new SimplePoolFactory(poolSize);

            Assert.AreEqual(poolSize, factory.PoolSize);
        }

        [Test]
        public void CreatePool_WhenCalled_CreateSimplePool()
        {
            MockRepository repo = new MockRepository();
            var poolableObjectFactory = repo.StrictMock<IPoolableObjectFactory>();
            Expect.Call(poolableObjectFactory.MakeObject()).Return(new object()).Repeat.Times(SimplePoolFactory.DefaultPoolSize);
            SimplePoolFactory factory = new SimplePoolFactory();
            repo.ReplayAll();

            var result = factory.CreatePool(poolableObjectFactory);

            Assert.IsInstanceOfType(typeof(SimplePool), result);
            repo.VerifyAll();
        }
    }
}
