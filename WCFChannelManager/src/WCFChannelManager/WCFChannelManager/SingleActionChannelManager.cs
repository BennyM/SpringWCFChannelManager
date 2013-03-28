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

namespace WCFChannelManager
{
    /// <summary>
    /// Creates a channel for one time use.
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    public class SingleActionChannelManager<TChannel>
        : ChannelManagerBase<TChannel>
    {

        /// <summary>
        /// Constructor which takes the name of a valid endpoint.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint.</param>
        public SingleActionChannelManager(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {

        }

        /// <summary>
        /// Constructor which takes a channelfactory which can be used by this instance.
        /// </summary>
        /// <param name="factory"></param>
        public SingleActionChannelManager(ICanCreateChannels<TChannel> factory)
            : base(factory)
        { 
     
        }

        /// <summary>
        /// Creates a channel that can be used.
        /// </summary>
        /// <returns>A usable channel.</returns>
        public override TChannel FetchChannelToWorkWith()
        {
            return CreateAndOpenChannel();
        }

        /// <summary>
        /// Closes the channel being passed.
        /// </summary>
        /// <param name="channel">The channel to close.</param>
        public override void FinishedWorkWithChannel(TChannel channel)
        {
            CloseChannel(channel);
        }

    }

    
}
