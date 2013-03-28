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
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCFChannelManager
{
    /// <summary>
    /// Base class for channelmanagers.
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel</typeparam>
    public abstract class ChannelManagerBase<TChannel>
         : IChannelManager<TChannel>
    {

        /// <summary>
        /// Constructor taking the name of an endpoint.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of an endpoint.</param>
        public ChannelManagerBase(string endpointConfigurationName)
            : this(new ChannelCreator<TChannel>(new ChannelFactory<TChannel>(endpointConfigurationName)))
        {
     
        }

        /// <summary>
        /// Constructor taking a channelfactory.
        /// </summary>
        /// <param name="factory">The channelfactory to use in this instance.</param>
        public ChannelManagerBase(ICanCreateChannels<TChannel> factory)
        {
            ChannelCreater = factory;
        }


        /// <summary>
        /// The channelfactory.
        /// </summary>
        public virtual IChannelFactory<TChannel> ChannelFactory
        {
            get
            {
                return ChannelCreater.ChannelFactory;
            }
        }

        public virtual ICanCreateChannels<TChannel> ChannelCreater
        {
            get;
            set;
        }

        /// <summary>
        /// Get a channel on which actions can be performed.
        /// </summary>
        /// <returns>A working channel.</returns>
        public abstract TChannel FetchChannelToWorkWith();

        /// <summary>
        /// Perform any cleanup work needed when an action has been performed on a channel.
        /// </summary>
        /// <param name="channel">The channel that was used.</param>
        public abstract void FinishedWorkWithChannel(TChannel channel);

        /// <summary>
        /// Creates and opens a channel.
        /// </summary>
        /// <returns>An open channel.</returns>
        protected virtual TChannel CreateAndOpenChannel()
        {
            return OpenChannel(CreateChannel());
           
        }

        /// <summary>
        /// Creates a channel
        /// </summary>
        /// <returns></returns>
        protected virtual TChannel CreateChannel()
        {
            TChannel channel = ChannelCreater.CreateChannel();
            ICommunicationObject comm = channel as ICommunicationObject;
            return channel;
        }

        /// <summary>
        /// Open the channel.
        /// </summary>
        /// <param name="channel">The channel to open.</param>
        /// <returns>An opened channel.</returns>
        protected virtual TChannel OpenChannel(TChannel channel)
        {
            ICommunicationObject communicationChannel = channel as ICommunicationObject;
            if (communicationChannel != null && communicationChannel.State == CommunicationState.Created)
            {
                communicationChannel.Open();
            }
            return channel;
        }

        protected virtual void CloseChannel(TChannel channel)
        {
            ICommunicationObject communicationChannel = channel as ICommunicationObject;
            if (communicationChannel != null)
            {
                communicationChannel.Close();
            }
        }
    }
}
