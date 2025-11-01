using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [Header("Start & Goal")]
    public Vector2Int start = new Vector2Int(0, 1);
    public Vector2Int goal = new Vector2Int(4, 4);

    [Header("Grid Settings")]
    public int[,] grid;
    public int gridWidth = 5;
    public int gridHeight = 5;

    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int current;
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private void Start()
    {
        GenerateRandomGrid(gridWidth, gridHeight, 0.2f);
        FindPath(start, goal);
    }

    #region Dynamic Start/Goal
    public void SetStart(Vector2Int newStart)
    {
        start = newStart;
        FindPath(start, goal);
    }

    public void SetGoal(Vector2Int newGoal)
    {
        goal = newGoal;
        FindPath(start, goal);
    }
    #endregion

    #region Grid Modification
    public void GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        grid = new int[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[y, x] = Random.value < obstacleProbability ? 1 : 0;
            }
        }
        FindPath(start, goal);
    }

    public void AddObstacle(Vector2Int position)
    {
        if (IsInBounds(position))
        {
            grid[position.y, position.x] = 1;
            FindPath(start, goal);
        }
    }
    #endregion

    #region Pathfinding
    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) &&
               point.y >= 0 && point.y < grid.GetLength(0);
    }

    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        path.Clear();
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (current == goal) break;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (IsInBounds(next) && grid[next.y, next.x] == 0 && !cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return;
        }

        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        float cellSize = 1f;

        // Draw grid on X-Y plane
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0); // X-Y plane
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(pos, new Vector3(cellSize, cellSize, 0.1f)); // thin on Z
            }
        }

        // Draw path
        foreach (var step in path)
        {
            Vector3 pos = new Vector3(step.x * cellSize, step.y * cellSize, 0);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(pos, new Vector3(cellSize, cellSize, 0.1f));
        }

        // Draw start & goal
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, start.y * cellSize, 0), new Vector3(cellSize, cellSize, 0.1f));
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, goal.y * cellSize, 0), new Vector3(cellSize, cellSize, 0.1f));
    }
    #endregion
}