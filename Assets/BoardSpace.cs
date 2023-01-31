using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardSpace : TicTacToeBehaviour
{
    public enum SpaceType { Empty, X, O };
    [SerializeField] SpaceType type = SpaceType.Empty;
    public SpaceType Type => type;
    public void SetType(SpaceType newType) => type = newType;
    [SerializeField] GameObject xObj;
    [SerializeField] GameObject oObj;

    GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponentInParent<GameManager>();
    }

    private void Start()
    {
        xObj.SetActive(false);
        oObj.SetActive(false);
    }

    public void SetSpaceType(SpaceType newType)
    {
        type = newType;
        xObj.SetActive(false);
        oObj.SetActive(false);

        if (type == SpaceType.X)
        {
            xObj.SetActive(true);
            StartCoroutine(GrowInTime(xObj, 0.2f));
        }
        else if (type == SpaceType.O)
        {
            oObj.SetActive(true);
            StartCoroutine(GrowInTime(oObj, 0.2f));
        }

        gameManager.CheckBoardState();

    }






}
