using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//实际调用Android SDK接口类
public class KsyunAdSdkAndroid : MonoBehaviour, KsyunAdSdkApi
{
	public const string KsyunAdNativeClassName = "com.ksc.ad.sdk.unity.KsyunAdSdkUnityMethods";

	public void init (string appId)
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		androidMethodClass.CallStatic ("init", appId);
	}

	public void init (string appId, string channelId)
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		androidMethodClass.CallStatic ("init", appId, channelId);
	}

	public void init (string appId, string channelId, string config)
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		androidMethodClass.CallStatic ("init", appId, channelId, config);
	}

	public void preloadAd ()
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		androidMethodClass.CallStatic ("preloadAd");
	}

	public void preloadAd (string adSlotId)
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		androidMethodClass.CallStatic ("preloadAd", adSlotId);
	}

	public bool hasAd (string adSlotId)
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		return androidMethodClass.CallStatic<bool> ("hasAd", adSlotId);
	}

	public bool hasLocalAd (string adSlotId)
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		return androidMethodClass.CallStatic<bool> ("hasLocalAd", adSlotId);
	}


	public void showAd (string adSlotId)
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		androidMethodClass.CallStatic ("showAd", adSlotId);
	}


	public string getSdkVersion ()
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		return androidMethodClass.CallStatic<string> ("getSdKVersion");
	}

	public void clearCache ()
	{
		AndroidJavaClass androidMethodClass = new AndroidJavaClass (KsyunAdNativeClassName);
		androidMethodClass.CallStatic ("clearCache");
	}

}
