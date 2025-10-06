using System.Collections.Generic;
using UnityEngine;

public enum TuneType
{
    Percussion,
    Bass,
    Melody
}

[System.Serializable]
public class TuneTypeDamage
{
    public TuneType Type;
    public int Damage;
}

public class CustomerManager : MonoBehaviour
{
    /*
     * Spawning strategy: We scale customers by difficulty rating. Every satisfied customer increases difficulty by 5, every fail decreases it by 2.
     * 0-9: Only percussion and bass will be requirements, one customer, max 1 requirement
     * 10-19: Only percussion and bass, 2 customers, max 1 requirement
     * 20-29: Percussion bass and melody, 2 customers, max 2 requirement
     * 30-49: All three, 3 customers, max 2 requirement
     * 50-69: All three, 3 customers, max 3 requirement
     * 70-100: All three, 3 customers, max 4 requirement
     */

    public static CustomerManager Instance;

    [Header("Tunes")]
    [SerializeField] List<GameObject> tuneList;



    [Header("Customers")]
    [SerializeField] List<Customer> customers;


    public int Difficulty { get { return difficulty; } }
    [SerializeField] private int difficulty = 0;

    HashSet<Customer> activeCustomers = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Debug
        SpawnCustomersByDifficulty();
    }

    /// <summary>
    /// Adjust difficulty when a customer is satisfied (+5) or failed (-2).
    /// </summary>
    public void AdjustDifficulty(bool success)
    {
        difficulty += success ? 5 : -2;
        difficulty = Mathf.Clamp(difficulty, 0, 100);
    }

    /// <summary>
    /// Determines the spawn parameters based on current difficulty and spawns customers.
    /// </summary>
    public void SpawnCustomersByDifficulty()
    {
        bool includeMelody = false;
        int numCustomers = 1;
        int requirementCap = 1;

        if (difficulty < 10)
        {
            includeMelody = false;
            numCustomers = 1;
            requirementCap = 1;
        }
        else if (difficulty < 20)
        {
            includeMelody = false;
            numCustomers = 2;
            requirementCap = 1;
        }
        else if (difficulty < 30)
        {
            includeMelody = true;
            numCustomers = 2;
            requirementCap = 2;
        }
        else if (difficulty < 50)
        {
            includeMelody = true;
            numCustomers = 3;
            requirementCap = 2;
        }
        else if (difficulty < 70)
        {
            includeMelody = true;
            numCustomers = 3;
            requirementCap = 3;
        }
        else
        {
            includeMelody = true;
            numCustomers = 3;
            requirementCap = 4;
        }

        SpawnCustomers(includeMelody, numCustomers, requirementCap);
    }

    /// <summary>
    /// Spawns customers and assigns random requirements based on parameters.
    /// </summary>
    public void SpawnCustomers(bool includeMelody, int numCustomers, int requirementCap)
    {
        // Determine possible tune types
        var availableTypes = new List<TuneType> { TuneType.Percussion, TuneType.Bass };
        if (includeMelody)
            availableTypes.Add(TuneType.Melody);

        // Fetch a random reward tune and customer to hold the reward
        GameObject rewardTune = null;
        if (tuneList.Count > 0)
        {
            rewardTune = tuneList[Random.Range(0, tuneList.Count)];
        }
        int rewardIndex = Random.Range(0, numCustomers);

        // Spawn
        for (int i = 0; i < numCustomers; i++)
        {
            var customer = customers[i];
            customer.gameObject.SetActive(true);

            customer.requirements.Clear();
            int numRequirements = Random.Range(1, requirementCap + 1);

            for (int r = 0; r < numRequirements; r++)
            {
                var idx = Random.Range(0, availableTypes.Count);
                TuneType randomType = availableTypes[idx];
                customer.requirements.Add(randomType);
            }
            if (i == rewardIndex && rewardTune != null)
            {
                Debug.Log("ASSIGN REWARD TUNE");
                customer.rewardTune = rewardTune;
            }
            activeCustomers.Add(customer);
            customer.ShowCustomer();
        }
        for (int i = numCustomers; i < customers.Count; i++)
        {
            customers[i].gameObject.SetActive(false);
        }

        EventBus.CustomersSpawnedEvent.Invoke();
    }

    public void DeactivateCustomer(Customer cust)
    {
        activeCustomers.Remove(cust);
        if (activeCustomers.Count == 0)
        {
            SpawnCustomersByDifficulty();
        }
    }

    public void RemoveTune(GameObject tunePrefab)
    {
        tuneList.Remove(tunePrefab);
    }
}
