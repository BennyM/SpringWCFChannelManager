using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using WCFChannelManager;
using System.Reflection;
using System.ServiceModel.Channels;
using NUnit.Framework;
using Spring.Context.Support;
using Rhino.Mocks;

namespace WCFChannelManagerTests
{
    [TestFixture]
    public class ProxyChannelTests
    {
        [Test]
        public void ProxyChannel_WhenCalledToExecuteMethod_DoesWorkflow()
        {
            StubProxyChannel wrapper = new StubProxyChannel();
            MockRepository repo = new MockRepository();
            var channelManager = repo.StrictMock<IChannelManager<IService>>();
            var channel = repo.StrictMock<IService>();

            Expect.Call(channelManager.FetchChannelToWorkWith()).Return(channel);
            channel.DoOperation1();
            channelManager.FinishedWorkWithChannel(channel);
            repo.ReplayAll();

            wrapper.ChannelManager = channelManager;
            MethodInfo methodToExecuteOnChannel = (typeof(IService)).GetMethod("DoOperation1");
            wrapper.DoOperation(methodToExecuteOnChannel, null);
            repo.VerifyAll();

        }
  
    }
}
