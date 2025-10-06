using UnityEngine;
using System.Collections.Generic;

public class TuneMenuManager : MonoBehaviour
{
    public static TuneMenuManager Instance = null;

    [SerializeField] GameObject contentObject;
    [SerializeField] GameObject iconPrefab;

    [SerializeField] List<GameObject> tuneList = new List<GameObject>();

    Dictionary<int, GameObject> typeToTuneDict = new Dictionary<int, GameObject>();

    private void Awake()
    {
        Instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowTunes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTune(GameObject gameObject)
    {
        tuneList.Add(gameObject);
        // TODO: add sorting of tunes by type
    }

    public void ShowTunes()
    {
        // Clear existing tunes
        foreach (Transform obj in contentObject.transform)
        {
            Destroy(obj.gameObject);
        }

        foreach(GameObject tune in tuneList)
        {
            GameObject obj = Instantiate(iconPrefab, contentObject.transform);
            BlockSpawner spawner = obj.GetComponentInChildren<BlockSpawner>();
            spawner.blockPrefab = tune;
        }
    }

}
