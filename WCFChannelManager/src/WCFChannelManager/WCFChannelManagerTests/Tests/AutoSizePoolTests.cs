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
    public class AutoSizePoolTests
    {
        private MockRepository repo;
        private IPoolableObjectFactory factory;
        private StubAutoSizePool pool;

        [SetUp]
        public void Init()
        {
             repo = new MockRepository();
             factory = repo.StrictMock<IPoolableObjectFactory>();
             pool = new StubAutoSizePool(factory);
        }

        [TearDown]
        public void StopTest()
        {
            repo.VerifyAll();
        }

        [Test]
        public void PoolConstructor_WhenCalled_InitializesProperties()
        {
            repo.ReplayAll();

            Assert.AreEqual(0, pool.NumActive);
            Assert.AreEqual(0, pool.NumIdle);
            Assert.AreSame(factory, pool.PoolableObjectFactory);
        }

        [Test]
        public void AddObject_WhenCalled_AddsObjectToIdle()
        {
            Expect.Call(factory.MakeObject()).Return(new object());
            repo.ReplayAll();
            pool.AddObject();
            Assert.AreEqual(1, pool.NumIdle);
        }

        [Test]
        public void BorrowObject_WhenCalled_ReturnsNewObjectWhenNoneExistedBefore()
        {
            object factoryProduct = new object();

            Expect.Call(factory.MakeObject()).Return(factoryProduct);
            factory.ActivateObject(factoryProduct);
            repo.ReplayAll();

            object borrowedObject = pool.BorrowObject();
            Assert.AreSame(borrowedObject, factoryProduct);
        }

        [Test]
        public void BorrowObject_WhenCalled_ReturnsIdleObject()
        {
            object idleObject = new object();
            pool.AddIdleObject(idleObject);

            factory.ActivateObject(idleObject);
            Expect.Call(factory.ValidateObject(idleObject)).Return(true);
            repo.ReplayAll();

            object borrowedObject = pool.BorrowObject();
            Assert.AreSame(borrowedObject, idleObject);
        }

        [Test]
        public void BorrowObject_WhenCalled_CleansupInvalidIdles()
        {
            object invalidIdle = new object();
            object validObject = new object();
            pool.AddIdleObject(invalidIdle);

            factory.ActivateObject(invalidIdle);
            Expect.Call(factory.ValidateObject(invalidIdle)).Return(false);
            factory.DestroyObject(invalidIdle);
            Expect.Call(factory.MakeObject()).Return(validObject);
            factory.ActivateObject(validObject);
            repo.ReplayAll();

            object borrowedObject = pool.BorrowObject();
            Assert.AreSame(borrowedObject, validObject);
        }

        [Test]
        public void BorrowObject_WhenCalled_AddObjectToActiveList()
        {
            object newObject = new object();
           
            Expect.Call(factory.MakeObject()).Return(newObject);
            factory.ActivateObject(newObject);
            repo.ReplayAll();

            object borrowed = pool.BorrowObject();
            Assert.AreEqual(1, pool.NumActive);
        }

        [Test]
        public void Clear_WhenCalled_RemovesAllIdleObjects()
        { 
            object idleOne = new object();
            object idleTwo = new object();
            pool.AddIdleObject(idleOne);
            pool.AddIdleObject(idleTwo);
           
            factory.DestroyObject(idleOne);
            factory.DestroyObject(idleTwo);
            repo.ReplayAll();
            
            pool.Clear();
            Assert.AreEqual(0, pool.NumIdle);
        }

        [Test]
        public void Close_WhenCalled_ClearsActiveAndIdleObjects()
        {
            object idle = new object();
            object active = new object();
            pool.AddIdleObject(idle);
            pool.AddActiveObject(active);
            
            factory.DestroyObject(idle);
            factory.DestroyObject(active);
            repo.ReplayAll();
            
            pool.Close();
            Assert.AreEqual(0, pool.NumIdle);
            Assert.AreEqual(0, pool.NumActive);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BorrowObject_WhenCalledOnClosedPool_ThrowsException()
        {
            repo.ReplayAll();
            pool.SetClosed();
            pool.BorrowObject();
        }

        [Test]
        public void ReturnObject_WhenCalled_MakesObjectIdle()
        { 
            object activeObject = new object();
            pool.AddActiveObject(activeObject);
           
            factory.PassivateObject(activeObject);
            repo.ReplayAll();
           
            pool.ReturnObject(activeObject);
            Assert.AreEqual(1, pool.NumIdle);
        }

        [Test]
        public void ReturnObject_WhenCalledWithNotKnownObject_DoesNotMakeObjectIdle()
        {
            object stranger = new object();

            repo.ReplayAll();
            
            pool.ReturnObject(stranger);
            Assert.AreEqual(0, pool.NumIdle);
        }
    }
}
