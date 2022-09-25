using System.Collections.Generic;
using UnityEngine;

public class EvaluationData
{
    private List<Vector2> points;
    private Vector2 x_limits;
    private Vector2 y_limits;

    public int Length => points.Count;
    public float X_min => x_limits.x;
    public float X_max => x_limits.y;
    public float Y_min => y_limits.x;
    public float Y_max => y_limits.y;

    public bool IsEmpty => points.Count <= 0;

    public EvaluationData()
    {
        points = new List<Vector2>();
        x_limits = new Vector2(float.NaN, float.NaN);
        y_limits = new Vector2(float.NaN, float.NaN);
    }

    public void Add(Vector2 point)
    {
        if (points.Count == 0)
        {
            x_limits = new Vector2(point.x, point.x);
            y_limits = new Vector2(point.y, point.y);
        }
        else
        {
            x_limits.x = point.x < x_limits.x ? point.x : x_limits.x;
            x_limits.y = point.x > x_limits.y ? point.x : x_limits.y;

            y_limits.x = point.y < y_limits.x ? point.y : y_limits.x;
            y_limits.y = point.y > y_limits.y ? point.y : y_limits.y;
        }

        points.Add(point);
    }

    public Vector2 GetItem(int index) => points[index];

    public void Clear()
    {
        points.Clear();
        x_limits = new Vector2(float.NaN, float.NaN);
        y_limits = new Vector2(float.NaN, float.NaN);
    }
}