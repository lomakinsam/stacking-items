using UnityEngine;

namespace SODynamics.Demo
{
    public class SecondOrderDemo : MonoBehaviour
    {
        [Range(0, 10)]
        public float f;

        [Range(0, 2)]
        public float z;

        [Range(-10, 10)]
        public float r;

        public Transform target;

        private float f0, z0, r0;

        private SecondOrderDynamics func;

        private void Awake() => InitFunction();

        private void Update()
        {
            if (target == null)
                return;

            if (f != f0 || r != r0 || z != z0)
                InitFunction();
            else
            {
                Vector3? funcOutput = func.Update(Time.deltaTime, target.position);

                if (funcOutput != null)
                    transform.position = new Vector3(funcOutput.Value.x, funcOutput.Value.y, funcOutput.Value.z);
            }
        }

        private void InitFunction()
        {
            f0 = f;
            z0 = z;
            r0 = r;

            func = new SecondOrderDynamics(f, z, r, transform.position);
        }
    }
}