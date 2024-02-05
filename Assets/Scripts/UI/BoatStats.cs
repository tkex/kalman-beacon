using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoatStats: MonoBehaviour
{
    public TextMeshProUGUI propulsion;
    public TextMeshProUGUI rudderAngle;

    public void SetStats(float propulsion, float rudderAngle)
    {
        this.propulsion.text = propulsion.ToString();
        this.rudderAngle.text = rudderAngle.ToString();
    }
}
