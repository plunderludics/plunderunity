using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MutCommon
{
    public class TransformChildrenEnabler : MonoBehaviour
    {
        private void EnableChildrenCallback(Func<Transform, bool> test)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(test(t));
            }
        }

        public void EnableChildrenByName(string s) =>
            EnableChildrenCallback(t => t.name == s);

        public void EnableChildrenByNameLowerInvariant(string s) =>
            EnableChildrenCallback(t => t.name.ToLowerInvariant() == s.ToLowerInvariant());

        public void EnableIndexAlone(int i) =>
            EnableChildrenCallback(t => t.GetSiblingIndex() == i);

        public void EnableIndexAloneRotate(int i) =>
            EnableChildrenCallback(t => t.GetSiblingIndex() == i % t.parent.childCount);
    }
}