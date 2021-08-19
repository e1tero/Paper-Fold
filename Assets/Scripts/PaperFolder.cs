using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.YieldInstructions;
using DG.Tweening;
using UnityEngine;

public class PaperFolder : MonoBehaviour
{
    [SerializeField] private Transform _upperPivot;
    [SerializeField] private Transform _lowerPivot;
    [SerializeField] private Transform _rightPivot;
    [SerializeField] private Transform _leftPivot;
    
    [SerializeField] private float _time;
    
    private GameObject[] _allDrawingParts;
    private Camera _camera;
    private readonly Stack<Step> _steps = new Stack<Step>(10);
    private readonly List<Transform> _partsBuffer = new List<Transform>(3);
    private void Awake()
    {
        _camera = Camera.main;
        _allDrawingParts = GameObject.FindGameObjectsWithTag("DrawingPart");
    }

    private void Start() => StartCoroutine(WaitForTap());
    
    private IEnumerator WaitForTap()
    {
        yield return new WaitForMouseButtonDown(0);
        Folding();
        StartCoroutine(WaitForTap());
    }
    
    private void Folding()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (!Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit)) return;
        
        switch (hit.transform.tag)
        {
            case "UpperObjects":
                FoldObjects(_upperPivot,new Vector3(-179, 0, 0),true,false);
                break;
            case "LeftObjects":
                FoldObjects(_leftPivot,new Vector3(0, 0, -179),false,true);
                break;
            case "RightObjects":
                FoldObjects(_rightPivot,new Vector3(0, 0, 179),false,false);
                break;
            case "LowerObjects":
                FoldObjects(_lowerPivot,new Vector3(179, 0, 0),true,true);
                break;
        }
    }

    private void FoldObjects(Transform pivot, Vector3 angle, bool isVertical, bool needPivotMore)
    {
        if (pivot.rotation.eulerAngles.magnitude > 0)
        {
            if (_steps.Peek().Pivot == pivot)
            {
                StartCoroutine(ReversePivot(_steps.Peek()));
                _steps.Pop();
            }
            return;
        }

        _partsBuffer.Clear();

        foreach (var drawingPart in _allDrawingParts)
        {
            var partTransform = drawingPart.transform;
            var pivotTransform = pivot;

            var partMoreThenPivot = isVertical
                ? partTransform.position.z > pivotTransform.position.z
                : partTransform.position.x > pivotTransform.position.x;

            if (!(partMoreThenPivot ^ needPivotMore)) continue;
            
            _partsBuffer.Add(drawingPart.transform);
            drawingPart.transform.SetParent(pivot);
        }

        var step = new Step
        {
            Parts = new List<Transform>(_partsBuffer),
            Pivot = pivot
        };

        _steps.Push(step);
        pivot.DORotate(angle, 1f);
    }
    
    private IEnumerator ReversePivot(Step step)
    {
        var elapsed = 0f;
        
        foreach (var currentPart in step.Parts)
        {
            currentPart.SetParent(step.Pivot);
        }

        while (elapsed < 1f)
        {
            elapsed += Time.fixedDeltaTime / _time;
            Quaternion rot = Quaternion.identity;

            step.Pivot.rotation = Quaternion.Slerp(step.Pivot.rotation, rot, elapsed);
            yield return null;
        }
    }
}

public struct Step
{
    public List<Transform> Parts;
    public Transform Pivot;
}
