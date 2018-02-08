using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Unity侧SDK管理类
public class KsyunAdSdk : MonoBehaviour
{
	public static Action<string> initSdkSuccess;
	public static Action<string> initSdkFailure;
	public static Action<string> preloadAdInfoSuccess;
	public static Action<string> preloadAdInfoFailure;
	public static Action<string> onAdLoaded;
	public static Action<string> onAdExist;
	public static Action<string> onNoAd;
	public static Action<string> onADAwardSuccess;
	public static Action<string> onADAwardFailed;
	public static Action<string> onShowSuccess;
	public static Action<string> onShowFailed;
	public static Action<string> onADComplete;
	public static Action<string> onADClick;
	public static Action<string> onADClose;

	private static bool hasInit = false;
	private static KsyunAdSdkApi mApiImplements;

	//初始化
	void Awake ()
	{
		#if UNITY_IOS && !UNITY_EDITOR
		Debug.Log("ios");
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.Log("android");
		#endif

		#if UNITY_EDITOR  
		Debug.Log("unity editor");
		#endif
	}

	//调用SDKinit方法
	public static void init (string appId)
	{
		//已经初始化，则返回
		if (hasInit) {
			return;
		}
		//Unity接口层初始化
		sdkInit ();
		//对应平台接口实现
		mApiImplements.init (appId);
	}

	public static void init (string appId, KsyunAdSdkConfig config)
	{
		//已经初始化，则返回
		if (hasInit) {
			return;
		}
		//Unity接口层初始化
		sdkInit ();
		//对应平台接口实现
		mApiImplements.init (appId, "", config.convertToString());
	}
		
	private static void sdkInit ()
	{
		//生成接收回调GameObject
		KsyunAdGameObj.getInstance ();
		//根据平台类型，生成对应接口实现类
		#if UNITY_IOS && !UNITY_EDITOR
		mApiImplements = new KsyunAdSdkIOS();
		#endif

		#if UNITY_ANDROID && !UNITY_EDITOR
		mApiImplements = new KsyunAdSdkAndroid();
		#endif

		#if UNITY_EDITOR
		mApiImplements = new KsyunAdSdkUnity();
		#endif
	}

	public static void preloadAd ()
	{
		mApiImplements.preloadAd ();
	}

	public static void preloadAd (string adSlotId)
	{
		mApiImplements.preloadAd (adSlotId);
	}

	public static bool hasAd (string adSlotId)
	{
		return mApiImplements.hasAd (adSlotId);
	}

	public static bool hasLocalAd (string adSlotId)
	{
		return mApiImplements.hasLocalAd (adSlotId);
	}

	public static void showAd (string adSlotId)
	{
		mApiImplements.showAd (adSlotId);
	}

	public static void clearCache ()
	{
		mApiImplements.clearCache ();
	}
}
