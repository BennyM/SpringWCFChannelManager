using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WCFChannelManagerTests
{
    public class Service : IService
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

        public object ReturnsNull()
        {
            return null;
        }

        #endregion

        #region IService Members


        public ComplexObject ReturnComplexObject()
        {
            return null;
        }

        public void DoOperationWithByRef(ref int number)
        {
            number = 5;
        }

        #endregion
    }

    [DataContract]
    public class ComplexObject
    {
        [DataMember]
        public int Value { get; set; }
    }
}
