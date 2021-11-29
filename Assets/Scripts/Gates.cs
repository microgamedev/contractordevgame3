using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Gates : MonoBehaviour
{
    [SerializeField] int gateValue;
    [SerializeField] bool isMoving = false;

    //[SerializeField] enum GatePosition { Left, Center, Right};
    //[SerializeField] GatePosition gatePosition;

    [SerializeField] TextMeshPro gatesText;
    [SerializeField] MeshRenderer glassMesh;
    [SerializeField] Material gateMaterialPlus;
    [SerializeField] Material gateMaterialMinus;

    private bool isTouch = false;
    private float moveLimit = 1f;
    private float startPosition;
    private float finishPosition;

    private void Start()
    {
        GateStartPosition();
        GateSetup();
        GateMove();
    }

    private void GateStartPosition()
    {
        //switch(gatePosition)
        //{
        //    case GatePosition.Left:
        //        startPosition = -moveLimit;
        //        finishPosition = moveLimit;
        //        break;
        //    case GatePosition.Center:
        //        startPosition = 0f;
        //        finishPosition = moveLimit;
        //        break;
        //    case GatePosition.Right:
        //        startPosition = moveLimit;
        //        finishPosition = -moveLimit;
        //        break;
        //}
        float gatePosition = transform.position.x;
        switch (gatePosition)
        {
            case -1:
                startPosition = -moveLimit;
                finishPosition = moveLimit;
                break;
            case 0:
                startPosition = 0f;
                finishPosition = moveLimit;
                break;
            case 1:
                startPosition = moveLimit;
                finishPosition = -moveLimit;
                break;
        }

        transform.position = new Vector3(startPosition, 0, transform.position.z);
    }

    private void GateSetup()
    {
        if (gateValue > 0)
        {
            gatesText.text = "+ " + gateValue;
            glassMesh.material = gateMaterialPlus;
        }
        else if (gateValue < 0)
        {
            gatesText.text = "- " + gateValue;
            glassMesh.material = gateMaterialMinus;
        }
    }

    private void GateMove()
    {
        if(isMoving)
        {
            transform.DOMoveX(finishPosition, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isTouch)
        {
            isTouch = true;

            Player _player = other.GetComponent<Player>();

            if (gateValue > 0)
            {
                _player.AddSnakePart(null, gateValue);
            }
            else if (gateValue < 0)
            {
                _player.RemoveSnakePart(gateValue);
            }

            _player.ShowSnakeFX();
        }
    }
}
