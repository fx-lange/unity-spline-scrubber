using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Path;
using UnityEngine;

namespace CinePath
{
    public class CartPath : PathBase
    {
        [SerializeField] private CinemachineSmoothPath path;

        public CinemachineSmoothPath Path => path;
        public override float Length => path.PathLength;

        [ContextMenu("CreatePinpPongLoop")]
        private void CreatePingPongLoop()
        {
            var go = new GameObject("PingPongLoop");
            go.transform.SetParent(transform, false);
            
            var newPath = go.AddComponent<CinemachineSmoothPath>();


            var length = path.m_Waypoints.Length;
            var newWayPoints = new List<CinemachineSmoothPath.Waypoint>();
            newPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[length * 2 - 2];
            for (var index = 0; index < length; index++)
            {
                if (index > 0 && index < length - 1)
                {
                    newWayPoints.Add(path.m_Waypoints[index]);
                }
            }

            var allPoints = path.m_Waypoints.ToList();
            newWayPoints.Reverse();
            allPoints.AddRange(newWayPoints);

            for (var index = 0; index < allPoints.Count; index++)
            {
                var waypoint = allPoints[index];
                newPath.m_Waypoints[index] = waypoint;
            }

            newPath.m_Looped = true;
        }
    }
}