using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Ant : MonoBehaviour
{
    public GameObject LiveCell;
    public GameObject DeadCell;
    public Grid grid;
    public int columns;
    public int rows;
    Vector2 currentPosition;
    Vector2 currentForward;
    public int speed = 1;
    void Start()
    {
        grid = new Grid(columns, rows, LiveCell, DeadCell, this);
        currentPosition = new Vector2((int)columns / 2, (int)rows / 2);
        currentForward = new Vector2(0,1);

        foreach(Cell cell in grid.cells)
        {
            grid.CheckCellState(cell);
        }
    }

    
    void Update()
    {
        for (int i = 0; i < speed; i++)
        {
            /*
            foreach(Cell cell in grid.cells)
            {
                cell.CheckState();
            }
            */
            
            
            if (currentPosition.x < 0)
            {
                currentPosition = new Vector2(currentPosition.x + columns, currentPosition.y);
                return;

            }
            else if (currentPosition.x > columns-1)
            {
                currentPosition = new Vector2(0, currentPosition.y);
                return;
            }

            if (currentPosition.y < 0)
            {
                currentPosition = new Vector2(currentPosition.x, currentPosition.y + rows);
                return;
            }
            else if (currentPosition.y > rows-1)
            {
                currentPosition = new Vector2(currentPosition.x, 0);
                return;
            }
            
            //Cell currentCell = grid.FindCellAtPosition(new Vector2(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y)));
            Cell currentCell = grid.cells[(int)Mathf.Round(currentPosition.x*rows) + (int)Mathf.Round(currentPosition.y)];

            if (currentCell.state == 0)
            {
                currentCell.state = 1;
                currentForward = Rotate(currentForward, -90);
                currentPosition += currentForward;
                currentPosition.x = Mathf.Round(currentPosition.x);
                currentPosition.y = Mathf.Round(currentPosition.y);
            }
            else if (currentCell.state == 1)
            {
                currentCell.state = 0;
                currentForward = Rotate(currentForward, 90);
                currentPosition += currentForward;
                currentPosition.x = Mathf.Round(currentPosition.x);
                currentPosition.y = Mathf.Round(currentPosition.y);
            }
            grid.CheckCellState(currentCell);

            transform.position = currentPosition;
        }
    }

    public GameObject SpawnCell(GameObject cell, Vector2 position)
    {
        GameObject obj = Instantiate(cell, position, Quaternion.identity);
        return obj;
    }
    public void DestroyCell(GameObject cell)
    {
        Destroy(cell);
    }

    public Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}

public class Grid
{
    public int cols;
    public int rows;
    public List<Cell> cells = new List<Cell>();
    GameObject LiveCell;
    GameObject DeadCell;
    public Ant ant;
    public Grid(int columns, int rows, GameObject LiveCell, GameObject DeadCell, Ant ant)
    {
        this.cols = columns;
        this.rows = rows;
        this.LiveCell = LiveCell;
        this.DeadCell = DeadCell;
        this.ant = ant;

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < this.rows; j++)
            {
                Vector2 cellPos = new Vector2(i,j);
                Cell cell = new Cell(cellPos, 0, LiveCell, DeadCell, ant, ant.SpawnCell(DeadCell, cellPos));
                cells.Add(cell);
            }
        }
    }

    public void CheckCellState(Cell cell)
    {
        if (cell.SpawnedObject == null)
        {
            cell.SpawnedObject = ant.SpawnCell(DeadCell, cell.coords);
        }
        else
        {
            if (cell.state == 0 && cell.SpawnedObject.tag != "DeadCell")
            {
                ant.DestroyCell(cell.SpawnedObject);
                cell.SpawnedObject = ant.SpawnCell(DeadCell, cell.coords);
            }
            else if (cell.state == 1 && cell.SpawnedObject.tag != "LiveCell")
            {
                ant.DestroyCell(cell.SpawnedObject);
                cell.SpawnedObject = ant.SpawnCell(LiveCell, cell.coords);
            }
        }
    }

    public Cell FindCellAtPosition(Vector2 position)
    {
        foreach(Cell cell in cells)
        {
            if (cell.coords == position)
                return cell;
            else
                continue;
        }
        return null;
    }
}

public class Cell
{
    public Vector2 coords;
    public GameObject LiveCell;
    public GameObject DeadCell;
    public int state;
    public Ant ant;
    public GameObject SpawnedObject;
    public Cell(Vector2 coords, int state, GameObject LiveCell, GameObject DeadCell, Ant ant, GameObject SpawnedObject)
    {
        this.coords = coords;
        this.state = state;
        this.LiveCell = LiveCell;
        this.DeadCell = DeadCell;
        this.ant = ant;
        this.SpawnedObject = SpawnedObject;
    }

    public GameObject SearchPosition()
    {
        GameObject[] LiveCells = GameObject.FindGameObjectsWithTag("LiveCell");
        GameObject [] DeadCells = GameObject.FindGameObjectsWithTag("DeadCell");
        GameObject[] AllCells = LiveCells.Concat(DeadCells).ToArray();

        foreach(GameObject cell in AllCells)
        {
            if (new Vector2(cell.transform.position.x, cell.transform.position.y) == coords)
            {
                return cell;
            }
            else
            {
                continue;
            }
        }
        return null;
    }

    public void CheckState()
    {
        GameObject currentlySpawned = SearchPosition();
        if (currentlySpawned == null)
        {
            currentlySpawned = ant.SpawnCell(DeadCell, coords);
        }
        else
        {
            if (state == 0 && currentlySpawned.tag != "DeadCell")
            {
                ant.DestroyCell(currentlySpawned);
                currentlySpawned = ant.SpawnCell(DeadCell, coords);
            }
            else if (state == 1 && currentlySpawned.tag != "LiveCell")
            {
                ant.DestroyCell(currentlySpawned);
                currentlySpawned = ant.SpawnCell(LiveCell, coords);
            }
        }
    }
}
