using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFChannelManager;
using System.Reflection;

namespace WCFChannelManagerTests
{
    public class StubProxyChannel
        : ProxyChannel<IService>
    {
        public void DoOperation(MethodInfo methodInfo, object[] parameters)
        {
            base.ExecuteInChannel(methodInfo, parameters);
        }
    }


}
