#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
// using UnityEditor.XCodeEditor;
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.IO;

public static class KsyunPostBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        Debug.LogWarning("BuildTargetxxxxxxxxxxxxxxxxxxxxxx");
        Debug.Log( path );
#if (UNITY_4 || UNITY_3 || UNITY_2)
        if (target == BuildTarget.iPhone)
#else
        if (target == BuildTarget.iOS)
#endif
        {
            // 创建 Xcode 工程配置的引用
            string _projPath = PBXProject.GetPBXProjectPath (path);
            PBXProject _pbxProj = new PBXProject ();
            _pbxProj.ReadFromString (File.ReadAllText (_projPath));
            string targetGuid = _pbxProj.TargetGuidByName ("Unity-iPhone");

            // 禁用 BitCode
            _pbxProj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

            // 添加 CoreTelephony.framework
            _pbxProj.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", true);

            // 增加 Objc 编译选项，引入 category
            _pbxProj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC"); 

            // 保存 Xcode 工程配置
            _pbxProj.WriteToFile(_projPath);

			// 添加定位选项
			string plistPath = path + "/Info.plist";  
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));  
			PlistElementDict rootDict = plist.root;  
			rootDict.SetString ("NSLocationAlwaysUsageDescription", "使用定位服务");  
			rootDict.SetString ("NSLocationWhenInUseUsageDescription", "使用定位服务"); 
			File.WriteAllText(plistPath, plist.WriteToString());  

        }
    }

    // public static void SetAttr(XCProject project)  
    // {  
        // var pbxproj = project.project;  
        // var attrs = pbxproj.attributes;  
        // var targetAttrs = (PBXDictionary)attrs["TargetAttributes"];  
        // PBXDictionary targetSetting = new PBXDictionary();  
        // targetSetting["ProvisioningStyle"] = "Manual";  
        // foreach (var t in targets)  
        // {  
        //     var targetID = (string)t;  
        //     if (targetAttrs.ContainsKey(targetID))  
        //     {  
        //         var TargetAttr = (PBXDictionary)targetAttrs[targetID];  
        //         TargetAttr.Append(targetSetting);  
        //     }  
        //     else  
        //     {  
        //         targetAttrs[targetID] = targetSetting;  
        //     }  
        // }  
    // }  


}
#endif
