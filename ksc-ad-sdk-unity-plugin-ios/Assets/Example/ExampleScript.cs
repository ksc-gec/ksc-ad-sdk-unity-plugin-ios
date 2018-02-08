using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using TinyJson;

public class ExampleScript : MonoBehaviour {

	// SDK 配置信息
	public string appId 
	{
		get {
			if (!PlayerPrefs.HasKey ("appId")) {
				PlayerPrefs.SetString ("appId", "d86d3e47");
				PlayerPrefs.Save ();
			}
			return PlayerPrefs.GetString ("appId");
		}

		set {
			PlayerPrefs.SetString ("appId", value);
			PlayerPrefs.Save ();
		}
	}

	public string closeButtonShowTime
	{
		get {
			if (!PlayerPrefs.HasKey ("closeButtonShowTime")) {
				PlayerPrefs.SetString ("closeButtonShowTime", "5");
				PlayerPrefs.Save ();
			}
			return PlayerPrefs.GetString ("closeButtonShowTime");
		}

		set {
			PlayerPrefs.SetString ("closeButtonShowTime", value);
			PlayerPrefs.Save ();
		}
	}

	public int closeButtonIsShow
	{
		get {
			if (!PlayerPrefs.HasKey ("closeButtonIsShow")) {
				PlayerPrefs.SetInt ("closeButtonIsShow", 1);
				PlayerPrefs.Save ();
			}
			return PlayerPrefs.GetInt ("closeButtonIsShow");
		}

		set {
			PlayerPrefs.SetInt ("closeButtonIsShow", value);
			PlayerPrefs.Save ();
		}
	}

	public int environment
	{
		get {
			if (!PlayerPrefs.HasKey ("environment")) {
				PlayerPrefs.SetInt ("environment", (int)Environment.Sandbox);
				PlayerPrefs.Save ();
			}
			return PlayerPrefs.GetInt ("environment");
		}

		set {
			PlayerPrefs.SetInt ("environment", value);
			PlayerPrefs.Save ();
		}
	}

	public int adPlayMode
	{
		get {
			if (!PlayerPrefs.HasKey ("adPlayMode")) {
				PlayerPrefs.SetInt ("adPlayMode", 0);
				PlayerPrefs.Save ();
			}
			return PlayerPrefs.GetInt ("adPlayMode");
		}

		set {
			PlayerPrefs.SetInt ("adPlayMode", value);
			PlayerPrefs.Save ();
		}
	}

	private enum Page
	{
		Home    = 0,
		Setting = 1,
		Log     = 2,
		Device  = 3,
	};

	private enum Environment
	{
		Release    = 2,
		Sandbox    = 3,
	}

	Vector2 scrollPosition = new Vector2(0, 0);
	Vector2 settingScrollPosition = new Vector2(0, 0);
	Vector2 logsScrollPosition = new Vector2(0, 0);
	Vector2 deviceScrollPosition = new Vector2(0, 0);

	private Page currentPage;
	private bool sdkHasInit;
	private List <Adslot> adSlotModels;
	private List <string> preloadSuccessAdSlotIds;
	private List <string> preloadFailureAdSlotIds;
	private string message;
	private int logLevel;

	public void OnInitBtnClicked ()
	{
		if (string.IsNullOrEmpty (appId)) {
			Log ("appId 为空！");
			return;
		}
		Log ("ExampleScene input appId = " + appId);

		KsyunAdSdk.initSdkSuccess = (string param) => {
			Adslots adslots = param.FromJson<Adslots>();
			this.adSlotModels = bringRewardVideoAdToFront (adslots.adSlots);
			this.sdkHasInit = true;
			Log ("KsyunAdSdk initSdkSuccess");
		};

		KsyunAdSdk.initSdkFailure = (string msg) => {
			Log ("KsyunAdSdk initSdkFailure, msg = " + msg, 1);
		};

		KsyunAdSdk.onADAwardSuccess = (string param) => {
			Log ("KsyunAdSdk onADAwardSuccess, param = " + param);
			KsyunAdSdk.preloadAd(param);
		};

		KsyunAdSdk.onADAwardFailed = (string param) => {
			Log ("KsyunAdSdk onADAwardFailed, param = " + param, 1);
		};

		KsyunAdSdk.onShowSuccess = (string param) => {
			Log ("KsyunAdSdk onShowSuccess, param = " + param);
		};

		KsyunAdSdk.onShowFailed = (string param) => {
			Log ("KsyunAdSdk onShowFailed, param = " + param, 1);
		};

		KsyunAdSdk.onADComplete = (string param) => {
			Log ("KsyunAdSdk onADComplete, param = " + param);
		};

		KsyunAdSdk.onADClick = (string param) => {
			Log ("KsyunAdSdk onADClick, param = " + param);
		};

		KsyunAdSdk.onADClose = (string param) => {
			Log ("KsyunAdSdk onADClose, param = " + param);
		};

		KsyunAdSdk.init (appId, getSdkConfig());
	}

	public void OnPreloadBtnClicked ()
	{
		preloadSuccessAdSlotIds = new List<string> {};
		preloadFailureAdSlotIds = new List<string> {};
		Log ("OnPreloadBtnClicked");

		KsyunAdSdk.preloadAdInfoSuccess = (string param) => {
			Log ("KsyunAdSdk preloadAdInfoSuccess, param = " + param);
		};

		KsyunAdSdk.preloadAdInfoFailure = (string param) => {
			preloadFailureAdSlotIds.Add(param);
			Log ("KsyunAdSdk preloadAdInfoFailure, param = " + param, 1);
		};

		KsyunAdSdk.onAdLoaded = (string param) => {
			preloadSuccessAdSlotIds.Add(param);
			Log ("KsyunAdSdk onAdLoaded, param = " + param);
		};

		KsyunAdSdk.preloadAd ();
	}

	public void OnShowAdBtnClicked (string adSlotId)
	{
		Log ("OnShowAdBtnClicked, adSlotId = " + adSlotId);
		if (string.IsNullOrEmpty (adSlotId)) {
			Log ("adSlotId 为空！");
			return;
		}
		if (adPlayMode == 1) {
			if (KsyunAdSdk.hasLocalAd (adSlotId)) {
				Log ("KsyunAdSdk onLocalAdExist, adSlotId = " + adSlotId);
			} else {
				Log ("KsyunAdSdk onNoLocalAd, adSlotId = " + adSlotId, 1);
				return;
			}
		} else {
			if (KsyunAdSdk.hasAd (adSlotId)) {
				Log ("KsyunAdSdk onAdExist, adSlotId = " + adSlotId);
			} else {
				Log ("KsyunAdSdk onNoAd, adSlotId = " + adSlotId, 1);
			}
		}
		KsyunAdSdk.showAd (adSlotId);
	}

	public void OnClearCacheBtnClicked ()
	{
		Log ("OnClearCacheBtnClicked");
		preloadSuccessAdSlotIds = new List<string> {};
		preloadFailureAdSlotIds = new List<string> {};
		KsyunAdSdk.clearCache ();
	}

	KsyunAdSdkConfig getSdkConfig() {
		KsyunAdSdkConfig config = new KsyunAdSdkConfig ();
		config.setCloseBtnComingTimeOfRewardVideo (int.Parse(closeButtonShowTime));
		config.setShowCloseBtnOfRewardVideo (closeButtonIsShow == 1 ? true : false);
		config.setSdkEnvironment (environment);
		config.setSdkDebugMode (true);
		config.setEnabeSdkRequestPermission (true);
		Log (config.convertToString ());
		return config;
	}

	void Start () {
		sdkHasInit = false;
		currentPage = Page.Home;
		adSlotModels = new List<Adslot> {};
		preloadSuccessAdSlotIds = new List<string> {};
		preloadFailureAdSlotIds = new List<string> {};
		message = "";
		logLevel = 0;
	}

	void Update () {
		// scroll
		if(Input.touchCount == 0) return;
		Touch touch = Input.touches[0];
		if (touch.phase == TouchPhase.Moved) {
			float dt = Time.deltaTime / touch.deltaTime;
			if (dt == 0 || float.IsNaN(dt) || float.IsInfinity(dt))
				dt = 1.0f;
			Vector2 glassDelta = touch.deltaPosition * dt;
			if (currentPage == Page.Home) {
				scrollPosition.y += glassDelta.y;
			} else if (currentPage == Page.Setting) {
				settingScrollPosition.y += glassDelta.y;
			} else if (currentPage == Page.Log) {
				logsScrollPosition.y += glassDelta.y;
			} else if (currentPage == Page.Device) {
				deviceScrollPosition.y += glassDelta.y;
			}
		}
	}

	void OnGUI () {
		if (currentPage == Page.Home) {
			home ();
		} else if (currentPage == Page.Setting) {
			setting ();
		} else if (currentPage == Page.Log) {
			logPage ();
		} else if (currentPage == Page.Device) {
			deviceInfo ();
		}
		Log (message, logLevel);
	}

	void home () {
		// setting button
		GUIStyle settingButtonStyle = new GUIStyle ();
		settingButtonStyle.normal.background = (Texture2D)Resources.Load ("settingImage");
		settingButtonStyle.active.background = (Texture2D)Resources.Load ("settingImage_active");

		if (GUI.Button (new Rect (20, 20, 100, 100), "", settingButtonStyle)) {
			Log ("setting");
			currentPage = Page.Setting;
		}

		// log button
		GUIStyle logButtonStyle = new GUIStyle ();
		logButtonStyle.normal.background = (Texture2D)Resources.Load ("logsImage");
		logButtonStyle.active.background = (Texture2D)Resources.Load ("logsImage_active");

		if (GUI.Button (new Rect (Screen.width - 20 - 80, 30, 80, 80), "", logButtonStyle)) {
			Log ("log");
			currentPage = Page.Log;
		}

		// title label
		GUIStyle titleLabelStyle = new GUIStyle ();
		titleLabelStyle.fontSize = 40;
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect (120, 20, Screen.width - 240, 100), "金山云移动广告Demo", titleLabelStyle);

		int currentMarginTop = 0;
		int contentHeight = 320 + 170;
		if (adSlotModels.Count > 0) {
			contentHeight += 240 * adSlotModels.Count;
		}
		scrollPosition = GUI.BeginScrollView (new Rect (0, 140, Screen.width, Screen.height - 140), scrollPosition, new Rect (0, 0, Screen.width - 100, contentHeight), GUIStyle.none, GUIStyle.none);
		// init
		appIdInfo (currentMarginTop);
		currentMarginTop += 250 + 20;

		// preload
		if (sdkHasInit) {
			preload (currentMarginTop);
			currentMarginTop += 150 + 20;
		}

		// adSlots
		adSlots (currentMarginTop);
		GUI.EndScrollView ();
	}

	void appIdInfo (int marginTop) {
		// background
		GUIStyle backgroundStyle = new GUIStyle ();
		backgroundStyle.normal.background = (Texture2D)Resources.Load ("appIdInfoBackgroundImage");
		GUI.Label (new Rect (20, marginTop, Screen.width - 40, 250), "", backgroundStyle);

		// appId
		GUIStyle appIdStyle = new GUIStyle ();
		appIdStyle.fontSize = 35;
		appIdStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label (new Rect (0, marginTop + 15, Screen.width, 80), "AppId：" + appId, appIdStyle);

		// init button
		GUIStyle initButtonStyle = new GUIStyle ();
		if (currentPage == Page.Home) {
			initButtonStyle.normal.background = (Texture2D)Resources.Load ("initButton");
		} else {
			initButtonStyle.normal.background = (Texture2D)Resources.Load ("initButton_selected");
		}
		initButtonStyle.active.background = (Texture2D)Resources.Load ("initButton_selected");
		initButtonStyle.active.textColor = new Color (195.0f / 255.0f, 205.0f / 255.0f, 210.0f / 255.0f);
		initButtonStyle.normal.textColor = new Color (255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
		initButtonStyle.fontSize = 35;
		initButtonStyle.alignment = TextAnchor.MiddleCenter;
		if (GUI.Button (new Rect ((Screen.width - 400) / 2, marginTop + 95, 400, 110), "初始化SDK", initButtonStyle)) {
			OnInitBtnClicked ();
		}
	}

	void preload (int marginTop) {
		// background
		GUIStyle backgroundStyle = new GUIStyle ();
		backgroundStyle.normal.background = (Texture2D)Resources.Load ("appIdInfoBackgroundImage");
		GUI.Label (new Rect (20, marginTop, Screen.width - 40, 150), "", backgroundStyle);

		// preload
		GUIStyle preloadButtonStyle = new GUIStyle ();
		preloadButtonStyle.normal.background = (Texture2D)Resources.Load ("initButton");
		preloadButtonStyle.active.background = (Texture2D)Resources.Load ("initButton_selected");
		preloadButtonStyle.active.textColor = new Color (195.0f / 255.0f, 205.0f / 255.0f, 210.0f / 255.0f);
		preloadButtonStyle.normal.textColor = new Color (255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
		preloadButtonStyle.fontSize = 35;
		preloadButtonStyle.alignment = TextAnchor.MiddleCenter;
		if (GUI.Button (new Rect ((Screen.width - 400) / 2, marginTop + 20, 400, 110), "预加载", preloadButtonStyle)) {
			OnPreloadBtnClicked ();
		}
	}

	void adSlots(int marginTop) {
		int newMarginTop = 0;
		for (int index = 0; index < adSlotModels.Count; index++) {
			newMarginTop = marginTop + (220 + 20) * index;

			Adslot adSlotModel = adSlotModels [index];

			GUIStyle backgroundStyle = new GUIStyle ();
			backgroundStyle.normal.background = (Texture2D)Resources.Load ("appIdInfoBackgroundImage");
			GUI.Label (new Rect (20, newMarginTop, Screen.width - 40, 220), "", backgroundStyle);

			GUIStyle titleLabelStyle = new GUIStyle ();
			titleLabelStyle.normal.textColor = new Color (0.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);
			titleLabelStyle.fontSize = 25;
			titleLabelStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label (new Rect (50, newMarginTop + 10, Screen.width, 50), "广告位Id：" + adSlotModel.adslot_id, titleLabelStyle);
			GUI.Label (new Rect (50, newMarginTop + 60, Screen.width, 50), "广告位名称：" + adSlotModel.adslot_name, titleLabelStyle);
			GUI.Label (new Rect (50, newMarginTop + 110, Screen.width, 50), "广告位类型：" + adSlotModel.adslot_type.ToString(), titleLabelStyle);
			GUI.Label (new Rect (50, newMarginTop + 160, Screen.width, 50), "广告位状态：" + adSlotModel.adslot_status.ToString(), titleLabelStyle);

			GUIStyle playButtonStyle = new GUIStyle ();
			if (preloadSuccessAdSlotIds.Contains (adSlotModel.adslot_id)) {
				playButtonStyle.normal.background = (Texture2D)Resources.Load ("playButton_preloadSuccess");
			} else if (preloadFailureAdSlotIds.Contains (adSlotModel.adslot_id)) {
				playButtonStyle.normal.background = (Texture2D)Resources.Load ("playButton_preloadFailure");
			} else {
				playButtonStyle.normal.background = (Texture2D)Resources.Load ("playButton");
			}
			playButtonStyle.active.background = (Texture2D)Resources.Load ("playButton_active");
			playButtonStyle.active.textColor = new Color (195.0f / 255.0f, 205.0f / 255.0f, 210.0f / 255.0f);
			playButtonStyle.normal.textColor = new Color (255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
			playButtonStyle.fontSize = 35;
			playButtonStyle.alignment = TextAnchor.MiddleCenter;

			if (preloadSuccessAdSlotIds.Contains (adSlotModel.adslot_id)) {
				if (GUI.Button (new Rect (Screen.width - 230 - 40, newMarginTop + (220 - 70 - 20), 230, 70), "缓存广告", playButtonStyle)) {
					OnShowAdBtnClicked (adSlotModel.adslot_id);
				}
			} else if (preloadFailureAdSlotIds.Contains (adSlotModel.adslot_id)) {
				if (GUI.Button (new Rect (Screen.width - 230 - 40, newMarginTop + (220 - 70 - 20), 230, 70), "缓存失败", playButtonStyle)) {
					OnShowAdBtnClicked (adSlotModel.adslot_id);
				}
			} else {
				if (GUI.Button (new Rect (Screen.width - 230 - 40, newMarginTop + (220 - 70 - 20), 230, 70), "播放广告", playButtonStyle)) {
					OnShowAdBtnClicked (adSlotModel.adslot_id);
				}
			}
		}
	}

	void setting () {
		// back button
		GUIStyle settingButtonStyle = new GUIStyle ();
		settingButtonStyle.normal.background = (Texture2D)Resources.Load ("backImage");
		settingButtonStyle.active.background = (Texture2D)Resources.Load ("backImage_active");

		if (GUI.Button (new Rect (20, 20, 100, 100), "", settingButtonStyle)) {
			Log ("back");
			currentPage = Page.Home;
		}

		// title label
		GUIStyle titleLabelStyle = new GUIStyle ();
		titleLabelStyle.fontSize = 40;
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect (120, 20, Screen.width - 240, 100), "设置", titleLabelStyle);

		// setting appId
		GUIStyle titleStyle = new GUIStyle ();
		titleStyle.fontSize = 30;
		titleStyle.alignment = TextAnchor.MiddleCenter;

		int contentHeight = 0;
		settingScrollPosition = GUI.BeginScrollView (new Rect (0, 150, Screen.width, Screen.height - 140), settingScrollPosition, new Rect (0, 0, Screen.width - 100, 1500), GUIStyle.none, GUIStyle.none);
		GUI.Label(new Rect (0, contentHeight, Screen.width, 50), "设置AppId", titleStyle);
		contentHeight += 50 + 20;

		GUIStyle textFieldStyle = new GUIStyle ();
		textFieldStyle.normal.textColor = new Color (0.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);
		textFieldStyle.fontSize = 30;
		textFieldStyle.alignment = TextAnchor.MiddleCenter;
		textFieldStyle.normal.background = (Texture2D)Resources.Load ("textFieldBackgroundImage");

		appId = GUI.TextField(new Rect((Screen.width - 400) / 2, contentHeight, 400, 70), appId, textFieldStyle);
		contentHeight += 70 + 40;

		// setting ad close button show time
		GUI.Label(new Rect (0, contentHeight, Screen.width, 50), "关闭按钮展示时机(秒)", titleStyle);
		contentHeight += 50 + 20;

		closeButtonShowTime = GUI.TextField(new Rect((Screen.width - 400) / 2, contentHeight, 400, 70), closeButtonShowTime, textFieldStyle);
		closeButtonShowTime = Regex.Replace(closeButtonShowTime, "[^0-9]", "");
		contentHeight += 70 + 40;

		GUIStyle swicthButtonStyle = new GUIStyle ();
		swicthButtonStyle.fontSize = 25;
		swicthButtonStyle.alignment = TextAnchor.MiddleCenter;

		// setting environment
		GUI.Label(new Rect (0, contentHeight, Screen.width, 50), "设置环境", titleStyle);
		contentHeight += 50 + 20;

		string [] environmentName = new string [] {"Release", "Sandbox"};
		for (int index = 0; index < environmentName.Length; index++) {
			if (environment == environmentNameToEnvironment (environmentName[index])) {
				swicthButtonStyle.normal.background = (Texture2D)Resources.Load ("swicthButton_on");
			} else {
				swicthButtonStyle.normal.background = (Texture2D)Resources.Load ("swicthButton_off");
			}
			int left = 0;
			if (environmentName.Length == 2) {
				left = (Screen.width - 150 * 2 - 40) / 2 + (150 + 40) * index;
			} else {
				left = index <= 2 ? (Screen.width - 150 * 3 - 40 * 2) / 2 + (150 + 40) * index : (Screen.width - 150 * 2 - 40) / 2 + (150 + 40) * (index - 3);
			}
			int top = index <= 2 ? contentHeight : contentHeight + 70 + 20;
			if (GUI.Button (new Rect (left, top, 150, 70), environmentName[index], swicthButtonStyle)) {
				Log (environmentName[index]);
				environment = environmentNameToEnvironment (environmentName[index]);
			}
		}
		if (environmentName.Length > 2) {
			contentHeight += 70 + 20;
		}
		contentHeight += 70 + 40;

		// setting ad close button show
		GUI.Label(new Rect (0, contentHeight, Screen.width, 50), "奖励视频是否显示关闭按钮", titleStyle);
		contentHeight += 50 + 20;

		string [] closeButtonName = new string [] {"是", "否"};
		for (int index = 0; index < closeButtonName.Length; index++) {
			if ((index == 0 && closeButtonIsShow == 1) || (index == 1 && closeButtonIsShow == 0)) {
				swicthButtonStyle.normal.background = (Texture2D)Resources.Load ("swicthButton_on");
			} else {
				swicthButtonStyle.normal.background = (Texture2D)Resources.Load ("swicthButton_off");
			}
			if (GUI.Button (new Rect ((Screen.width - 150 * 2 - 40) / 2 + (150 + 40) * index, contentHeight, 150, 70), closeButtonName[index], swicthButtonStyle)) {
				Log (closeButtonName[index]);
				closeButtonIsShow = index == 0 ? 1 : 0;
			}
		}
		contentHeight += 70 + 40;

		// ad play mode
		GUI.Label(new Rect (0, contentHeight, Screen.width, 50), "广告播放模式", titleStyle);
		contentHeight += 50 + 20;

		string [] playModeName = new string [] {"网络+本地", "本地 Only"};
		for (int index = 0; index < playModeName.Length; index++) {
			if (index == adPlayMode) {
				swicthButtonStyle.normal.background = (Texture2D)Resources.Load ("swicthButton_on");
			}  else {
				swicthButtonStyle.normal.background = (Texture2D)Resources.Load ("swicthButton_off");
			}
			if (GUI.Button (new Rect ((Screen.width - 150 * 2 - 40) / 2 + (150 + 40) * index, contentHeight, 150, 70), playModeName[index], swicthButtonStyle)) {
				Log (playModeName[index]);
				adPlayMode = index;
			}
		}
		contentHeight += 70 + 40;

		// clear cache
		GUI.Label(new Rect (0, contentHeight, Screen.width, 50), "清除缓存", titleStyle);
		contentHeight += 50 + 20;

		swicthButtonStyle.normal.background = (Texture2D)Resources.Load ("swicthButton_on");
		if (GUI.Button (new Rect ((Screen.width - 150) / 2, contentHeight, 150, 70), "清除缓存", swicthButtonStyle)) {
			OnClearCacheBtnClicked ();
		}
		contentHeight += 70 + 40;

		// device info
		GUI.Label(new Rect (0, contentHeight, Screen.width, 50), "查看设备信息", titleStyle);
		contentHeight += 50 + 20;

		if (GUI.Button (new Rect ((Screen.width - 150) / 2, contentHeight, 150, 70), "查看", swicthButtonStyle)) {
			Log ("device info");
			currentPage = Page.Device;
		}
		contentHeight += 70 + 40;

		GUI.EndScrollView ();
	}

	void deviceInfo () {
		// back button
		GUIStyle settingButtonStyle = new GUIStyle ();
		settingButtonStyle.normal.background = (Texture2D)Resources.Load ("backImage");
		settingButtonStyle.active.background = (Texture2D)Resources.Load ("backImage_active");

		if (GUI.Button (new Rect (20, 20, 100, 100), "", settingButtonStyle)) {
			Log ("back");
			currentPage = Page.Setting;
		}

		// title label
		GUIStyle titleLabelStyle = new GUIStyle ();
		titleLabelStyle.fontSize = 40;
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect (120, 20, Screen.width - 240, 100), "设备信息", titleLabelStyle);

		string str = "";
		var fileAddress = "";
		#if UNITY_IOS && !UNITY_EDITOR
		fileAddress = System.IO.Path.Combine(Application.persistentDataPath, "KsyunDevice.txt");
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		fileAddress = System.IO.Path.Combine(Application.persistentDataPath, "statis_folder/device_info.txt");
		#endif

		FileInfo fileInfo = new FileInfo(fileAddress);
		if (fileInfo.Exists) {
			StreamReader r = new StreamReader (fileAddress);
			str = r.ReadToEnd ();
		} else {
			str = "暂时还未记录，需发出请求后获取";
		}

		GUILayout.BeginArea (new Rect(20, 140, Screen.width - 40, Screen.height - 170));
		deviceScrollPosition = GUILayout.BeginScrollView (deviceScrollPosition, GUILayout.Width (Screen.width), GUILayout.Height (Screen.height));
		GUIStyle style = GUIStyle.none;
		style.fontSize = 30;
		style.wordWrap = true;
		GUILayout.Box (str, style);
		GUILayout.EndScrollView ();
		GUILayout.EndArea ();
	}

	void logPage () {
		// back button
		GUIStyle settingButtonStyle = new GUIStyle ();
		settingButtonStyle.normal.background = (Texture2D)Resources.Load ("backImage");
		settingButtonStyle.active.background = (Texture2D)Resources.Load ("backImage_active");

		if (GUI.Button (new Rect (20, 20, 100, 100), "", settingButtonStyle)) {
			Log ("back");
			currentPage = Page.Home;
		}

		// title label
		GUIStyle titleLabelStyle = new GUIStyle ();
		titleLabelStyle.fontSize = 40;
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect (120, 20, Screen.width - 240, 100), "日志", titleLabelStyle);

		string str = "";
		var fileAddress = "";

		#if UNITY_IOS && !UNITY_EDITOR
		fileAddress = System.IO.Path.Combine(Application.persistentDataPath, "KsyunBehavior.txt");
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		fileAddress = System.IO.Path.Combine(Application.persistentDataPath, "statis_folder/statis_info.txt");
		#endif

		FileInfo fileInfo = new FileInfo(fileAddress);
		if (fileInfo.Exists) {
			StreamReader r = new StreamReader (fileAddress);
			str = r.ReadToEnd ();
		} else {
			str = "没有日志";
		}
		str = str.Replace ("---ksyun---", "----------------------");

		GUILayout.BeginArea (new Rect(20, 140, Screen.width - 40, Screen.height - 170));
		logsScrollPosition = GUILayout.BeginScrollView (logsScrollPosition, GUILayout.Width (Screen.width), GUILayout.Height (Screen.height));
		GUIStyle style = GUIStyle.none;
		style.fontSize = 30;
		style.wordWrap = true;
		GUILayout.Box (str, style);
		GUILayout.EndScrollView ();
		GUILayout.EndArea ();
	}

	void Log (string message, int logLevel = 0) {
		if (message == "") {
			return;
		}
		if (this.message != message) {
			Debug.Log ("Log: " + message);
		}
		this.message = message;
		this.logLevel = logLevel;

		GUIStyle logLabelStyle = new GUIStyle ();
		logLabelStyle.fontSize = 35;
		logLabelStyle.alignment = TextAnchor.LowerLeft;
		logLabelStyle.wordWrap = true;
		if (logLevel == 1) {
			logLabelStyle.normal.textColor = new Color (213.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);
		} else {
			logLabelStyle.normal.textColor = new Color (255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 0.8f);
		}
		GUI.Label(new Rect (20, Screen.height - 120, Screen.width - 40, 100), "Log: " + message, logLabelStyle);
	}

	List <Adslot> bringRewardVideoAdToFront(List <Adslot> adSlotModels) {
		List <Adslot> newAdSlotModels = new List<Adslot> {};
		for (int index = 0; index < adSlotModels.Count; index++) {
			Adslot adSlot = adSlotModels [index];
			if (adSlot.adslot_type == 9) {
				newAdSlotModels.Insert (0, adSlot);
			} else {
				newAdSlotModels.Add (adSlot);
			}
			if (KsyunAdSdk.hasLocalAd (adSlot.adslot_id)) {
				preloadSuccessAdSlotIds.Add (adSlot.adslot_id);
			}
		}
		return newAdSlotModels;
	}

	int environmentNameToEnvironment(string environmentName)
	{
		int selectedEnvironment = 0;
		switch (environmentName) {
		case "Release":
			selectedEnvironment = 2;
			break;
		case "Sandbox":
			selectedEnvironment = 3;
			break;
		default:
			selectedEnvironment = 3;
			break;
		}
		return selectedEnvironment;
	}

	class Adslots
	{
		public List <Adslot> adSlots = new List<Adslot> {};
	}

	class Adslot
	{
		public string adslot_id = "";
		public string adslot_name = "";
		public int adslot_status = 0;
		public int adslot_type = 0;
		public int ads_num = 0;
	}
}
