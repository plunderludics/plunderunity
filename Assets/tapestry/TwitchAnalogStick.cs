// If type or namespace TwitchLib could not be found. Make sure you add the latest TwitchLib.Unity.dll to your project folder
// Download it here: https://github.com/TwitchLib/TwitchLib.Unity/releases
// Or download the repository at https://github.com/TwitchLib/TwitchLib.Unity, build it, and copy the TwitchLib.Unity.dll from the output directory
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

public class TwitchAnalogStick : MonoBehaviour
{
	[SerializeField] private TwitchSecrets secrets;

	[SerializeField] private float m_PollDuration;

	[SerializeField] private float m_MaxMagnitude;
	[SerializeField] private float m_InputRecenterSpeed;

	[SerializeField] private float m_InputIncreasePerMessage;

	[SerializeField] private Vector2 m_Input;

	[SerializeField] private TankMover m_Mover;
	[SerializeField] private SampleLoader m_SampleLoader;
	[SerializeField] private TapestryBlender m_Blender;

	private Client client;
	private float pollTime;

	private void Start()
	{
		// To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
		// Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
		// This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
		// Application.runInBackground = true;

		//Create Credentials instance
		ConnectionCredentials credentials = new ConnectionCredentials(secrets.USERNAME_FROM_OAUTH_TOKEN, secrets.OAUTH_TOKEN);

		// Create new instance of Chat Client
		client = new Client();

		// Initialize the client with the credentials instance, and setting a default channel to connect to.
		client.Initialize(credentials, secrets.USERNAME_FROM_OAUTH_TOKEN);

		// Bind callbacks to events
		client.OnConnected += OnConnected;
		client.OnJoinedChannel += OnJoinedChannel;
		client.OnMessageReceived += OnMessageReceived;
		client.OnChatCommandReceived += OnChatCommandReceived;

		// Connect
		client.Connect();
	}

	private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
	{
		Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch.");

		if (!string.IsNullOrWhiteSpace(e.AutoJoinChannel))
			Debug.Log($"The bot will now attempt to automatically join the channel provided when the Initialize method was called: {e.AutoJoinChannel}");
	}

	private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
	{
		Debug.Log($"The bot {e.BotUsername} just joined the channel: {e.Channel}");
		client.SendMessage(e.Channel, "I just joined the channel! PogChamp");
	}

	private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
	{
		Debug.Log($"Message received from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
        switch (e.ChatMessage.Message.ToLowerInvariant()) {
            case "right":
            case "east":
                m_Input += Vector2.right * m_InputIncreasePerMessage;
                break;
            case "left":
            case "west":
                m_Input += Vector2.left * m_InputIncreasePerMessage;
                break;
            case "up":
            case "forward":
            case "north":
                m_Input += Vector2.up * m_InputIncreasePerMessage;
                break;
            case "down":
            case "back":
            case "backward":
            case "south":
                m_Input += Vector2.down * m_InputIncreasePerMessage;
                break;
        }
        m_Input = Vector2.ClampMagnitude(m_Input, 1);
	}

	private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
	{
		switch (e.Command.CommandText)
		{
			case "hello":
				client.SendMessage(e.Command.ChatMessage.Channel, $"Hello {e.Command.ChatMessage.DisplayName}!");
				break;
			case "about":
				client.SendMessage(e.Command.ChatMessage.Channel, "I am a Twitch bot running on TwitchLib!");
				break;
			default:
				client.SendMessage(e.Command.ChatMessage.Channel, $"Unknown chat command: {e.Command.CommandIdentifier}{e.Command.CommandText}");
				break;
		}
	}

    private void Update() {
        m_Input = Vector2.MoveTowards(m_Input, Vector2.zero, m_InputRecenterSpeed * Time.deltaTime);
        m_Input = Vector2.ClampMagnitude(m_Input, m_MaxMagnitude);

        m_Mover.Input = m_Input;
        pollTime += Time.deltaTime;

        if(pollTime > m_PollDuration) {
            pollTime = 0;
            foreach(var track in m_Blender.AllTracks) {
                m_SampleLoader.SendAnalogInput(track, m_Input);
            }
        }
    }
}

