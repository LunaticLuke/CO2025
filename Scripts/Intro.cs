using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public VideoClip vid;
    double length;

    private void Start()
    {
        length = vid.length;
        StartCoroutine(Process());
    }

    IEnumerator Process()
    {
        yield return new WaitForSeconds((float)length);
        SceneManager.LoadScene(1);
    }
}
