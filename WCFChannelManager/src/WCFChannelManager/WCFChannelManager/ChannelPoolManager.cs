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
using Spring.Pool;
using System.ServiceModel;
using Spring.Pool.Support;
using System.ServiceModel.Channels;

namespace WCFChannelManager
{

    /// <summary>
    /// A ChannelManager that pools channels that can be used.
    /// </summary>
    /// <typeparam name="TChannel">The channeltype.</typeparam>
    public class ChannelPoolManager<TChannel>
        : ChannelManagerBase<TChannel>, IPoolableObjectFactory
    {

        /// <summary>
        /// Constructs a channelpoolmanager.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in your system.Servicemodel section.</param>
        public ChannelPoolManager(string endpointConfigurationName, IPoolFactory poolFactory)
            : base(endpointConfigurationName)
        {
            Pool = CreatePool(poolFactory);
        }


        /// <summary>
        /// Constructs a channelpoolmanager.
        /// </summary>
        /// <param name="factory">The channel factory to use.</param>
        public ChannelPoolManager(ICanCreateChannels<TChannel> factory, IPoolFactory poolFactory)
            : base(factory)
        {
            Pool = CreatePool(poolFactory);
        }

        /// <summary>
        /// Create a pool to use.
        /// </summary>
        /// <param name="size">The size of the pool.</param>
        /// <returns>A valid pool that can be used.</returns>
        protected virtual IObjectPool CreatePool(IPoolFactory poolFactory)
        {
            return poolFactory.CreatePool(this);
        }

        /// <summary>
        /// The pool of channels.
        /// </summary>
        public virtual IObjectPool Pool
        {
            get;
            private set;
        }

        #region IPoolableObjectFactory Members

        /// <summary>
        /// Activates a channel.
        /// </summary>
        /// <param name="obj">The channel to activate.</param>
        void IPoolableObjectFactory.ActivateObject(object obj)
        {
            OpenChannel((TChannel)obj);
        }

        /// <summary>
        /// Destory a channel.
        /// </summary>
        /// <param name="obj">The channel to destroy.</param>
        void IPoolableObjectFactory.DestroyObject(object obj)
        {
            CloseChannel((TChannel)obj);
        }

        /// <summary>
        /// Create a channel for the pool.
        /// </summary>
        /// <returns>A channel for the pool.</returns>
        object IPoolableObjectFactory.MakeObject()
        {
            return CreateChannel();
        }

        /// <summary>
        /// Passivate a channel.
        /// </summary>
        /// <param name="obj">The channel to passivate.</param>
        void IPoolableObjectFactory.PassivateObject(object obj)
        {

        }

        /// <summary>
        /// Check if the channel is valid.
        /// </summary>
        /// <param name="obj">The channel to check.</param>
        /// <returns>Returns true if the channel can be used, otherwise false.</returns>
        bool IPoolableObjectFactory.ValidateObject(object obj)
        {
            ICommunicationObject communicationObject = obj as ICommunicationObject;
            bool valid = false;
            if (communicationObject != null && communicationObject.State == CommunicationState.Opened)
            {
                valid = true;
            }
            return valid;
        }

        #endregion

        /// <summary>
        /// Get a channel to send messages to.
        /// </summary>
        /// <returns>A channel that can be used to send messages to.</returns>
        public override TChannel FetchChannelToWorkWith()
        {
            return (TChannel)Pool.BorrowObject();
        }

        /// <summary>
        /// Perform cleanup work after sending messages to the channel.
        /// </summary>
        /// <param name="channel">The channel to send messages to.</param>
        public override void FinishedWorkWithChannel(TChannel channel)
        {
            Pool.ReturnObject(channel);
        }


    }
}
