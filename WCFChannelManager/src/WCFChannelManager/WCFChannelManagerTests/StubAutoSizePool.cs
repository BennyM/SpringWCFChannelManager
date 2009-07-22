using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFChannelManager;
using Spring.Pool;

namespace WCFChannelManagerTests
{
    public class StubAutoSizePool
        : AutoSizePool
    {

        public StubAutoSizePool(IPoolableObjectFactory factory)
            : base(factory)
        {

        }

        public void AddIdleObject(object o)
        {
            base.passiveObjects.Add(o);
        }

        public void AddActiveObject(object o)
        {
            base.activeObjects.Add(o);
        }

        public void SetClosed()
        {
            closed = true;
        }
    }
}
