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
        discord.RunCallbacks();
    }
    void OnDisable()
    {
        discord.Dispose();
    }
    void DiscordRich()
    {
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
