using UnityEditor;
using UnityEngine;

namespace STYLY.Uploader
{
    /// <summary>
    /// STYLYプラグイン設定ウィンドウ。
    /// とりあえずViewクラスなので、なるべくこのクラスへの参照が少なくなるようにしてください。
    /// </summary>
    public class SettingsWindow : EditorWindow
    {
        private enum CacheServerMode { Local, Remote, Disabled }
        private enum CacheServer2Mode { Enabled, Disabled }

        const string CacheServerModeKey = "CacheServerMode";
        const string CacheServerEnabledKey = "CacheServerEnabled";
        const string CacheServer2ModeKey = "CacheServer2Mode";

        public const string SETTING_KEY_STYLY_EMAIL = "SUITE.STYLY.CC.ASSET_UPLOADER_EMAIL";
        public const string SETTING_KEY_STYLY_API_KEY = "SUITE.STYLY.CC.API_KEY";
        public const string SETTING_KEY_STYLY_AZCOPY_ENABLED = "SUITE.STYLY.CC.AZCOPY_ENABLED";

        public string email;
        public string api_key;
        public bool azcopyEnabled;

        private System.Reflection.MethodInfo isPlatformSupportLoaded;
        private System.Reflection.MethodInfo getTargetStringFromBuildTarget;

        private int onGUICallCounter = 0;

        bool IsPlatformSupportLoaded(string platform)
        {
            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { platform });
        }

        string GetTargetStringFromBuildTarget(BuildTarget target)
        {
            return (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { target });
        }

        /// <summary>
        /// Check if cache server is enabled or not, including cache server for AssetPipeline-v2.
        /// </summary>
        private static bool IsCacheServerEnabled()
        {
            if (IsCacheServerV1Enabled())
            {
                return true;
            }

            if (IsCacheServerV2Enabled())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// returns true if the cache server is enabled.
        /// </summary>
        private static bool IsCacheServerV1Enabled()
        {
            var defaultValue = EditorPrefs.GetBool(CacheServerEnabledKey) ? CacheServerMode.Remote : CacheServerMode.Disabled;
            var cacheServerMode = (CacheServerMode)EditorPrefs.GetInt(CacheServerModeKey, (int)defaultValue);
            // cache server v1 is enabled when CacheServerMode.Local or CacheServerMode.Remote.
            return cacheServerMode == CacheServerMode.Local || cacheServerMode == CacheServerMode.Remote;
        }

        /// <summary>
        /// returns true if the cache server for AssetPipelineV2 is enabled.
        /// </summary>
        private static bool IsCacheServerV2Enabled()
        {
#if UNITY_2019_1_OR_NEWER
            return (CacheServer2Mode)EditorPrefs.GetInt(CacheServer2ModeKey, (int)CacheServer2Mode.Disabled) == CacheServer2Mode.Enabled;
#else
            return false;
#endif
        }

        void Awake()
        {
            this.minSize = new Vector2(400, 300);
            email = EditorPrefs.GetString(SETTING_KEY_STYLY_EMAIL);
            api_key = EditorPrefs.GetString(SETTING_KEY_STYLY_API_KEY);
            azcopyEnabled = Config.IsAzCopyEnabled;
        }

        private void RenderFooter()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("About STYLY", GUILayout.Width(120)))
            {
                Application.OpenURL(Config.AboutStylyUrl);
            }
            GUILayout.EndHorizontal();
        }

        void OnGUI()
        {
            // http://answers.unity3d.com/questions/1324195/detect-if-build-target-is-installed.html
            var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            GUILayout.Label("Unity plugin for STYLY", EditorStyles.boldLabel);
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            EditorGUILayout.LabelField("Right click on a prefab and select \"STYLY\"-\"Upload prefab or scene to STYLY\". Your prefab will appear in \"3D Model\"-\"My Models\" section in STYLY. ", style);

            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Get STYLY API Key", GUILayout.Width(120)))
            {
                Application.OpenURL(Config.GetAPIKeyUrl);
            }
            if (GUILayout.Button("Get Started", GUILayout.Width(120)))
            {
                Application.OpenURL(Config.GetStartedUrl);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUIStyle platformStyle = new GUIStyle();
            GUIStyleState platformStyleState = new GUIStyleState();
            platformStyleState.textColor = Color.red;
            platformStyle.normal = platformStyleState;
            bool allPlatformInstalled = true;

            foreach (var platform in Config.PlatformList)
            {
                var target = Config.PlatformBuildTargetDic[platform];
                if (!IsPlatformSupportLoaded(GetTargetStringFromBuildTarget(target)))
                {
                    EditorGUILayout.LabelField(target.ToString() + " module not installed.", platformStyle);
                    allPlatformInstalled = false;
                }
            }
            if (allPlatformInstalled == false)
            {
                EditorGUILayout.LabelField("STYLY Uploader requires above modules. Please install these modules.", platformStyle);

                GUILayout.BeginHorizontal("box");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("More Information", GUILayout.Width(120)))
                {
                    Application.OpenURL(Config.ModulesErrorUrl);
                }
                GUILayout.EndHorizontal();
            }

            if (!IsCacheServerEnabled())
            {
                EditorGUILayout.LabelField("Cache Server is strongly recommended.\n Change the setting at Edit-Preferences-CacheServer", platformStyle);
            }

            GUILayout.Space(20);
            EditorGUI.BeginChangeCheck();

            email = EditorGUILayout.TextField("Email", email);
            GUILayout.Space(5);
            api_key = EditorGUILayout.TextField("API Key", api_key);
            GUILayout.Space(30);

            // Validate AzCopy exec file presence
            // NOTE this will run even if the "enable" toggle wasn't pressed
            // In order to avoid freeze of Unity Editor, avoid calling DisplayDialog() at the initial several calls of OnGUI().
            if (onGUICallCounter > 5 && azcopyEnabled && !AzcopyExecutableFileInstaller.Validate())
            {
                azcopyEnabled = false;
                // mark as dirty to register this change
                GUI.changed = true;

                EditorUtility.DisplayDialog(
                    "AzCopy exec file not found",
                    "Please click the 'Enable AzCopy' checkbox to install AzCopy again.",
                    "OK");
            }

            // azcopy setting
            var azcopyEnabledNew = GUILayout.Toggle(azcopyEnabled, "Enable AzCopy");
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("AzCopy can be used for faster upload", MessageType.Info);
            if (!azcopyEnabled && azcopyEnabledNew)
            {
                EditorUtility.DisplayProgressBar("Installing AzCopy", "", 0);

                if (AzcopyExecutableFileInstaller.Install(out var error))
                {
                    azcopyEnabled = true;

                    EditorUtility.DisplayDialog(
                        "AzCopy installation success",
                        "AzCopy was installed properly!",
                        "OK");
                }
                else
                {
                    Debug.LogError($"AzCopy was not installed: {error}");
                    EditorUtility.DisplayDialog(
                        "Failed installing AzCopy",
                        $"AzCopy was not installed.\n{error}",
                        "OK");
                }

                EditorUtility.ClearProgressBar();
            }
            else
            {
                // Disable AzCopy
                azcopyEnabled = azcopyEnabledNew;
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(SETTING_KEY_STYLY_EMAIL, email.Trim());
                EditorPrefs.SetString(SETTING_KEY_STYLY_API_KEY, api_key.Trim());
                Config.IsAzCopyEnabled = azcopyEnabled;
            }

            GUILayout.Space(20);
            this.Repaint();

            // about Styly
            GUILayout.BeginVertical();
            RenderFooter();
            GUILayout.EndVertical();

            onGUICallCounter++;
        }
    }
}