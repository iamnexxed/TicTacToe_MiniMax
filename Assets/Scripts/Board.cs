using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public Board_Tile[] boardTiles;

    bool isPlayerTurn = false;
    bool isAITurn = false;

    Board_Tile.Type playerSymbol;
    [SerializeField] Board_Tile.Type aiSymbol;

    public GameObject choicePanel;
    public GameObject gamePanel;

    public Text winText;

    int[] tempBoard;

    int winValue = 10;

    // Test Case
    int[] sampleBoard = { 2, 1, -1, 1, 2, 1, -1, -1, -1 };

    // Start is called before the first frame update
    void Start()
    {
        ResetGame();


        // FindBestMove(sampleBoard);

        

        tempBoard = new int[boardTiles.Length];
        for(int i = 0; i < tempBoard.Length; i++)
        {
            tempBoard[i] = -999;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Board_Tile tile = hit.collider.GetComponent<Board_Tile>();
                        
                    if (tile)
                    {
                        if(tile.GetTileType() == Board_Tile.Type.None)
                        {
                            tile.SetTileType(playerSymbol);
                            isPlayerTurn = false;
                            isAITurn = true;
                            CheckWin();
                        }
                            
                    }
                }
            }
        }
        if(isAITurn)
        {
            // Start AI's turn
            // Calculate the best possible outcome
            GenerateBoardCopy(boardTiles, ref tempBoard);
            int bestMoveIndex = FindBestMove(tempBoard);

            if (bestMoveIndex != -1)
            {
                boardTiles[bestMoveIndex].SetTileType(aiSymbol);
            }

            // end AI's turn

            isPlayerTurn = true;
            isAITurn = false;
            CheckWin();
        }


    }

    int EvaluateBoardSymbol(int[] getBoard, Board_Tile.Type val)
    {

        int value = ((int)val);


        for (int i = 0; i < getBoard.Length; ++i)
        {
            if(getBoard[i] == value)
            {
                if(i % 3 == 0)
                    // Horizontal Check
                    if ((getBoard[i] == getBoard[i + 1]) && (getBoard[i + 1] == getBoard[i + 2]))
                    {
                        // Debug.Log("Horizontal Win : " + i + " " + (i + 1)  + " " + (i + 2) + " ");
                        return winValue;
                    }
                if (i < 3)
                // Vertical Check
                    if ((getBoard[i] == getBoard[i + 3]) && (getBoard[i + 3] == getBoard[i + 6]))
                    {
                        // Debug.Log("Vertical Win : " + i + " " + (i + 3) + " " + (i + 6) + " ");
                        return winValue;
                    }
                
            }

            
        }
        if (getBoard[0] == value)
        {
            // Left Diagonal Check
            if ((getBoard[0] == getBoard[4]) && (getBoard[4] == getBoard[8]))
            {
                // Debug.Log("Diagonal Win : 0 4 8");
                return winValue;
            }
        }
        if(getBoard[2] == value)
        {

        
            // Right Diagonal Check
            if ((getBoard[2] == getBoard[4]) && (getBoard[4] == getBoard[6]))
            {
                // Debug.Log("Diagonal Win : 2 4 6");
                return winValue;
            }
        }

        // Game in Progress
        return 0;
    }


    int MiniMax(int[] getBoard, int depth, bool isMaximizing)
    {
        int score = 0;
        
        int aiScore = EvaluateBoardSymbol(getBoard, aiSymbol);
        int plScore = EvaluateBoardSymbol(getBoard, playerSymbol);
            
        if(aiScore == 10)
        {
            // AI won
            score = 10;
        }
        if(plScore == 10)
        {
            // Player won
            score = -10;
        }
        
        
        

        // If Node is a terminal node then return static evaluation of node
        // Depth will be determined by the number of available moves instead of a hard coded value
        int emptyIndex = GetEmptyIndex(getBoard);

        // Win or Lose 
        if ((score == 10 || score == -10))
            return score * (8 - depth);
       
        // Tie
        if (emptyIndex == -1)
        {
            return 0;
        }

        if(isMaximizing)
        {
            int maxValue = -999;
         

            for(int i = 0; i < getBoard.Length; ++i)
            {
                if (getBoard[i] == -1)
                {
                    getBoard[i] = (int)aiSymbol;
                    int value = MiniMax(getBoard, depth + 1, false);
                    maxValue = Mathf.Max(value, maxValue);
                    // Debug.Log("Maximizing value : " + maxValue);
                    getBoard[i] = -1;
                }
            }

            return maxValue;
            
        }
        else
        {
            int minValue = 999;
            for (int i = 0; i < getBoard.Length; ++i)
            {
                if (getBoard[i] == -1)
                {
                    getBoard[i] = (int)playerSymbol;
                    int value = MiniMax(getBoard, depth + 1, true);
                    minValue = Mathf.Min(value, minValue);
                    // Debug.Log("Minimizing value : " + minValue);
                    getBoard[i] = -1;
                }

                
            }
            return minValue;
        }
        
    }

    int FindBestMove(int[] tempBoard)
    {
        int bestValue = -99999;
        
        int bestMoveIndex = -1;

        
        // int emptyIndex = GetEmptyIndex(tempBoard);
        
        for (int i = 0; i < tempBoard.Length; ++i)
        {
            if (tempBoard[i] == -1)
            {
                tempBoard[i] = (int)aiSymbol;
                int checkValue = MiniMax(tempBoard, 0, false);
                tempBoard[i] = -1;

                Debug.Log("Index : " + i + " with a value : " + checkValue);

                if (checkValue > bestValue)
                {
                    bestValue = checkValue;
                    bestMoveIndex = i;
                }
            }
        }
        

        Debug.Log("Best move index is : " + bestMoveIndex + " with a value : " + bestValue);
        return bestMoveIndex;
       
    }

    void CheckWin()
    {
        // Check Win/Draw
        // If Win/Draw/Lose
        int[] checkBoard = new int[boardTiles.Length];
        GenerateBoardCopy(boardTiles, ref checkBoard);

        int playerScore = EvaluateBoardSymbol(checkBoard, playerSymbol);
        int aiScore = EvaluateBoardSymbol(checkBoard, aiSymbol);


        if(playerScore == 10)
        {
            Debug.Log("Player Wins");
            if (winText)
            {
                winText.text = "Player Wins";
            }
            isAITurn = false;
            isPlayerTurn = false;
        }
        if(aiScore == 10)
        {
            Debug.Log("AI Wins");
            if (winText)
            {
                winText.text = "AI Wins";
            }
            isAITurn = false;
            isPlayerTurn = false;
        }

        if(GetEmptyIndex(checkBoard) == -1)
        {
            Debug.Log("Draw!");
            if (winText)
            {
                winText.text = "Draw!";
            }
            isAITurn = false;
            isPlayerTurn = false;
        }
        
    }

    public void ResetGame()
    {
        choicePanel.SetActive(true);
        gamePanel.SetActive(false);
        winText.text = "";
    }

    void ResetTiles()
    {
        foreach (Board_Tile tile in boardTiles)
        {
            tile.ResetTile();
        }
    }

    public void SetPlayerSymbolX()
    {
        playerSymbol = Board_Tile.Type.X;
        aiSymbol = Board_Tile.Type.O;
        StartGame();
    }

    public void SetPlayerSymbolO()
    {
        playerSymbol = Board_Tile.Type.O;
        aiSymbol = Board_Tile.Type.X;
        StartGame();
    }

    void StartGame()
    {
        choicePanel.SetActive(false);
        gamePanel.SetActive(true);
        isPlayerTurn = false;
        isAITurn = false;
        ResetTiles();
        StartCoroutine(StartFromPlayer());
    }

    IEnumerator StartFromPlayer()
    {
        yield return new WaitForSeconds(0.3f);
        isAITurn = false;
        isPlayerTurn = true;
    }

    void ToggleTurns()
    {
        isAITurn = !isAITurn;
        isPlayerTurn = !isPlayerTurn;
    }

    int GetEmptyIndex(int[] board)
    {
        for(int i = 0; i < board.Length; ++i)
        {
            if(board[i] == -1)
            {
                return i;
            }
        }
        return -1;
    }

    void GenerateBoardCopy(Board_Tile[] getBoard, ref int[] outBoard)
    {
        for(int i = 0; i < getBoard.Length; ++i)
        {
            if(getBoard[i].GetTileType() == Board_Tile.Type.None)
            {
                outBoard[i] = -1;
            }
            else if (getBoard[i].GetTileType() == Board_Tile.Type.X)
            {
                outBoard[i] = 1;
            }
            else if (getBoard[i].GetTileType() == Board_Tile.Type.O)
            {
                outBoard[i] = 2;
            }
            else
            {
                outBoard[i] = -999;
                Debug.Log("Wrong value at : " + i);
            }
        }
    }
}
