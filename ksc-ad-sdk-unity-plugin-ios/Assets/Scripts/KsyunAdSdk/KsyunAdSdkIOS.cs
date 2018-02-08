using System;
using System.Runtime.InteropServices;


public class KsyunAdSdkIOS : KsyunAdSdkApi
{
	[DllImport ("__Internal")]  
	private static extern void _init (string appId);

	[DllImport ("__Internal")]  
	private static extern void _init2 (string appId, string channelId);

	[DllImport ("__Internal")]  
	private static extern void _init3 (string appId, string channelId, string config);

	[DllImport ("__Internal")]  
	private static extern void _preloadAd ();

	[DllImport ("__Internal")]  
	private static extern void _preloadAdWithAd (string adSlotId);

	[DllImport ("__Internal")]  
	private static extern bool _hasAd (string appId);

	[DllImport ("__Internal")]
	private static extern bool _hasLocalAd (string adSlotId);

	[DllImport ("__Internal")]  
	private static extern void _showAd (string appId);

	[DllImport ("__Internal")]  
	private static extern string _getSdkVersion ();

	[DllImport ("__Internal")] 
	private static extern void _clearCache ();


	public void init (string appId)
	{
		_init (appId);
	}

	public void init (string appId, string channelId)
	{
		_init2 (appId, channelId);
	}

	public void init (string appId, string channelId, string config)
	{
		_init3 (appId, channelId, config);
	}

	public void preloadAd ()
	{
		_preloadAd ();
	}

	public void preloadAd (string adSlotId)
	{
		_preloadAdWithAd (adSlotId);
	}

	public void showAd (string adSlotId)
	{
		_showAd (adSlotId);
	}

	public bool hasAd (string adSlotId)
	{
		return _hasAd (adSlotId);
	}

	public bool hasLocalAd (string adSlotId)
	{
		return _hasLocalAd (adSlotId);
	}

	public string getSdkVersion ()
	{
		return _getSdkVersion ();
	}

	public void clearCache ()
	{
		_clearCache ();
	}

}

