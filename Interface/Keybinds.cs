using System.Collections.Generic;
using UnityEngine;

public class Keybinds : MonoBehaviour
{
    public static Keybinds Instance {get; private set;}

    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    public Dictionary<string, KeyCode> keybinds;
}
