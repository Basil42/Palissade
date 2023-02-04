using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class TextureGen 
{

    [MenuItem("Generate/Single pixel texture")]
    public static void GenerateSinglePixelTexture()
    {
        Texture2D tex = new Texture2D(8, 8);
        Color[] colors = new Color[8 * 8];
        for (var index = 0; index < colors.Length; index++)
        {
            colors[index] = Color.black;
        }

        tex.SetPixels(0, 0, 8, 8, colors);
        var savePath = EditorUtility.SaveFilePanel("Save generated texture", Application.dataPath, "newTexture", "TGA");
                    if (string.IsNullOrEmpty(savePath)) return;
                    byte[] textureData = tex.EncodeToTGA();
                    if (textureData != null)
                    {
                        Debug.Log("Saved to: " + savePath);
                        File.WriteAllBytes(savePath,textureData);
                    
                    }
                    else
                    {
                        Debug.LogWarning("could not encode texture");
                    
                    }
                    AssetDatabase.Refresh();
        
    }
}
