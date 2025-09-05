using UnityEditor;
using UnityEngine;

namespace ProfilerReference
{
    /// <summary>
    /// Script khởi tạo cho Profiler Reference Tool
    /// Đảm bảo tool hoạt động đúng khi được import vào project
    /// </summary>
    [InitializeOnLoad]
    public static class ProfilerReferenceInitializer
    {
        static ProfilerReferenceInitializer()
        {
            // Đăng ký callback khi Unity load
            EditorApplication.delayCall += Initialize;
        }

        private static void Initialize()
        {
            // Kiểm tra và tạo các file cần thiết
            ValidateInstallation();

            // Hiển thị welcome message lần đầu
            ShowWelcomeMessage();

            // Thiết lập các define symbols nếu cần
            SetupDefineSymbols();
        }

        private static void ValidateInstallation()
        {
            var requiredFiles = new[]
            {
                "Assets/ProfilerReference/ProfilerAnalyzer.cs",
                "Assets/ProfilerReference/OptimizationSuggestion.cs",
                "Assets/ProfilerReference/ProfilerReferenceWindow.cs",
                "Assets/ProfilerReference/ProfilerReferenceMenu.cs",
                "Assets/ProfilerReference/ProfilerDataCollector.cs",
                "Assets/ProfilerReference/README.md"
            };

            var missingFiles = new System.Collections.Generic.List<string>();

            foreach (var file in requiredFiles)
            {
                if (!System.IO.File.Exists(file))
                {
                    missingFiles.Add(file);
                }
            }

            if (missingFiles.Count > 0)
            {
                var message = "Profiler Reference Tool - Thiếu files:\n\n";
                foreach (var file in missingFiles)
                {
                    message += $"• {file}\n";
                }
                message += "\nVui lòng reinstall package hoặc tạo lại các files bị thiếu.";

                Debug.LogError(message);
                EditorUtility.DisplayDialog("Profiler Reference - Lỗi cài đặt", message, "OK");
            }
            else
            {
                Debug.Log("✅ Profiler Reference Tool đã được cài đặt thành công");
            }
        }

        private static void ShowWelcomeMessage()
        {
            const string WelcomeKey = "ProfilerReference_WelcomeShown";
            if (EditorPrefs.GetBool(WelcomeKey, false)) return;

            var message = "🎉 Chào mừng đến với Profiler Reference Tool!\n\n" +
                         "Công cụ này sẽ giúp bạn:\n" +
                         "• Phân tích performance của Unity project\n" +
                         "• Phát hiện function/script cần tối ưu\n" +
                         "• Gợi ý cách tối ưu chi tiết\n\n" +
                         "Để bắt đầu: Tools > Profiler Reference > Open Tool";

            EditorUtility.DisplayDialog("Profiler Reference Tool", message, "Bắt đầu", "Để sau");

            EditorPrefs.SetBool(WelcomeKey, true);
        }

        private static void SetupDefineSymbols()
        {
            // Lấy define symbols cho tất cả platforms
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

            const string DefineSymbol = "PROFILER_REFERENCE";

            if (!defines.Contains(DefineSymbol))
            {
                defines += ";" + DefineSymbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
                Debug.Log("✅ Đã thêm define symbol: PROFILER_REFERENCE");
            }
        }

        /// <summary>
        /// Reset welcome message (for testing)
        /// </summary>
        [MenuItem("Tools/Profiler Reference/Reset Welcome Message", false, 22)]
        private static void ResetWelcomeMessage()
        {
            EditorPrefs.DeleteKey("ProfilerReference_WelcomeShown");
            Debug.Log("Đã reset welcome message");
        }

        /// <summary>
        /// Validate installation manually
        /// </summary>
        [MenuItem("Tools/Profiler Reference/Validate Installation", false, 23)]
        private static void ManualValidateInstallation()
        {
            ValidateInstallation();
        }
    }
}
