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

    [Header("Tunes")]
    [SerializeField] List<GameObject> tuneList;


    [Header("Customer Settings")]
    [SerializeField] private GameObject customerPrefab;

    [Header("Spawn Anchors")]
    [SerializeField] private Transform customer1Anchor;
    [SerializeField] private Transform customer2Anchor;
    [SerializeField] private Transform customer3Anchor;

    [SerializeField] private int difficulty = 0;

    private Transform[] anchors;

    private void Awake()
    {
        anchors = new Transform[] { customer1Anchor, customer2Anchor, customer3Anchor };
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
        // Clear any existing customers first (optional)
        foreach (Transform anchor in anchors)
        {
            if (anchor.childCount > 0)
            {
                for (int i = anchor.childCount - 1; i >= 0; i--)
                    Destroy(anchor.GetChild(i).gameObject);
            }
        }

        // Determine possible tune types
        List<TuneType> availableTypes = new List<TuneType> { TuneType.Percussion, TuneType.Bass };
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
        for (int i = 0; i < numCustomers && i < anchors.Length; i++)
        {
            GameObject customerObj = Instantiate(customerPrefab, anchors[i].position, anchors[i].rotation, anchors[i]);
            Customer customer = customerObj.GetComponent<Customer>();

            if (customer != null)
            {
                customer.requirements.Clear();
                int numRequirements = Random.Range(1, requirementCap + 1);

                for (int r = 0; r < numRequirements; r++)
                {
                    TuneType randomType = availableTypes[Random.Range(0, availableTypes.Count)];
                    customer.requirements.Add(randomType);
                }
                if (i == rewardIndex && rewardTune != null)
                {
                    customer.rewardTune = rewardTune;
                }
                customer.ShowCustomer();
            }
        }
    }

    public void RemoveTune(GameObject tunePrefab)
    {
        tuneList.Remove(tunePrefab);
    }
}
