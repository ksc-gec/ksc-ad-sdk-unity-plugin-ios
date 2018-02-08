#import "KsyunAD.h"
#import "UnityAppController.h"


@implementation KsyunADLib

static NSString* KS_U3D_OBJECT = @"KsyunAdGameObj";

static NSString* KEY_SHOW_CLOSE_BTN_OF_REWARD_VIDEO = @"KeyIsShowClose";
static NSString* KEY_CLOSE_BTN_COMING_TIME_OF_REWARD_VIDEO = @"KeyCloseComingTime";
static NSString* KEY_SDK_ENV = @"KeySdkEnv";
static NSString* KEY_SDK_DEBUG_MODE = @"KeyDebugMode";
static NSString* KEY_ENABLE_SDK_REQUEST_PERMISSION = @"KeyEnableSdkRequestPermission";


+ (instancetype) sharedSdkLib
{
    static KsyunADLib *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken,^{
        sharedInstance = [[KsyunADLib alloc] init];
    });
    return sharedInstance;
}

- (void)initializeSDKWithAppId:(NSString *)appId config:(NSString *)configStr
{
    KsyunAdSDKConfig *config;
    if (configStr) {
        configStr = [configStr stringByReplacingOccurrencesOfString:@"True" withString:@"1"];
        configStr = [configStr stringByReplacingOccurrencesOfString:@"False" withString:@"0"];
        NSDictionary *confDict = [self jsonWithString:configStr];
        NSNumber *isDebug = confDict[KEY_SDK_DEBUG_MODE] ?: @(YES);
        NSNumber *env = confDict[KEY_SDK_ENV] ?: @(KsyunAdEnvironment_Sandbox);
        NSNumber *isShowReward = confDict[KEY_SHOW_CLOSE_BTN_OF_REWARD_VIDEO] ?: @(YES);
        NSNumber *premisson = confDict[KEY_ENABLE_SDK_REQUEST_PERMISSION] ?: @(YES);
        NSNumber *showTime = confDict[KEY_CLOSE_BTN_COMING_TIME_OF_REWARD_VIDEO] ?: @(5);
        config = [KsyunAdSDKConfig sdkConfigWithDebugMode:isDebug.boolValue
                                            adEnvironment:KsyunAdEnvironment(env.unsignedIntegerValue)
                                isShowRewardVideoCloseBtn:isShowReward.boolValue
                                   enableObtainPremission:premisson.boolValue
                              rewardVideoCloseBtnShowTime:showTime.unsignedIntegerValue];
    } else {
        config = [KsyunAdSDKConfig sdkConfigWithDebugMode:YES
                                            adEnvironment:KsyunAdEnvironment_Sandbox
                                isShowRewardVideoCloseBtn:YES
                                   enableObtainPremission:YES
                              rewardVideoCloseBtnShowTime:5];
    }
    
    [KsyunAdSDK initializeWithAppid:appId sdkConfig:config successBlock:^(NSDictionary * _Nullable info) {
        [self sendCallback:@"initSdkSuccess" param:[self toJson:info]];
    } failureBlock:^(KsyunAdErrCode errCode, NSString * _Nullable errMsg) {
        [self sendErrorCallback:@"initSdkFailure" withErrorCode:errCode errMsg:errMsg];
    }];
}

- (void)preloadAd 
{
    [KsyunAdSDK preloadAd:self];
}

- (void)preloadAdWithAd:(NSString *)adSlotId
{
    [KsyunAdSDK preloadAd:adSlotId delegate:self];
}

- (int)hasAd:(NSString *)adSlotId
{
    return [KsyunAdSDK hasAd:adSlotId];
}

- (int)hasLocalAd:(NSString *)adSlotId
{
    return [KsyunAdSDK hasLocalAd:adSlotId];
}


- (void)showAd:(NSString *)adSlotId
{
    UnityAppController *unityAppController = (UnityAppController *)[UIApplication sharedApplication].delegate;
    [KsyunAdSDK showAdWithAdSlotId:adSlotId viewController:unityAppController.rootViewController adDelegate:self];
    [KsyunAdSDK setRewardVideoDelegate:self];
}

- (void)clearCache
{
    [KsyunAdSDK clearCache];
}

- (NSString *)sdkVsrsion
{
    return [KsyunAdSDK version];
}

#pragma mark - KsyunPreloadADDelegate

// 广告信息加载成功
- (void)onAdInfoSuccess
{
    NSString *callbackName = @"preloadAdInfoSuccess";
    [self sendSuccessCallback:callbackName];
}

// 广告信息加载失败
- (void)onAdInfoFailed:(NSArray<NSString *> *_Nullable)adSlotIds errCode:(NSInteger)errCode errMsg:(NSString *_Nullable)errMsg
{
    NSString *callbackName = @"preloadAdInfoFailure";
    NSMutableDictionary *dict = [@{} mutableCopy];
    [dict setValue:adSlotIds forKey:@"adSlotIds"];
    [self sendErrorCallback:callbackName withErrorCode:errCode errMsg:errMsg data:[dict copy]];
}

// 广告预加载成功
- (void)onAdLoaded:(NSString *_Nonnull)adSlotId
{
    NSString *callbackName = @"onAdLoaded";
    [self sendCallback:callbackName param:adSlotId];
}

#pragma -mark  KsyunRewardVideoAdDelegate

// 奖励视频获得奖励的回调
- (void)onADAwardSuccess:(NSString *_Nonnull)adSlotId
{
    NSString *callbackName = @"onADAwardSuccess";
    [self sendCallback:callbackName param:adSlotId];
}

// 奖励视频获取奖励失败的回调
- (void)onADAwardFailed:(NSString *_Nonnull)adSlotId errCode:(KsyunRewardVideoAdErrCode)errCode errMsg:(NSString *_Nullable)errMsg
{
    NSString *callbackName = @"onADAwardFailed";
    NSDictionary *dict = @{@"adSlotId" : adSlotId ?: @""};
    [self sendErrorCallback:callbackName withErrorCode:errCode errMsg:errMsg data:dict];
}

#pragma mark KsyunADDelegate
// 广告展示成功
- (void)onShowSuccess:(NSString *_Nonnull)adSlotId
{
    NSString *callbackName = @"onShowSuccess";
    [self sendCallback:callbackName param:adSlotId];
}

// 广告展示失败
- (void)onShowFailed:(NSString *_Nonnull)adSlotId errCode:(int)errCode errMsg:(NSString *_Nullable)errMsg
{
    NSString *callbackName = @"onShowFailed";
    NSDictionary *dict = @{@"adSlotId" : adSlotId ?: @""};
    [self sendErrorCallback:callbackName withErrorCode:errCode errMsg:errMsg data:dict];
}

// 广告播放完成
- (void)onADComplete:(NSString *_Nonnull)adSlotId
{
    NSString *callbackName = @"onADComplete";
    [self sendCallback:callbackName param:adSlotId];
}

// 广告点击的回调
- (void)onADClick:(NSString *_Nonnull)adSlotId
{
    NSString *callbackName = @"onADClick";
    [self sendCallback:callbackName param:adSlotId];
}

// 广告被关闭的回调
- (void)onADClose:(NSString *_Nonnull)adSlotId
{
    NSString *callbackName = @"onADClose";
    [self sendCallback:callbackName param:adSlotId];
}

#pragma mark CallBack
/**
 *  回调函数
 */
- (void)sendSuccessCallback:(NSString *)callbackName
{
    [self sendCallback:KS_U3D_OBJECT callbackName:callbackName param:@"{\"on\":\"success\"}"];
}
//
//- (void) sendSuccessCallback:(NSString *)callbackName CallbackId:(int) callbackId
//{
//    NSMutableDictionary *dic = [NSMutableDictionary dictionary];
//    [dic setObject:@"success" forKey:@"on"];
//    [dic setObject:[NSNumber numberWithInt:callbackId] forKey:@"callbackid"];
//    NSString *json = [self toJson:dic];
//    if(json != nil)
//        [self sendCallback:KS_U3D_OBJECT callbackName:callbackName param:json];
//}
//
//- (void) sendSuccessCallback:(NSString *)callbackName CallbackId:(int) callbackId data:(NSString *)jsonData
//{
//    NSMutableDictionary *dic = [NSMutableDictionary dictionary];
//    [dic setObject:@"success" forKey:@"on"];
//    [dic setObject:[NSNumber numberWithInt:callbackId] forKey:@"callbackid"];
//    [dic setObject:jsonData forKey:@"data"];
//    NSString *json = [self toJson:dic];
//    if(json != nil)
//        [self sendCallback:KS_U3D_OBJECT callbackName:callbackName param:json];
//}
- (void)sendErrorCallback:(NSString *)callbackName withErrorCode:(NSInteger)errorCode errMsg:(NSString *)errMsg
{
    [self sendErrorCallback:callbackName withErrorCode:errorCode errMsg:errMsg data:nil];
}

- (void)sendErrorCallback:(NSString *)callbackName withErrorCode:(NSInteger)errorCode errMsg:(NSString *)errMsg data:(NSDictionary *)dataDict
{
    NSMutableDictionary *dic = [NSMutableDictionary dictionary];
    if (dataDict && dataDict.count) {
        [dic setValue:dataDict forKey:@"data"];
    }
    [dic setObject:@"error" forKey:@"on"];
    [dic setObject:@(errorCode)  forKey:@"code"];
    [dic setObject:errMsg ?: @"" forKey:@"message"];
    NSString *json = [self toJson:dic];
    if(json != nil){
        [self sendCallback:KS_U3D_OBJECT callbackName:callbackName param:json];
    }
}

//- (void)sendErrorCallback:(NSString *)callbackName withError:(NSError *)error
//{
//    NSMutableDictionary *dic = [NSMutableDictionary dictionary];
//    [dic setObject:@"error" forKey:@"on"];
//    [dic setObject:[NSNumber numberWithInt:[error code]]  forKey:@"code"];
//    [dic setObject:[error localizedDescription] forKey:@"message"];
//    NSString *json = [self toJson:dic];
//    if(json != nil){
//        [self sendCallback:KS_U3D_OBJECT callbackName:callbackName param:json];
//    }
//}

- (void)sendCallback:(NSString *)callbackName param:(NSString *)jsonParam
{
    [self sendCallback:KS_U3D_OBJECT callbackName:callbackName param:jsonParam];
}

- (void)sendCallback:(NSString *)objName callbackName:(NSString *)callbackName param:(NSString *)jsonParam
{
    NSLog(@"Send to objName=%@, callbackName=%@, param=%@",objName,callbackName,jsonParam);
    UnitySendMessage([objName UTF8String], [callbackName UTF8String], [jsonParam UTF8String]);
}

- (NSString *)toJson:(id)ocData
{
    NSError *error;
    NSData *data = [NSJSONSerialization dataWithJSONObject:ocData options:0 error:&error];
    if(!error){
        return [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    }
    return nil;
}

- (id)jsonWithString:(NSString *)jsonString
{
    NSError *error;
    NSData *strData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    id jsonObj = [NSJSONSerialization JSONObjectWithData:strData options:0 error:&error];
    return jsonObj;
}

@end

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}
// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}


#ifdef __cplusplus
extern "C" {
#endif

	void _init(const char *appId)
	{
		[[KsyunADLib sharedSdkLib] initializeSDKWithAppId:CreateNSString(appId) config:nil];
	}
    
    void _init2(const char *appId, const char *channelId)
    {
        [[KsyunADLib sharedSdkLib] initializeSDKWithAppId:CreateNSString(appId) config:nil];
    }
    
    void _init3(const char * appId, const char * channelId, const char * config)
    {
        NSString *conf = CreateNSString(config);
        [[KsyunADLib sharedSdkLib] initializeSDKWithAppId:CreateNSString(appId) config:conf];
    }

	void _preloadAd()
	{
		[[KsyunADLib sharedSdkLib] preloadAd];
	}
    
    void _preloadAdWithAd (const char * adSlotId)
    {
        [[KsyunADLib sharedSdkLib] preloadAdWithAd:CreateNSString(adSlotId)];
    }
    

	int _hasAd (const char * adSlotId)//11684010
	{
		return [[KsyunADLib sharedSdkLib] hasAd:CreateNSString(adSlotId)];
	}
    
    int _hasLocalAd(const char * adSlotId)
    {
        return [[KsyunADLib sharedSdkLib] hasLocalAd:CreateNSString(adSlotId)];
    }

	void _showAd (const char * adSlotId)
	{
		[[KsyunADLib sharedSdkLib] showAd:CreateNSString(adSlotId)];
	}
    
    void _clearCache (const char * adSlotId)
    {
        [[KsyunADLib sharedSdkLib] clearCache];
    }
    
    const char * _getSdkVersion()
    {
        NSString *version = [[KsyunADLib sharedSdkLib] sdkVsrsion];
        return [version UTF8String];
    }


#ifdef __cplusplus
}
#endif
