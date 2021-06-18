using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomTypes
    {
        Left,
        LeftLower,
        LeftLowerRight,
        LeftLowerUpper,
        LeftLowerRightUpper,
        LeftRight,
        LeftRightUpper,
        LeftUpper,
        Lower,
        LowerRight,
        LowerRightUpper,
        LowerUpper,
        Right,
        RightUpper,
        Upper       
    }

    public RoomTypes roomType;

    public Puzzle puzzle;

    public Wall WallObj;
    public List<Wall> Walls;
    public Vector2 Pos;
    public int RoomWidth;
    public int DistanceFromStart = 0;
    public bool IsGoal;

    public bool LeftWall = true, RightWall = true, UpperWall = true, LowerWall = true;
    public bool Visited;

    public List<Room> Neighbors = new List<Room>();

    public int RoomNumber;

    // Start is called before the first frame update
    void Start()
    {

        //for (var i = -5; i <= 5; i++)
        //{
        //    var obj = Instantiate(WallObj);
        //    obj.rb.position = new Vector2(i, 4);
        //    Walls.Add(obj);
        //}

        //var myNewSmoke = Instantiate(WallObj, Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        //myNewSmoke.transform.parent = gameObject.transform;

        for (var i = 0; i < RoomWidth; i++)
        {
            for (var j = 0; j < RoomWidth; j++)
            {
                if ((i == 0 || j == 0 || i == 9 || j == 9))
                {
                    if ((i == 4 || i == 5))
                    {
                        if (j == 0 && !LowerWall)
                        {
                            //Debug.Log($"Skip {i}, {j}");
                            continue;
                        }
                        if (j == RoomWidth - 1 && !UpperWall)
                        {
                            //Debug.Log($"Skip {i}, {j}");
                            continue;
                        }
                    }
                    if ((j == 4 || j == 5))
                    {
                        if (i == 0 && !LeftWall)
                        {
                            //Debug.Log($"Skip {i}, {j}");
                            continue;
                        }
                        if (i == RoomWidth - 1 && !RightWall)
                        {
                            //Debug.Log($"Skip {i}, {j}");
                            continue;
                        }
                    }
                    var obj = Instantiate(WallObj, new Vector3(transform.position.x + i, transform.position.y + j, transform.position.z), Quaternion.identity);
                    //obj.rb.position = new Vector2(Pos.x, Pos.y);
                    obj.transform.parent = gameObject.transform;
                    Walls.Add(obj);
                }

            }
        }

        //obj = Instantiate(WallObj);
        ////obj.rb.position = new Vector2(Pos.x + 10, Pos.y);
        //Walls.Add(obj);

        //obj = Instantiate(WallObj);
        ////obj.rb.position = new Vector2(Pos.x + 10, Pos.y + 10);
        //Walls.Add(obj);

        //obj = Instantiate(WallObj);
        ////obj.rb.position = new Vector2(Pos.x, Pos.y + 10);
        //Walls.Add(obj);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetRoomType(List<Puzzle> puzzles)
    {
        if (!LeftWall && !LowerWall && !RightWall && !UpperWall)
        {
            roomType = RoomTypes.LeftLowerRightUpper;
        }
        else if (!LowerWall && !RightWall && !UpperWall)
        {
            roomType = RoomTypes.LowerRightUpper;
        }
        else if (!LeftWall && !RightWall && !UpperWall)
        {
            roomType = RoomTypes.LeftRightUpper;
        }
        else if (!LeftWall && !LowerWall && !UpperWall)
        {
            roomType = RoomTypes.LeftLowerUpper;
        }
        else if (!LeftWall && !LowerWall && !RightWall)
        {
            roomType = RoomTypes.LeftLowerRight;
        }
        else if (!RightWall && !UpperWall)
        {
            roomType = RoomTypes.RightUpper;
        }
        else if (!LowerWall && !UpperWall)
        {
            roomType = RoomTypes.LowerUpper;
        }
        else if (!LowerWall && !RightWall)
        {
            roomType = RoomTypes.LowerRight;
        }
        else if (!LeftWall && !UpperWall)
        {
            roomType = RoomTypes.LeftUpper;
        }
        else if (!LeftWall && !RightWall)
        {
            roomType = RoomTypes.LeftRight;
        }
        else if (!LeftWall && !LowerWall)
        {
            roomType = RoomTypes.LeftLower;
        }
        else if (!UpperWall)
        {
            roomType = RoomTypes.Upper;
        }
        else if (!RightWall)
        {
            roomType = RoomTypes.Right;
        }
        else if (!LowerWall)
        {
            roomType = RoomTypes.Lower;
        }
        else if (!LeftWall)
        {
            roomType = RoomTypes.Left;
        }

        var validPuzzles = puzzles.Where(p => p.roomType == this.roomType).ToList();
        if(validPuzzles.Count > 0)
        {
            puzzle = validPuzzles[Random.Range(0, validPuzzles.Count)];
            puzzle.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            var obj = Instantiate(puzzle);
            obj.transform.parent = gameObject.transform;
        }
    }

    public override string ToString()
    {
        return $"Position: {Pos}\r\nRoom Type: {roomType}\r\nIs Goal: {IsGoal}";
    }
}
