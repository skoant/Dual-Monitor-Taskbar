using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DualMonitor.Entities
{
    class SyncProcessCache
    {
        private static object _syncCachedProc = new object();
        private Dictionary<int, SecondDisplayProcess> _cachedProcesses = new Dictionary<int,SecondDisplayProcess>();

        public void Remove(int key)
        {
            lock (_syncCachedProc)
            {
                if (_cachedProcesses.ContainsKey(key))
                    _cachedProcesses.Remove(key);
            }
        }

        public bool Contains(int key)
        {
            lock (_syncCachedProc)
            {
                return _cachedProcesses.ContainsKey(key);
            }
        }

        public SecondDisplayProcess Get(int key)
        {
            SecondDisplayProcess result;
            lock (_syncCachedProc)
            {
                if (!_cachedProcesses.TryGetValue(key, out result)) return null;
            }

            return result;
        }

        public SecondDisplayProcess Add(int key, SecondDisplayProcess value)
        {
            lock (_syncCachedProc)
            {
                SecondDisplayProcess tmp;
                if (_cachedProcesses.TryGetValue(key, out tmp)) return tmp;

                _cachedProcesses.Add(key, value);
            }
            return value;
        }

        public void ForEach(Action<SecondDisplayProcess> action)
        {
            lock (_syncCachedProc)
            {
                _cachedProcesses.Values.ToList().ForEach(action);
            }
        }

        public bool Any(Func<SecondDisplayProcess, bool> action)
        {
            lock (_syncCachedProc)
            {
                return _cachedProcesses.Values.ToList().Any(action);
            }
        }
    }
}
