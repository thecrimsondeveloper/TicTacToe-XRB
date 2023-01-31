using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : TicTacToeBehaviour
{

    [SerializeField] GameObject panel;
    [SerializeField] GameObject itsYourTurnText;
    [SerializeField] TMP_Text gameText;



    [SerializeField] BoardSpace[] spaces;
    int[] preferedSpaces = new int[] { 4, 1, 3, 5, 7 };
    bool gameRunning = true;
    [SerializeField] AnimationCurve panelGrowCurve;




    private void Start()
    {
        panel.SetActive(false);
        StartCoroutine(PlayerTurn());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator PlayerTurn()
    {
        itsYourTurnText.SetActive(true);
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Mouse0)));
        if (gameRunning == false) { yield break; }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.TryGetComponent(out BoardSpace boardSpace))
            {
                if (boardSpace.Type == BoardSpace.SpaceType.Empty)
                {
                    boardSpace.SetSpaceType(BoardSpace.SpaceType.X);
                    StartCoroutine(ComputerTurn());
                    yield break;
                }
            }

        }
        StartCoroutine(PlayerTurn());
    }

    IEnumerator ComputerTurn()
    {
        itsYourTurnText.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        if (gameRunning == false) { yield break; }

        int choice = -1;

        //see if the computer can win
        for (int i = 0; i < spaces.Length; i++)
        {
            if (spaces[i].Type != BoardSpace.SpaceType.Empty) continue;
            if (WillWinFor(BoardSpace.SpaceType.O, i))
            {
                choice = i;
                break;
            }
        }

        //see if the player will win next turn
        if (choice == -1)
        {
            for (int i = 0; i < spaces.Length; i++)
            {
                if (spaces[i].Type != BoardSpace.SpaceType.Empty) continue;
                if (WillWinFor(BoardSpace.SpaceType.X, i))
                {
                    choice = i;
                    break;
                }
            }
        }

        //make educated choice
        if (choice == -1)
        {
            int numOfX = spaces.Count(x => x.Type == BoardSpace.SpaceType.X);
            int[] corners = new int[] { 0, 2, 6, 8 };

            //if there is only one X in the middle, choose a corner
            if (numOfX == 1 && spaces[4].Type == BoardSpace.SpaceType.X)
            {
                choice = corners[Random.Range(0, corners.Length)];
            }
            else if (numOfX == 2 &&
                spaces[4].Type == BoardSpace.SpaceType.X &&
                corners.Any(x => spaces[x].Type == BoardSpace.SpaceType.X))
            {//choose the spot opposite from last choice if the center and corner are taken
                if (spaces[0].Type == BoardSpace.SpaceType.O && spaces[2].Type == BoardSpace.SpaceType.Empty) choice = 2; ;
                if (spaces[2].Type == BoardSpace.SpaceType.O && spaces[0].Type == BoardSpace.SpaceType.Empty) choice = 0;
                if (spaces[6].Type == BoardSpace.SpaceType.O && spaces[8].Type == BoardSpace.SpaceType.Empty) choice = 8;
                if (spaces[8].Type == BoardSpace.SpaceType.O && spaces[6].Type == BoardSpace.SpaceType.Empty) choice = 6;
                if (spaces[6].Type == BoardSpace.SpaceType.O && spaces[2].Type == BoardSpace.SpaceType.Empty) choice = 2;
                if (spaces[2].Type == BoardSpace.SpaceType.O && spaces[6].Type == BoardSpace.SpaceType.Empty) choice = 6;
                if (spaces[8].Type == BoardSpace.SpaceType.O && spaces[0].Type == BoardSpace.SpaceType.Empty) choice = 0;
                if (spaces[0].Type == BoardSpace.SpaceType.O && spaces[8].Type == BoardSpace.SpaceType.Empty) choice = 8;
            }
            else //choose the center or a side
            {
                for (int i = 0; i < preferedSpaces.Length; i++)
                {
                    if (spaces[i].Type != BoardSpace.SpaceType.Empty) continue;
                    if (spaces[preferedSpaces[i]].Type == BoardSpace.SpaceType.Empty)
                    {
                        choice = preferedSpaces[i];
                        break;
                    }
                }
            }
        }

        //make random choice if educated choice was not available
        if (choice == -1)
        {
            List<int> emptySpaces = new List<int>();
            for (int i = 0; i < spaces.Length; i++)
            {
                if (spaces[i].Type == BoardSpace.SpaceType.Empty)
                {
                    emptySpaces.Add(i);
                }
            }
            choice = emptySpaces[Random.Range(0, emptySpaces.Count)];
        }

        spaces[choice].SetSpaceType(BoardSpace.SpaceType.O);
        CheckBoardState();
        StartCoroutine(PlayerTurn());
    }



    public void CheckBoardState()
    {
        bool gameOver = IsDraw(spaces) || IsWin(spaces) || IsLoss(spaces);
        if (IsDraw(spaces)) gameText.text = "Tie!";
        if (IsWin(spaces)) gameText.text = "You Win!";
        if (IsLoss(spaces)) gameText.text = "You Lose!";

        if (gameOver == false) return;

        StartCoroutine(GrowInTime(panel, 1f, 0.5f));
        gameRunning = false;
    }






    bool IsDraw(BoardSpace[] spaces) => spaces.Count(x => x.Type != BoardSpace.SpaceType.Empty) == 9;
    bool IsWin(BoardSpace[] spaces) => CheckPlayerWin(spaces, BoardSpace.SpaceType.X);
    bool IsLoss(BoardSpace[] spaces) => CheckPlayerWin(spaces, BoardSpace.SpaceType.O);

    bool CheckPlayerWin(BoardSpace[] spaces, BoardSpace.SpaceType type)
    {
        //rows
        for (int i = 0; i < 9; i += 3)
        {
            if (spaces[i].Type == spaces[i + 1].Type && spaces[i + 1].Type == spaces[i + 2].Type && spaces[i].Type == type)
            {
                return true;
            }
        }

        //columns
        for (int i = 0; i < 3; i++)
        {
            if (spaces[i].Type == spaces[i + 3].Type && spaces[i + 3].Type == spaces[i + 6].Type && spaces[i].Type == type)
            {
                return true;
            }
        }

        //diagonals
        for (int i = 0; i < 3; i += 2)
        {
            if (spaces[i].Type == spaces[4].Type && spaces[4].Type == spaces[8 - i].Type && spaces[i].Type == type)
            {
                return true;
            }
        }

        return false;
    }



    bool WillWinFor(BoardSpace.SpaceType type, int index)
    {
        string boardString = "";
        BoardSpace[] copyBoard = new BoardSpace[9];
        for (int i = 0; i < spaces.Length; i++)
        {
            BoardSpace space = new BoardSpace();
            space.SetType(spaces[i].Type);
            copyBoard[i] = space;
            boardString += spaces[i].Type.ToString() + ", ";
        }
        Debug.Log("Board: " + boardString);



        copyBoard[index].SetType(type);
        bool isPlayer = type == BoardSpace.SpaceType.X;
        bool isWinForType = isPlayer ? IsWin(copyBoard) : IsLoss(copyBoard);
        Debug.Log("(" + type + ")WillWinFor: " + isWinForType);
        return isPlayer ? IsWin(copyBoard) : IsLoss(copyBoard);
    }






}
