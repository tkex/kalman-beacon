using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SensorStats : MonoBehaviour
{
    public TextMeshProUGUI sensorSTD;
    public TextMeshProUGUI compassSTD;
    public TextMeshProUGUI headingGT;

    public void SetStats(float sensorSTD, float compassSTD, float headingGT)
    {
        this.sensorSTD.text = sensorSTD.ToString();
        this.compassSTD.text = compassSTD.ToString();
        this.headingGT.text = headingGT.ToString();
    }


}
