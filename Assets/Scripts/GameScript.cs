using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{

    public Player PlayerObj;
    public Room RoomObj;
    public GameObject Maze;
    public Stairs stairs;
    public List<Puzzle> Puzzles;

    public HUD hud;
    
    public List<Room> RoomObjs;

    public static Player Player;
    public GameObject Goal;
    public GameObject WinningMessage;

    public int RoomCount;
    public int RoomWidth;

    private Room currentRoom;

    public List<Room> Rooms;
    private static readonly int Death = Animator.StringToHash("Death");

    private bool _deathAnimation;

    // Start is called before the first frame update
    void Start()
    {
        InitPlayer();
        _deathAnimation = false;
        
        InitRooms();
        GenerateMaze(Rooms[0]);
    }

    // Update is called once per frame
    void Update()
    {
        // camera follows the player;
        //gameObject.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);

        currentRoom = Rooms.FirstOrDefault(r =>
        {
            return r.transform.position.x <= Player.transform.position.x
                    && Player.transform.position.x <= r.transform.position.x + r.RoomWidth
                    && r.transform.position.y <= Player.transform.position.y
                    && Player.transform.position.y <= r.transform.position.y + r.RoomWidth;
        });

        transform.position = new Vector3(currentRoom.transform.position.x + currentRoom.RoomWidth / 2, currentRoom.transform.position.y + currentRoom.RoomWidth / 2, transform.position.z);

        if (currentRoom.IsGoal)
        {
            //WinningMessage.SetActive(true);
        }

        // if (Player.CurrentHP <= 0)
        // {
        //     StartCoroutine(PlayerDeath());
        // }
    }

    private IEnumerator PlayerDeath()
    {
        Player.animator.SetTrigger(Death);
        yield return new WaitForSeconds(5);
        Destroy(Player.gameObject);
        InitPlayer();
    }

    private void FixedUpdate()
    {

    }

    private void InitPlayer()
    {
        GameScript.Player = Instantiate(PlayerObj);
        Player.transform.position = new Vector3(6, 6, 0);
        Player.transform.gameObject.name = "Player";

        hud.Player = Player;
        
    }

    private void InitRooms()
    {
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
                obj.transform.parent = Maze.gameObject.transform;
                Rooms.Add(obj);
            }
        }

        hud.Init();

    }

    public void ReGenerateMaze()
    {
        var pos = currentRoom.transform.position;
        var mazePos = currentRoom.Pos;
        // Debug.Log($"{pos}");
        Player.transform.position = new Vector3(pos.x + 6, pos.y + 6, 0);
        foreach (Transform child in Maze.transform)
        {
            Destroy(child.gameObject);
        }
        Rooms = new List<Room>();
        InitRooms();
        currentRoom = GetRoomByIndex((int)mazePos.x, (int)mazePos.y);
        GenerateMaze(currentRoom);
    }

    private void GenerateMaze(Room startPosition)
    {
        // Debug.Log(startPosition);
        //Random.InitState(42);
        //Debug.Log("Generating Maze");
        var stack = new List<Room>();
        var current = GetRoomByIndex((int)startPosition.Pos.x, (int)startPosition.Pos.y);
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

        foreach (var room in Rooms)
        {
            room.SetRoomType(Puzzles);
        }

        TraverseMaze(startPosition, 1);
        var goal = Rooms.OrderByDescending(item => item.DistanceFromStart).First();
        goal.IsGoal = true;
        // Debug.Log(goal);
        var obj = Instantiate(Goal);
        
        obj.transform.position = new Vector3(goal.transform.position.x, goal.transform.position.y, goal.transform.position.z);
        obj.transform.parent = Maze.transform;

        var stair = Instantiate(stairs);
        
        stair.transform.position = new Vector3(goal.transform.position.x + 5, goal.transform.position.y + 5, goal.transform.position.z - 10);
        stair.transform.parent = Maze.transform;
        stair.gs = this;
        
        // SetLayerRecursively(Maze.gameObject, LayerMask.NameToLayer("Walls"));
    }
    
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
   
        foreach( Transform child in obj.transform )
        {
            SetLayerRecursively( child.gameObject, newLayer );
        }
    }


    private void TraverseMaze(Room room, int distance)
    {
        if (room.DistanceFromStart != 0)
        {
            return;
        }
        room.DistanceFromStart = distance;
        foreach (var neighbor in room.Neighbors)
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

        if (neighbors.Count == 0)
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
