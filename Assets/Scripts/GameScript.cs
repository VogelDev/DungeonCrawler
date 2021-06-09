using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameScript : MonoBehaviour
{

    public GameObject PlayerObj;
    public Room RoomObj;
    public GameObject Player;
    public GameObject Goal;
    public GameObject WinningMessage;

    public int RoomCount;
    public int RoomWidth;

    public List<Room> Rooms;

    // Start is called before the first frame update
    void Start()
    {
        Player = Instantiate(PlayerObj);
        Player.transform.position = new Vector3(6, 6, 0);
        var count = 0;
        for (var i = 0; i < RoomCount; i++)
        {
            for (var j = 0; j < RoomCount; j++)
            {
                var obj = Instantiate(RoomObj);
                obj.RoomNumber = ++count;
                obj.RoomWidth = RoomWidth;
                obj.Pos = new Vector2(i, j);

                obj.transform.position = new Vector3(i * RoomWidth, j * RoomWidth, 0);
                //if (i == 0)
                //{
                //    obj.LeftWall = true;
                //}
                //if (i == RoomCount - 1)
                //{
                //    obj.RightWall = true;
                //}
                //if (j == 0)
                //{
                //    obj.LowerWall = true;
                //}
                //if (j == RoomCount - 1)
                //{
                //    obj.UpperWall = true;
                //}
                Rooms.Add(obj);
            }
        }
        GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        // camera follows the player;
        //gameObject.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);

        var currentRoom = Rooms.FirstOrDefault(r =>
        {
            return r.transform.position.x <= Player.transform.position.x
                    && Player.transform.position.x <= r.transform.position.x + r.RoomWidth
                    && r.transform.position.y <= Player.transform.position.y
                    && Player.transform.position.y <= r.transform.position.y + r.RoomWidth;
        });

        transform.position = new Vector3(currentRoom.transform.position.x + currentRoom.RoomWidth / 2, currentRoom.transform.position.y + currentRoom.RoomWidth / 2, transform.position.z);

        if (currentRoom.IsGoal)
        {
            WinningMessage.SetActive(true);
        }
    }

    private void FixedUpdate()
    {

    }

    private void GenerateMaze()
    {
        //Random.InitState(42);
        Debug.Log("Generating Maze");
        var stack = new List<Room>();
        var current = Rooms[0];
        while (Rooms.Any(r => !r.Visited))
        {
            current.Visited = true;
            var next = RandomNeighbor(current);
            if (next != null)
            {
                next.Visited = true;

                // STEP 2
                stack.Insert(0, current);

                // STEP 3
                RemoveWalls(current, next);

                // STEP 4
                current = next;
            }
            else if (stack.Count > 0)
            {
                current = stack[0];
                stack.RemoveAt(0);
            }
        }

        TraverseMaze(Rooms[0], 1);
        var goal = Rooms.OrderByDescending(item => item.DistanceFromStart).First();
        goal.IsGoal = true;
        var obj = Instantiate(Goal);
        obj.transform.position = new Vector3(goal.transform.position.x, goal.transform.position.y, goal.transform.position.z);
    }

    private void TraverseMaze(Room room, int distance)
    {
        if(room.DistanceFromStart != 0)
        {
            return;
        }
        room.DistanceFromStart = distance;
        foreach(var neighbor in room.Neighbors)
        {
            TraverseMaze(neighbor, distance + 1);
        }
    }

    private Room RandomNeighbor(Room room)
    {
        var neighbors = new List<Room>();

        //if(room.RoomNumber == 2)
        //{
        //    Debug.Log($"room 2!!!!!!!!!!!!!!!!!!!!!!!");
        //    Debug.Log($"{room.Pos.x}, {room.Pos.y}");
        //}

        var top = GetRoomByIndex((int)room.Pos.x, (int)room.Pos.y + 1);
        var bottom = GetRoomByIndex((int)room.Pos.x, (int)room.Pos.y - 1);
        var left = GetRoomByIndex((int)room.Pos.x - 1, (int)room.Pos.y);
        var right = GetRoomByIndex((int)room.Pos.x + 1, (int)room.Pos.y);

        if (top != null && !top.Visited)
        {
            neighbors.Add(top);
        }
        if (bottom != null && !bottom.Visited)
        {
            neighbors.Add(bottom);
        }
        if (left != null && !left.Visited)
        {
            neighbors.Add(left);
        }
        if (right != null && !right.Visited)
        {
            neighbors.Add(right);
        }

        if(neighbors.Count == 0)
        {
            return null;
        }

        var neighborToCheck = Random.Range(0, neighbors.Count);
        return neighbors[neighborToCheck];

    }

    private void RemoveWalls(Room room, Room neighbor)
    {
        room.Neighbors.Add(neighbor);
        neighbor.Neighbors.Add(room);

        if (room.Pos.x < neighbor.Pos.x)
        {
            room.RightWall = false;
            neighbor.LeftWall = false;
        }
        else if (room.Pos.x > neighbor.Pos.x)
        {
            neighbor.RightWall = false;
            room.LeftWall = false;
        }
        else if (room.Pos.y < neighbor.Pos.y)
        {
            room.UpperWall = false;
            neighbor.LowerWall = false;
        }
        else if (room.Pos.y > neighbor.Pos.y)
        {
            neighbor.UpperWall = false;
            room.LowerWall = false;
        }
    }

    private Room GetRoomByIndex(int x, int y)
    {
        if (x < 0 || y < 0 || x > RoomCount - 1 || y > RoomCount - 1)
        {
            return null;
        }
        return Rooms[y + x * RoomCount];
    }
}