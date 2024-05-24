using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutCommon
{
    public class MousePositionRaycaster : Raycaster
    {
        protected override Ray Ray()
        => camera.ScreenPointToRay(Input.mousePosition);
        //new Ray(camera.transform.position, camera.transform.forward);
    }
}