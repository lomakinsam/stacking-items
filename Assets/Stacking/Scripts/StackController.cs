using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stacking
{
    public class StackController : MonoBehaviour
    {
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float bendingForce;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float itemsSpacing;

        [SerializeField]
        private AnimationCurve bendPattern;

        [SerializeField]
        private Transform[] stackItems;

        private float stackHeigt;

        private void Awake() => Init();

        private void Init()
        {
            stackHeigt = 0.0f;
            InitStackItems();
        }

        private void InitStackItems()
        {
            Vector3 defaultPosition = transform.position;

            for (int i = 0; i < stackItems.Length; i++)
            {
                if (stackItems[i].parent != transform)
                    stackItems[i].SetParent(transform, true);

                float itemHeight = GetItemHeight(stackItems[i]);

                stackItems[i].position = new Vector3(defaultPosition.x,
                                                     defaultPosition.y + stackHeigt + itemHeight/2,
                                                     defaultPosition.z);

                stackHeigt += itemHeight + itemsSpacing;
            }
        }
        
        private float GetItemHeight(Transform item)
        {
            Renderer itemRenderer = item.GetComponent<Renderer>();

            if (itemRenderer == null)
                return 0;
            else
                return itemRenderer.localBounds.size.y * item.localScale.y;
        }

    }
}