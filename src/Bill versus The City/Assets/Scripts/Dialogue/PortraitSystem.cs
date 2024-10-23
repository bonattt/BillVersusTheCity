using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public static class PortraitSystem {

    public const int PORTRAIT_HEIGHT = 200;
    public const int PORTRAIT_WIDTH = 100;
    public static readonly Color ERROR_COLOR = new Color(1f, 0f, 0.82f, 0.4f);
    public const string DEFAULT_POSE = "standing";

    public const string BASE_PATH = "portraits";
    
    public static Texture2D GetPortrait(string character_name) {
        // Gets a portrait of the requested character using the default pose
        // TODO --- implement
        return GetPortrait(character_name, DEFAULT_POSE);
    }
    
    public static Texture2D GetPortrait(string character_name, string pose) {
        // Gets a portrait of the requested character using the given pose
        // if the character exists, but does not have the requested pose, return the character in the default pose.
        // if the character cannot be found, returns the Error Portrait
        string path = $"{BASE_PATH}/{character_name.ToLower()}/{pose.ToLower()}";
        Texture2D texture = Resources.Load<Texture2D>(path);
        if (texture != null) {
            return texture;
        }
        Debug.LogError($"failed to load character portrait '{path}'!");
        return GetErrorPortrait();
    }

    public static Texture2D GetErrorPortrait() {
        // returns a Texture2D that can be displayed in a portrait if there is an error looking up the requested portrait

        // Create a new texture with the given size
        Texture2D texture = new Texture2D(PORTRAIT_HEIGHT, PORTRAIT_WIDTH);

        // Fill the texture with the specified color
        Color[] pixels = new Color[PORTRAIT_HEIGHT * PORTRAIT_WIDTH];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = ERROR_COLOR;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }
}