//Copyright 2009 Benny Michielsen

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Proxy;
using System.Reflection.Emit;

namespace WCFChannelManager
{
    /// <summary>
    /// Builds subclasses of a ProxyChannel type so all calls get handled by the Execute method.
    /// </summary>
    public class WcfChannelProxyTypeBuilder
        : AbstractProxyTypeBuilder
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="channelActionWrapper">The type to subclass.</param>
        public WcfChannelProxyTypeBuilder(Type channelActionWrapper)
        {
            if (!IsSubclassOfRawGeneric(typeof(ProxyChannel<>), channelActionWrapper))
            {
                throw new ArgumentException("The type used as channelActionWrapper should inherit from ChannelActionWrapper<T>");
            }

            ChannelManagerType = channelActionWrapper;
        }

        /// <summary>
        /// Checks if a class is a subclass of generic class.
        /// </summary>
        /// <param name="generic">The generic class.</param>
        /// <param name="toCheck">The type to check.</param>
        /// <returns>True if its a subclass, otherwise false.</returns>
        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            bool isSubclass = false;
            while (toCheck != typeof(object) && !isSubclass)
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return isSubclass;
        }

        /// <summary>
        /// Finds the channeltype from a ChannelActionWrapperType.
        /// </summary>
        /// <param name="channelManagerType">The ChannelManagerType.</param>
        /// <returns>Returns the channaltype if found, otherwise null.</returns>
        public static Type FindChannelType(Type channelManagerType)
        {
            Type t = channelManagerType;
            Type channelType = null;
            while (t != typeof(object)&& channelType == null)
            {
                var cur = t.IsGenericType ? t.GetGenericTypeDefinition() : t;
                if (typeof(ProxyChannel<>) == cur)
                {
                    channelType = t.GetGenericArguments()[0];
                }
                t = t.BaseType;
            }
            return channelType;
        }

        /// <summary>
        /// The ChannelManagerType.
        /// </summary>
        public Type ChannelManagerType
        {
            get;
            set;
        }

        /// <summary>
        /// The channeltype.
        /// </summary>
        public Type ChannelType
        {
            get
            {
                return FindChannelType(ChannelManagerType);
            }
        }

        #region AbstractProxyTypeBuilder Members

        /// <summary>
        /// Create the new type.
        /// </summary>
        /// <returns>A subclass of ProxyChannel with channel interface implementation.</returns>
        public override Type BuildProxyType()
        {
            TypeBuilder builder = CreateTypeBuilder(Name, ChannelManagerType);
            ImplementInterface(builder, new WcfMethodBuilder(builder, this, ChannelManagerType), ChannelType, TargetType);
            return builder.CreateType();
        }

        /// <summary>
        /// Pushed the target.
        /// </summary>
        /// <param name="il">IL generator.</param>
        public override void PushTarget(System.Reflection.Emit.ILGenerator il)
        {
            PushProxy(il);
        }

        #endregion
    }
}
