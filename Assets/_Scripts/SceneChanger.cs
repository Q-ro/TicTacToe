/*
    
Author : Andres Mrad
Date : Wednesday 24/February/2021 @ 03:48:24 
Description : Transitions the game scene to a given one
    
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    // Loads the scene with the given name
    public void LoadSceneByName(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void LoadSceneByIndex(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}