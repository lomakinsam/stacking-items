using System;
using UnityEngine;
using System.Collections.Generic;

namespace Stacking
{
    public class StackController : MonoBehaviour
    {
        public Transform[] stackItems;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float bendingForce;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float itemsSpacing;

        [SerializeField]
        private AnimationCurve bendPattern;

        [SerializeField]
        private float maxVelocity = 2.0f;

        private Vector3 velocity;

        [SerializeField]
        private bool visualizeBendingLimits;

        [SerializeField]
        private bool modifyBendingSpeed;

        [SerializeField]
        [Range(1.0f, 25.0f)]
        private float bendingSpeed;

        [SerializeField]
        [Range(1.0f, 25.0f)]
        private float stabilizationSpeed;

        [SerializeField]
        private bool shakeEnabled;

        [Range(0.1f, 1.0f)]
        [SerializeField]
        private float shakePower = 0.15f;

        [SerializeField]
        private AnimationCurve shakeDistribution;

        [SerializeField]
        private bool sidesBendingEnabled;

        [SerializeField]
        private float angularVelocity;

        [SerializeField]
        private float maxAngularVelocity = 360;

        private List<StackItem> _stackItems;

        private float stackHeight;
        private float velocityMgn;

        private Vector3 prevPosition;
        private Vector3 forwardDir;

        private float maxOffset => stackHeight * bendingForce;

        private void Awake() => Init();

        private void Update()
        {
            UpdateVelocity();
            ApplyVelocity();
            ApplyRotation();
        }

        private void Init()
        {
            velocityMgn = 0.0f;
            angularVelocity = 0.0f;
            velocity = Vector3.zero;
            prevPosition = transform.position;
            forwardDir = transform.forward;
            
            InitStackItems();
        }

        private void InitStackItems()
        {
            stackHeight = 0.0f;

            _stackItems = new List<StackItem>();

            Vector3 defaultPosition = transform.position;

            for (int i = 0; i < stackItems.Length; i++)
            {
                Vector3 stackBottom = new (defaultPosition.x, defaultPosition.y + stackHeight, defaultPosition.z);
                Vector3 SOD_params = new (1.5f, 0.1f, 0.0f);

                var item = new StackItem(stackItems[i], stackBottom, SOD_params);
                _stackItems.Add(item);

                stackHeight += item.Height + itemsSpacing;
            }
        }

        private void UpdateVelocity()
        {
            velocity = (transform.position - prevPosition) / Time.deltaTime;
            prevPosition = transform.position;
        }

        private void ApplyVelocity()
        {
            float height = 0.0f;
            float lerpStepX;
            float lerpStepZ;

            if (modifyBendingSpeed)
                velocityMgn = GetCustomVelocityMagnitude();
            else
                velocityMgn = velocity.magnitude;

            if (sidesBendingEnabled)
            {
                angularVelocity = Vector3.Angle(transform.forward, forwardDir) / Time.deltaTime;
                angularVelocity = transform.InverseTransformDirection(forwardDir).x > 0 ? angularVelocity : -angularVelocity;
                forwardDir = transform.forward;
                lerpStepX = Mathf.InverseLerp(-maxAngularVelocity, maxAngularVelocity, angularVelocity);
            }
            else
                lerpStepX = 0.5f;

            lerpStepZ = Mathf.InverseLerp(0, maxVelocity, velocityMgn);

            if (shakeEnabled)
            {
                float shake = Mathf.PingPong(Time.time, shakePower) - shakePower / 2;
                shake *= shakeDistribution.Evaluate(lerpStepZ);
                lerpStepZ += shake;
            }

            for (int i = 0; i < _stackItems.Count; i++)
            {
                height += _stackItems[i].HalfHeight;
                
                float heightNormilized = height / stackHeight;
                float offsetLimit = bendPattern.Evaluate(heightNormilized) * maxOffset;

                float offsetZ = Mathf.Lerp(0, -offsetLimit, lerpStepZ);
                float offsetX = Mathf.Lerp(-offsetLimit, offsetLimit, lerpStepX);

                Vector3 targetPosition = new Vector3(offsetX, height, offsetZ);
                _stackItems[i].UpdatePosition(Time.deltaTime, targetPosition);

                height += _stackItems[i].HalfHeight + itemsSpacing;
            }
        }

        private float GetCustomVelocityMagnitude()
        {
            float targetVelocityMgn = velocity.magnitude;

            if (velocityMgn < targetVelocityMgn)
                return Mathf.MoveTowards(velocityMgn, targetVelocityMgn, bendingSpeed * Time.deltaTime);
            else if (velocityMgn > targetVelocityMgn)
                return Mathf.MoveTowards(velocityMgn, targetVelocityMgn, stabilizationSpeed * Time.deltaTime);
            else
                return targetVelocityMgn;
        }

        private void ApplyRotation()
        {
            if (_stackItems.Count == 0)
                return;

            Vector3 lookAtPoint;

            for (int i = 0; i < _stackItems.Count; i++)
            {
                if (i == 0)
                    lookAtPoint = transform.TransformPoint(Vector3.zero);
                else
                    lookAtPoint = _stackItems[i - 1].LookAtPoint;

                _stackItems[i].LookAt(lookAtPoint, transform.right);
            }
        }

        private void OnDrawGizmos()
        {
            if (!visualizeBendingLimits || _stackItems == null || _stackItems.Count == 0)
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

            foreach (var item in _stackItems)
            {
                height += item.HalfHeight;

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

                height += item.HalfHeight + itemsSpacing;
            }
        }
    }
}