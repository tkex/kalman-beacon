
using UnityEngine;
using NativeWebSocket;
using System.Collections.Generic;

public class WebSocketClient : MonoBehaviour
{
    WebSocket websocket;
    // TODO: Measurement was deprecated
    // public Measurement measurementData;

    // Reference.
    // [Header("Reference Settings")]
    // [Tooltip("Reference to the sensor script.")]
    // TODO: SensorOld was deprecated
    // public SensorOld sensor;


    [Header("WebSocket Settings")]
    [Tooltip("WebSocket server URL")]
    public string webSocketUrl = "ws://localhost:2567";


    // Slider from 0.1 to 5 secs.    
    [Header("Transmitting Settings")]
    [Tooltip("Configured the speed in which the data get transmitted i.e. 1 is 1 sec.")]
    [Range(0.1f, 5.0f)]
    public float sendInterval = 1.0f;

    // Control vars to define which data are transmitted.
    [Header("Data Settings")]
    [Tooltip("Configure which data should be sent - otherwise null values are transmitted.")]
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

    // JSON structure for sensor data.
    [System.Serializable]
    public class MeasurementData
    {
        public long timestamp;
        public int beaconId;
        // Important: beaconPos: Vector3 as string since JsonUtility cannot serialize Vector3 directly
        // see below in SendDataBlock
        public string beaconPos;
        public float angleGroundTruth;
        public float angleDistorted;
        public float sensorSTD;
        public float headingAngleGroundTruth;
        public float headingAngleDistorted;
        public float compassSTD;
        public int beaconFlag;
    }

    // Control flag to only send data if measurement data are available. 
    private bool isReadyToSend = false;


    // Start is called before the first frame update
    async void Start()
    {
        // *** SENSOR EVENT CONFIGURATION ***
        // TODO: sensor was deprecated
        // if (sensor == null)
        // {
        //     Debug.LogError("IMPORTANT: Sensor is NOT assigned in WebSocketClient gameobject!");
        //     return;
        // }

        // Add Sensor Listener.
        // TODO: conductedMeasruement was deprecated
        // sensor.conductedMeasurement.AddListener(HandleNewMeasurement);


        // *** WEBSOCKET EVENT CONFIGURATION ***
        websocket = new WebSocket(webSocketUrl);

        // Event handler.
        websocket.OnOpen += () => Debug.Log("Connection open!");
        websocket.OnError += (e) => Debug.Log("Error! " + e);
        websocket.OnClose += (e) => Debug.Log("Connection closed!");

        websocket.OnMessage += (bytes) =>
        {
            // Received Python bytes into UTF-8 string
            string msg = System.Text.Encoding.UTF8.GetString(bytes);

            Debug.Log("Received message (from Python webserver): " + msg);

            // Handle received python data.
            ProcessReceivedMessageFromPython(msg);

        };

        // Keep sending messages dependend on configured sendInterval.
        InvokeRepeating("SendWebSocketMessage", 0.0f, sendInterval);

        // Waiting for messages.
        await websocket.Connect();
    }

    void ProcessReceivedMessageFromPython(string msg)
    {
        // Important: Message (received data) is in format "Echo key: value"
        // defined in the WebSocketServer; otherwise change here.
        if (msg.StartsWith("Echo"))
        {
            string[] parts = msg.Split(new[] { ':' }, 2);

            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                // Show K/V Pair in Unity.
                Debug.Log($"Key: {key}, Value: {value}");

                // Here in future eventually more...
            }
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    string SendDataBlock()
    {
        MeasurementData data = new MeasurementData
        {
            // Ternary operator (Bedingung ? WertWennWahr : WertWennFalsch)
            // TODO: types were deprecated
            // timestamp = sendTimestamp ? measurementData.timestamp : 0,
            // beaconId = sendBeaconId ? measurementData.beaconId : 0,
            // beaconPos = sendBeaconPos ? measurementData.beaconPos.ToString() : null,
            // angleGroundTruth = sendAngleGroundTruth ? measurementData.angleGroundTruth : 0,
            // angleDistorted = sendAngleDistorted ? measurementData.angleDistorted : 0,
            // sensorSTD = sendSensorSTD ? measurementData.sensorSTD : 0,
            // headingAngleGroundTruth = sendHeadingAngleGroundTruth ? measurementData.headingAngleGroundTruth : 0,
            // headingAngleDistorted = sendHeadingAngleDistorted ? measurementData.headingAngleDistorted : 0,
            // compassSTD = sendCompassSTD ? measurementData.compassSTD : 0,
            // beaconFlag = sendBeaconFlag ? measurementData.beaconFlag : 0
        };

        return JsonUtility.ToJson(data);
    }


    // TODO: Measurement is deprecated
    // private void HandleNewMeasurement(Measurement measurement)
    // {
    //     // Refresh/update measurementData with new fetched data.
    //     // TODO: measurementData is deprecated
    //     // measurementData = measurement;

    //     Debug.Log($"Received new measurement data: {measurement.ToString()}");

    //     if (!isReadyToSend)
    //     {
    //         isReadyToSend = true;

    //         InvokeRepeating("SendWebSocketMessage", 0.0f, sendInterval);
    //     }
    // }

    async void SendWebSocketMessage()
    {
        // TODO: measurementData is deprecated
        // if (websocket.State == WebSocketState.Open && isReadyToSend && measurementData != null)
        // {
        //     string jsonData = SendDataBlock();

        //     await websocket.SendText(jsonData);

        //     Debug.Log("Sending Json Data: " + jsonData);
        // }
        /*
        else
        {
            Debug.LogError("WebSocket is neither open or ready to send any data...");
        }
        */
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}
