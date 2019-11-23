using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

    Renderer rend;
    Material mat;
    [Header("라이트 밝기 조절")][Range(0,1)]
    [SerializeField] float lightValue;
    [Header("색상 선택")][Tooltip("1. Nomal Color, 2. White, 3. Cyan, 4. Red, 5. Pink, 6. Yellow, 7. Green, 8. Blue")]
    [SerializeField] int selectColor = 1;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        mat = rend.material;
    }

    void Update() {
        SelectLightColor(selectColor);
        mat.SetColor("_EmissionColor", mat.color * lightValue);
    }

    void SelectLightColor(int _lightColor)
    {
        switch (_lightColor)
        {
            case 1:
                break;
            case 2:
                mat.color = Color.white;
                break;
            case 3:
                mat.color = Color.cyan;
                break;
            case 4:
                mat.color = Color.red;
                break;
            case 5:
                mat.color = new Color(255, 0, 225, 225);
                break;
            case 6:
                mat.color = Color.yellow;
                break;
            case 7:
                mat.color = Color.green;
                break;
            case 8:
                mat.color = Color.blue;
                //mat.color = new Color(255, 106, 0, 255);
                break;
        }
    }
}