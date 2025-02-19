using SCKRM.Threads;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SCKRM.Renderer
{
    [WikiDescription("렌더러를 관리하는 클래스 입니다")]
    public static class RendererManager
    {
        [WikiDescription("새로고침 가능한 모든 오브젝트 새로고침")]
        public static void AllRefresh(bool thread = true)
        {
#if UNITY_2023_1_OR_NEWER
            Refresh(UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IRefreshable>().ToArray(), thread);
#else
            Refresh(UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IRefreshable>().ToArray(), thread);
#endif
        }

        [WikiDescription("모든 렌더러 새로고침")]
        public static void AllRerender(bool thread = true)
        {
#if UNITY_2023_1_OR_NEWER
            Refresh(UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IRendererRefreshable>().ToArray(), thread);
#else
            Refresh(UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IRendererRefreshable>().ToArray(), thread);
#endif
        }

        [WikiDescription("모든 텍스트 렌더러 새로고침")]
        public static void AllTextRerender(bool thread = true)
        {
#if UNITY_2023_1_OR_NEWER
            Refresh(UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ITextRefreshable>().ToArray(), thread);
#else
            Refresh(UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<ITextRefreshable>().ToArray(), thread);
#endif
        }

        static ThreadMetaData rerenderThread;
        [WikiDescription("새로고침 가능한 특정 오브젝트들을 새로고침")]
        public static void Refresh(IRefreshable[] refreshableObjects, bool thread = true)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();

            if (thread)
            {
                if (rerenderThread != null)
                    rerenderThread.Remove();

                ThreadMetaData threadMetaData = new ThreadMetaData((ThreadMetaData threadMetaData) => Rerender(refreshableObjects, threadMetaData), "sc-krm:notice.running_task.rerender.name");
                rerenderThread = threadMetaData;
            }
            else
            {
                for (int i = 0; i < refreshableObjects.Length; i++)
                    refreshableObjects[i].Refresh();
            }
        }

        static void Rerender(IRefreshable[] refreshableObjects, ThreadMetaData threadMetaData)
        {
            int stopLoop = 0;

            threadMetaData.maxProgress = refreshableObjects.Length - 1;

            threadMetaData.cancelEvent += CancelEvent;
            threadMetaData.cantCancel = false;

            for (int i = 0; i < refreshableObjects.Length; i++)
            {
                if (Interlocked.Add(ref stopLoop, 0) > 0)
                    return;

                refreshableObjects[i].Refresh();
                threadMetaData.progress = i;
            }

            void CancelEvent()
            {
                Interlocked.Increment(ref stopLoop);

                threadMetaData.maxProgress = 1;
                threadMetaData.progress = 1;
            }
        }
    }
}