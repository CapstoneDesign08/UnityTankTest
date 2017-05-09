using UnityEngine;
using UnityEngine.UI;

public class TestInfo : MonoBehaviour {

    public GameObject overlay;
    public TestManager testManager;
    public Text testText;

    void Awake()
    {
        ShowLaunchScreen();
        testManager = GameObject.Find("GameManager").GetComponent<TestManager>();
    }

    public void ShowLaunchScreen()
    {
        overlay.SetActive(true);
	}

	public void StartGame()
    {
        overlay.SetActive(false);
        Time.timeScale = 1f;
        testManager.SceneSetup(TestManager.testNum);
        testManager.StartCoroutine(testManager.GradeTest(TestManager.testNum));
    }

    public void NextTest()
    {
        if (TestManager.testNum < TestManager.maxTestNum)
        {
            TestManager.testNum++;
			testText.GetComponent<Text>().text = "TEST NUMBER : " + TestManager.testNum;
        }
	}

    public void PreviousTest()
    {
        if (TestManager.testNum > 1)
        {
            TestManager.testNum--;
			testText.GetComponent<Text>().text = "TEST NUMBER : " + TestManager.testNum;
        }

    }
}
