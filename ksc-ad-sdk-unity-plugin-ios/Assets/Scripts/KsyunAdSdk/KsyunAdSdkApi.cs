using System;

public interface KsyunAdSdkApi
{

	void init (string appId);

	void init (string appId, string channelId);

	void init (string appId, string channelId, string config);

	void preloadAd ();

	void preloadAd (string adSlotId);

	bool hasAd (string adSlotId);

	bool hasLocalAd (string adSlotId);

	void showAd (string adSlotId);

	string getSdkVersion ();

	void clearCache();

}

