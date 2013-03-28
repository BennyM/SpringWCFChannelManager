using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using WCFChannelManager;
using Spring.Pool;
using System.ServiceModel;

namespace WCFChannelManagerTests
{
    [TestFixture]
    public class ChannelPoolManagerTests
    {
        MockRepository repo;
        ICanCreateChannels<IService> channelCreator;
        IPoolFactory poolFactory;
        IObjectPool pool;

        [SetUp]
        public void BeginTest()
        {
            repo = new MockRepository();
            channelCreator = repo.StrictMock<ICanCreateChannels<IService>>();
            poolFactory = repo.StrictMock<IPoolFactory>();
            pool = repo.StrictMock<IObjectPool>();
            Expect.Call(poolFactory.CreatePool(null)).IgnoreArguments().Return(pool);
            LastCall.IgnoreArguments();
        }

        [TearDown]
        public void EndTest()
        {
            repo.VerifyAll();
        }

        private ChannelPoolManager<IService> CreatePoolManager()
        {
            return new ChannelPoolManager<IService>(channelCreator, poolFactory);
        }

        private ICommunicationObject CreateCommunicationObject()
        {
           return repo.StrictMultiMock<ICommunicationObject>(typeof(IService));
        }

        private void AssertValidateObjectWithChannelInState(CommunicationState state, bool validateObjectReturns)
        {
            var commObject = CreateCommunicationObject();
            Expect.Call(commObject.State).Return(state);
            repo.ReplayAll();

            var isValid = ((IPoolableObjectFactory)CreatePoolManager()).ValidateObject(commObject);

            Assert.AreEqual(validateObjectReturns, isValid);
        }

        [Test]
        public void Constructor_WhenCalled_CreatesPool()
        {
            repo.ReplayAll();
            
            Assert.IsNotNull(CreatePoolManager().Pool);

        }

        [Test]
        public void ActivateObject_WhenCalled_OpensChannel()
        {
            var commObject = CreateCommunicationObject();
            Expect.Call(commObject.State).Return(CommunicationState.Created);
            commObject.Open();
            repo.ReplayAll();

            ((IPoolableObjectFactory)CreatePoolManager()).ActivateObject(commObject);
        }

        [Test]
        public void DestroyObject_WhenCalled_ClosesChannel()
        {
            var commObject = CreateCommunicationObject();
            commObject.Close();
            repo.ReplayAll();

            ((IPoolableObjectFactory)CreatePoolManager()).DestroyObject(commObject);
        }

        [Test]
        public void MakeObject_WhenCalled_CreatesChannel()
        {
            Service channel = new Service();
            Expect.Call(channelCreator.CreateChannel()).Return(channel);
            repo.ReplayAll();

            var result = ((IPoolableObjectFactory)CreatePoolManager()).MakeObject();

            Assert.AreSame(channel, result);
        }

        [Test]
        public void ValidateObject_WhenCalledWithOpenChannel_ReturnsTrue()
        {
            bool ValidateObjectReturns = true;
            AssertValidateObjectWithChannelInState(CommunicationState.Opened, ValidateObjectReturns);
        }

        [Test]
        public void ValidateObject_WhenCalledWithClosedChannel_ReturnsFalse()
        {
            bool ValidateObjectReturns = false;
            AssertValidateObjectWithChannelInState(CommunicationState.Closed, ValidateObjectReturns);
        }

        [Test]
        public void FetchChannelToWorkWith_WhenCalled_AsksPoolBorrow()
        {
            Service channel = new Service();
            Expect.Call(pool.BorrowObject()).Return(channel);
            repo.ReplayAll();

            var result = CreatePoolManager().FetchChannelToWorkWith();

            Assert.AreSame(result, channel);
        }


        [Test]
        public void FinishedWorkWithChannel_WhenCalled_ReturnsObjectToPool()
        {
            pool.ReturnObject(null);
            LastCall.IgnoreArguments();
            repo.ReplayAll();

            CreatePoolManager().FinishedWorkWithChannel(null);
        }
    }
}
