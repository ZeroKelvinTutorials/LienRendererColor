using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Polygon : MonoBehaviour
{
    public int sides;
    public float radius;
    public bool looped;
    public bool isTwo;
    public int extraSteps = 2;

    // Update is called once per frame
    void Update()
    {
        if (looped)
        {
            DrawLoopedPolygon(sides, radius);
        }
        else
        {
            DrawClosedPolygon();
        }
    }

    void DrawLoopedPolygon(int sides, float radius)
    {
        //we need int points to be 2 more than sides.
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = sides;
        lineRenderer.loop = true;

        for(int currentPoint = 0; currentPoint<sides; currentPoint++)
        {
            float currentProgress = (float)currentPoint/sides;
            float currentRadian = currentProgress*2*Mathf.PI;
            float y = Mathf.Sin(currentRadian) * radius;
            float x = Mathf.Cos(currentRadian) * radius;
            Vector3 currentPosition = new Vector3(x,y,0);
            lineRenderer.SetPosition(currentPoint,currentPosition);
        }
    }
    void DrawClosedPolygon()
    {
        DrawLoopedPolygon(sides,radius);
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = false;
        lineRenderer.positionCount += extraSteps;

        for(int i=0; i<extraSteps; i++){
            lineRenderer.SetPosition(lineRenderer.positionCount-extraSteps+i, lineRenderer.GetPosition(i));
        }
    }
}
