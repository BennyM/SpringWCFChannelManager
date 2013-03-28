using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WCFChannelManager;
using Rhino.Mocks;
using Spring.Pool;

namespace WCFChannelManagerTests
{
    [TestFixture]
    public class AutoSizePoolFactoryTests
    {
        [Test]
        public void CreatePool_WhenCalled_ReturnsAutoSizePool()
        {
            var repo = new MockRepository();
            var poolableObjectFactory = repo.StrictMock<IPoolableObjectFactory>();
            repo.ReplayAll();
            AutoSizePoolFactory factory = new AutoSizePoolFactory();
            
            var result = factory.CreatePool(poolableObjectFactory);

            Assert.IsInstanceOfType(typeof(AutoSizePool), result);
            repo.VerifyAll();
        }
    }
}
