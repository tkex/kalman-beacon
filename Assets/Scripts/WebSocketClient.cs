
using UnityEngine;
using NativeWebSocket;
using System.Collections.Generic;

public class WebSocketClient : MonoBehaviour
{
    WebSocket websocket;
    public Measurement measurementData;

    // Reference.
    public Sensor sensor;

    // Slider from 0.1 to 5secs.
    [Range(0.1f, 5.0f)]
    public float sendInterval = 1.0f;

    // Control vars to define which data are transmitted.
    public bool sendBeaconFlag;
    public bool sendTimestamp;
    public bool sendBeaconId;
    public bool sendBeaconPos;
    public bool sendAngleGroundTruth;
    public bool sendAngleDistorted;
    public bool sendSensorSTD;
    public bool sendHeadingAngleGroundTruth;
    public bool sendHeadingAngleDistorted;
    public bool sendCompassSTD;

    // Control flag to only send data if measurement data are available. 
    private bool isReadyToSend = false;

    // Start is called before the first frame update
    async void Start()
    {
        // *** SENSOR EVENT CONFIGURATION ***
        if (sensor == null)
        {
            Debug.LogError("IMPORTANT: Sensor is NOT assigned in WebSocketClient gameobject!");
            return;
        }

        // Add Sensor Listener.
        sensor.conductedMeasurement.AddListener(HandleNewMeasurement);


        // *** WEBSOCKET EVENT CONFIGURATION ***
        websocket = new WebSocket("ws://localhost:2567");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(bytes);

            // getting the message as a string
            // var message = System.Text.Encoding.UTF8.GetString(bytes);
            // Debug.Log("OnMessage! " + message);
        };

        // Keep sending messages dependend on configured sendInterval.
        InvokeRepeating("SendWebSocketMessage", 0.0f, sendInterval);

        // Waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

  
    string SendDataBlock()
    {
        List<string> filterDataParts = new List<string>();

        // Add individual measureData.
        if (sendTimestamp) filterDataParts.Add(measurementData.timestamp.ToString());
        if (sendBeaconId) filterDataParts.Add(measurementData.beaconId.ToString());
        if (sendBeaconPos) filterDataParts.Add(measurementData.beaconPos.ToString());
        if (sendAngleGroundTruth) filterDataParts.Add(measurementData.angleGroundTruth.ToString());
        if (sendAngleDistorted) filterDataParts.Add(measurementData.angleDistorted.ToString());
        if (sendSensorSTD) filterDataParts.Add(measurementData.sensorSTD.ToString());
        if (sendHeadingAngleGroundTruth) filterDataParts.Add(measurementData.headingAngleGroundTruth.ToString());
        if (sendHeadingAngleDistorted) filterDataParts.Add(measurementData.headingAngleDistorted.ToString());
        if (sendCompassSTD) filterDataParts.Add(measurementData.compassSTD.ToString());
        if (sendBeaconFlag) filterDataParts.Add(measurementData.beaconFlag.ToString());

        // Space data via tabs.
        return string.Join("\t", filterDataParts);
    }

    private void HandleNewMeasurement(Measurement measurement)
    {
        // Refresh/update measurementData with new fetched data.
        measurementData = measurement;

        // Send new msg with new data.
        //SendWebSocketMessage();

        if (!isReadyToSend)
        {
            isReadyToSend = true;

            InvokeRepeating("SendWebSocketMessage", 0.0f, sendInterval);
        }
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open && isReadyToSend && measurementData != null)
        {
            string message = SendDataBlock();

            await websocket.SendText(message);
        }
    }

    /*
    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText("plain text message");
        }
    }
    */


    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}