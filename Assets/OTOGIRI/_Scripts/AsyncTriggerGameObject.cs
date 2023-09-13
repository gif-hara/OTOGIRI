using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace OTOGIRI
{
    public class AsyncTriggerGameObject : MonoBehaviour
    {
        private static AsyncTriggerGameObject instance;

        public static AsyncUpdateTrigger GetAsyncUpdateTrigger()
        {
            InitializeIfNeed();
            return instance.GetAsyncUpdateTrigger();
        }
        
        private static void InitializeIfNeed()
        {
            if (instance != null)
            {
                return;
            }

            var go = new GameObject(nameof(AsyncTriggerGameObject));
            instance = go.AddComponent<AsyncTriggerGameObject>();
            DontDestroyOnLoad(go);
        }
    }
}