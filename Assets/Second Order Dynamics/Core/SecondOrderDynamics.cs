#region Information
/*****
* 
* 26.09.2022
* 
* SecondOrderDynamics.cs implementation based on the video source provided by the author - t3ssel8r
* Link to the original YouTube video: https://www.youtube.com/watch?v=KPoeNZZ6H4s&t=554s&ab_channel=t3ssel8r
* 
*****/
#endregion Information

using UnityEngine;
using Unity.Mathematics;

namespace SODynamics
{
    public class SecondOrderDynamics
    {
        private Vector3? xp;
        private Vector3? y, yd;
        private float _w, _z, _d, k1, k2, k3;

        public SecondOrderDynamics(float f, float z, float r, Vector3 x0)
        {
            _w = 2 * math.PI * f;
            _z = z;
            _d = _w * math.sqrt(math.abs(z * z - 1));
            k1 = z / (math.PI * f);
            k2 = 1 / (_w * _w);
            k3 = r * z / _w;

            xp = x0;
            y = x0;
            yd = Vector3.zero;
        }

        public Vector3? Update(float T, Vector3 x, Vector3? xd = null)
        {
            if (xd == null)
            {
                xd = (x - xp) / T;
                xp = x;
            }

            float k1_stable, k2_stable;
            if (_w * T < _z)
            {
                k1_stable = k1;
                k2_stable = Mathf.Max(k2, T * T / 2 + T * k1 / 2, T * k1);
            }
            else
            {
                float t1 = math.exp(-_z * _w * T);
                float alpha = 2 * t1 * (_z <= 1 ? math.cos(T * _d) : math.cosh(T * _d));
                float beta = t1 * t1;
                float t2 = T / (1 + beta - alpha);
                k1_stable = (1 - beta) * t2;
                k2_stable = T * t2;
            }

            y = y + T * yd;
            yd = yd + T * (x + k3 * xd - y - k1_stable * yd) / k2_stable;
            return y;
        }
    }
}