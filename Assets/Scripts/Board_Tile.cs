using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_Tile : MonoBehaviour
{
    public enum Type
    {
        None,
        X,
        O
    }

    public Type type;

    public void SetTileType(Type val)
    {
        type = val;
        if(val == Type.X)
        {
            // Child 0 is "X"
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if(val == Type.O)
        {
            // Child 1 is "O"
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Not a valid symbol. Choose X or O.");
        }
    }

    public Type GetTileType() 
    {
        return type;
    }

    public void ResetTile()
    {
        type = Type.None;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
}
