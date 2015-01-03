// use Win32Api so currently support only windows

using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

//Version 0.22 | twitter:@cubic9com add keybind `esc` to exit VR Mode 
//Version 0.21 | twitter:@izm update for DK2 
//Version 0.2 | s.b.Newsom Edition

//Source from http://answers.unity3d.com/questions/179775/game-window-size-from-editor-window-in-editor-mode.html
//Modified by seieibob for use at the Virtual Environment and Multimodal Interaction Lab at the University of Maine.
//Use however you'd like!

//Modified by sbNewsom. Like it is said above, use as you like! If you're interested in my work, check out:
//http://www.sbnewsom.com

/// <summary>
/// Displays a popup window that undocks, repositions and resizes the game window according to
/// what is specified by the user in the popup. Offsets are applied to ensure screen borders are not shown.
/// </summary>
/// 
#if UNITY_EDITOR
public class GameWindowMover : EditorWindow
{
//	private static int window_size_x=1920;
//	private static int window_pos_x=1920;

	//The size of the toolbar above the game view, excluding the OS border.
	private int tabHeight = 22;
	
	private bool toggle = true;
	
	//Get the size of the window borders. Changes depending on the OS.
	#if UNITY_STANDALONE_WIN
	//Windows settings
	private int osBorderWidth = 5;
	#elif UNITY_STANDALONE_OSX
	//Mac settings (untested)
	private int osBorderWidth = 0; //OSX windows are borderless.
	#else
	//Linux / other platform; sizes change depending on the variant you're running
	private int osBorderWidth = 5;
	#endif
	//default setting 
	private static Vector2 _gameSize = new Vector2(0, 0); //window positon init size
	private static Vector2 _gamePosition = new Vector2(0, 0);
	
	//Desired window resolution
	public Vector2 gameSize = new Vector2(_gameSize.x, _gameSize.y);
	
	//Desired window position
	public Vector2 gamePosition = new Vector2(_gamePosition.x, _gamePosition.y);
	
	//Tells the script to use the default resolution specified in the player settings.
	//private bool usePlayerSettingsResolution = false;
	
	//For those that duplicate screen
	//private bool useDesktopResolution = false;
	private bool useDesktopResolution = true;

#if UNITY_STANDALONE_WIN

	// ---------------------------------------------------------------------------------------------------
	// add for win32 api to get display resolution
	// ---------------------------------------------------------------------------------------------------
	// resolutions list exclude main monitor
	private List<RectApi> screenInfo;
	
	delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdc, ref RectApi pRect, int dwData);
	[DllImport("user32")]
	static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);
	
	[StructLayout(LayoutKind.Sequential)]
	struct RectApi
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
		public int width
		{
			get
			{
				return right - left;
			}
		}
		public int height
		{
			get
			{
				return bottom - top;
			}
		}
	}
	
	void GetDisplays()
	{
		if (!EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, enumCallBack, 0))
			Debug.Log("An error occured while enumerating monitors");
	}
	
	bool enumCallBack(IntPtr hMonitor, IntPtr hdc, ref RectApi prect, int d)
	{
		if (!(prect.left == 0 && prect.top == 0) && 
		    ((prect.width == 1920 && prect.height == 1080) || 
		 (prect.width == 1080 && prect.height == 1920) ||
		 (prect.width == 1280 && prect.height == 800))
		    )
		{
			// (left,top) is not (0,0) and screen resolution is fullHD or WXGA(suppose they are OculusRifts)
			screenInfo.Add(prect);
		}
		return true;
	}
	
	void Awake()
	{
		SetDefault(false);
	}
	
	void SetDefault(bool flg)
	{
		screenInfo = new List<RectApi>();
		GetDisplays();
		// Set first display(without main screen)
		if (screenInfo.Count > 0)
		{
			if (flg || gameSize.x == 0)
			{
				gameSize = new Vector2(screenInfo[0].width, screenInfo[0].height);
				gamePosition = new Vector2(screenInfo[0].left, screenInfo[0].top);
			}
		}
	}
	// ---------------------------------------------------------------------------------------------------
	// end win32 api
	// ---------------------------------------------------------------------------------------------------

#endif
	//Shows the popup
	[MenuItem("Window/Rift VR Game Mode")]
	static void OpenPopup()
	{
		GameWindowMover window = (GameWindowMover)(EditorWindow.GetWindow(typeof(GameWindowMover)));
		//Set popup window properties
		Vector2 popupSize = new Vector2(300, 140);
		//When minSize and maxSize are the same, no OS border is applied to the window.
		window.minSize = popupSize;
		window.maxSize = popupSize;
		window.title = "RiftMode";
		window.ShowPopup();
	}
	
	//Returns the current game view as an EditorWindow object.
	public static EditorWindow GetMainGameView()
	{
		//Creates a game window. Only works if there isn't one already.
		EditorApplication.ExecuteMenuItem("Window/Game");
		
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetMainGameView.Invoke(null, null);
		return (EditorWindow)Res;
	}
	
	void OnGUI()
	{
		
		EditorGUILayout.Space();
		
		if (useDesktopResolution)
		{
			gameSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		}
		gameSize = EditorGUILayout.Vector2Field("Rift Display Size:", gameSize);
		gamePosition = EditorGUILayout.Vector2Field("Rift Display Position:", gamePosition);
		if (GUILayout.Button("Reset"))
		{
			SetDefault(true);
		}
		GUILayout.Label("Rift Game Mode is now activated. ");
		
		GUILayout.Label("Don't close this panel to keep script in effect.");
		
	}
	
	void Update()
	{
		if (toggle)
		{
			if (Application.isPlaying)
			{
				if (Input.GetKey(KeyCode.Escape))
				{
					CloseGameWindow();
					toggle = false;
					UnityEditor.EditorApplication.isPlaying = false;
				}
			}
			else
			{
				CloseGameWindow();
				toggle = false;
			}
		}
		else
		{
			if (Application.isPlaying)
			{
				MoveGameWindow();
				toggle = true;
			}
		}
	}
	
	void MoveGameWindow()
	{
		EditorWindow gameView = GetMainGameView();
		gameView.title = "Game (Oculus Rift)";
		//When minSize and maxSize are the same, no OS border is applied to the window.
		gameView.minSize = new Vector2(gameSize.x, gameSize.y + tabHeight - osBorderWidth);
		gameView.maxSize = gameView.minSize;
		Rect newPos = new Rect(gamePosition.x, gamePosition.y - tabHeight, gameSize.x, gameSize.y + tabHeight - osBorderWidth);
		gameView.position = newPos;
		gameView.ShowPopup();
	}
	
	void CloseGameWindow()
	{
		EditorWindow gameView = GetMainGameView();
		gameView.Close();
	}
}
#endif