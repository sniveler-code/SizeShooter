using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AndroidBuild
{
    const string PackageName = "com.DefaultCompany.SizeShooter";
    const string OutDir = "Build/Android";

    [MenuItem("Build/Android APK (IL2CPP ARM64)")]
    public static void BuildApk()
    {
        Build(true);
    }

    [MenuItem("Build/Android AAB (IL2CPP ARM64)")]
    public static void BuildAab()
    {
        Build(false);
    }

    static void Build(bool apk)
    {
        ApplySettings();

        Directory.CreateDirectory(OutDir);
        string ext = apk ? "apk" : "aab";
        string path = Path.Combine(OutDir, "SizeShooter." + ext);

        EditorUserBuildSettings.buildAppBundle = !apk;
        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        if (scenes.Length == 0)
            throw new BuildFailedException("No enabled scenes in EditorBuildSettings.");

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = path,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        Debug.Log($"[AndroidBuild] {summary.result} {path} size={summary.totalSize / (1024 * 1024)}MB errors={summary.totalErrors} warnings={summary.totalWarnings} time={summary.totalTime}");

        if (summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"[AndroidBuild] FAILED {summary.result}");
            if (Application.isBatchMode) EditorApplication.Exit(1);
            return;
        }

        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    static void ApplySettings()
    {
        PlayerSettings.companyName = "DefaultCompany";
        PlayerSettings.productName = "SizeShooter";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, PackageName);

        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;

        PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.Android, ManagedStrippingLevel.Low);
        PlayerSettings.SetIl2CppCompilerConfiguration(NamedBuildTarget.Android, Il2CppCompilerConfiguration.Release);

        PlayerSettings.Android.blitType = AndroidBlitType.Always;
        PlayerSettings.Android.renderOutsideSafeArea = true;
        PlayerSettings.MTRendering = true;
        PlayerSettings.gpuSkinning = true;
        PlayerSettings.graphicsJobs = false;

        AssetDatabase.SaveAssets();
    }
}
