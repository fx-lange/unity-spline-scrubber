using UnityEngine;

namespace PathScrubber.Path
{
    public interface IPathingObj
    {
        public void SetPosition(float t, Vector3 offset, bool backwards = false);
    }
}