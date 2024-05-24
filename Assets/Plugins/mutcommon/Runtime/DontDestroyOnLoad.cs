using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MutCommon
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        [SerializeField] bool singleInstance;
        private static List<string> instances = new List<string>();

        private void Awake()
        {
            if (singleInstance)
            {
                if (instances.Any(i => i == name))
                {
                    Destroy(gameObject);
                    return;
                }
                instances.Add(name);
            }
            DontDestroyOnLoad(this);
        }
    }
}