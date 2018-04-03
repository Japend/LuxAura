using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberDemoAgent : Agent {

    [SerializeField]
    private float currentNumber;
    [SerializeField]
    private float targetNumber;
    [SerializeField]
    private Text text;

    int solved;

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        state.Add(currentNumber);
        state.Add(targetNumber);
        return state;
    }

    public override void AgentReset()
    {
        targetNumber = UnityEngine.Random.RandomRange(-1f, 1f);
        currentNumber = 0f;
    }

    public override void AgentStep(float[] action)
    {
        if (text != null)
            text.text = string.Format("{0} / {1} [{2}]", currentNumber, targetNumber, solved);

        switch ((int)action[0])
        {
            case 0:
                currentNumber -= 0.01f;
                break;
            case 1:
                currentNumber += 0.01f;
                break;
            default:
                return;
        }

        if (currentNumber < -1.2f || currentNumber > 1.2f)
        {
            reward = -1f;
            done = true;
            return;
        }

        float difference = Mathf.Abs(targetNumber - currentNumber);

        if (difference <= 0.01f)
        {
            reward = 1;
            done = true;
            solved++;
            return;
        }
    }
}
