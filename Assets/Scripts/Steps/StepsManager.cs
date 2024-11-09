using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that manages the steps that need to be completed in order to finish the game
/// </summary>
public class StepsManager : MonoBehaviour, IDataPersistence
{
    // Singleton pattern
    private static StepsManager instance;

    public static StepsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StepsManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<StepsManager>();
                    singletonObject.name = typeof(StepsManager).ToString() + " (Singleton)";
                }
            }
            return instance;
        }
    }

    private List<StepData> steps;
    private StepData currentStep;

    [Header("Sound")]
    [Tooltip("Make a sound when a step is completed")]
    [SerializeField] public bool makeSound = true;
    [Tooltip("Sound to play when a step is completed")]
    [HideInInspector]
    public AudioSource stepCompletedSound;
    [Header("Events")]
    public Action<List<StepData>> onSceneStepsLoaded;
    public Action<StepData> onManagerStepCompleted;
    public Action onAllStepsCompleted;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        LocalXRINetworkGameManager.Instance.onGameStarted += onGameStartedCallback;
    }

    private void onGameStartedCallback()
    {
        List<SimpleStep> simpleSteps = new List<SimpleStep>(FindObjectsOfType<SimpleStep>());
        simpleSteps.Sort((a, b) => a.GetStepNumber().CompareTo(b.GetStepNumber()));
        steps = new List<StepData>();

        foreach (var step in simpleSteps)
        {
            step.onStepCompleted += onStepCompletedCallback;
            steps.Add(step.GetStepData());
        }

        if (CheckForDuplicateSteps() || CheckForMissingSteps())
        {
            Debug.LogError("StepsManager: Steps are not set up correctly");
        }

        currentStep = StepData.GetEmptyStep();

        onSceneStepsLoaded.Invoke(steps);

        Debug.Log("StepsManager initialized with " + steps.Count + " steps");
    }


    private void onStepCompletedCallback(StepData completedStep)
    {
        //Tuve que poner esto por un error que nunca entendi.
        steps[completedStep.GetStepNumber() - 1] = completedStep;

        StepCompleted(completedStep);

        if (makeSound && stepCompletedSound != null)
        {
            stepCompletedSound.Play();
        }

        currentStep = completedStep;
    }

    private void StepCompleted(StepData completedStep)
    {
        onManagerStepCompleted.Invoke(completedStep);

        if (completedStep.GetStepNumber() == steps.Count)
        {
            onAllStepsCompleted.Invoke();
            LocalXRINetworkGameManager.Instance.FinishGame();
        }
    }

    public void ResetSteps()
    {
        currentStep = steps[0];
        foreach (var step in steps)
        {
            step.resetStep();
        }
    }

    public StepData GetCurrentStep()
    {
        return currentStep;
    }

    public int GetTotalSteps()
    {
        return steps.Count;
    }

    private bool CheckForDuplicateSteps()
    {
        HashSet<int> seenStepNumbers = new HashSet<int>();

        foreach (var step in steps)
        {
            int stepNumber = step.GetStepNumber();
            if (seenStepNumbers.Contains(stepNumber))
            {
                Debug.LogError("Two steps with the same number found: " + stepNumber);
                return true;
            }
            else
            {
                seenStepNumbers.Add(stepNumber);
            }
        }
        return false;
    }

    private bool CheckForMissingSteps()
    {
        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i].GetStepNumber() != i + 1)
            {
                Debug.LogError("Missing step with number: " + (i + 1));
                return true;
            }
        }
        return false;
    }

    public List<StepData> GetAllSteps()
    {
        return steps;
    }

    public void LoadData(GameData data)
    {
        steps = data.stepsList;
        currentStep = steps.Find(step => !step.IsCompleted()) ?? steps[^1];

        foreach (StepData step in steps)
        {
            Debug.Log("Step: " + step.toJson());
        }


    }

    // We pass the data by reference so we can modify it
    public void SaveData(ref GameData data)
    {
        if (data.stepsList == null)
        {
            data.stepsList = new List<StepData>();
        }

        data.stepsList = steps;

        //get last step
        StepData lastStep = steps[steps.Count - 1];
        Debug.Log("Last step: " + lastStep.toJson());
    }

    // public string ToJson()
    // {
    //     string json = JsonConvert.SerializeObject(this);
    //     Debug.Log(json);
    //     return json;
    // }

}
