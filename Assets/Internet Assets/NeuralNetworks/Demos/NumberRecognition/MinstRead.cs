using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class MinstRead
{

    public  List<DigitImage> Images;

    public MinstRead(string labelsPath, string imagesPath, int loadAmount = 10000, int imageSize = 28)
    {
        try
        {

            Images = new List<DigitImage>();

            FileStream ifsLabels = new FileStream(labelsPath, FileMode.Open);
            FileStream ifsImages = new FileStream(imagesPath, FileMode.Open);

            BinaryReader brLabels = new BinaryReader(ifsLabels);
            BinaryReader brImages = new BinaryReader(ifsImages);

            //Discarding
            int magic1 = brImages.ReadInt32();
            int numImages = brImages.ReadInt32();
            int numRows = brImages.ReadInt32();
            int numCols = brImages.ReadInt32();

            int magic2 = brLabels.ReadInt32();
            int numLabels = brLabels.ReadInt32();

            Color32[] pixels = new Color32[28*28];

            // each test image
            for (int di = 0; di < loadAmount; ++di)
            {
                for (int i = imageSize-1; i >=0; i--)
                {
                    for (int j = 0; j < imageSize; ++j)
                    {
                        byte b = brImages.ReadByte();
                        pixels[j + imageSize * i] = new Color32(b,b,b,255);
                    }
                }

                byte lbl = brLabels.ReadByte();

                DigitImage dImage = new DigitImage(pixels, lbl, imageSize);
                Images.Add(dImage);
            } 

            ifsImages.Close();
            brImages.Close();
            ifsLabels.Close();
            brLabels.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    } 
} 

public class DigitImage
{
    public int ImageSize;
    public Color32[] pixels;
    public double[] values;
    public byte label;

    public DigitImage(Color32[] pixels, byte label, int ImageSize, bool invert = false)
    {
        this.pixels = (Color32[])pixels.Clone();
        this.values = new double[pixels.Length];
        for (int i = 0; i < values.Length; i++)
        {
            if (invert)
            {
                values[i] = 1- Mathf.RoundToInt(pixels[i].r / 255f);
            }
            else
            {
                values[i] = Mathf.RoundToInt(pixels[i].r / 255f);
            }
        }
        this.label = label;
        this.ImageSize = ImageSize;
    }


    public Texture2D ConvertToTexture()
    {
        var tex = new Texture2D(ImageSize, ImageSize );
        tex.SetPixels32(pixels);
        tex.Apply();
        return tex;
    }


}
