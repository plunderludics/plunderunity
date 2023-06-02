using UnityEngine;

[CreateAssetMenu(fileName = "TwitchSecrets", menuName = "plunity/TwitchSecrets", order = 0)]
public class TwitchSecrets : ScriptableObject {
    public string CLIENT_ID = "CLIENT_ID"; //Your application's client ID, register one at https://dev.twitch.tv/dashboard
	public string OAUTH_TOKEN = "OAUTH_TOKEN"; //A Twitch OAuth token which can be used to connect to the chat
	public string USERNAME_FROM_OAUTH_TOKEN = "USERNAME_FROM_OAUTH_TOKEN"; //The username which was used to generate the OAuth token
}