using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartStartInstrManager : MonoBehaviour
{
   
    [SerializeField]
    private AudioSource _audioSourceInstr;
    [SerializeField]
    private AudioClip _participantInstrClip;
   


    public void PlayParticipantInstr()
    {
        _audioSourceInstr.PlayOneShot(_participantInstrClip);
    }
}
