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

namespace WCFChannelManager
{
    /// <summary>
    /// Interface which defines the methods channel lifecycle managers should implements. Implementors can be used
    /// by the ProxyChannel.
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    public interface IChannelManager<TChannel>
    {
        /// <summary>
        /// Creates a channel that can be used.
        /// </summary>
        /// <returns>A usable channel.</returns>
        TChannel FetchChannelToWorkWith();
        /// <summary>
        /// When work is finished with a channel this method is called to hand it back to the manager.
        /// </summary>
        /// <param name="channel">The channel which no longer is needed.</param>
        void FinishedWorkWithChannel(TChannel channel);
    }
}
