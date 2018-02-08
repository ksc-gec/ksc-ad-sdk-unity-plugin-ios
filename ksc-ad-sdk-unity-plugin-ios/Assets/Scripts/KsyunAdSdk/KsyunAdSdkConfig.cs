using System;
using System.Collections.Generic;


public class KsyunAdSdkConfig
{

	private const string KEY_SHOW_CLOSE_BTN_OF_REWARD_VIDEO = "KeyIsShowClose";
	private const string KEY_CLOSE_BTN_COMING_TIME_OF_REWARD_VIDEO = "KeyCloseComingTime";
	private const string KEY_SDK_ENV = "KeySdkEnv";
	private const string KEY_SDK_DEBUG_MODE = "KeyDebugMode";
	private const string KEY_ENABLE_SDK_REQUEST_PERMISSION = "KeyEnableSdkRequestPermission";

	public Dictionary<string,object> mSdkConfigDictionary = new Dictionary<string, object> ();


	public void setSingleConfig (string key, object value)
	{
		mSdkConfigDictionary.Add (key, value);
	}

	public void setShowCloseBtnOfRewardVideo (bool isShow)
	{
		mSdkConfigDictionary.Add (KEY_SHOW_CLOSE_BTN_OF_REWARD_VIDEO, isShow);
	}

	public void setCloseBtnComingTimeOfRewardVideo (int time)
	{
		mSdkConfigDictionary.Add (KEY_CLOSE_BTN_COMING_TIME_OF_REWARD_VIDEO, time);
	}

	public void setSdkEnvironment (int environment)
	{
		mSdkConfigDictionary.Add (KEY_SDK_ENV, environment);
	}

	public void setSdkDebugMode (bool debug)
	{
		mSdkConfigDictionary.Add (KEY_SDK_DEBUG_MODE, debug);
	}

	public void setEnabeSdkRequestPermission (bool isEnableRequestPermission)
	{
		mSdkConfigDictionary.Add (KEY_ENABLE_SDK_REQUEST_PERMISSION, isEnableRequestPermission);
	}

	public Object getSingleConfig (String key)
	{
		return mSdkConfigDictionary [key];
	}

	public String convertToString()
	{
		
		String[] keys = { 
			KEY_SHOW_CLOSE_BTN_OF_REWARD_VIDEO,
			KEY_CLOSE_BTN_COMING_TIME_OF_REWARD_VIDEO,
			KEY_SDK_ENV,
			KEY_SDK_DEBUG_MODE,
			KEY_ENABLE_SDK_REQUEST_PERMISSION
		};

		List<String> itemsList = new List<String>();

		foreach (String key in keys) {
			if (mSdkConfigDictionary.ContainsKey(key)) {
				String pair = string.Format("\"{0}\": {1}", key, mSdkConfigDictionary[key]);
				itemsList.Add(pair);
			}
		}

		String itemsStr = String.Join (",", itemsList.ToArray ());
		String jsonStr = "{" + itemsStr + "}";
		return jsonStr;
	}

}

