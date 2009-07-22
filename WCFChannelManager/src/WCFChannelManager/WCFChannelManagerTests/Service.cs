using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFChannelManagerTests
{
    public class Service
        : IService
    {
        #region IService Members

        public void DoOperation1()
        {

        }

        public int DoOperation2()
        {
            return 5;
        }

        public int DoOperation2WithNumber(int number)
        {
            return number;
        }

        public void Calculate(int a, int b, out int c)
        {
            c = a + b;
        }

        #endregion
    }
}
