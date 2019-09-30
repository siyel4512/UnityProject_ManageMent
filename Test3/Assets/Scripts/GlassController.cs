using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassController : MonoBehaviour {

    [SerializeField]
    [Header("해당 오브젝트 투명도")]
    private float _transparency;
    [SerializeField]
    [Header("해당 오브젝트 렌더링 모드")]
    private float _mode;

    private MeshRenderer meshRenderer;

    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, _transparency);
        meshRenderer.material.SetFloat("_Mode", _mode);
        meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        meshRenderer.material.SetInt("_ZWrite", 0);
        meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
        meshRenderer.material.DisableKeyword("_ALPHABLEND_ON");
        meshRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        meshRenderer.material.renderQueue = 3000;
    }
}
