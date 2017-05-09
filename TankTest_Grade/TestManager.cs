using System.Collections;
using UnityEngine;

public class TestManager : MonoBehaviour {
    #region -----Public Field-----
    public int colums = 9;
    public int rows = 9;
    public static int testNum = 4;
    public static int maxTestNum = 4;
    public static int[] scores;
    public int waitingTime = 60;
    public int[][,] maps = new int[5][,];
    #endregion
    #region -----GameObject-----
    public GameObject wall;
    public GameObject tankPrefab;
    public GameObject playerTank;
    public GameObject spawnPoint;
    public GameObject player;
    public GameObject playerPrefab;
    public GameObject goal;
    #endregion
    #region ------Map Information-----

    public int[,] map1 = new int[9, 9] { {3, 1, 1, 1, 1, 1, 1, 1, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {1, 1, 1, 1, 1, 1, 1, 1, 0 }};
    public int[,] map2 = new int[9, 9] { {3, 1, 1, 1, 1, 1, 1, 1, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 3, 0, 0, 2 },
                                        {2, 0, 0, 0, 0, 0, 3, 1, 2 },
                                        {2, 0, 0, 0, 0, 0, 2, 0, 2 },
                                        {1, 1, 1, 1, 1, 1, 1, 1, 0 }};
    public int[,] map3 = new int[9, 9] { {3, 1, 3, 1, 1, 1, 1, 1, 2 },
                                        {2, 0, 1, 1, 0, 0, 3, 0, 2 },
                                        {3, 1, 1, 1, 1, 0, 0, 0, 2 },
                                        {2, 0, 3, 1, 1, 1, 3, 1, 2 },
                                        {2, 1, 2, 1, 0, 2, 0, 0, 2 },
                                        {2, 0, 2, 1, 3, 1, 1, 2, 2 },
                                        {3, 0, 2, 2, 0, 0, 0, 2, 2 },
                                        {2, 0, 1, 1, 1, 1, 0, 2,2 },
                                        {1, 1, 1, 1, 1, 1, 1, 1, 0 }};
    public int[,] map4 = new int[9, 9] { {3, 1, 3, 1, 1, 1, 1, 1, 2 },
                                        {2, 0, 2, 0, 0, 0, 0, 0, 2 },
                                        {2, 0, 3, 0, 3, 0, 3, 0, 2 },
                                        {2, 0, 0, 0, 2, 0, 0, 0, 2 },
                                        {2, 1, 3, 1, 3, 1, 3, 0, 2 },
                                        {3, 0, 0, 0, 3, 0, 0, 0, 2 },
                                        {2, 0, 3, 0, 0, 0, 3, 0, 2 },
                                        {2, 0, 0, 0, 2, 0, 0, 0, 2 },
                                        {1, 1, 1, 1, 1, 1, 1, 1, 0 }};
    #endregion
    #region -----Private Field-----
    private GameObject boardHolder;
    private string address = "http://192.168.1.184:3000/api/unitys/scores/1";
    private Vector3 horOrigin = new Vector3(40f, 2.5f, 35);
    private Vector3 verOrigin = new Vector3(35f, 2.5f, 40);
    #endregion

    private void Awake()
    {
        maps[1] = map1;
        maps[2] = map2;
        maps[3] = map3;
        maps[4] = map4;
        scores = new int[5];
    }
    void Start()
    {
        Time.timeScale = 4.0F;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        StartCoroutine(GradeLoop());
    }

    void Update()
    {
    }
    #region ------Public Grade Method-----

    public IEnumerator GradeLoop()
    {
        for (int i = 1; i <= 4; i++)
        {
            SceneSetup(i);
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(GradeTest(i));
            yield return new WaitForSeconds(2f);
        }

        Application.Quit();
    }

    public IEnumerator GradeTest(int n)
    {
        player = Instantiate(playerPrefab, gameObject.transform.position, Quaternion.identity);

        for (int i = 0; i < 100; i++)
        {
            if (n == 2 && i >= 10)
                break;

            if (TankMovement.status != 3 && Vector3.Distance(playerTank.transform.position, goal.transform.position) > 0.5f)
                yield return new WaitForSeconds(1f);
            else
                 break;
            
        }

        Debug.Log("end " + Time.time);

        if (Vector3.Distance(playerTank.transform.position, goal.transform.position) < 0.5f)
            scores[n] = 25;
        else
        {
            if (n == 2 && playerTank.transform.position == new Vector3(-35, 0, -35))       // 두번째 scene일때
                scores[n] = 25;
            else
                scores[n] = 0;
        }

        Debug.Log("SCORE : " + scores[n]);
        WWW wwwUrl = new WWW(address + "?user_id=" + Player_ID.playerId + "&test_num=" + n + "&score=" + scores[n]);
        yield return wwwUrl;
    }

    #endregion
    #region -----Private Method-----
    private void BoardSetup(int n)
    {
        boardHolder = new GameObject("Board");

        for (int x = 0; x < colums; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int type = maps[n][y, x];

                switch (type)
                {
                    case 1:
                        GameObject instanceHor = Instantiate(wall, horOrigin + new Vector3(-(y * 10), 0f, -(x * 10)), Quaternion.Euler(0f, 90f, 0f)) as GameObject;
                        instanceHor.transform.SetParent(boardHolder.transform);
                        break;
                    case 2:
                        GameObject instanceVer = Instantiate(wall, verOrigin + new Vector3(-(y * 10), 0f, -(x * 10)), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
                        instanceVer.transform.SetParent(boardHolder.transform);
                        break;
                    case 3:
                        instanceHor = Instantiate(wall, horOrigin + new Vector3(-(y * 10), 0f, -(x * 10)), Quaternion.Euler(0f, 90f, 0f)) as GameObject;
                        instanceHor.transform.SetParent(boardHolder.transform);
                        instanceVer = Instantiate(wall, verOrigin + new Vector3(-(y * 10), 0f, -(x * 10)), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
                        instanceVer.transform.SetParent(boardHolder.transform);
                        break;  
                    default:
                        break;

                }
            }
        }
    }

    private void TankSetup(){
        playerTank = Instantiate(tankPrefab, spawnPoint.transform.position, Quaternion.identity) as GameObject;
    }
    
    private void SceneSetup(int n) {
        SceneInitialize();
        BoardSetup(n);
        TankSetup();
    }

    private void SceneInitialize()
    {
        if (player != null)
            Destroy(player);
        if(boardHolder != null)
            Destroy(boardHolder);
        if (playerTank != null)
            Destroy(playerTank);
    }
    #endregion
}
