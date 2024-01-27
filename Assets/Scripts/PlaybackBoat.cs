using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackBoat : MonoBehaviour
{
    public TextAsset csvFile;
    private List<Dictionary<string, float>> datapoints = new List<Dictionary<string, float>>();
    private int playbackDataCounter = 0;
    private int playbackFrameCounter = 0;

    void Start()
    {
        if (csvFile == null)
        {
            Debug.LogWarning("NO CSV FILE");
        }
        else
        {
            ReadDataFromCSV();
        }
    }

    void FixedUpdate()
    {
        // TODO: implement correct time (from file). For now: update 60/5 times per second 
        if (playbackFrameCounter % 5 == 0 && playbackDataCounter < datapoints.Count)
        {
            // (wir könnten hier alternativ auch über die Beschleunigung gehen)
            var newX = datapoints[playbackDataCounter]["pos_x"];
            var newY = datapoints[playbackDataCounter]["pos_y"];
            transform.position = new Vector3(newX, newY, 0);
            playbackDataCounter += 1;
        }
        playbackFrameCounter += 1;
    }

    void ReadDataFromCSV()
    {
        string[] lines = csvFile.text.Split('\n');

        foreach (string line in lines)
        {
            string[] lineData = line.Split('\t');
            try
            {
                var datapoint = new Dictionary<string, float>();
                datapoint.Add("pos_x", float.Parse(lineData[0]));
                datapoint.Add("pos_y", float.Parse(lineData[1]));
                datapoint.Add("vel_x", float.Parse(lineData[2]));
                datapoint.Add("vel_y", float.Parse(lineData[3]));
                datapoints.Add(datapoint);
            }
            catch (System.Exception)
            {
                Debug.LogWarning("DATA MALFORMED IN: " + csvFile.name);
                throw;
            }
        }
    }
}
