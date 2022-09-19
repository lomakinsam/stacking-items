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

        [SerializeField]
        private bool visualizeBendingLimits;

        private float stackHeight;

        private float[] stackItemHeights;

        private void Awake() => Init();

        private void Init()
        {
            stackHeight = 0.0f;
            InitStackItems();
        }

        private void InitStackItems()
        {
            Vector3 defaultPosition = transform.position;

            stackItemHeights = new float[stackItems.Length];

            for (int i = 0; i < stackItems.Length; i++)
            {
                if (stackItems[i].parent != transform)
                    stackItems[i].SetParent(transform, true);

                stackItemHeights[i] = GetItemHeight(stackItems[i]);

                stackItems[i].position = new Vector3(defaultPosition.x,
                                                     defaultPosition.y + stackHeight + stackItemHeights[i] / 2,
                                                     defaultPosition.z);

                stackHeight += stackItemHeights[i] + itemsSpacing;
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

        private void OnDrawGizmos()
        {
            if (!visualizeBendingLimits || stackItems.Length < 1 || stackItemHeights == null || stackItemHeights.Length < 1)
                return;

            DrawBendingLimits();
        }

        private void DrawBendingLimits()
        {
            float maxOffset = stackHeight * bendingForce;
            float offset = 0.0f;
            float offsetNormalized = 0.0f;
            
            float height = 0.0f;
            float heightNormalized = 0.0f;

            Vector3 minPoint = transform.position;
            Vector3 maxPoint = transform.position;

            for (int i = 0; i < stackItems.Length; i++)
            {
                height += stackItemHeights[i] / 2;

                minPoint.y = transform.position.y + height;
                maxPoint.y = transform.position.y + height;

                heightNormalized = height / stackHeight;
                offsetNormalized = bendPattern.Evaluate(heightNormalized);

                offset = offsetNormalized * maxOffset;

                minPoint.x -= offset;
                maxPoint.x += offset;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(minPoint, maxPoint);

                minPoint.x = maxPoint.x = transform.position.x;

                minPoint.z -= offset;
                maxPoint.z += offset;

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(minPoint, maxPoint);

                minPoint.z = maxPoint.z = transform.position.z;

                height += stackItemHeights[i] / 2 + itemsSpacing;
            }
        }
    }
}