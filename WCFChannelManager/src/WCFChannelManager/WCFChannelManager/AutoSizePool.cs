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

namespace WCFChannelManager
{
    /// <summary>
    /// A pool that automatically grows if more instances are requested than there are available in the queue.
    /// </summary>
    public class AutoSizePool
        : IObjectPool
    {
        protected List<object> activeObjects;
        protected List<object> passiveObjects;
        protected bool closed;
        private object sync;

        public AutoSizePool(IPoolableObjectFactory factory)
        {
            activeObjects = new List<object>();
            passiveObjects = new List<object>();
            sync = new object();
            PoolableObjectFactory = factory;
        }

        #region IObjectPool Members

        public void AddObject()
        {
            lock (sync)
            {
                passiveObjects.Add(PoolableObjectFactory.MakeObject());
            }
        }

        public object BorrowObject()
        {
            lock (sync)
            {
                if (closed)
                {
                    throw new InvalidOperationException("The pool is closed.");
                }
                object useable = null;
                IEnumerator<object> objects = passiveObjects.GetEnumerator();
                List<object> destroyedObjects = new List<object>();
                while (objects.MoveNext() && useable == null)
                {
                    PoolableObjectFactory.ActivateObject(objects.Current);
                    if (PoolableObjectFactory.ValidateObject(objects.Current))
                    {
                        useable = objects.Current;
                    }
                    else
                    {
                        PoolableObjectFactory.DestroyObject(objects.Current);
                        destroyedObjects.Add(objects.Current);
                    }
                }
                passiveObjects.RemoveAll(o => destroyedObjects.Contains(o));
                if (useable == null)
                {
                    useable = PoolableObjectFactory.MakeObject();
                    PoolableObjectFactory.ActivateObject(useable);
                }
                activeObjects.Add(useable);
                return useable;
            }
        }

        public void Clear()
        {
            lock (sync)
            {
                RemoveAllPassiveObjects();
            }
        }

        public void Close()
        {
            lock (sync)
            {
                closed = true;
                RemoveAllPassiveObjects();
                RemoveAllActiveObjects();
            }
        }

        public int NumActive
        {
            get
            {
                lock (sync)
                {
                    return activeObjects.Count;
                }
            }
        }

        public int NumIdle
        {
            get
            {
                lock (sync)
                {
                    return passiveObjects.Count;
                }
            }
        }

        public IPoolableObjectFactory PoolableObjectFactory
        {
            get;
            set;
        }

        public void ReturnObject(object target)
        {
            lock (sync)
            {
                if (activeObjects.Contains(target))
                {
                    activeObjects.Remove(target);
                    PoolableObjectFactory.PassivateObject(target);
                    passiveObjects.Add(target);

                }
            }
        }

        #endregion

        private void RemoveAllPassiveObjects()
        {
            ClearList(passiveObjects);
        }

        private void RemoveAllActiveObjects()
        {
            ClearList(activeObjects);
        }

        private void ClearList(List<object> list)
        {
            foreach (object o in list)
            {
                PoolableObjectFactory.DestroyObject(o);
            }
            list.Clear();
        }
    }
}
