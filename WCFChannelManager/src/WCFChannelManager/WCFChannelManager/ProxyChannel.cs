//Copyright 2013 Sergio Moreno Calzada

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
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Reflection;
using System.Collections;

namespace WCFChannelManager
{
    /// <summary>
    /// ProxyChannel has the workflow that needs to be done before an action on a channel is done, calls the action on the channel
    /// and finally any cleanup that needs to be done after the action is completed.
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    public class ProxyChannel<TChannel>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProxyChannel()
            : this(null)
        {
           
        }

        /// <summary>
        /// Constructor that takes a channelManager.
        /// </summary>
        /// <param name="channelManager">The channelmanager to use in this instance of ProxyChannel</param>
        public ProxyChannel(IChannelManager<TChannel> channelManager)
        {
            ChannelManager = channelManager;
        }

        /// <summary>
        /// The ChannelManager used. The ProxyChannel delegates channel creation and cleanup to this instance.
        /// </summary>
        public virtual IChannelManager<TChannel> ChannelManager 
        { 
            get; 
            set;
        }

        /// <summary>
        /// Retrieve a channel to work with.
        /// </summary>
        /// <returns>A channel that can be used.</returns>
        protected virtual TChannel GetChannelToWorkWith()
        {
            return ChannelManager.FetchChannelToWorkWith();
        }

        /// <summary>
        /// Execute a method on a channel.
        /// </summary>
        /// <param name="method">The method to invoke on the channel.</param>
        /// <param name="parameters">Any parameters that are needed when invoking the method.</param>
        /// <returns>AReturns any value returned by the method invoked on the channel.</returns>
        protected object ExecuteInChannel(MethodInfo method, object[] parameters)
        {
            TChannel channel = GetChannelToWorkWith();
            object returnValue;
            try
            {
                returnValue = method.Invoke(channel, parameters);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                FinishedWorkWithChannel(channel);
            }
            return  returnValue ;
        }

        /// <summary>
        /// When work on a channel has been completed the channel is handed for any cleanup work.
        /// </summary>
        /// <param name="channel">The channel which has been used.</param>
        protected virtual void FinishedWorkWithChannel(TChannel channel)
        {
            ChannelManager.FinishedWorkWithChannel(channel);
        }

        /// <summary>
        /// Executes the method matched by the methodname and parameters on a channel of TChannel.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">Parameters used by the method.</param>
        /// <returns>Returns any value returned by the method invoked on the channel.</returns>
        public object Execute(MethodInfo method, object[] parameters)
        {

            Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray(); 
            //Type[] parameterTypes;
            //if(parameters == null || parameters.Length == 0)
            //{
            //    parameterTypes = Type.EmptyTypes;
            //}
            //else
            //{
            //    List<Type> types = new List<Type>();
            //    foreach(object o in parameters)
            //    {
            //        types.Add(o.GetType());
            //    }
            //    parameterTypes = types.ToArray();
            //}
            MethodInfo methodToCall = typeof(TChannel).GetMethod(method.Name, BindingFlags.Instance | BindingFlags.Public, null, parameterTypes,null);
            return ExecuteInChannel(methodToCall, parameters);
        }

    }
}
