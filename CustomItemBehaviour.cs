using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomItemBehaviour : MonoBehaviour
{
    public string itemName;
    public string itemInteraction = "";
    public List<string> itemInteractionStr;
    public bool activatedBehaviour;
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
    }
    public void Update()
    {
        if (activatedBehaviour && musicSource.time == 0)
        {
            musicSource.Play();
        }
        musicTime = musicSource.time;
    }
    public void OnActivate()
    {
        if (!activateCooldown)
        {
            activateCooldown = true;
            activatedBehaviour = !activatedBehaviour;
            itemInteraction = itemInteractionStr[(activatedBehaviour)? 1 : 0];
            StartCoroutine(disableActivateCooldown(0.25f));
            switch (itemName)
            {
                case "Boombox":
                    if (activatedBehaviour)
                    {
                        musicSource.Stop();
                        musicTime = 0;
                    }
                    else
                    print(musics.Count);
                    musicSource.clip = musics[Random.Range(0, musics.Count)];
                    musicSource.Play();
                    break;
            }
            gameInterface.SoundSync();
        }
    }
    IEnumerator disableActivateCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        activateCooldown = false;
    }
}
