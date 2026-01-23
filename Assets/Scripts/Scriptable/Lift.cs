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
    public GameObject pole;
    public GameObject chair;
    public GameObject loader;

    // Adjustments for the chair spline
    public float lineHeight;
    public float lineSide;

    // How many chairs on average between each pole
    public float chairDensity;

    public Vector3 start;
    public Vector3 end;

    // Pole locations in ORDER from bottom to top
    public List<Vector3> poleLocations;

    public void Start()
    {
        Debug.Log("Ran!");
        BuildLift();
    }

    public void BuildLift()
    {
        GameObject startStructure = Instantiate(loader, start, Quaternion.identity);
        GameObject endStructure = Instantiate(loader, end, Quaternion.identity);

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

        // Generate chairs along spline
        GameObject ingameChair = Instantiate(chair, start, Quaternion.identity);
        ingameChair.GetComponent<SplineAnimate>().Container = splineContainer;
        ingameChair.GetComponent<SplineAnimate>().Duration = splineContainer[0].GetLength() * 2;
        ingameChair.GetComponent<SplineAnimate>().Play();
    }   
}
