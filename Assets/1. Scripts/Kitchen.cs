using UnityEngine;
using System.Collections;
using static UnityEditor.Recorder.OutputPath;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;

public class Kitchen : MonoBehaviour
{
    float term = 1.0f;

    private List<Resource>[] stacks = new List<Resource>[2];
    private Transform[] nextAnchor = new Transform[2];

    private int addCursor = 0;
    private int removeCursor = 0;
    private int steakCount = 0;

    private AudioSource audioSource;

    private bool playEffect = false;

    public Transform[] slots = new Transform[2];
    public Storage storage;
    
    public Resource steak;
    public GameObject steakObject;

    public GameObject[] cookEffects;
    public AudioClip cookAudio;

    public CookerNPC cooker;

    private void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            nextAnchor[i] = slots[i];
            stacks[i] = new List<Resource>();
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = cookAudio;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        StartCoroutine(CookLoop());
    }

    IEnumerator CookLoop()
    {
        while (true)
        {
            if (storage.TryPopMeat(false, out Resource meat))
            {
                if (meat != null)
                {
                    Cook();
                    cooker.Cooking();
                    if(playEffect == false)
                    {
                        CookEffect(true);
                    }
                }
                yield return new WaitForSeconds(term);
            }
            else
            {
                cooker.StopCooking();

                if (playEffect == true)
                {
                    CookEffect(false);
                }    
                // 재고 소진 → 요리 중단
                yield return null;
            }
        }
    }

    void Cook()
    {
        Debug.Log("Cook!");
        Resource cookedSteak = Instantiate(steakObject, transform.position, transform.rotation).GetComponent<Resource>();
        
        cookedSteak.state = Resource.meatState.Cooked;

        AddFood(cookedSteak);
    }

    void AddFood(Resource cookedSteak)
    {
        Transform anchor = nextAnchor[addCursor];

        cookedSteak.transform.SetParent(anchor, true);
        cookedSteak.transform.localPosition = Vector3.zero;
        cookedSteak.transform.localRotation = Quaternion.identity;

        stacks[addCursor].Add(cookedSteak);
        nextAnchor[addCursor] = cookedSteak.chain;
        steakCount++;

        addCursor = (addCursor + 1) % slots.Length;
    }

    void CookEffect(bool cooking)
    {
        for(int i = 0; i < cookEffects.Length; i++)
        {
            cookEffects[i].SetActive(cooking);
        }

        if (cooking)
        {
            audioSource.Play();
            playEffect = cooking;
        }
        else
        {
            audioSource.Stop();
            playEffect = cooking;
        }
    }

    public bool TryPopSteak(out Resource cookedMeat)
    {
        int checkedSlots = 0;
        while (checkedSlots < slots.Length)
        {
            if (stacks[removeCursor].Count > 0)
            {
                int last = stacks[removeCursor].Count - 1;
                cookedMeat = stacks[removeCursor][last];
                stacks[removeCursor].RemoveAt(last);

                nextAnchor[removeCursor] = (stacks[removeCursor].Count == 0)
                                            ? slots[removeCursor] : stacks[removeCursor][stacks[removeCursor].Count - 1].chain;


                cookedMeat.transform.SetParent(null);
                cookedMeat.gameObject.SetActive(false);

                steakCount--;
                removeCursor = (removeCursor + 1) % stacks.Length;

                return true;
            }
            removeCursor = (removeCursor + 1) % stacks.Length;
            checkedSlots++;
        }

        cookedMeat = null;
        return false;
    }


    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (col.TryGetComponent(out Player player) && TryPopSteak(out Resource cookedMeat))
            {
                cookedMeat.gameObject.SetActive(true);
                player.StackResource(cookedMeat, true);
            }
        }
    }
}
