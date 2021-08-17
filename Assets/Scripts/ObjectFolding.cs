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

    private GameObject[] _allDrawingParts;
    private float _counter;

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
        foreach (var drawingPart in _allDrawingParts)
        {
            if (drawingPart.transform.position.z > _upperPivot.position.z)
            {
                drawingPart.transform.parent = _upperPivot;
            }
        }
        
        _upperPivot.DORotate(new Vector3(-180, 0, 0), 1f);
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
                drawingPart.transform.parent = _rightPivot;
            }
        }
        _rightPivot.DORotate(new Vector3(0, 0, 180), 1f);
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
}
