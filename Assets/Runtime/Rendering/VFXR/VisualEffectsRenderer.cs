using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public enum EVFXRType
{
    Shader, VFX
}

[Serializable]
public class VFXRType
{
    [SerializeField]
    private EVFXRType eType;
    [SerializeField]
    private Material material;
    [SerializeField]
    private VisualEffectAsset vfx;

    public Material Material { get { return material; } }
    public VisualEffectAsset Vfx { get { return vfx; } }
    public EVFXRType EType { get { return eType; } }
}

[Serializable]
public class VFX
{
    [SerializeField]
    private string keyName;
    [SerializeField]
    private GameObject self;
    [SerializeField]
    private VFXRType type;
    [SerializeField]
    private int awaitingTime;

    public VFX(string name, GameObject gameObject, VFXRType type)
    {
        self = gameObject;
        Name = name;
        this.type = type;
    }

    public GameObject GameObject { get { return self; } }
    public VFXRType Type { get { return type; } }

    public string Name
    {
        get { return this.keyName; }
        set { this.keyName = value; }
    }

    public int AwaitingTime
    {
        get { return this.awaitingTime;}
        set { this.awaitingTime = value;}
    }
}

public class VisualEffectsRenderer : MonoBehaviour
{
    private Camera vfxrCamera;

    [SerializeField]
    private List<VFX> vfxrPool;
    private VFX currentVFX = null;
    [SerializeField]
    private VisualEffect VFXEmitter;
    [SerializeField]
    private SpriteRenderer ShaderEmitter;
    [SerializeField]
    private RenderTexture vfxrTT;

    public VisualEffectsRenderer()
    {

    }

    private void Awake()
    {
        vfxrCamera = this.GetComponentInChildren<Camera>();
        vfxrCamera.gameObject.SetActive(false);
    }

    private void Start()
    {

    }

    public Camera VFXCamera
    {
        get { return vfxrCamera; }
    }

    public async System.Threading.Tasks.Task ActivateVFX(string name)
    {
        if(name != null & currentVFX == null) 
        {
            Debug.Log("ActivateVFX command");

            TryGetVFX(name, out currentVFX);

            vfxrCamera.gameObject.SetActive(true);

            SwitchEmmiter(currentVFX.Type.EType);

            SetEmmiter(currentVFX.Type.EType);

            await System.Threading.Tasks.Task.Delay(currentVFX.AwaitingTime);

            DeactivateVFX(name);

            vfxrTT.Release();
            vfxrCamera.gameObject.SetActive(false);
        }
    }

    public void DeactivateVFX(string name)
    {
        VFX checkWith;
        TryGetVFX(name, out checkWith);

        if (TrySwitchEmmiterOff(ref checkWith))
        {
            currentVFX = null;
            Debug.Log($"VFX is Successfully finished");
        }
    }

    private bool TryGetVFX(string name, out VFX value)
    {
        foreach (var info in vfxrPool)
        {
            if (info.Name == name)
            {
                value = info;
                return true;
            }
        }

        value = default;
        return false;
    }

    private void SwitchEmmiter(EVFXRType emmiterType)
    {
        switch (emmiterType)
        {
            case EVFXRType.Shader:
                if (ShaderEmitter.gameObject.activeSelf)
                {
                    ShaderEmitter.gameObject.SetActive(false);
                    Debug.Log($"{ShaderEmitter.name} is Deactivated");
                }
                else
                {
                    ShaderEmitter.gameObject.SetActive(true);
                    Debug.Log($"{ShaderEmitter.name} is Activated");
                }
                break;
            case EVFXRType.VFX:
                if (VFXEmitter.gameObject.activeSelf)
                {
                    VFXEmitter.gameObject.SetActive(false);
                    Debug.Log($"{VFXEmitter.name} is Deactivated");
                }
                else
                {
                    VFXEmitter.gameObject.SetActive(true);
                    Debug.Log($"{VFXEmitter.name} is Activated");
                } 
                break;

            default:
                break;
        }
    }

    private void SetEmmiter(EVFXRType emmiterType)
    {
        switch (emmiterType)
        {
            case EVFXRType.Shader:
                if(ShaderEmitter != null & currentVFX.Type.Material != null)
                    ShaderEmitter.material = currentVFX.Type.Material;
                break;
            case EVFXRType.VFX:
                if(VFXEmitter != null & currentVFX.Type.Vfx != null)
                    VFXEmitter.visualEffectAsset = currentVFX.Type.Vfx;
                break;
            default:
                break;
        }
    }

    private bool TrySwitchEmmiterOff(ref VFX emmiter)
    {
        bool isSucces = false;
        switch (emmiter.Type.EType)
        {
            case EVFXRType.Shader:
                if (currentVFX.Type == emmiter.Type)
                {
                    ShaderEmitter.gameObject.SetActive(false);
                    isSucces = true; 
                }
                else isSucces = false;
                break;
            case EVFXRType.VFX:
                if (currentVFX.Type == emmiter.Type)
                {
                    VFXEmitter.gameObject.SetActive(false);
                    isSucces = true;
                }
                else isSucces= false;
                break;
            default:
                break;
        }

        return isSucces;
    }
}
