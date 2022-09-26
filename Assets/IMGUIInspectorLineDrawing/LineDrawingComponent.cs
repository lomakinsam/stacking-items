using UnityEngine;
using B83.Collections;

namespace B83.UnityAnswers.InspectorExamples
{
    public class LineDrawingComponent : MonoBehaviour
    {
        public RingBuffer<float> m_Data = new RingBuffer<float>(300);

        void Start()
        {
            for (int i = 0; i < 300; i++)
            {
                m_Data.Add(10);
            }
        }

        void Update()
        {
            m_Data.Add(1f / Time.deltaTime);
        }
    }
}