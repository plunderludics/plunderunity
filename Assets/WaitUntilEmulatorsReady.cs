using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaitUntilEmulatorsReady : MonoBehaviour
{
    public List<UnityHawk.Emulator> emulators;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (emulators.All(e => e.Texture != null)) {
            gameObject.SetActive(false);
        }
    }
}
