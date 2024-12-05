using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Effects;

public class GhostBusterManager : MonoBehaviour
{


    //Quest Variables
    private float _Q1Time = 0f;
    private float _Q2Time = 0f;
    private float _Q3Time = 0f;
    private float _Q4Time = 0f;
    private float levelDuration = 300f;


    //Room Effects variables
    private float _effectSwitchSpeed = 10f;
    private float _soundSwitchSpeed = 10f;
    [Header("Room Effect Materials")]
    [SerializeField]
    private Color _greenEffectRoomMaterial;
    [SerializeField]
    private Color _redEffectRoomMaterial;
    [SerializeField]
    private Color _blackEffectRoomMaterial;
    [SerializeField]
    private Renderer _roomEffectMat;


    //Room Sound Effect Variables
    [Header("Room Effect sounds")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip _correctActionClip;
    [SerializeField]
    private AudioClip _wrongActionClip;
    [SerializeField]
    private AudioClip _Q1InstructionClip;
    [SerializeField]
    private AudioClip _Q2InstructionClip;
    [SerializeField]
    private AudioClip _Q3InstructionClip;
    [SerializeField]
    private AudioClip _Q4InstructionClip;

    //Actions Variables
    [Header("Action Variables")]
    [SerializeField]
    private GameObject _boundaries;
    private const int _correctActionCost = 20;
    private const int _wrongActionCost = -20;


    //Ghost Spawn Variable
    private Vector3 _ghostSpawnWPos = Vector3.zero;
    private List<GhostBehavior> ghosts = new List<GhostBehavior>();
    [Header("Ghosts Items")]
    [SerializeField]
    private List<GameObject> ghostsPref = new List<GameObject>();// _drunkenRedGhost;
    private float _ghostSpawnTO = 5f;
    private float radius = 3f;  // Radius of the movement circle

    void Awake()
    {
        AssignQTime();
    }

    // Update is called once per frame
    private void Start()
    {
        MoneyManager.ResetMoney();
    }

    public void StartTask()
    {
        StartCoroutine(Q1());
    }
    private IEnumerator Q1()
    {
        audioSource.PlayOneShot(_Q1InstructionClip);
        yield return new WaitForSeconds(_Q1InstructionClip.length);
        StartCoroutine(StartQ1());
    }
    private IEnumerator Q2()
    {
        audioSource.PlayOneShot(_Q2InstructionClip);
        yield return new WaitForSeconds(_Q2InstructionClip.length);
        StartCoroutine(StartQ2());
    }
    private IEnumerator Q3()
    {
        audioSource.PlayOneShot(_Q3InstructionClip);
        yield return new WaitForSeconds(_Q3InstructionClip.length);
        StartCoroutine(StartQ3());
    }
    private IEnumerator Q4()
    {
        audioSource.PlayOneShot(_Q4InstructionClip);
        yield return new WaitForSeconds(_Q4InstructionClip.length);
        StartCoroutine(StartQ4());
    }

    //Shoot the Red Ghost + room red
    private IEnumerator StartQ1()
    {
        StartCoroutine(SpawnRoomEffect(true));
        StartCoroutine(SpawnGhost(_Q1Time, _ghostSpawnTO, 10f));
        while (_Q1Time > 0)
        {
            _Q1Time -= Time.deltaTime;
            ghosts.ForEach(
                x =>
                {
                    //If killed red
                    if (x.ghostRed && x.ghostDead)
                    {
                        x.GetComponent<GhostBustBehavior>()._in = "   Ouch Killed me";
                        audioSource.Stop();
                        audioSource.PlayOneShot(_correctActionClip);
                        MoneyManager.UpdateMoney(_correctActionCost);
                        ghosts.Remove(x);
                        Destroy(x.gameObject);
                    }
                    //if killed not red
                    else if (!x.ghostRed && x.ghostDead)
                    {
                        x.GetComponent<GhostBustBehavior>()._in = "   killed me wrong";
                        audioSource.Stop();
                        audioSource.PlayOneShot(_wrongActionClip);
                        MoneyManager.UpdateMoney(_wrongActionCost);
                        ghosts.Remove(x);
                        Destroy(x.gameObject);
                    }
                }
                );
            /*ghosts.RemoveAll(x=>
            {
                //If killed red
                if(x.ghostRed && x.ghostDead)
                {
                    x.GetComponent<GhostBustBehavior>()._in = "   Ouch Killed me";
                    audioSource.PlayOneShot(_correctActionClip);
                    MoneyManager.UpdateMoney(_actionCost);
                    return true;
                }
                //if killed not red
                else if(!x.ghostRed && x.ghostDead)
                {
                    x.GetComponent<GhostBustBehavior>()._in = "   killed me wrong";
                    audioSource.PlayOneShot(_wrongActionClip);
                    MoneyManager.UpdateMoney(-_actionCost);
                    return false;
                }
                //missed
                return false;
            });*/
            yield return null;
        }
        if (_Q1Time <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Q2());
        }

    }
    //Shoot the blue ghost + room lit blue
    private IEnumerator StartQ2()
    {
        StartCoroutine(SpawnRoomEffect(false));
        StartCoroutine(SpawnGhost(_Q2Time, _ghostSpawnTO, 20f));
        while (_Q2Time > 0)
        {
            _Q2Time -= Time.deltaTime;

            ghosts.RemoveAll(x =>
            {
                //If killed drunken
                if (!x.ghostRed && x.ghostDead)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(_correctActionClip);
                    MoneyManager.UpdateMoney(_correctActionCost);
                    //ghosts.Remove(x);
                    //Destroy(x.gameObject);
                    return true;
                }
                //if killed not drunken
                else if (x.ghostRed && x.ghostDead)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(_correctActionClip);
                    MoneyManager.UpdateMoney(_correctActionCost);
                    //ghosts.Remove(x);
                    //Destroy(x.gameObject);
                    return false;
                }
                //missed
                return false;
            });

            yield return null;
        }
        if (_Q2Time <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Q3());
        }
    }
    //shoot sober ghost - flickering light with random interval - money sound randomly
    private IEnumerator StartQ3()
    {
        _ghostSpawnTO -= 2;
        StartCoroutine(RoomEffectQ3());
        StartCoroutine(SoundEffectQ3());
        StartCoroutine(SpawnGhost(_Q3Time, _ghostSpawnTO, 20f));
        while (_Q3Time > 0)
        {
            _Q3Time -= Time.deltaTime;

            ghosts.RemoveAll(x =>
            {
                //If killed sober
                if (!x.ghostDrunken && x.ghostDead)
                {
                    MoneyManager.UpdateMoney(_correctActionCost);
                    return true;
                }
                //if killed not sober
                else if (x.ghostDrunken && x.ghostDead)
                {
                    MoneyManager.UpdateMoney(_wrongActionCost);
                    return false;
                }
                //missed
                return false;
            });

            yield return null;
        }
        if (_Q3Time <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Q4());
        }
    }
    //shoot green drunken ghost - flickering light with random interval - money sound randomly
    private IEnumerator StartQ4()
    {
        _ghostSpawnTO -= 2;
        StartCoroutine(SoundEffectQ4());
        StartCoroutine(RoomEffectQ4());
        StartCoroutine(SpawnGhost(_Q4Time, _ghostSpawnTO, 30f));
        while (_Q4Time > 0)
        {
            _Q4Time -= Time.deltaTime;

            ghosts.RemoveAll(x =>
            {
                //If killed drunken & green  
                if (x.ghostDrunken && !x.ghostRed && x.ghostDead)
                {
                    MoneyManager.UpdateMoney(_correctActionCost);
                    return true;
                }
                //if killed other
                else if (x.ghostDead)
                {
                    MoneyManager.UpdateMoney(_wrongActionCost);
                    return false;
                }
                //missed
                return false;
            });
            yield return null;
        }
        //Save all data here 
        if (_Q4Time <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Q4()); GBData.Data.SaveData();
            //Update the money manager 
            //See what happens with the 
        }
    }
    private void AssignQTime()
    {
        //levelDuration = GameSettings.Instance.LevelDurations[GameSettings.Instance.CurrentLevelIndex - 1];
        _Q1Time = Mathf.FloorToInt(levelDuration / 4);
        _Q2Time = Mathf.FloorToInt(levelDuration / 4);
        _Q3Time = Mathf.FloorToInt(levelDuration / 4);
        _Q4Time = levelDuration - _Q1Time - _Q2Time - _Q3Time;
    }

    private IEnumerator SpawnGhost(float QTime, float spawnTO, float rotationSpeedQ)
    {
        float currTime = QTime;

        while (currTime > 0)
        {
            yield return new WaitForSeconds(spawnTO);
            currTime -= spawnTO;
            //Spawn random position around player in upper hemisphere
            _ghostSpawnWPos = Random.insideUnitSphere * 3f;
            _ghostSpawnWPos.y = Mathf.Clamp(_ghostSpawnWPos.y, 0f, 3f);
            int _rand = Random.Range(0, 6);
            GhostBehavior _ghost = Instantiate(ghostsPref[_rand], _ghostSpawnWPos, Quaternion.identity).GetComponent<GhostBehavior>();
            ghosts.Add(_ghost);
            StartCoroutine(MoveAndDestroyGhost(_ghost.gameObject.transform, rotationSpeedQ));
            --currTime;
        }
    }
    private IEnumerator MoveAndDestroyGhost(Transform ghost, float rotationSpeed)
    {
        float circleDuration = 360f / rotationSpeed;
        float _elapsedTime = 0f;
        // Save the initial position of the ghost for circular movement
        Vector3 initialPosition = ghost.position;
        Quaternion initialRotation = ghost.rotation;

        while (_elapsedTime < circleDuration)
        {
            _elapsedTime += Time.deltaTime;

            // Rotate the ghost around its own forward axis
            ghost.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Move the ghost in a circular path in its own local forward direction
            float angle = rotationSpeed * _elapsedTime;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            ghost.position = initialPosition + ghost.forward * offset.magnitude;

            // Ensure ghost stays in the upper hemisphere
            ghost.position = new Vector3(ghost.position.x, Mathf.Clamp(ghost.position.y, 0f, 3f), ghost.position.z);

            yield return null;
        }
        //remove from list and destroy if time elapsed but ghost still alive
        if (ghosts.Contains(ghost.gameObject.GetComponent<GhostBehavior>()) && _elapsedTime >= circleDuration)
        {
            ghosts.Remove(ghost.gameObject.GetComponent<GhostBehavior>());
            Destroy(ghost.gameObject);
        }
        //remove from list and destroy if it was missed but killed ones --> doesn't give second chances
        /*else if(ghosts.Contains(ghost.gameObject.GetComponent<GhostBehavior>()) && ghost.gameObject.GetComponent<GhostBehavior>().ghostDead)
        {
            ghosts.Remove(ghost.gameObject.GetComponent<GhostBehavior>());
            Destroy(ghost.gameObject);
        }*/
    }

    private IEnumerator SpawnRoomEffect(bool red)
    {
        _roomEffectMat.material.SetColor("_Color", red ? _redEffectRoomMaterial : _greenEffectRoomMaterial);
        yield return new WaitForSeconds(4);
        _roomEffectMat.material.SetColor("_Color", Color.white);
    }
    private IEnumerator SpawnRoomEffectQ4(Color _color)
    {
        _roomEffectMat.material.SetColor("_Color", _color);
        yield return new WaitForSeconds(4);
        _roomEffectMat.material.SetColor("_Color", Color.white);
    }
    private IEnumerator RoomEffectQ3()
    {
        float _q3Time = _Q3Time;
        while (_q3Time > 0)
        {

            int _randTO = Random.Range(0, 5);
            int _randEffect = Random.Range(0, 2);
            StartCoroutine(SpawnRoomEffect(_randEffect == 0 ? true : false));
            _q3Time -= _randTO;
            yield return new WaitForSeconds(_randTO);
        }

    }
    private IEnumerator RoomEffectQ4()
    {
        float _q4Time = _Q4Time;
        while (_q4Time > 0)
        {
            int _randTO = Random.Range(0, 10);
            int _randEffect = Random.Range(0, 3);
            switch (_randEffect)
            {
                case 0:
                    StartCoroutine(SpawnRoomEffectQ4(_redEffectRoomMaterial));
                    break;
                case 1:
                    StartCoroutine(SpawnRoomEffectQ4(_greenEffectRoomMaterial));
                    break;
                case 2:
                    StartCoroutine(SpawnRoomEffectQ4(_blackEffectRoomMaterial));
                    break;
            }
            _q4Time -= _randTO;
            yield return new WaitForSeconds(_randTO);
        }

    }
    private IEnumerator SoundEffectQ3()
    {
        float _q3Time = _Q3Time;
        while (_q3Time > 0)
        {
            int _randTO = Random.Range(0, 5);
            int _randEffect = Random.Range(0, 2);
            audioSource.PlayOneShot(_randEffect == 0 ? _correctActionClip : _wrongActionClip);
            _q3Time -= _randTO;
            yield return new WaitForSecondsRealtime(_randTO);
        }

    }
    private IEnumerator SoundEffectQ4()
    {
        float _q4Time = _Q4Time;
        while (_q4Time > 0)
        {
            int _randTO = Random.Range(0, 3);
            int _randEffect = Random.Range(0, 2);
            audioSource.PlayOneShot(_randEffect == 0 ? _correctActionClip : _wrongActionClip);
            _q4Time -= _randTO;
            yield return new WaitForSecondsRealtime(_randTO);
        }

    }
}
