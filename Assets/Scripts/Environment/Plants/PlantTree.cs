using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class PlantTree : MonoBehaviour, IInteractable
{
    
    [SerializeField] private float health;
    [SerializeField] private bool isGrowed;
    private int daysBeforeGrow;
    [SerializeField] private ItemData[] returningItems;

    [SerializeField] private GameObject smallTreeObject;
    [SerializeField] private List<GameObject> GrowedTreeObjects;
    [SerializeField] private GameObject actualTreeObject;
    [SerializeField] private bool isNeedGenerate;
    [SerializeField] private bool UsedForTreeGenerator;
    [SerializeField] private GameObject EditorGO;

    private Coroutine dampCoroutine;
    private Vector3 startPos;


    void Awake()
    {
        if (!UsedForTreeGenerator)
        {
            GenerateTree();
        }
    }

    internal void InitTree(TreeData data)
    {
        if (data != null)
        {
            isGrowed = data.Phase == 0? false : true;
            if (isGrowed && data.Type < GrowedTreeObjects.Count)
            {
                GenerateTree(data.Type);
            }
            else
            {
                GenerateTree();
            }
        }
        //Awake();
    }

    private void GenerateTree(int type = -1)
    {
        Destroy(EditorGO);

        if (type == -1) type = UnityEngine.Random.Range(0, GrowedTreeObjects.Count - 1);
        
        this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), this.transform.position.y, Mathf.Round(this.transform.position.z));

        if (isGrowed && isNeedGenerate)
        {
            actualTreeObject = Instantiate(GrowedTreeObjects[type], this.transform);
            health = UnityEngine.Random.Range(10, 70);
        }
        else if (isGrowed && !isNeedGenerate)
        {
            health = UnityEngine.Random.Range(10, 70);
        }
        else
        {
            daysBeforeGrow = UnityEngine.Random.Range(4, 12);
            DayLightHandler._OnTimeReached += Growing;
            actualTreeObject = Instantiate(smallTreeObject, this.transform);
        }

        startPos = actualTreeObject.transform.localPosition;
    }


    (bool isDebitNeed, List<IItem> gettingItems) IInteractable.Interact(IItem item)
    {
        if (item.GameObject == null)
        {
            return (false, null);
        }


        if(item.GameObject.CompareTag("Axe"))
        {
            IInstrument axe = item.GameObject.GetComponent<IInstrument>();
            float damage = axe?.Damage ?? 0;
            health -= damage;

            Debug.Log($"Нанесено {damage} урона дереву");

            if (dampCoroutine != null)
            {
                StopCoroutine(dampCoroutine);
                actualTreeObject.transform.localPosition = new Vector3(startPos.x, startPos.y, startPos.z); 
            } 

            dampCoroutine = StartCoroutine(DampTree());


            if (actualTreeObject != null && health <= 0)
            {
                if (isGrowed)
                {
                    //destroy the object
                    List<IItem> retItems = new List<IItem>();

                    foreach (ItemData returningitem in returningItems)
                    {
                        for (int i = 0; i < UnityEngine.Random.Range(2, 5); i++)
                        {
                            retItems.Add(returningitem);
                        }
                    }

                    Destroy(actualTreeObject);
                    actualTreeObject = null;
                    Destroy(this.gameObject);
                    return (false, retItems);
                }

                else
                {
                    Destroy(actualTreeObject);
                    actualTreeObject = null;
                    Destroy(this.gameObject);
                    return (false, null);
                }
            }

            
        }  

        return (false, null);
    }


    void Growing((int hh, int mm) time)
    {
        switch (time)
        {
            case (6, 00):
                daysBeforeGrow -= 1;

                if (daysBeforeGrow == 0)
                {
                    daysBeforeGrow = -99;
                    Growed();
                }
                break;
            case (0, 0):
                if (actualTreeObject == null)
                    Destroy(this.gameObject);
                break;
        }
    }

    void Growed()
    {
        Destroy(actualTreeObject);
        actualTreeObject = Instantiate(GrowedTreeObjects[UnityEngine.Random.Range(0, GrowedTreeObjects.Count - 1)], this.transform);
        isGrowed = true;
        health = UnityEngine.Random.Range(10, 70);
    }

    IEnumerator DampTree()
    {
        float x = 0f;
        float res = 0f;
        while (true)
        {
            res = DampedWave(x);
            actualTreeObject.transform.localPosition = new Vector3(startPos.x + res, startPos.y, startPos.z);
            x+=0.06f;

            if (x > 10)
            {
                actualTreeObject.transform.localPosition = new Vector3(startPos.x, startPos.y, startPos.z);
                yield break;
            }

            yield return null;
        }  
    }



    /// <summary>
    /// Вычисляет значение затухающей волны в момент времени t.
    /// </summary>
    /// <param name="t">Время или координата X</param>
    /// <param name="amplitude">Начальная амплитуда</param>
    /// <param name="frequency">Частота колебаний</param>
    /// <param name="decay">Коэффициент затухания (чем больше, тем быстрее гаснет)</param>
    float DampedWave(float t, float amplitude = 0.09f, float frequency = 1.0f, float decay = 1.0f)
    {
        // Формула: A * e^(-decay * t) * cos(2 * PI * f * t)
        return amplitude * Mathf.Exp(-decay * t) * Mathf.Cos(2 * Mathf.PI * frequency * t);
    }

    void OnDestroy()
    {
        DayLightHandler._OnTimeReached -= Growing;
    }

}
