using UnityEngine;

namespace PathScrubber.Path
{
    public abstract class PathingObjBase : MonoBehaviour, IPathingObj
    {
        public abstract void Set(float posNormalized, float speed, Vector3 offset, bool backwards = false); //TODO instead of bool we could also do a vector
        public abstract void SetPath(IPath newPath);
        public abstract bool Paused { get; set; }
    }
}