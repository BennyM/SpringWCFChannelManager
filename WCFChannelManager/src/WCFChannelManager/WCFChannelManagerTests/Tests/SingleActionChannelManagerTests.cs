using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WCFChannelManager;
using Rhino.Mocks;
using System.ServiceModel;

namespace WCFChannelManagerTests.Tests
{
    [TestFixture]
    public class SingleActionChannelManagerTests
    {
        MockRepository repo;
        ICanCreateChannels<IService> channelFactory;
        ICommunicationObject communicationObject;
        SingleActionChannelManager<IService> manager;
       
        [SetUp]
        public void BeginTest()
        {
            repo = new MockRepository();
            channelFactory = repo.StrictMock<ICanCreateChannels<IService>>();
            communicationObject = repo.StrictMultiMock<ICommunicationObject>(typeof(IService));
            manager = new SingleActionChannelManager<IService>(channelFactory);
        }

        [TearDown]
        public void EndTest()
        {
            repo.VerifyAll();
        }
        
        [Test]
        public void FetchChannelToWorkWith_WhenCalled_CreatesAndOpensChannel()
        { 
            Expect.Call(channelFactory.CreateChannel()).Return((IService)communicationObject);
            Expect.Call(communicationObject.State).Return(CommunicationState.Created);
            communicationObject.Open();
            repo.ReplayAll();

            var channel = manager.FetchChannelToWorkWith();

            Assert.AreSame(communicationObject, channel);
        }

        [Test]
        public void FinishedWorkWithChannel_WhenCalled_ClosesChannel()
        {
            communicationObject.Close();
            repo.ReplayAll();

            manager.FinishedWorkWithChannel((IService)communicationObject);
        }
    }
}
