using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region ------Field-----
	TankMovement tankmovement;
	Queue<Node> node_queue = new Queue<Node>();
	private int[,] map = new int[17, 17];
	private bool[,] visited = new bool[17, 17];
	private int[,] dir = new int[8, 2] { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 } };
	private GameObject[] walls;
	private GameObject[] Tanks;
	private Node path;
	private MoveState m_moveState;
	#endregion

	#region -----Class-----
	private class Node
	{
		public float degree;
		public float distance;
		public float value_factor;
		public int x, z;
		public Node prev_node;
		public Node next_node;

		public Node(float degree, float distance, int x, int z)
		{
			this.degree = degree;
			this.distance = distance;
			value_factor = degree + distance;
			this.x = x;
			this.z = z;
		}
	}

	private enum MoveState : int
	{
		IDLE,
		TURNING,
		MOVING,
		COLLIDE,
		TURNED,
		MOVED        
	}
	#endregion

	#region ------Public Method-----
	//map 과 방문정보 초기화
	void Awake()
	{
		m_moveState = MoveState.IDLE;
	}

	void Start()
	{
		StartCoroutine(BasicLogic());        
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void MoveStatus(int status)
	{
		switch (status)
		{
		case 0:
			m_moveState = MoveState.IDLE;
			break;
		case 1:
			m_moveState = MoveState.TURNING;
			break;
		case 2:
			m_moveState = MoveState.MOVING;
			break;
		case 3:
			m_moveState = MoveState.COLLIDE;
			break;
		case 4:
			m_moveState = MoveState.TURNED;
			break;
		case 5:
			m_moveState = MoveState.MOVED;
			break;
		}
	}

	IEnumerator BasicLogic()
	{
		yield return new WaitForSeconds(0.1f);
		Debug.Log("Start");
		yield return StartCoroutine(MovePath(new Vector3(35, 0, 35)));
	}

	IEnumerator MoveTo(Vector3 p)
	{
		if (m_moveState == MoveState.IDLE)
			tankmovement.TurnTo(p);

		while (m_moveState == MoveState.TURNING)
		{
			yield return null;
		}
		if (m_moveState == MoveState.TURNED)
		{
			tankmovement.MoveTo(p);
			while (m_moveState == MoveState.MOVING)
			{
				yield return null;
			}
		}
    }

	IEnumerator MovePath(Vector3 p)
	{
		MapInitialize();

		tankmovement = GameObject.FindWithTag("Tank").GetComponent<TankMovement>();

		tankmovement.moveStatusDelegate = MoveStatus;

		//초기화
		node_queue = new Queue<Node>();
		Node n = new Node(0, 0, (int)(( 40 + GameObject.FindWithTag("Tank").transform.position.x )/5), (int)((40 - GameObject.FindWithTag("Tank").transform.position.z ) / 5));
		n.prev_node = null;
		node_queue.Enqueue(n);
		path = FindPath(p);
		while (path != null)
		{
			if (path.prev_node != null)
			{
				path.prev_node.next_node = path;
				path = path.prev_node;
			}
			else
				break;
		}

		path = path.next_node;
		yield return new WaitForSeconds(0.01f);
		
		while (m_moveState == MoveState.IDLE)
		{
			if (path != null) {
				yield return StartCoroutine(MoveTo(new Vector3((path.x * 5) - 40, 0, 40 - (path.z * 5))));
			}
			else
				break;
			if (m_moveState == MoveState.MOVED)
			{
				path = path.next_node;
				m_moveState = MoveState.IDLE;
			}
		}
		yield return new WaitForSeconds(0.1f);
	}
	#endregion

	#region -----Private Method-----
	//Wall 의 위치정보를 읽어와 map 에 매핑
	private void ScanMap()
	{
		walls = GameObject.FindGameObjectsWithTag("wall");
		foreach (GameObject wall in walls)
		{
			int z = (int)((40 - wall.transform.position.z) / 5);
			int x = (int)((40 + wall.transform.position.x) / 5);

			if (wall.transform.rotation.y != 0)
			{
				map[x, z] = 1;
				map[x, z - 1] = 1;
				map[x, z + 1] = 1;
			}
			else
			{
				map[x, z] = 1;
				map[x + 1, z] = 1;
				map[x - 1, z] = 1;
			}
		}
	}

	//Path Finding
	private Node FindPath(Vector3 p)
	{
		while (node_queue.Count != 0)
		{
			Node tmp = node_queue.Peek();
			node_queue.Dequeue();
			if (tmp.x == (int)(( p.x + 40) /5) && tmp.z == (int)((40 - p.z) / 5))
			{
				return tmp;
			}
			FindDirection (tmp);
		}
		return null;
	}

	//이동가능한 방향 탐색
	private void FindDirection(Node n)
	{
		int x = n.x;
		int z = n.z;
		for (int i = 0; i < 8; i++)
		{            
			int tmp_x = x + dir[i, 0];
			int tmp_z = z + dir[i, 1];

			if (tmp_x > 0 && tmp_z > 0 && tmp_x < 16 && tmp_z < 16 && map[tmp_x, tmp_z] == 0 && visited[tmp_x, tmp_z] == false)
			{
				visited[tmp_x, tmp_z] = true;
				// float distance = Vector3.Distance(new Vector3(tmp_x, 0, tmp_z), p);
				Node tmp = new Node(0, 0, tmp_x, tmp_z);
				tmp.prev_node = n;
				node_queue.Enqueue(tmp);
			}
		}
	}

	private void MapInitialize()
	{
		for (int i = 0; i < 17; i++)
			for (int j = 0; j < 17; j++)
			{
				map[i, j] = 0;
				visited[i, j] = false;
			}
		ScanMap ();
	}
	#endregion
}