﻿//Copyright 2009 Benny Michielsen

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

namespace WCFChannelManager
{
    /// <summary>
    /// PoolFactories can create object pools.
    /// </summary>
    public interface IPoolFactory
    {
        /// <summary>
        /// Create an ObjectPool.
        /// </summary>
        /// <param name="poolableObjectFactory">The objectFactory to be used by the pool.</param>
        /// <returns>An objectpool.</returns>
        IObjectPool CreatePool(IPoolableObjectFactory poolableObjectFactory);
    }
}
