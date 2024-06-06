using Photon.Pun;
using UnityEngine;

namespace Bases
{
    public class Singleton<T>: MonoBehaviourPunCallbacks where T: Singleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (!_instance) Debug.LogError($"{typeof(T)} is Empty");
                return _instance;
            }
        }

        protected void CreateSingleton(T instance)
        {
            if (_instance==null)
            {
                _instance = instance;
                DontDestroyOnLoad(instance.gameObject);
            }
            else Destroy(gameObject);
        }
    }
}
