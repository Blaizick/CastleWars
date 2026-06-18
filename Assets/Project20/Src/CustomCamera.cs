using System;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

namespace Proj21
{
    public class CustomCamera : MonoBehaviour
    {
        public CinemachineCamera cinemachineCamera;
        [NonSerialized] public float targetZoom;
        public float zoomSpeed;

        public void Init()
        {
            targetZoom = cinemachineCamera.Lens.OrthographicSize;
        }

        public void Update()
        {
            cinemachineCamera.Lens.OrthographicSize = Mathf.MoveTowards(cinemachineCamera.Lens.OrthographicSize, targetZoom, zoomSpeed * Time.deltaTime);

            if (TryGetComponent(out CinemachineConfiner2D c))
            {
                c.InvalidateLensCache();
            }
        }

        public float Zoom
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return targetZoom;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                targetZoom = value;
                cinemachineCamera.Lens.OrthographicSize = value;                
            }
        }
    }
}