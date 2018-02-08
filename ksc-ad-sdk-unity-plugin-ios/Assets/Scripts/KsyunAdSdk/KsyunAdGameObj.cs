using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KsyunAdGameObj : MonoBehaviour
{
	private static KsyunAdGameObj instance;
	private const string KsyunAdGameObjTag = "KsyunAdGameObj";

	private KsyunAdGameObj ()
	{
	}

	public static KsyunAdGameObj getInstance ()
	{
		if (instance == null) {
			instance = new KsyunAdGameObj ();
			createSdkGameObj ();
		}
		return instance;
	}

	void initSdkSuccess (string adSlotsStr)
	{
		Debug.Log ("initSdkSuccess,adSlotsStr =" + adSlotsStr);
		if (KsyunAdSdk.initSdkSuccess != null) {
			KsyunAdSdk.initSdkSuccess (adSlotsStr);
		}
	}

	void initSdkFailure (string msg)
	{
		Debug.Log ("initSdkFailure,msg = " + msg);
		if (KsyunAdSdk.initSdkFailure != null) {
			KsyunAdSdk.initSdkFailure (msg);
		}
	}

	void preloadAdInfoSuccess (string msg)
	{
		Debug.Log ("preloadAdInfoSuccess,msg = " + msg);
		if (KsyunAdSdk.preloadAdInfoSuccess != null) {
			KsyunAdSdk.preloadAdInfoSuccess (msg);
		}
	}

	void preloadAdInfoFailure (string msg)
	{
		Debug.Log ("preloadAdInfoFailure,msg = " + msg);
		if (KsyunAdSdk.preloadAdInfoFailure != null) {
			KsyunAdSdk.preloadAdInfoFailure (msg);
		}
	}

	void onAdLoaded (string msg)
	{
		Debug.Log ("onAdLoaded,msg = " + msg);
		if (KsyunAdSdk.onAdLoaded != null) {
			KsyunAdSdk.onAdLoaded (msg);
		}
	}

	void onADAwardSuccess (string adSlotId)
	{
		Debug.Log ("onADAwardSuccess,adSlotId = " + adSlotId);
		if (KsyunAdSdk.onADAwardSuccess != null) {
			KsyunAdSdk.onADAwardSuccess (adSlotId);
		}

	}

	void onADAwardFailed (string msg)
	{
		Debug.Log ("onADAwardFailed,msg = " + msg);
		if (KsyunAdSdk.onADAwardFailed != null) {
			KsyunAdSdk.onADAwardFailed (msg);
		}
	}

	void onShowSuccess (string msg)
	{
		Debug.Log ("onShowSuccess,msg = " + msg);
		if (KsyunAdSdk.onShowSuccess != null) {
			KsyunAdSdk.onShowSuccess (msg);
		}
	}

	void onShowFailed (string msg)
	{
		Debug.Log ("onShowFailed,msg = " + msg);
		if (KsyunAdSdk.onShowFailed != null) {
			KsyunAdSdk.onShowFailed (msg);
		}
	}

	void onADComplete (string msg)
	{
		Debug.Log ("onADComplete,msg = " + msg);
		if (KsyunAdSdk.onADComplete != null) {
			KsyunAdSdk.onADComplete (msg);
		}
	}

	void onADClick (string msg)
	{
		Debug.Log ("onADClick,msg = " + msg);
		if (KsyunAdSdk.onADClick != null) {
			KsyunAdSdk.onADClick (msg);
		}
	}

	void onADClose (string msg)
	{
		Debug.Log ("onADClose,msg = " + msg);
		if (KsyunAdSdk.onADClose != null) {
			KsyunAdSdk.onADClose (msg);
		}
	}



	static void createSdkGameObj ()
	{
		//生成SDK内部GameObj，供Android侧回调用
		GameObject ksyunAdGameObj = new GameObject ("KsyunAdGameObj");
		ksyunAdGameObj.hideFlags = HideFlags.HideAndDontSave;
		DontDestroyOnLoad (ksyunAdGameObj);
		instance = ksyunAdGameObj.AddComponent<KsyunAdGameObj> ();
	}
}
