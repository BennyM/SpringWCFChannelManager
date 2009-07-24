using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace WCFChannelManagerTests
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void DoOperation1();

        [OperationContract]
        int DoOperation2();

        [OperationContract]
        int DoOperation2WithNumber(int number);

        [OperationContract]
        void Calculate(int a, int b, out int c);

        [OperationContract]
        ComplexObject ReturnComplexObject();
    }
}
