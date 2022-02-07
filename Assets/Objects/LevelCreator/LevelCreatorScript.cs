using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreatorScript : MonoBehaviour
{
    private enum WallDirection {Long, Bold}

    public GameObject mainWorldPreset;
    public GameObject wallPiece;
    private GameObject wallParent;

    public int minWallLenght = 1;
    public int maxWallLenght = 1;
    public int minWallCount = 1;
    public int maxWallCount = 1;

    private List<GameObject> walls;

    public byte[,] worldGrid;
    [SerializeField]private int matrixX = 28;
    [SerializeField]private int matrixY = 28;
    [SerializeField] private int maxTrysToFindPlace = 10;
    
    public void CreateWorld()
    {
        GameObject world = Instantiate<GameObject>(mainWorldPreset, Vector3.zero, Quaternion.identity);
        wallParent = GameObject.Find("EntryWalls");
    }

    public void GenerateWalls()
    {
        int wallAmount = Random.Range(minWallCount, maxWallCount + 1);
        if(walls != null)
        {
            DestroyAllWalls();
            walls = null;
        }
        walls = new List<GameObject>();
        for (int i = 0; i < wallAmount; i++)
        {
            GameObject wall = CreateWall();
            walls.Add(wall);
            wall.transform.SetParent(wallParent.transform);
            wall.SetActive(false);
        }
        
    }
    
    private void DestroyAllWalls()
    {
        foreach (GameObject item in walls)
        {
            DestroyImmediate(item);
        }
    }

    private GameObject CreateWall()
    {
        GameObject wall;
        wall = Instantiate<GameObject>(wallPiece);
        WallDirection dir = (WallDirection)Random.Range(0, 2);
        if(dir == WallDirection.Long)
        {
            wall.transform.localScale = new Vector3(Random.Range(minWallLenght, maxWallLenght + 1), 3, 1);
        }
        else
        {
            wall.transform.localScale = new Vector3(1, 3, Random.Range(minWallLenght, maxWallLenght + 1));
        }
        return wall;
    }

    public void SetAllWallsIntoPlace()
    {
        InicializeDefaultGrid();
        bool checkAllWalls = false;
        for (int i = 0; i < walls.Count; i++)
        {
            checkAllWalls = SetWallOnPlace(walls[i]);
        }
        if(!checkAllWalls)
        {
            Debug.LogError("cant place wall");
        }
    }

    private void InicializeDefaultGrid()
    {
        worldGrid = new byte[matrixX,matrixY];
        for (int i = 0; i < matrixY; i++)
        {
            for (int j = 0; j < matrixX; j++)
            {
                worldGrid[i, j] = 0;
            }
        }
    }

    private bool SetWallOnPlace(GameObject wall)
    {
        Vector3 position;
        int wallX = (int)wall.transform.localScale.x;
        int wallY = (int)wall.transform.localScale.z;
        position = TryFindRandomPosition(wallX, wallY);

        if (position == Vector3.zero)
        {
            position = FindAnyPosition(wallX, wallY);
        }

        if (position != Vector3.zero)
        {
            wall.transform.position = position;
            wall.SetActive(true);
            SetOffsetToPlace(wall);
            return true;
        }

        return false;
    }

    private Vector3 TryFindRandomPosition(int wallX, int wallY)
    {
        Vector3 position;
        bool xPosition = false;
        bool yPosition = false;
        int xPos = 0;
        int yPos = 0;
        int counter = 0;
        do
        {
            if (counter >= maxTrysToFindPlace)
                break;
            xPosition = false;
            yPosition = false;
            xPos = Random.Range(0, matrixX);
            yPos = Random.Range(0, matrixY);
            xPosition = CheckForEmptyXRow(xPos, yPos, wallX, xPosition);
            yPosition = CheckForEmptyYRow(xPos, yPos, wallY, yPosition);
            counter++;
        } while (!xPosition && !yPosition);

        if (xPosition && yPosition)
        {
            SetWallInWorldGrid(xPos, yPos, wallX, wallY);
            position = new Vector3((float)xPos, 1.5f, (float)yPos);
        }
        else
        {
            position = Vector3.zero;
        }

        return position;
    }

    private Vector3 FindAnyPosition(int wallX, int wallY)
    {
        
        Vector3 position = Vector3.zero;
        for (int i = 0; i < matrixY; i++)
        {
            bool xPosition = false;
            bool yPosition = false;
            for (int j = 0; j < matrixX; j++)
            {
                xPosition = CheckForEmptyXRow(j, i, wallX, xPosition);
                yPosition = CheckForEmptyYRow(j, i, wallY, yPosition);
                if (xPosition && yPosition)
                {
                    position = new Vector3((float)j, 1.5f, (float)i);
                    SetWallInWorldGrid(j, i, wallX, wallY);
                    break;
                }
                    
            }
            if(xPosition && yPosition)
            {
                break;
            }
        }
        return position;
    }

    private void SetOffsetToPlace(GameObject obj)
    {
        Vector3 scaleOffset = new Vector3((obj.transform.localScale.x - 1) / 2, 0, -(obj.transform.localScale.y - 1) / 2);
        Vector3 positionOffset = new Vector3(-matrixX / 2, 0, -matrixY / 2);
        //obj.transform.position = scaleOffset + positionOffset + obj.transform.position;
        obj.transform.position = new Vector3(obj.transform.position.x - matrixX/2 + (obj.transform.localScale.x-1)/2, obj.transform.position.y, obj.transform.position.z - matrixY / 2 + (obj.transform.localScale.z - 1) / 2);

    }

    private void SetWallInWorldGrid(int xPos, int yPos, int wallX, int wallY)
    {
        for (int i = xPos; i < xPos+wallX; i++)
        {
            worldGrid[yPos, i] = 1;
        }

        for (int i = yPos; i < yPos+wallY; i++)
        {
            worldGrid[i, xPos] = 1;
        }
    }

    private bool CheckForEmptyYRow(int xPos, int yPos, int wallY, bool yPosition)
    {
        if(yPos+wallY > matrixY)
        {
            return false;
        }
        for (int i = yPos; i < yPos + wallY; i++)
        {
            if (worldGrid[i, xPos] == 0)
            {
                yPosition = true;
            }
            else
            {
                yPosition = false;
                break;
            }
        }

        return yPosition;
    }

    private bool CheckForEmptyXRow(int xPos, int yPos, int wallX, bool xPosition)
    {
        if (xPos + wallX > matrixX)
        {
            return false;
        }
        for (int i = xPos; i < xPos + wallX; i++)
        {
            if (worldGrid[yPos, i] == 0)
            {
                xPosition = true;
            }
            else
            {
                xPosition = false;
                break;
            }
        }

        return xPosition;
    }
}
