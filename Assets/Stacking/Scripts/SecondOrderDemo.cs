using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

public class SecondOrderDemo : MonoBehaviour
{
    [Range(0, 10)]
    public float F;

    [Range(0, 2)]
    public float Z;

    [Range(-1, 1)]
    public float R;
}

[CustomEditor(typeof(SecondOrderDemo))]
public class TestOnInspector : Editor
{
    private Material mat;

    private const float defaultLenght = 2.0f;
    private const float defaultValue = 1.0f;

    private const int drawSteps = 300;

    private const float paddingLeft = 10f;
    private const float paddingRight = 2f;
    private const float paddingTop = 15f;
    private const float paddingBottom = 15f;

    private float f, f0, z, z0, r, r0;

    private SecondOrderDynamics func;
   
    private Vector2[] data;

    private void OnEnable()
    {
        var shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);

        InitFunction();
    }

    private void OnDisable()
    {
        DestroyImmediate(mat);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UpdateInput();

        Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            GL.PushMatrix();

            GL.Clear(true, false, Color.black);
            mat.SetPass(0);

            // draw base graph
            GL.Begin(GL.LINES);
            GL.Color(new Color(1, 1, 1, 1));
            // draw Y axis
            GL.Vertex3(paddingLeft, paddingTop, 0);
            GL.Vertex3(paddingLeft, rect.height - paddingBottom, 0);
            // draw X axis
            GL.Vertex3(paddingLeft, rect.height - paddingBottom, 0);
            GL.Vertex3(rect.width - paddingRight, rect.height - paddingBottom, 0);
            // draw default values
            GL.Color(Color.green);
            //GL.Vertex3(paddingLeft, paddingTop, 0);
            //GL.Vertex3(rect.width - paddingRight, paddingTop, 0);
            GL.End();

            // init func values
            if (data == null)
            {
                Debug.Log("Init");
                data = new Vector2[drawSteps];

                float rectWidth = rect.width - paddingLeft - paddingRight;
                float rectHeight = rect.height - paddingTop - paddingBottom;

                UpdateData(rectWidth, rectHeight);
            }

            // validate input changes
            if (f != f0 || z != z0 || r != r0)
            {
                Debug.Log("Value changed");
                InitFunction();

                float rectWidth = rect.width - paddingLeft - paddingRight;
                float rectHeight = rect.height - paddingTop - paddingBottom;

                UpdateData(rectWidth, rectHeight);
            }

            // draw graph
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.cyan);
            for (int i = 0; i < data.Length; i++)
                GL.Vertex3(data[i].x + paddingLeft, rect.height - (data[i].y + paddingTop), 0.0f);
            GL.End();

            GL.PopMatrix();
            GUI.EndClip();
        }

        // draw values
        rect = GUILayoutUtility.GetLastRect();
        float squareSize = 10;
        EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, rect.y + paddingTop - squareSize/2, squareSize, squareSize), "1"); // heigt "1" mark
        EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, rect.y + rect.height + (squareSize * 0.2f) - paddingBottom, squareSize, squareSize), "0"); // height "0" mark
        EditorGUI.LabelField(new Rect(rect.x + rect.width - paddingRight - squareSize, rect.y + rect.height + (squareSize * 0.2f) - paddingBottom, squareSize, squareSize), "2"); // max lenght mark
    }

    private void InitFunction()
    {
        f0 = f = ((SecondOrderDemo)target).F;
        z0 = z = ((SecondOrderDemo)target).Z;
        r0 = r = ((SecondOrderDemo)target).R;

        func = new SecondOrderDynamics(f, z, r, new Vector3(-defaultLenght, 0, 0));
    }

    private void UpdateData(float graphWidth, float graphHeight)
    {
        float x_min = 0, x_max = 0, y_min = 0, y_max = 0;

        for (int i = 0; i < drawSteps; i++)
        {
            float T = 0.0069f;
            float x = math.remap(0, drawSteps - 1, -defaultLenght, defaultLenght, i);
            float y = x > 0 ? defaultValue : 0;

            Vector3? funcValues = func.Update(T, new Vector3(x, y, 0));

            if (i == 0)
            {
                x_min = x_max = funcValues.Value.x;
                y_min = y_max = funcValues.Value.y;
            }
            else
            {
                x_max = funcValues.Value.x > x_max ? funcValues.Value.x : x_max;
                x_min = funcValues.Value.x < x_min ? funcValues.Value.x : x_min;

                y_max = funcValues.Value.y > y_max ? funcValues.Value.y : y_max;
                y_min = funcValues.Value.y < y_min ? funcValues.Value.y : y_min;
            }


            data[i] = new Vector2(funcValues.Value.x, funcValues.Value.y);
        }

        for (int i = 0; i < drawSteps; i++)
        {
            float x = math.remap(x_min, x_max, 0, graphWidth, data[i].x);
            float y = math.remap(y_min, y_max, 0, graphHeight, data[i].y);

            data[i] = new Vector2(x, y);
        }

        Debug.Log($"X limits: [{x_min}; {x_max}]\nY limits [{y_min}; {y_max}]");
    }

    private void UpdateInput()
    {
        f = ((SecondOrderDemo)target).F;
        z = ((SecondOrderDemo)target).Z;
        r = ((SecondOrderDemo)target).R;
    }
}
