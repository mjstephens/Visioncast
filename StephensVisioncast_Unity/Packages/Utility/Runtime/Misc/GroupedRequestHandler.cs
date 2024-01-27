using System.Collections.Generic;
using Stephens.Tick;

namespace Stephens.Utility
{
    /// <summary>
    /// A GRH can be subscribed to by listeners of type TListener - it will do some processing, then inform listeners of
    /// the results. This base class helps with grouping and then re-routing results to the correct listeners.
    /// </summary>
    public abstract class GroupedRequestHandler<TListener, TRequest> : ITickable
    {
        #region VARIABLES
        
        public abstract TickGroup TickGroup { get; }

        protected readonly List<TListener> _listeners = new List<TListener>();
        protected readonly List<TRequest> _requests = new List<TRequest>();
        private readonly List<List<int>> _requestMap = new List<List<int>>();
        private int _runningTotalRequests;

        #endregion VARIABLES


        #region SCHEDULE

        public void Schedule(TListener listener, List<TRequest> requests)
        {
            if (_listeners.Contains(listener))
                return;
            
            _listeners.Add(listener);
            _requests.AddRange(requests);
            AddRequestsToGroupMap(requests);

            OnScheduled();
        }

        private void AddRequestsToGroupMap(List<TRequest> requests)
        {
            List<int> listenerRequestIndices = new List<int>();
            for (int i = 0; i < requests.Count; i++)
            {
                listenerRequestIndices.Add(_runningTotalRequests);
                _runningTotalRequests++;
            }
            
            _requestMap.Add(listenerRequestIndices);
        }

        protected internal virtual void OnScheduled()
        {
            
        }

        #endregion SCHEDULE


        #region EXECUTE

        void ITickable.Tick(float delta)
        {
            if (_requests.Count > 0)
            {
                ExecuteScheduledRequests();
            }
            else
            {
                ClearBaseCaches();
            }
        }
        
        protected internal abstract void ExecuteScheduledRequests();

        #endregion EXECUTE

        
        #region UTILITY

        protected int GetListenerIndexForRequest(int requestIndex)
        {
            int listenerIndex = 0;
            for (int i = 0; i < _requestMap.Count; i++)
            {
                listenerIndex = i;
                if (_requestMap[i].Contains(requestIndex))
                {
                    break;
                }
            }
            
            return listenerIndex;
        }

        #endregion UTILITY


        #region CLEANUP

        protected void ClearBaseCaches()
        {
            _runningTotalRequests = 0;
            _listeners.Clear();
            _requests.Clear();
            _requestMap.Clear();
        }

        #endregion CLEANUP
    }
}