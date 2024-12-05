using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
public class GhostBustBehavior : MonoBehaviour
{
    [SerializeField]
    private InputActionReference Activate;
    private ETObject GazeObject;
    private GhostBehavior ghostBehavior;
    public string _in = "started";
    [SerializeField]
    private Material _material;
    private Vector4 _color;




    private void Start()
    {
        GazeObject=GetComponent<ETObject>();
        ghostBehavior = GetComponent<GhostBehavior>();
        Activate.action.started += ShootGhost;
        _color = _material.GetVector("_EmissionColor");
    }

    private void OnDestroy()
    {
        Activate.action.started -= ShootGhost;
    }
    
    private void ShootGhost(InputAction.CallbackContext callbackContext)
    {
        if (GazeObject.IsGazeLocked())
        {
            ghostBehavior.ghostDead = true;
        }
    }

    public void ResetColor()
    {
        _material.SetVector("_EmissionColor",_color);
    }
    public void GlowColor()
    {
        _material.SetVector("_EmissionColor", _color * Mathf.Lerp(0.5f, 2f, Time.deltaTime));
    }
}
