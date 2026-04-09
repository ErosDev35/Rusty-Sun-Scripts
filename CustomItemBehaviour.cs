using System.Collections;
using UnityEngine;

public class CustomItemBehaviour : MonoBehaviour
{
    public string itemName;
    public bool activatedBehaviour;
    public AudioSource musicSource;
    public float musicTime = 0;
    public GameInterface gameInterface;
    public bool activateCooldown = false;
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
                        musicSource.Play();
                    break;
            }
            gameInterface.SoundSync();
            activatedBehaviour = !activatedBehaviour; 
        }
    }
    IEnumerator disableActivateCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        activateCooldown = false;
    }
}
