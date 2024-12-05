using Pico.Platform;
using Pico.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.PXR;
using UnityEngine;

public class testMic : MonoBehaviour
{

    private const string fileName = "test.pcm";
    private string filePath;
    private FileStream fileStream;

    [SerializeField]
    private TMPro.TextMeshProUGUI Log;

    void Start()
    {
        // Initialize the RTC
        RtcService.InitRtcEngine();

        // Set the file path for saving the audio
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        // Start the RTC audio capture
        StartRTCRecording();
    }
    void StartRTCRecording()
    {
        // Set up the audio recording configurations
        RtcService.RegisterLocalAudioProcessor(OnAudioFrameReceived, RtcAudioChannel.Mono, RtcAudioSampleRate.F48000);

        // Join a dummy room to start the audio capture
        string roomId = "dummy_room";
        string userId = "a82b245668e5b37ce1843d69a596ac53"; // Use your actual user ID
        string token = "token"; // Use your actual token
        RtcRoomProfileType roomProfileType = RtcRoomProfileType.Communication;
        bool isAutoSubscribeAudio = true;

        RtcService.JoinRoom(roomId, userId, token, roomProfileType, isAutoSubscribeAudio);
        RtcService.StartAudioCapture();
    }

    private int OnAudioFrameReceived(RtcAudioFrame frame)
    {
        // Initialize the file stream if it's not already done
        if (fileStream == null)
        {
            fileStream = new FileStream(filePath, FileMode.Create);
        }

        // Write the audio data to the file
        byte[] audioData = frame.GetData();
        fileStream.Write(audioData, 0, audioData.Length);
        return 0;
    }

    public void StopRecording()
    {
        // Leave the dummy room to stop the audio capture
        RtcService.LeaveRoom("dummy_room");
        RtcService.StopAudioCapture();

        // Close the file stream
        if (fileStream != null)
        {
            fileStream.Close();
            fileStream = null;
        }

        Log.text += "\nRecording saved to: " + filePath;
    }

}

