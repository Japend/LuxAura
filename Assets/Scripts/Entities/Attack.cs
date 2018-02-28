using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public struct AttackInfo
{
    public GameObject origin;
    public GameObject objective;
    public int player;
    public int units;

    public AttackInfo(GameObject origin, GameObject objective, int playerId, int units)
    {
        this.origin = origin;
        this.objective = objective;
        player = playerId;
        this.units = units;
    }
}

public class Attack : MonoBehaviour {

    

    public const float MOVEMENT_SPEED = 200F;
    private const float SIZE_MARGIN = 200f;
    private const float MIN_DISTANCE_TO_OBJECTIVE = 50f;

    private AttackInfo currentAttackInfo;
    public UnityEngine.UI.Text text;

    public void Prepare(AttackInfo info)
    {
        currentAttackInfo = info;
        gameObject.transform.position = info.origin.transform.position;
        transform.localScale = Vector3.zero;
        text.text = info.units.ToString();
    }

    public void Update()
    {
            transform.position = Vector3.MoveTowards(transform.position, currentAttackInfo.objective.transform.position, MOVEMENT_SPEED * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentAttackInfo.origin.transform.position) < SIZE_MARGIN + 1)
            {
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (SIZE_MARGIN - Vector3.Distance(transform.position, currentAttackInfo.origin.transform.position)) / SIZE_MARGIN);
            }

            else if(SIZE_MARGIN + 1 > Vector3.Distance(transform.position, currentAttackInfo.objective.transform.position))
            {
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (SIZE_MARGIN - Vector3.Distance(transform.position, currentAttackInfo.objective.transform.position)) / SIZE_MARGIN);
            }

            if (Vector3.Distance(transform.position, currentAttackInfo.objective.transform.position) < MIN_DISTANCE_TO_OBJECTIVE)
            {
                currentAttackInfo.objective.GetComponent<EventEntity>().SufferAttack(currentAttackInfo);
                gameObject.SetActive(false);
            }
    }

    public TAttackInfo GetTrainingSnapshot()
    {
        int turns = Utilities.Utilities.GetDistanceInTurns(currentAttackInfo.objective.transform.position, transform.position);
        return new TAttackInfo(turns, currentAttackInfo.player, currentAttackInfo.units, currentAttackInfo.objective.GetComponent<EventEntity>().Id);
    }
}
