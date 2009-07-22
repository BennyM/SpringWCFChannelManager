using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Context.Support;
using Server;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var myService = ContextRegistry.GetContext().GetObject("MyService") as IService1;
            string value = myService.GetData(5);
            var returnValue = myService.GetDataUsingDataContract(new CompositeType() 
                            { 
                                BoolValue = false, 
                                StringValue = "Perponcher" });
            Console.ReadLine();
        }
    }

}
