using Microsoft.CognitiveServices.Speech.Transcription;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParticipantData : MonoBehaviour
{
    public List<ParticipantRow> Rows = new List<ParticipantRow>(); // Rows of data

}
