using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayManager : MonoBehaviour
{
    [SerializeField] BallController ballController;
    [SerializeField] CameraController camController;
    [SerializeField] GameObject finishWindow;
    [SerializeField] TMP_Text finishText;
    [SerializeField] TMP_Text shootCountText;
    [SerializeField] GameObject optionsPanel;

    private const string saveLevel = "SAVELEVEL";
    private bool isBallOutside;
    private bool isBallTeleporting;
    private bool isGoal;
    private Vector3 lastBallPosition;
    public float idLevel;
    public UnityEvent GoalSound;


    private void OnEnable()
    {
        ballController.onBallShooted.AddListener(UpdateShootCount);
    }

    private void OnDisable()
    {
        ballController.onBallShooted.RemoveListener(UpdateShootCount);
    }

    // Update is called once per frame
    void Update()
    {

        if (ballController.ShootingMode)
        {
            lastBallPosition = ballController.transform.position;
        }

        var inputActive = Input.GetMouseButton(0)
            && ballController.IsMove() == false
            && ballController.ShootingMode == false
            && isBallOutside == false;

        camController.SetInputActive(inputActive);
    }

    public void OnBallGoalEnter()
    {

        isGoal = true;
        ballController.enabled = false;
        var lastLevel = PlayerPrefs.GetInt(saveLevel);
        if (lastLevel == idLevel)
        {
            lastLevel += 1;
            PlayerPrefs.SetInt(saveLevel, lastLevel);
        }
        finishWindow.gameObject.SetActive(true);
        finishText.text = "Finish!!\n" + "Level Completed.\n" + "Shoot Count: " + ballController.ShootCount;
        shootCountText.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(false);
        GoalSound.Invoke();
    }

    public void OnBallOutside()
    {

        if (isGoal)
        {
            return;
        }
        if (isBallTeleporting == false)
        {
            Invoke("TeleportBallLastPosition", 3);
        }
        ballController.enabled = false;
        isBallOutside = true;
        isBallTeleporting = true;
    }

    public void TeleportBallLastPosition()
    {
        TeleportBall(lastBallPosition);
    }

    public void TeleportBall(Vector3 targetPosition)
    {
        var rb = ballController.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        ballController.transform.position = targetPosition;
        rb.isKinematic = false;

        ballController.enabled = true;
        isBallOutside = false;
        isBallTeleporting = false;
    }

    public void UpdateShootCount(int shootCount)
    {
        shootCountText.text = shootCount.ToString();
    }
}
