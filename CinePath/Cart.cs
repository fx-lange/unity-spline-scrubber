using UnityEngine;
using Cinemachine;
using PathScrubber.Path;

namespace CinePath
{
    public class Cart : PathingObjBase
    {
        [SerializeField] private CinemachineDollyCart cart;
        [SerializeField] private Transform anchor;
        
        public float Speed { get; private set; }

        private CartPath path;

        public override bool Paused
        {
            get => cart.gameObject.activeInHierarchy;
            set => cart.gameObject.SetActive(value);
        }

        private void OnEnable()
        {
            cart.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
            cart.m_Speed = 0;
        }

        public override void Set(float posNormalized, float speed, Vector3 offset, bool backwards = false)
        {
            cart.m_Position = posNormalized;
            Speed = speed;
            
            anchor.localPosition = offset;
            if (backwards)
            {
                anchor.localRotation = Quaternion.LookRotation(Vector3.back);
            }
            else
            {
                anchor.localRotation = Quaternion.identity;
            }
        }

        public override void SetPath(IPath newPath)
        {
            if (!ReferenceEquals(path, newPath))
            {
                path = newPath as CartPath;
                cart.m_Path = path.Path;
            }
        }
    }
}