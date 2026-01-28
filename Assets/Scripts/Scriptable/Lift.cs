using System;
using System.Collections.Generic;
using TreeEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class Lift : MonoBehaviour
{
    public String liftName;

    // Models that will make up the lift
    public GameObject pole;
    public GameObject chair;
    public GameObject loader;

    // Adjustments for the chair spline
    public float lineHeight;
    public float lineSide;

    // How many chairs across the entire wire
    // Will be automatically spaced evenly
    public float numChairs;
    // How long for a chair to make a full revolution on the spline 
    // This will be multiplied by the length for consistency among all lifts
    public float duration;

    // Location of the entrance
    public Vector3 start;
    // Location of the exit
    public Vector3 end;

    // Pole locations in ORDER from bottom to top
    public List<Vector3> poleLocations;

    public void Start()
    {
        BuildLift();
    }

    // Pole locations
    void OnDrawGizmosSelected()
    {
        foreach (Vector3 location in poleLocations)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(location, Vector3.one);
        }
    }

    public void BuildLift()
    {
        // Builds entrance and exit
        GameObject startStructure = Instantiate(loader, start, Quaternion.identity);
        GameObject endStructure = Instantiate(loader, end, Quaternion.identity);

        // Builds spline and places poles
        SplineContainer splineContainer = startStructure.AddComponent<SplineContainer>();
        splineContainer[0].Closed = true;
        splineContainer[0].Add(new BezierKnot(new Vector3(0,0,0) + new Vector3(0, 0, lineSide) / loader.transform.localScale.x));

        foreach (Vector3 location in poleLocations)
        {
            Debug.Log(location);
            Instantiate(pole, location, pole.transform.rotation);
            splineContainer[0].Add(new BezierKnot((location - start + new Vector3(0, lineHeight, lineSide)) / loader.transform.localScale.x));
        }

        splineContainer[0].Add(new BezierKnot((end - start + new Vector3(0, 0, lineSide)) / loader.transform.localScale.x));
        splineContainer[0].Add(new BezierKnot((end - start + new Vector3(0, 0, -lineSide)) / loader.transform.localScale.x));

        poleLocations.Reverse();

        foreach (Vector3 location in poleLocations)
        {
            splineContainer[0].Add(new BezierKnot((location - start + new Vector3(0, lineHeight, -lineSide)) / loader.transform.localScale.x));
        }

        splineContainer[0].Add(new BezierKnot(new Vector3(0,0,0) + new Vector3(0, 0, -lineSide) / loader.transform.localScale.x));

        // Generates animated chairs along the spline
        for (int i = 0; i < numChairs; i++)
        {
            float offset = (1 / numChairs) * i;
            
            GameObject ingameChair = Instantiate(chair, start, Quaternion.identity);
            SplineAnimate animator = ingameChair.GetComponent<SplineAnimate>();

            animator.Container = splineContainer;
            animator.Duration = splineContainer[0].GetLength() * duration;
            animator.StartOffset = offset;
            animator.Play();
        }
        
    }   
}
