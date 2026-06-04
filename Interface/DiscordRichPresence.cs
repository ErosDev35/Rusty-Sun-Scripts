using UnityEngine;
using Discord;

public class DiscordRichPresence : MonoBehaviour
{
    Discord.Discord discord;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        discord = new Discord.Discord(1500540850302746804, (ulong)Discord.CreateFlags.NoRequireDiscord);
        DiscordRich();
    }

    // Update is called once per frame
    void Update()
    {
        if(discord != null)
        discord.RunCallbacks();
    }
    void OnDisable()
    {
        if(discord != null)
        discord.Dispose();
    }
    void DiscordRich()
    {
        if(discord != null){
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = "Lost in the void"
        };
        activityManager.UpdateActivity(activity, (res) =>
        {
            Debug.Log("Updated discord activity");
        });
        }
    }
}
