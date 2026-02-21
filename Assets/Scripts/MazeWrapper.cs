using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

public class MazeWrapper : MonoBehaviour
{  
    public int rows = 10;
    public int cols = 10;
    public GameObject wallPrefab;
    public GameObject startPrefab;
    public GameObject finishPrefab;
    public GameObject floorPrefab;
    public GameObject rampPrefab;
    public GameObject playerBall;
    public Transform mazeParent;


    private enum MazeCell{
        WALL = 0,
        PATH = 1,
        FINISH = 2,
        START = 3
    };

    private MazeCell[,] mazeCells;
    private int start;
    private int finish;
    

    [DllImport("Dll-Maze")]
    private static extern IntPtr GenerateMaze(int rows, int cols);
    

    private void getMazeInput(string mazeInput)
    {
      
        string[] lines = mazeInput.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

        start = int.Parse(lines[0].Substring(6));
        finish = int.Parse(lines[1].Substring(7));
        
        List<string> mazeLines = new List<string>();
        
        foreach (string line in lines)
        {
            if(!line.StartsWith("START=") && !line.StartsWith("FINISH=") && line.Any(char.IsDigit)){
                mazeLines.Add(line); 
            }

        }

        rows = mazeLines.Count;
        cols = mazeLines[0].Length;

        mazeCells = new MazeCell[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                char cell = mazeLines[i][j];
                switch (cell)
                {
                    case '0':
                        mazeCells[i, j] = MazeCell.WALL;
                        break;
                    case '1':
                        mazeCells[i, j] = MazeCell.PATH;
                        break;
                    case '2':
                        mazeCells[i, j] = MazeCell.FINISH;
                        break;
                    case '3':
                        mazeCells[i, j] = MazeCell.START;
                        break;
                }
            }
        }
        Debug.Log($"Maze parsed: {rows} rows x {cols} cols \n" + mazeCells[rows-1, start]);
        
    }

    private void buildMaze()
    {
        

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 pos = new Vector3(j, 0.4f, -i);
                GameObject prefabToInstantiate = null;

                switch (mazeCells[i, j])
                {
                    case MazeCell.WALL:
                        prefabToInstantiate = wallPrefab;
                        break;
                    case MazeCell.START:
                        {
                            GameObject startObj = Instantiate(startPrefab, pos, Quaternion.identity, mazeParent);
                            startObj.transform.localScale = new Vector3(1f, 2f, 1f);

                            // Place the player ball above the START cube
                            Vector3 spawnOffset = new Vector3(0, 1.0f, -1); // Adjust height if needed
                            Vector3 spawnPos = pos + spawnOffset;

                            GameObject ball = Instantiate(playerBall, spawnPos, Quaternion.identity);

                            Camera mainCam = Camera.main;
                            if (mainCam != null)
                            {
                                mainCam.transform.position = ball.transform.position + new Vector3(0f, 4f, -8f); // Adjust as needed
                                mainCam.transform.LookAt(ball.transform);

                                PlayerCamera camFollow = mainCam.GetComponent<PlayerCamera>();
                                if (camFollow != null)
                                    camFollow.target = ball.transform;
                            }

                            prefabToInstantiate = startPrefab;
                            // Spawn ramp just before the START tile (adjust direction if needed)
                            Vector3 rampPos = pos + new Vector3(0f, -0.05f, -1f);  // In front of START tile
                            Quaternion rampRot = Quaternion.Euler(-30f, 0f, 0f);   // Sloped downward

                            GameObject ramp = Instantiate(rampPrefab, rampPos, rampRot, mazeParent);
                            ramp.transform.localScale = new Vector3(1f, 0.1f, 2f); // Long, low slope
                            break;
                        }
                    case MazeCell.FINISH:
                        prefabToInstantiate = finishPrefab;
                        break;
                }

                if (prefabToInstantiate != null)
                {
                    GameObject obj = Instantiate(prefabToInstantiate, pos, Quaternion.identity, mazeParent);
                    obj.transform.localScale = new Vector3(1f, 3f, 1f);
                }

            }

        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        IntPtr resultPtr = GenerateMaze(rows, cols);
        string maze = Marshal.PtrToStringAnsi(resultPtr);
        Debug.Log("Maze Output:\n" + maze);

        getMazeInput(maze);
        buildMaze();
        // Find and configure floor generator dynamically
        GameObject floorInstance = Instantiate(floorPrefab);
        FloorMeshGenerator floorGen = floorInstance.GetComponent<FloorMeshGenerator>();
        floorGen.GenerateMesh(cols, rows);



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
