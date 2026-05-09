using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NastyFlowerGenerator : MonoBehaviour
{
    [Header("BASEN")]
    [SerializeField] private GameObject NastyFlower;
    [SerializeField] private int maxCount = 5;
    [SerializeField] private ShapeChecker[] prohibitedAreas;
    [SerializeField] private TreeManager treeMan;
    [SerializeField] private PlowedLandManager plowedLandMan;
    [SerializeField] private BushManager bushManager;

    [Header("DISTANCES")]
    [SerializeField] private float minGenDistance = 10f;
    [SerializeField] private float maxGenDistance = 30f;

    [Header("TIMES")]
    [SerializeField] private float tick = 5f;

    private bool isGenerationTime = false;
    private List<GameObject> flowers;
    public static NastyFlowerGenerator instance;
    private Transform playerTransform;
    private HashSet<Vector3Int> restrictedAreas;
    private Coroutine coroutine;

    void Awake()
    {
        instance = this;
        flowers = new List<GameObject>();
        restrictedAreas = new HashSet<Vector3Int>();
        playerTransform = PlayerSeeker.GetPlayerTransform();
        DayLightHandler._OnTimeReached += CheckTime;

        restrictedAreas.UnionWith(treeMan.GetTreeCoords());
        restrictedAreas.UnionWith(plowedLandMan.GetPlowedLandCoords());
        restrictedAreas.UnionWith(bushManager.GetBushCoords());

        CheckTime((DayLightHandler.Hours, DayLightHandler.Minutes));
        //coroutine = StartCoroutine(GenerateFlowers());
    }

    private void CheckTime((int hh, int mm) time)
    {
        isGenerationTime = time.hh >= 22 || time.hh < 6;
        if (!isGenerationTime && coroutine != null) 
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        if (isGenerationTime)
        {
            coroutine = StartCoroutine(GenerateFlowers());
        }
    }

    void OnDestroy()
    {
        DayLightHandler._OnTimeReached -= CheckTime;
        if (coroutine != null) StopCoroutine(coroutine);
    }

    bool InProhibitedArea(Vector3 position)
    {
        foreach (ShapeChecker parea in prohibitedAreas)
        {
            if (parea.InArea(position))
                return true;
        }
        return false;
    }

    IEnumerator GenerateFlowers()
    {
        restrictedAreas.UnionWith(treeMan.GetTreeCoords());
        restrictedAreas.UnionWith(plowedLandMan.GetPlowedLandCoords());
        restrictedAreas.UnionWith(bushManager.GetBushCoords());

        while(isGenerationTime)
        {
            for (int i = flowers.Count-1; i >= 0; i--)
            {
                if (flowers[i] == null)
                {
                    flowers.RemoveAt(i);
                }
            }

            if (flowers.Count < maxCount)
            {
                Vector3 actPos = playerTransform.position;

                int X = UnityEngine.Random.Range((int)Mathf.Round(actPos.x - maxGenDistance), (int)Mathf.Round(actPos.x + maxGenDistance));
                int Z = UnityEngine.Random.Range((int)Mathf.Round(actPos.z - maxGenDistance), (int)Mathf.Round(actPos.z + maxGenDistance));

                if ((X < actPos.x - minGenDistance || X > actPos.x + minGenDistance) 
                && (Z < actPos.z - minGenDistance || Z > actPos.z + minGenDistance))
                {
                    Vector3Int candidatePos = new Vector3Int(X, 0, Z);
                    if (!InProhibitedArea(candidatePos) && restrictedAreas.Add(candidatePos))
                    {
                        flowers.Add(Instantiate(NastyFlower, candidatePos, Quaternion.identity));
                    }
                }
                else
                {
                    yield return null;
                }
            }
            yield return new WaitForSecondsRealtime(tick);
        }
    }


}
