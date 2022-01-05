using UnityEngine;

namespace PathScrubber.Path
{
    public interface IPathingObj
    {
        public void Set(float posNormalized, float speed, Vector3 offset, bool backwards = false);
    }
}