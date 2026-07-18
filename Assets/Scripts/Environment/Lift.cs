using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class Lift : MonoBehaviour
{
    public String liftName;

    [Header("Lift Models")]
    // Models that will make up the lift
    public GameObject pole;
    public GameObject chair;
    public GameObject loader;
    public GameObject unloader;
    public GameObject empty;

    [Header("Chair Rendering Adjustments")]
    // Adjustments for the chair spline
    public float lineHeight;
    public float lineSide;
    public float loaderLen;
    public float unloaderLen;

    // How many chairs across the entire wire
    // Will be automatically spaced evenly
    public float numChairs;
    // How long for a chair to make a full revolution on the spline 
    // This will be multiplied by the length for consistency among all lifts
    public float duration;

    [Header("Lift Component Locations")]
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
        GameObject startLoader = Instantiate(loader, start, Quaternion.identity);
        Instantiate(unloader, end, Quaternion.identity);
        LiftInteract interact = startLoader.GetComponentInChildren<LiftInteract>();
        if (!interact)
        {
            Debug.LogError($"LiftInteract could not be found on {liftName}.");
        } else
        {
            interact.SetTPLocation(end + new Vector3(0, 2, 2));
            Debug.Log($"TP Location set to {end + new Vector3(0, 10, 2)}");
        }

        // Builds spline and places poles
        GameObject startStructure = Instantiate(empty, start, Quaternion.identity);
        SplineContainer splineContainer = startStructure.AddComponent<SplineContainer>();
        splineContainer[0].Closed = true;
        splineContainer[0].Add(new BezierKnot(new Vector3(0,0,0) + new Vector3(lineSide * 1.2f, lineHeight / 2, -loaderLen / 2) / empty.transform.localScale.x));
        splineContainer[0].Add(new BezierKnot(new Vector3(0,0,0) + new Vector3(lineSide * 1.2f, lineHeight / 2, loaderLen / 2) / empty.transform.localScale.x));

        foreach (Vector3 location in poleLocations)
        {
            Debug.Log(location);
            Instantiate(pole, location, pole.transform.rotation);
            splineContainer[0].Add(new BezierKnot((location - start + new Vector3(lineSide, lineHeight, 0)) / empty.transform.localScale.x));
        }
        splineContainer[0].Add(new BezierKnot((end - start + new Vector3(lineSide, lineHeight / 1.8f, -loaderLen / 2)) / empty.transform.localScale.x));
        splineContainer[0].Add(new BezierKnot((end - start + new Vector3(lineSide, lineHeight / 1.8f, loaderLen / 2)) / empty.transform.localScale.x));
        splineContainer[0].Add(new BezierKnot((end - start + new Vector3(-lineSide, lineHeight / 1.8f, loaderLen / 2)) / empty.transform.localScale.x));
        splineContainer[0].Add(new BezierKnot((end - start + new Vector3(-lineSide, lineHeight / 1.8f, -loaderLen / 2)) / empty.transform.localScale.x));

        poleLocations.Reverse();

        foreach (Vector3 location in poleLocations)
        {
            splineContainer[0].Add(new BezierKnot((location - start + new Vector3(-lineSide, lineHeight, 0)) / empty.transform.localScale.x));
        }

        splineContainer[0].Add(new BezierKnot(new Vector3(0,0,0) + new Vector3(-lineSide / 2, lineHeight / 2, loaderLen / 2) / empty.transform.localScale.x));
        splineContainer[0].Add(new BezierKnot(new Vector3(0,0,0) + new Vector3(-lineSide / 2, lineHeight / 2, -loaderLen / 2) / empty.transform.localScale.x));

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
