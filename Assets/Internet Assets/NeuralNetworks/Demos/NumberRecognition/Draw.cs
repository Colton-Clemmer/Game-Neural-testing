using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draw : MonoBehaviour {

    public RawImage Image;
    public Texture2D Tex { get; private set; }
    public int ImageSize = 28;

    Vector3 oldMousePosition;

    // Use this for initialization
    void Start () {
        Tex = new Texture2D(ImageSize, ImageSize);
        Tex.filterMode = FilterMode.Trilinear;
        Reset();
        Image.texture = Tex;
	}

    // Update is called once per frame
    void Update()
    {
        var smoothMP = Vector3.Lerp(oldMousePosition, Input.mousePosition, Time.deltaTime * 10);
        var mp = (smoothMP - Image.transform.position) / Image.canvas.scaleFactor;
        if (mp.x >= 0 && mp.x < Image.rectTransform.sizeDelta.x && mp.y >= 0 && mp.y < Image.rectTransform.sizeDelta.y)
        {
            if(Input.GetMouseButton(0))
            {
                var x = mp.x * (float)ImageSize / Image.rectTransform.sizeDelta.x;
                var y = mp.y * (float)ImageSize / Image.rectTransform.sizeDelta.y;

                Brush(5, Mathf.RoundToInt(x), Mathf.RoundToInt(y));
            }
        }
        oldMousePosition = smoothMP;
    }


    public void Brush(int size, int cx, int cy)
    {
        int half = size / 2;
        for (int x = cx - half; x < cx + half; x++)
        {
            for (int y = cy - half; y < cy + half; y++)
            {

                if (Vector3.Distance(new Vector3(cx, cy), new Vector3(x, y)) < size / 4f)
                {
                    Tex.SetPixel(x, y, Color.black);
                }
            }
        }
            Tex.Apply();

    }

    public void Reset()
    {
        for (int i = 0; i < ImageSize * ImageSize; i++)
            Tex.SetPixel(i / ImageSize, i % ImageSize, Color.white);
        Tex.Apply();

    }


}
