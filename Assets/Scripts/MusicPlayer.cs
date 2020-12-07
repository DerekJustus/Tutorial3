using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource winMusic;
    public AudioSource loseMusic;

    public RubyController rubyController;

    // Start is called before the first frame update
    void Start()
    {
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");

        backgroundMusic.Play();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (rubyController.gameOver == true)
        {
            backgroundMusic.Stop();
            loseMusic.enabled = true;
        }

        if (rubyController.gameWin == true)
        {
            backgroundMusic.Stop();
            winMusic.enabled = true;
        }
    }



}
