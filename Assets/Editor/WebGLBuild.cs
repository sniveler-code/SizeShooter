using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuild
{
    const string OutDir = "docs";

    [MenuItem("Build/WebGL (GitHub Pages)")]
    public static void BuildWebGL()
    {
        ApplySettings();

        Directory.CreateDirectory(OutDir);

        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        if (scenes.Length == 0)
            throw new BuildFailedException("No enabled scenes in EditorBuildSettings.");

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OutDir,
            target = BuildTarget.WebGL,
            targetGroup = BuildTargetGroup.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        Debug.Log($"[WebGLBuild] {summary.result} {OutDir} size={summary.totalSize / (1024 * 1024)}MB errors={summary.totalErrors} time={summary.totalTime}");

        if (summary.result != BuildResult.Succeeded)
            throw new BuildFailedException($"[WebGLBuild] failed: {summary.result}");

        File.WriteAllText(Path.Combine(OutDir, ".nojekyll"), string.Empty);
        File.WriteAllText(Path.Combine(OutDir, "404.html"), "<!DOCTYPE html><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>SizeShooter</title><script>var s=location.pathname.split(\"/\")[1];location.replace(\"/\"+(s?s+\"/\":\"\"))<\/script><a href=\"/\">SizeShooter</a>");
    }

    static void ApplySettings()
    {
        PlayerSettings.productName = "SizeShooter";
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.WebGL, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.Medium);
        PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSize);

        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
        PlayerSettings.WebGL.threadsSupport = false;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        PlayerSettings.WebGL.memorySize = 256;
        PlayerSettings.WebGL.template = "PROJECT:GitHubPages";

        PlayerSettings.runInBackground = true;
        PlayerSettings.defaultWebScreenWidth = 1280;
        PlayerSettings.defaultWebScreenHeight = 720;

        AssetDatabase.SaveAssets();
    }
}
