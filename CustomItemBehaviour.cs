using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomItemBehaviour : MonoBehaviour
{
    public string itemName;
    public List<string> itemInteractionStr;
    public AudioSource musicSource;
    public float musicTime = 0;
    public GameInterface gameInterface;
    public bool activateCooldown = false;
    public List<AudioClip> musics = new List<AudioClip>();
    void Start()
    {
        gameInterface = GameObject.Find("UI").GetComponent<GameInterface>();
        musicSource = GetComponent<AudioSource>();
        itemName = GetComponent<Item>().itemName;
        if (musicSource && musicSource.clip && musicSource.time == 0)
        {
            musicSource.Play();
        }
    }
    public void Update()
    {
        if(musicSource && musicSource.clip){
            musicTime = musicSource.time;
        }
    }
    public void OnActivate()
    {
        if (!activateCooldown)
        {
            activateCooldown = true;
            StartCoroutine(disableActivateCooldown(0.5f));

            switch (itemName)
            {
                case "Boombox":
                    if (musicSource.clip)
                    {
                        print("on arrête la musique");
                        musicSource.clip = null;
                        musicSource.Stop();
                    }
                    else{
                        musicSource.clip = musics[Random.Range(0, musics.Count)];
                        musicSource.Play();
                    }
                    break;
            }

            gameInterface.SoundSync();
        }
    }
    public string GetDescriptorState()
    {
        string descriptor = itemInteractionStr[musicSource.clip? 1 : 0];
        return descriptor;
    }
    IEnumerator disableActivateCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        activateCooldown = false;
    }
}
