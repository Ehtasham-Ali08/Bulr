using System.Collections;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [Header("Race Settings")]
    public int totalLaps = 3;
    public float countdownTime = 3f;

    private bool[] checkpointPassed;

    [Header("Racers")]
    public RCC_CarControllerV4[] racers;

    [Header("Position Points")]
    public Transform[] positionPoints;

    [Header("Position Settings")]
    public float positionUpdateInterval = 0.1f;

    [Header("Position UI")]
    public PositionUI[] positionUI;

    [Header("UI")]
    public TMPro.TextMeshProUGUI lapText;
    public TMPro.TextMeshProUGUI finishText;
    public GameObject resultsPanel;
    public TMPro.TextMeshProUGUI resultsText;
    public TMPro.TextMeshProUGUI countdownText;

    private int[] currentLaps;
    private bool[] raceFinished;

    // Progress of each racer around the track
    private float[] racerProgress;

    // Stores racers in the order they finish
    private int[] finishOrder;

    // Number of racers that have finished
    private int finishedCount;

    private void Start()
    {
        currentLaps = new int[racers.Length];
        raceFinished = new bool[racers.Length];
        checkpointPassed = new bool[racers.Length];

        racerProgress = new float[racers.Length];
        finishOrder = new int[racers.Length];
        finishedCount = 0;

        for (int i = 0; i < racers.Length; i++)
        {
            currentLaps[i] = 1;
            raceFinished[i] = false;
            checkpointPassed[i] = false;
            racerProgress[i] = 0f;
        }

        if (finishText != null)
            finishText.gameObject.SetActive(false);
        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        UpdateLapUI(0);

        StartCoroutine(UpdateRacePositions());
        StartCoroutine(StartRaceCountdown());
    }


    // --------------------------------------------------
    // CHECKPOINT
    // --------------------------------------------------

    public void CarReachedCheckpoint(RCC_CarControllerV4 car)
    {
        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] != car)
                continue;

            if (raceFinished[i])
                return;

            checkpointPassed[i] = true;

            return;
        }
    }


    // --------------------------------------------------
    // LAP / FINISH SYSTEM
    // --------------------------------------------------

    public void CarCrossedFinish(RCC_CarControllerV4 car)
    {
        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] != car)
                continue;

            if (raceFinished[i])
                return;

            // Checkpoint must be reached first
            if (!checkpointPassed[i])
            {
                Debug.Log(
                    car.gameObject.name +
                    " crossed finish without passing checkpoint."
                );

                return;
            }

            // Final lap
            if (currentLaps[i] >= totalLaps)
            {
                FinishRace(i);
                return;
            }

            // Complete lap
            currentLaps[i]++;

            // Require checkpoint again
            checkpointPassed[i] = false;

            Debug.Log(
                car.gameObject.name +
                " completed lap " +
                (currentLaps[i] - 1)
            );

            UpdateLapUI(i);

            return;
        }
    }

        private void FinishRace(int racerIndex)
    {
        // Prevent finishing twice
        if (raceFinished[racerIndex])
            return;

        raceFinished[racerIndex] = true;

        RCC_CarControllerV4 car = racers[racerIndex];

        // Add racer to finish order
        finishOrder[finishedCount] = racerIndex;

        finishedCount++;

        if (finishedCount >= racers.Length)
        {
            Debug.Log("RACE COMPLETELY FINISHED!");

            ShowFinalResults();
        }

        int finalPosition = finishedCount;

        Debug.Log(
            car.gameObject.name +
            " FINISHED! Position: " +
            finalPosition
        );

        // Disable driving
        car.enabled = false;

        // Stop physics
        Rigidbody rb = car.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.drag = 0.6f;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Stop damage
        CarHealth health = car.GetComponent<CarHealth>();

        if (health != null)
        {
            health.SetRaceFinished();
        }

        // Show finish UI
        if (finishText != null)
        {
            finishText.gameObject.SetActive(true);

            finishText.text =
                car.gameObject.name +
                " FINISHED!";
        }
    }


    // --------------------------------------------------
    // POSITION SYSTEM
    // --------------------------------------------------

    private IEnumerator UpdateRacePositions()
    {
        while (true)
        {
            CalculatePositions();

            yield return new WaitForSeconds(
                positionUpdateInterval
            );
        }
    }


    private void CalculatePositions()
    {
        if (positionPoints == null ||
            positionPoints.Length < 2)
            return;

        // Calculate progress for every racer
        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] == null)
                continue;

            CalculateRacerProgress(i);
        }

        // Create racer order
        int[] order = new int[racers.Length];

        for (int i = 0; i < racers.Length; i++)
        {
            order[i] = i;
        }

        // Sort from furthest progress to lowest progress
        System.Array.Sort(order, (a, b) =>
        {
            return racerProgress[b]
                .CompareTo(racerProgress[a]);
        });

        // Assign positions
        for (int position = 0;
             position < order.Length;
             position++)
        {
            int racerIndex = order[position];

            Debug.Log(
            racers[racerIndex].gameObject.name +
            " | Progress: " +
            racerProgress[racerIndex] +
            " | Position: " +
            (position + 1)
        );

            if (racerIndex < positionUI.Length &&
                positionUI[racerIndex] != null)
            {
                positionUI[racerIndex]
                    .SetPosition(position + 1);
            }
        }
    }


    private void CalculateRacerProgress(int racerIndex)
    {
        Vector3 carPosition =
            racers[racerIndex].transform.position;

        float closestDistance = float.MaxValue;
        float bestProgress = 0f;

        int pointCount = positionPoints.Length;

        // Check every track segment
        for (int i = 0; i < pointCount; i++)
        {
            int nextIndex = i + 1;

            if (nextIndex >= pointCount)
                nextIndex = 0;

            Vector3 pointA =
                positionPoints[i].position;

            Vector3 pointB =
                positionPoints[nextIndex].position;

            Vector3 segment =
                pointB - pointA;

            float segmentLengthSquared =
                segment.sqrMagnitude;

            if (segmentLengthSquared < 0.001f)
                continue;

            // Find position of car along this segment
            float t =
                Vector3.Dot(
                    carPosition - pointA,
                    segment
                ) /
                segmentLengthSquared;

            t = Mathf.Clamp01(t);

            // Closest position on segment
            Vector3 closestPosition =
                pointA + segment * t;

            float distance =
                Vector3.SqrMagnitude(
                    carPosition -
                    closestPosition
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;

                // Exact position along track
                bestProgress = i + t;
            }
        }

        // Lap + track progress
        racerProgress[racerIndex] =
    (currentLaps[racerIndex] - 1) +
    bestProgress / pointCount; ;
    }

    // --------------------------------------------------
    // CountDown System
    // --------------------------------------------------
    private IEnumerator StartRaceCountdown()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();

            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";

        EnableRacerControls();

        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }

    private void EnableRacerControls()
    {
        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] == null)
                continue;

            PlayerCarInput input =
                racers[i].GetComponent<PlayerCarInput>();

            if (input != null)
            {
                input.raceStarted = true;
            }
        }
    }


    // --------------------------------------------------
    // LAP UI
    // --------------------------------------------------

    private void UpdateLapUI(int racerIndex)
    {
        if (lapText == null)
            return;

        lapText.text =
            "Lap " +
            currentLaps[racerIndex] +
            " / " +
            totalLaps;
    }
    private string GetPositionSuffix(int position)
    {
        if (position % 100 >= 11 && position % 100 <= 13)
            return "th";

        switch (position % 10)
        {
            case 1:
                return "st";

            case 2:
                return "nd";

            case 3:
                return "rd";

            default:
                return "th";
        }
    }

    private void ShowFinalResults()
    {
        Debug.Log("===== FINAL RESULTS =====");

        string results = "RACE FINISHED!\n\n";

        for (int i = 0; i < finishedCount; i++)
        {
            int racerIndex = finishOrder[i];

            string position =
                (i + 1) +
                GetPositionSuffix(i + 1);

            string racerName =
                racers[racerIndex].gameObject.name;

            Debug.Log(position + " - " + racerName);

            results += position + "   " + racerName + "\n";
        }

        Debug.Log("=========================");

        // Show results UI
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }

        if (resultsText != null)
        {
            resultsText.text = results;
        }
    }
}