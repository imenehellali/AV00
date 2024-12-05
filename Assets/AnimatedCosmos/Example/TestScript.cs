using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityStandardAssets.Effects;

public class TestScript : MonoBehaviour
{
    public bool _test = false;
    public float _Q4Time;
    public float spawnTO;

    [Header("Room Effect Materials")]
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _correctClip;
    [SerializeField]
    private AudioClip _wrongClip;
    [SerializeField]
    private AudioClip _instrClip;

    [Header("Room Effect Materials")]
    [SerializeField]
    private Color _greenEffectRoomMaterial;
    [SerializeField]
    private Color _redEffectRoomMaterial;
    [SerializeField]
    private Color _blackEffectRoomMaterial;
    [SerializeField]
    private Renderer _roomEffectMat;
    [SerializeField]
    private Material _roomEffectOriginal;
    [Header("Ghost Items")]
    [SerializeField]
    private List<GameObject> ghostsPref = new List<GameObject>();
    private List<GameObject> ghosts = new List<GameObject>();
    public Vector3 _ghostSpawnWPos = Vector3.zero;

    [Header("Ghost rotation variables")]
    public float rotationSpeed = 2f;  // Control the speed of rotation
    public float radius = 3f;  // Radius of the movement circle
    public float elapsedTime = 0f;
     // Time to complete a full circle
    public void StartTest()
    {
        _test = true;
        if(_test)
        {
            Debug.Log("startedTest");
            StartCoroutine(Q4());
            
        }

    }
   
    private IEnumerator Q4()
    {
        _audioSource.PlayOneShot(_instrClip);
        yield return new WaitForSeconds(_instrClip.length);
        StartCoroutine(RoomEffectQ4());
        StartCoroutine(SpawnGhost(_Q4Time, spawnTO));
        StartCoroutine(SoundEffectQ3());
    }
    public void StopTest()
    {
        _test = false;
        StopAllCoroutines();
    }
    private IEnumerator SoundEffectQ3()
    {
        float _q3Time = _Q4Time;
        while (_q3Time > 0)
        {
            int _randTO = Random.Range(0, 5);
            int _randEffect = Random.Range(0, 2);
            _audioSource.PlayOneShot(_randEffect == 0 ? _correctClip : _wrongClip);
            _q3Time -= _randTO;
            yield return new WaitForSecondsRealtime(_randTO);
        }

    }
    private IEnumerator RoomEffectQ4()
    {
        Debug.Log("entered Corot room effect 4");
        float _q4Time = _Q4Time;
        while (_q4Time > 0)
        {
            int _randTO = Random.Range(0, 10);
            int _randEffect = Random.Range(0, 3);
            Debug.Log($"rand effect:   {_randEffect}");
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
            Debug.Log($"q time:   {_q4Time} ");
            yield return new WaitForSeconds(_randTO);
        }
        if (_q4Time <= 0)
            StartCoroutine(TestFolge());

    }
    private IEnumerator SpawnRoomEffectQ4(Color _color)
    {
        Debug.Log("entered Spawn Color Q4");
        _roomEffectMat.material.SetColor("_Color", _color);
        yield return new WaitForSeconds(4);
        _roomEffectMat.material.SetColor("_Color", Color.white);
    }
    private IEnumerator SpawnGhost(float QTime, float spawnTO)
    {
        float currTime = QTime;
       
        while (currTime > 0)
        {
            yield return new WaitForSeconds(spawnTO);
            _ghostSpawnWPos = Random.insideUnitSphere * 3f;
            _ghostSpawnWPos.y = Mathf.Clamp(_ghostSpawnWPos.y, 0f, 3f);
            Debug.Log($"random spawn position:  {_ghostSpawnWPos}");
            int _rand = Random.Range(0, 6);
            currTime -= spawnTO;
            GameObject _ghost = Instantiate(ghostsPref[_rand], _ghostSpawnWPos, Quaternion.identity);
            ghosts.Add(_ghost);
            StartCoroutine(MoveAndDestroyGhost(_ghost.transform));
            --currTime;
        }
    }
    private IEnumerator MoveAndDestroyGhost(Transform ghost)
    {
        float circleDuration = 360f / rotationSpeed;
        float _elapsedTime = elapsedTime;
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
        if (_elapsedTime >= circleDuration && ghosts.Contains(ghost.gameObject))
        {
            ghosts.Remove(ghost.gameObject);
            Destroy(ghost.gameObject);
        }
        
        Debug.Log($"list contains :   {ghosts.Count}  elements");
    }
    private IEnumerator TestFolge()
    {
        Debug.Log("Finished Reihenfolge    done corretly ?????");
        yield return null;
    }
}
