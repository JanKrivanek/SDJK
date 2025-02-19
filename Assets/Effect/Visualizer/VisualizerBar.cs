using SCKRM;
using System.Threading;
using UnityEngine;

namespace SDJK.Effect
{
    public class VisualizerBar : MonoBehaviour
    {
        public RectTransform rectTransform;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public float size
        {
            get
            {
                while (Interlocked.CompareExchange(ref sizeLock, 1, 0) != 0)
                    Thread.Sleep(1);

                float size = _size.Clamp(0);

                Interlocked.Decrement(ref sizeLock);

                return size;
            }
            set
            {
                while (Interlocked.CompareExchange(ref sizeLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _size = value.Clamp(0);

                Interlocked.Decrement(ref sizeLock);
            }
        }
        float _size = 0;
        int sizeLock = 0;

        public void SizeUpdate()
        {
            if (size > 0)
                size -= 5 * Kernel.fpsUnscaledSmoothDeltaTime;

            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(transform.localScale.x, size), 0.75f * Kernel.fpsUnscaledSmoothDeltaTime);

            /*float yPosOffset = transform.localScale.y * 0.5f;
            float radRotation = -transform.eulerAngles.z * Mathf.Deg2Rad;
            rectTransform.anchoredPosition = new Vector2(radRotation.Sin() * yPosOffset, radRotation.Cos() * yPosOffset);*/
        }
    }
}