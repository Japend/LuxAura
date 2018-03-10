using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPool : MonoBehaviour
{

    private int totalAvailable;

    private static AttackPool instance;
    public static AttackPool Instance
    {
        get { return AttackPool.instance; }
    }

    public GameObject AttackPrefab;
    public List<Attack> availableAttacks;
    private List<AttackInfo> pendingAttacksInfo;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        totalAvailable = gameObject.transform.childCount;
        availableAttacks = new List<Attack>();
        pendingAttacksInfo = new List<AttackInfo>();
        foreach (Transform child in transform)
        {
            availableAttacks.Add(child.GetComponent<Attack>());
        }
    }




    /// <summary>
    /// Creates an attack from within the pool. If there are not enough attacks, it instanciates one more
    /// </summary>
    /// <param name="attackInfo">The information of the attack</param>
    public void Attack(AttackInfo attackInfo)
    {
        foreach (Attack at in availableAttacks)
        {
            if (!at.gameObject.activeSelf)
            {
                at.gameObject.SetActive(true);
                at.Prepare(attackInfo);
                return;
            }
        }

        //if every attack is being used, we add a new one
        GameObject aux = GameObject.Instantiate(AttackPrefab);
        aux.SetActive(false);
        availableAttacks.Add(aux.GetComponent<Attack>());
        pendingAttacksInfo.Add(attackInfo);
        aux.GetComponent<Attack>().Prepare(attackInfo);
        StopCoroutine("PrepareNextFrame");
        StartCoroutine("PrepareNextFrame");
    }

    IEnumerator PrepareNextFrame()
    {
        //if an object is instantiatied, it cant be used in the next frame, so we store it and use it here
        //wait one frame
        yield return null;

        for (int i = pendingAttacksInfo.Count - 1; i >= 0; i--)
        {
            Attack(pendingAttacksInfo[i]);
            pendingAttacksInfo.Remove(pendingAttacksInfo[i]);
        }
    }
}
