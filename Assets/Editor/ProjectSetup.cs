using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LumenRush.Editor
{
    [InitializeOnLoad]
    public static class ProjectSetup
    {
        static readonly string[] Scenes = {"Boot", "MainMenu", "Gameplay", "CharacterSelection", "Shop"};
        static ProjectSetup()
        {
            EditorApplication.delayCall += Ensure;
        }

        static void Ensure()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                return;
            if (!File.Exists("Assets/Settings/SetupComplete.txt"))
                Configure();
        }

        [MenuItem("Lumen Rush/Configure project")]
        public static void Configure()
        {
            Directory.CreateDirectory("Assets/Resources");
            Directory.CreateDirectory("Assets/Settings");
            var config = AssetDatabase.LoadAssetAtPath<RunnerConfig>("Assets/Resources/RunnerConfig.asset");
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<RunnerConfig>();
                AssetDatabase.CreateAsset(config, "Assets/Resources/RunnerConfig.asset");
            }

            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/MobileRenderer.asset");
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, "Assets/Settings/MobileRenderer.asset");
            }

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/MobileURP.asset");
            if (pipeline == null)
            {
                pipeline = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                AssetDatabase.CreateAsset(pipeline, "Assets/Settings/MobileURP.asset");
            }

            var serialized = new SerializedObject(pipeline);
            var list = serialized.FindProperty("m_RendererDataList");
            list.arraySize = 1;
            list.GetArrayElementAtIndex(0).objectReferenceValue = renderer;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            pipeline.renderScale = 1;
            pipeline.msaaSampleCount = 2;
            pipeline.shadowDistance = 40;
            pipeline.supportsHDR = false;
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
            // Retain the runtime-created primitive material shader in player builds.
            var graphics = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            var shaders = graphics.FindProperty("m_AlwaysIncludedShaders");
            var lit = Shader.Find("Universal Render Pipeline/Lit");
            bool found = false;
            for (int i = 0; i < shaders.arraySize; i++)
                if (shaders.GetArrayElementAtIndex(i).objectReferenceValue == lit)
                    found = true;
            if (!found)
            {
                int i = shaders.arraySize;
                shaders.InsertArrayElementAtIndex(i);
                shaders.GetArrayElementAtIndex(i).objectReferenceValue = lit;
                graphics.ApplyModifiedPropertiesWithoutUndo();
            }

            PlayerSettings.companyName = "Afterlight Studio";
            PlayerSettings.productName = "Lumen Rush";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.afterlight.lumenrush");
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.iOS, "com.afterlight.lumenrush");
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            var buildScenes = new EditorBuildSettingsScene[Scenes.Length];
            for (int i = 0; i < Scenes.Length; i++)
                buildScenes[i] = new EditorBuildSettingsScene("Assets/Scenes/" + Scenes[i] + ".unity", true);
            EditorBuildSettings.scenes = buildScenes;
            EditorUtility.SetDirty(pipeline);
            AssetDatabase.SaveAssets();
            File.WriteAllText("Assets/Settings/SetupComplete.txt", "Lumen Rush setup complete\n");
            AssetDatabase.Refresh();
            Debug.Log("Lumen Rush configured. Open Boot and press Play.");
        }

        [MenuItem("Lumen Rush/Build Android APK")]
        public static void BuildAndroid()
        {
            Configure();
            Directory.CreateDirectory("Builds/Android");
            Build(BuildTarget.Android, "Builds/Android/LumenRush.apk");
        }

        [MenuItem("Lumen Rush/Export iOS Xcode project")]
        public static void BuildIOS()
        {
            Configure();
            Directory.CreateDirectory("Builds/iOS");
            Build(BuildTarget.iOS, "Builds/iOS");
        }

        static void Build(BuildTarget target, string path)
        {
            var scenes = System.Array.ConvertAll(EditorBuildSettings.scenes, s => s.path);
            var report = BuildPipeline.BuildPlayer(scenes, path, target, BuildOptions.None);
            if (report.summary.result != BuildResult.Succeeded)
                throw new System.Exception("Build failed: " + report.summary.result);
        }
    }
}
