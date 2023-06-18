using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace SplineScrubber.Tests
{
    public class MoveMany : MonoBehaviour
    {
        [SerializeField] private SplineCart _prefab;
        [SerializeField] private SplineContainer _container;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _duration = 5;
        
        [SerializeField] private int _count;
        private List<SplineCart> _carts = new();

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            var t = Time.time % _duration;
            Run(t/_duration);
        }

        private void Init()
        {
            _carts.Clear();
            
            for (int i = 0; i < _count; ++i)
            {
                var cart = Instantiate(_prefab, transform);
                cart.SetContainer(_container);
                _carts.Add(cart);
            }
        }
        
        private void Run(float t)
        {
            for (int i = 0; i < _count; ++i)
            {
                _carts[i].Set(t, _speed, Vector3.zero);
            }
        }
    }
}