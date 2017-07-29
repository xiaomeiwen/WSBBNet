using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
public class multiPanelControl : PunBehaviour {

	public Transform multiPanel;
	public Transform roomPanel;
	public Transform loginPanel;
	public Transform lobbyPanel;
    public Transform createRoomPanel;

    public Text messages;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    //SelectPanel
    void RoomPanel() {
        multiPanel.gameObject.SetActive(true);
		roomPanel.gameObject.SetActive(true);
		loginPanel.gameObject.SetActive(false);
		lobbyPanel.gameObject.SetActive(false);
        createRoomPanel.gameObject.SetActive(false);
    }
	void LoginPanel()
	{
		multiPanel.gameObject.SetActive(true);
        roomPanel.gameObject.SetActive(false);
		loginPanel.gameObject.SetActive(true);
		lobbyPanel.gameObject.SetActive(false);
        createRoomPanel.gameObject.SetActive(false);
	}
	void LobbyPanel()
	{
		multiPanel.gameObject.SetActive(true);
        roomPanel.gameObject.SetActive(false);
		loginPanel.gameObject.SetActive(false);
		lobbyPanel.gameObject.SetActive(true);
        createRoomPanel.gameObject.SetActive(false);
	}
    void CreateRoomPanel() {
		multiPanel.gameObject.SetActive(true);
		roomPanel.gameObject.SetActive(false);
		loginPanel.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(false);
		createRoomPanel.gameObject.SetActive(true);
    }
    // Called when finished editing nickname (which will also serve as 
    // room name - if player creates one)
    InputField edtNickname;
    public string RoomName;
	public void EnteredNickname()
	{
        RoomPanel();
		edtNickname = loginPanel.GetComponentInChildren<InputField>();
        PhotonNetwork.player.NickName = edtNickname.text;
		PhotonNetwork.ConnectUsingSettings("v1.0");
		messages.text = PhotonNetwork.player.NickName + "Connecting...";
        RoomName = PhotonNetwork.player.NickName;
	}
	// When connected to Photon, enable nickname editing (too)
	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
		messages.text = "Entering lobby...";

	}
	// When connected to Photon Lobby, disable nickname editing and messages text, enables room list
	public override void OnJoinedLobby()
	{
        LobbyPanel();
	}
	// For each valid game room, creates a join button.
    private List<RoomJoiner> rooms = new List<RoomJoiner>();
    public Transform racesPanel;
	public override void OnReceivedRoomListUpdate()
	{
		foreach (RoomJoiner roomButton in rooms)
		{
			Destroy(roomButton.gameObject);
		}
		rooms.Clear();
		int i = 0;
		foreach (RoomInfo room in PhotonNetwork.GetRoomList())
		{
			if (!room.IsOpen)
				continue;
			GameObject buttonPrefab = Resources.Load<GameObject>("RoomGUI");
			GameObject roomButton = Instantiate<GameObject>(buttonPrefab);
			roomButton.GetComponent<RoomJoiner>().RoomName = room.Name;
            string info = room.Name.Trim() + " (" + room.PlayerCount + "/" + room.MaxPlayers + ")";
			roomButton.GetComponentInChildren<Text>().text = info;
			rooms.Add(roomButton.GetComponent<RoomJoiner>());

			roomButton.transform.SetParent(racesPanel, false);
			roomButton.transform.position.Set(0, i * 120, 0);
		}
	}
	// Called from UI
	public void CreateGame()
	{
		RoomOptions options = new RoomOptions();
		options.MaxPlayers = 2;
		PhotonNetwork.CreateRoom(PhotonNetwork.player.NickName, options, TypedLobby.Default);
        CreateRoomPanel();
	}

	public override void OnPhotonCreateRoomFailed(object[] codeMessage)
	{
        messages.text = "On Photon Create Room Failed...";
		//if ((short)codeMessage[0] == ErrorCode.GameIdAlreadyExists)
		//{
		//	PhotonNetwork.playerName += "-2";
		//	CreateGame();
		//}
	}

	// if we join (or create) a room, no need for the create button anymore;
	public override void OnJoinedRoom()
	{
		RoomPanel();
		SetCustomProperties(PhotonNetwork.player, PhotonNetwork.playerList.Length - 1);
	}
	// sets and syncs custom properties on a network player (including masterClient)
	private void SetCustomProperties(PhotonPlayer player, int position)
	{
		ExitGames.Client.Photon.Hashtable customProperties = 
            new ExitGames.Client.Photon.Hashtable() { { "position", position }};
		player.SetCustomProperties(customProperties);
	}

}
