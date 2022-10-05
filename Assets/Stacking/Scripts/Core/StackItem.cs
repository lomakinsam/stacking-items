using UnityEngine;
using SODynamics;

namespace Stacking
{
    public class StackItem
    {
        private readonly Transform transform;

        private SecondOrderDynamics func;

        public float Height { get; private set; }
        public float HalfHeight { get; private set; }

        public float Width { get; private set; }
        public float HalfWidth { get; private set; }

        public Vector3 LookAtPoint => transform.position + transform.TransformDirection(0.0f, HalfHeight, 0.0f);

        public StackItem(Transform transform, Vector3 stackBottom, Vector3 SOD_params, Transform parent)
        {
            this.transform = transform;

            if (transform.parent != parent)
                transform.SetParent(parent, true);

            Renderer renderer = transform.GetComponent<Renderer>();
            Height = renderer == null ? 0 : renderer.localBounds.size.y * transform.localScale.y;
            HalfHeight = Height / 2;

            Width = renderer == null ? 0 : renderer.localBounds.size.x * transform.localScale.x;
            HalfWidth = Width / 2;

            transform.position = new Vector3(stackBottom.x, stackBottom.y + HalfHeight, stackBottom.z);

            func = new SecondOrderDynamics(SOD_params.x, SOD_params.y, SOD_params.z, transform.localPosition);
        }

        public void UpdatePosition(float T, Vector3 x, Vector3? xd = null)
        {
            Vector3? funcValues = func.Update(T, x, xd);
            transform.localPosition = new Vector3(funcValues.Value.x, funcValues.Value.y, funcValues.Value.z);
        }

        public void LookAt(Vector3 target, Vector3 worldRight)
        {
            //direction object's local down should face
            Vector3 downDir = (target - transform.position).normalized;
            // direction object's local forward should face
            Vector3 forwardDir = Vector3.Cross(downDir, worldRight);

            transform.rotation = Quaternion.LookRotation(forwardDir, -downDir);
        }

        public bool IsRelatedGameObject(GameObject gameObject)
        {
            return ReferenceEquals(transform.gameObject, gameObject);
        }

        public void UpdateSODParams(float f, float z, float r)
        {
            func = new SecondOrderDynamics(f, z, r, transform.localPosition);
        }

        public void DetachParent() => transform.parent = null;
    }
}