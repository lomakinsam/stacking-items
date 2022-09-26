using UnityEngine;
using UnityEditor;
using B83.Collections;

namespace B83.UnityAnswers.InspectorExamples
{
    [CustomEditor(typeof(LineDrawingComponent))]
    public class LineDrawingInspector : Editor
    {
        Material mat;
        RingBuffer<float> m_Data;
        private void OnEnable()
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            m_Data = ((LineDrawingComponent)target).m_Data;
            
        }
        private void OnDisable()
        {
            DestroyImmediate(mat);
        }

        Color GetColor(float aSample)
        {
            if (aSample < 20)
                return Color.red;
            if (aSample < 50)
                return Color.yellow;
            return Color.green;
        }
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(rect);
                GL.PushMatrix();

                GL.Clear(true, false, Color.black);
                mat.SetPass(0);

                // background
                GL.Begin(GL.QUADS);
                GL.Color(Color.black);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(rect.width, 0, 0);
                GL.Vertex3(rect.width, rect.height, 0);
                GL.Vertex3(0, rect.height, 0);
                GL.End();

                // draw grid
                GL.Begin(GL.LINES);
                int offset = (Time.frameCount * 2) % 50;
                int count = (int)(rect.width / 10) + 20;
                for (int i = 0; i < count; i++)
                {
                    float f = (i % 5 == 0) ? 0.5f : 0.2f;
                    GL.Color(new Color(f, f, f, 1));
                    float x = i * 10 - offset;
                    if (x >= 0 && x < rect.width)
                    {
                        GL.Vertex3(x, 0, 0);
                        GL.Vertex3(x, rect.height, 0);
                    }
                    if (i < rect.height / 10)
                    {
                        GL.Vertex3(0, i * 10, 0);
                        GL.Vertex3(rect.width, i * 10, 0);
                    }
                }
                GL.End();

                // draw data
                GL.Begin(GL.LINE_STRIP);
                for (int i = 0; i < m_Data.Count; i++)
                {
                    float val = m_Data[i];
                    GL.Color(GetColor(val));
                    GL.Vertex3(i * 2, rect.height-val * 2, 0);
                }
                GL.End();
                GL.PopMatrix();

                GUI.EndClip();
            }
        }
    }
}