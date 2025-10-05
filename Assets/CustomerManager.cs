using UnityEngine;

public enum TuneType
{
    Percussion,
    Bass,
    Melody
}

public class CustomerManager : MonoBehaviour
{
    /*
     *  Spawning strategy: We scale customers by difficulty rating. Every satisfied customer increases difficulty by 5, every fail decreases it by 2.
     *  0-9: Only percussion and bass will be requirements, one customer, max 1 requirement
     *  10-19: Only percussion and bass, 2 customers, max 1 requirement
     *  20-29: Percussion bass and melody, 2 customers, max 2 requirement
     *  30-49: All three, 3 customers, max 2 requirement
     *  50-69: All three, 3 customers, max 3 requirement
     *  70-100: All three, 3 customers, max 4 requirement 
     */

    public void SpawnCustomers(bool includeMelody, int numCustomers, int requirementCap)
    {

    }



}
