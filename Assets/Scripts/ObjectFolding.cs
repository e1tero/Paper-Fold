using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class ObjectFolding : MonoBehaviour
{
    [SerializeField] private Transform _upperPivot;
    [SerializeField] private Transform _leftPivot;
    [SerializeField] private Transform _rightPivot;
    [SerializeField] private Transform _lowerPivot;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private float _time;

    private GameObject[] _allDrawingParts;
    private Stack<Step> _steps = new Stack<Step>(10);
    private bool isFolding;

    private void Awake()
    {
        _allDrawingParts = GameObject.FindGameObjectsWithTag("DrawingPart");
    }
    
    private void Update()
    {
       Folding();
    }
    private void Folding()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.transform.tag == "UpperObjects")
                {
                    FoldingUpperObjects();
                }
                
                else if (hit.transform.tag == "LeftObjects")
                {
                    FoldingLeftObjects();
                }
                
                else if (hit.transform.tag == "RightObjects")
                {
                    FoldingRightObjects();
                }
                
                else if (hit.transform.tag == "LowerObjects")
                {
                    FoldingLowerObjects();
                }
            }
        }
    }

    private void FoldingUpperObjects()
    {
        
        var step = new Step {Pivot = _upperPivot};

        foreach (var drawingPart in _allDrawingParts)
        {
            if (drawingPart.transform.position.z > _upperPivot.position.z)
            {
                step.PreviousPivot = drawingPart.transform.parent;
                drawingPart.transform.parent = _upperPivot;
            }
        }
        
        var angleRotation = new Vector3(-180, 0, 0);
        _upperPivot.DORotate(angleRotation, 1f);

        //ReverseFoldingObjects(step,_upperPivot,angleRotation);
    }

    private void ReverseFoldingObjects(Step step, Transform pivot,Vector3 angle)
    {
        if (pivot != step.Pivot) return;
        //pivot.DORotate(Vector3.zero, 1f);
        
    }
    
    private void FoldingLowerObjects()
    {
        foreach (var drawingPart in _allDrawingParts)
        {
            if (drawingPart.transform.position.z < _lowerPivot.position.z)
            {
                drawingPart.transform.parent = _lowerPivot;
            }
        }
        
        _lowerPivot.DORotate(new Vector3(180, 0, 0), 1f);
    }

    private void FoldingRightObjects()
    {
        foreach (var drawingPart in _allDrawingParts)
        {
            if (drawingPart.transform.position.x > _rightPivot.position.x)
            {
                drawingPart.transform.SetParent(_rightPivot);
            }
        }
        //_rightPivot.DORotate(new Vector3(0, 0, 180), 1f);
        StartCoroutine(RotatePivot(_rightPivot, 1));
    }
    private void FoldingLeftObjects()
    {
        foreach (var drawingPart in _allDrawingParts)
        {
            if (drawingPart.transform.position.x < _leftPivot.position.x)
            {
                drawingPart.transform.parent = _leftPivot;
            }
        }

        _leftPivot.DORotate(new Vector3(0, 0, -180), 1f);
    }
    
    private IEnumerator RotatePivot(Transform pivot, int offset) 
    {
        var elapsed = 0f;
        var eulers = transform.eulerAngles;
        
        while (elapsed < 1f)
        {
            elapsed += Time.fixedDeltaTime / _time;
            Quaternion rot = Quaternion.Euler(eulers.x, eulers.y,
                eulers.z + (_animationCurve.Evaluate(elapsed) * offset));

            pivot.rotation = Quaternion.Lerp(pivot.rotation, rot, elapsed);
            yield return new WaitForFixedUpdate();
        }
    }

}

public struct Step
{
    public Transform Pivot;
    public Transform[] Parts;
    public Transform PreviousPivot;
}
