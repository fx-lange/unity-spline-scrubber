using UnityEngine;

namespace PathScrubber.Path
{
    public abstract class PathBase : MonoBehaviour, IPath
    {
        public abstract float Length { get; }
    }
}