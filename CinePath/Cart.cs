using UnityEngine;
using Cinemachine;
using PathScrubber.Path;

namespace CinePath
{
    public class Cart : PathingObjBase
    {
        [SerializeField] private CinemachineDollyCart cart;
        [SerializeField] private Transform anchor;

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

        public override void SetPosition(float t, Vector3 offset, bool backwards = false)
        {
            cart.m_Position = t;
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