using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WCFChannelManager;
using Spring.Objects.Factory.Support;
using Spring.Context.Support;
using Spring.Objects;
using System.ServiceModel;
using Spring.Pool.Support;

namespace WCFChannelManagerTests
{
    [TestFixture]
    public class ChannelManagerFactoryObjectTests
    {
        public const string EndpointName = "MyEndpoint";

        public ChannelManagerFactoryObject Factory 
        { 
            get; 
            set; 
        }

        [SetUp]
        public void StartTest()
        {
            Factory = new ChannelManagerFactoryObject();
        }

        private void SetupFactoryWithEndpointAndService()
        {
            SetupFactory(EndpointName, typeof(IService));
        }

        private void SetupFactory(string endpointName, Type channelType)
        {
            Factory.EndpointConfigurationName = endpointName;
            Factory.ChannelType = channelType;
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void AfterPropertiesSet_WhenNothingIsConfigured_ThrowsException()
        {
            Factory.AfterPropertiesSet();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void AfterPropertiesSet_WhenMissingEndpoint_ThrowsException()
        {
            Factory.ChannelType = typeof(IService);

            Factory.AfterPropertiesSet();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void AfterPropertiesSet_WhenMissingChannelType_ThrowsException()
        {
            Factory.EndpointConfigurationName = EndpointName;

            Factory.AfterPropertiesSet();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void AfterPropertiesSet_WhenChannelTypeCanNotBeDerived_ThrowsException()
        {
            SetupFactory(EndpointName, null);
            Factory.ChannelActionWrapperType = typeof(OpenGenericProxyChannel<>);

            Factory.AfterPropertiesSet();
        }

        [Test]
        public void AfterProperties_WhenChannelTypeAndEndpointConfigured_Succeeds()
        {
            SetupFactoryWithEndpointAndService();

            Factory.AfterPropertiesSet();
        }

        [Test]
        public void AfterProperties_WhenChannelTypeCanBeDerivedAndEndpointConfigured_Succeeds()
        {
            SetupFactory(EndpointName, typeof(StubProxyChannel));

            Factory.AfterPropertiesSet();
        }

        [Test]
        public void GetObject_WhenCalled_ReturnsChannelMatchingChannelType()
        {
            SetupFactoryWithEndpointAndService();

            object result = Factory.GetObject();

            Assert.IsTrue(result is IService);
        }

        [Test]
        public void GetObject_WhenCalled_ReturnsChannelWithDefaultValues()
        {
            SetupFactoryWithEndpointAndService();

            var result = Factory.GetObject() as ProxyChannel<IService>;

            Assert.IsNotNull(result.ChannelManager, "No default channelmanager was set");
            Assert.IsInstanceOfType(typeof(SingleActionChannelManager<IService>), result.ChannelManager);
        }

        [Test]
        public void GetObject_WhenConfiguredWithFixedPool_ReturnsChannelWithFixedPool()
        {
            AssertChannelHasPool(ChannelManagerFactoryObject.FixedPool, typeof(SimplePool));
        }

        private void AssertChannelHasPool(string channelManagementMode, Type poolType)
        {
            SetupFactoryWithEndpointAndService();
            Factory.ChannelManagementMode = channelManagementMode;

            var result = Factory.GetObject() as ProxyChannel<IService>;

            Assert.IsInstanceOfType(typeof(ChannelPoolManager<IService>), result.ChannelManager);
            Assert.IsInstanceOfType(poolType, ((ChannelPoolManager<IService>)result.ChannelManager).Pool);
        }


        [Test]
        public void GetObject_WhenCalledWithAutoSizePool_ReturnsChannelWithAutoSizePool()
        {
            AssertChannelHasPool(ChannelManagerFactoryObject.VariablePool, typeof(AutoSizePool));
        }

        [Test]
        public void Factory_WhenObtainedFromContainer_HasContainerConfiguration()
        {
            RootObjectDefinition productTemplate = new RootObjectDefinition();
            productTemplate.PropertyValues.Add(new Spring.Objects.PropertyValue("ChannelManager.ChannelFactory.Credentials.UserName.UserName", "un"));
            productTemplate.PropertyValues.Add(new Spring.Objects.PropertyValue("ChannelManager.ChannelFactory.Credentials.UserName.Password", "pw"));
            GenericApplicationContext context = new GenericApplicationContext();
            IObjectDefinitionFactory objectDefinitionFactory = new DefaultObjectDefinitionFactory();
            ObjectDefinitionBuilder builder = ObjectDefinitionBuilder.RootObjectDefinition(objectDefinitionFactory, typeof(ChannelManagerFactoryObject));
            builder.AddPropertyValue("EndpointConfigurationName",EndpointName);
            builder.AddPropertyValue("ChannelType",typeof(IService));
            builder.AddPropertyValue("ProductTemplate", productTemplate);
            context.RegisterObjectDefinition("myChannelFactoryObject", builder.ObjectDefinition);
            
            var obj = context.GetObject("myChannelFactoryObject")as ProxyChannel<IService>; 
            
            Assert.IsNotNull(obj);
            ChannelManagerBase<IService> channelManager = obj.ChannelManager as ChannelManagerBase<IService>;
            Assert.IsNotNull(channelManager);
            Assert.AreEqual("un", ((ChannelFactory<IService>)channelManager.ChannelFactory).Credentials.UserName.UserName);
            Assert.AreEqual("pw", ((ChannelFactory<IService>)channelManager.ChannelFactory).Credentials.UserName.Password);
        }
    }
}
