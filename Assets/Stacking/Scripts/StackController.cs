using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        [Range(1.0f, 10.0f)]
        private float resetSpeed;

        [SerializeField]
        private AnimationCurve bendPattern;

        [SerializeField]
        private Transform[] stackItems;

        [SerializeField]
        private bool visualizeBendingLimits;

        private float stackHeight;
        private float[] stackItemHeights;
        private Vector2[] stackItemOffsets;

        private Vector3 prevPosition;
        private Vector2 velocity;

        private float maxOffset => stackHeight * bendingForce;

        [SerializeField]
        private TMP_Text debugText;

        private void Awake() => Init();

        private void Update()
        {
            UpdateVelocity();
            ApplyVelocity();
            ApplyRotation();
        }

        private void Init()
        {
            prevPosition = transform.position;
            velocity = Vector2.zero;
            InitStackItems();
        }

        private void InitStackItems()
        {
            stackHeight = 0.0f;
            stackItemHeights = new float[stackItems.Length];
            stackItemOffsets = new Vector2[stackItems.Length];

            Vector3 defaultPosition = transform.position;

            for (int i = 0; i < stackItems.Length; i++)
            {
                stackItemOffsets[i] = Vector2.zero;

                if (stackItems[i].parent != transform)
                    stackItems[i].SetParent(transform, true);

                stackItemHeights[i] = GetItemHeight(stackItems[i]);

                stackItems[i].position = new Vector3(defaultPosition.x,
                                                     defaultPosition.y + stackHeight + stackItemHeights[i] / 2,
                                                     defaultPosition.z);

                stackHeight += stackItemHeights[i] + itemsSpacing;
            }
        }

        private void UpdateVelocity()
        {
            float dx = transform.position.x - prevPosition.x;
            float dz = transform.position.z - prevPosition.z;
            prevPosition = transform.position;

            velocity = new Vector2(dx, dz) * Time.deltaTime * 100;

            debugText.text = $"Velocity X: {velocity.x:0.00} \n Velocity Z: {velocity.y:0.00}";
        }

        private void ApplyVelocity()
        {
            float height = 0.0f;

            for (int i = 0; i < stackItems.Length; i++)
            {
                height += stackItemHeights[i] / 2;

                float heightNormilized = height / stackHeight;
                float offsetLimit = bendPattern.Evaluate(heightNormilized) * maxOffset;


                /*float lerpStepX = Mathf.InverseLerp(-offsetLimit, offsetLimit, stackItemOffsets[i].x + velocity.x);
                float lerpStepZ = Mathf.InverseLerp(-offsetLimit, offsetLimit, stackItemOffsets[i].y + velocity.y);

                float offsetX = Mathf.Lerp(-offsetLimit, offsetLimit, lerpStepX);
                float offsetZ = Mathf.Lerp(-offsetLimit, offsetLimit, lerpStepZ);

                stackItemOffsets[i] = new Vector2(offsetX, offsetZ);

                stackItems[i].localPosition = new Vector3(0, stackItems[i].localPosition.y, 0) + new Vector3(offsetX, 0, offsetZ);*/

                float lerpStepZ = 0.0f;

                if (velocity.magnitude > 0.001f)
                    lerpStepZ = Mathf.InverseLerp(0, -offsetLimit, stackItemOffsets[i].y - velocity.magnitude);
                else
                    lerpStepZ = Mathf.InverseLerp(0, -offsetLimit, stackItemOffsets[i].y + resetSpeed * Time.deltaTime);

                float offsetZ = Mathf.Lerp(0, -offsetLimit, lerpStepZ);
                stackItemOffsets[i] = new Vector2(0, offsetZ);
                stackItems[i].localPosition = new Vector3(stackItems[i].localPosition.x, stackItems[i].localPosition.y, offsetZ);
                //Debug.Log(offsetZ);

                height += stackItemHeights[i] / 2 + itemsSpacing;
            }
        }

        private void ApplyRotation()
        {
            if (stackItems.Length < 1)
                return;

            for (int i = 0; i < stackItems.Length; i++)
            {
                if (i == 0)
                {
                    stackItems[i].LookAt(transform.position, transform.forward);
                    stackItems[i].rotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                }
                else
                {
                    stackItems[i].LookAt(stackItems[i - 1].position, transform.forward);
                    stackItems[i].rotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                }
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