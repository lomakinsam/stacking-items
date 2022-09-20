using UnityEngine;

namespace Stacking
{
    public class StackItem
    {
        public Transform transform { get; private set; }
        public float height { get; private set; }
        public float halfHeight { get; private set; }

        public StackItem(Transform transform)
        {
            this.transform = transform;

            Renderer renderer = transform.GetComponent<Renderer>();

            height = renderer == null ? 0 : renderer.localBounds.size.y * transform.localScale.y;
            halfHeight = height / 2;
        }
    }
}