using UnityEditor;
using UnityEngine;

namespace ProfilerReference
{
    /// <summary>
    /// Menu integration cho Profiler Reference Tool
    /// </summary>
    public static class ProfilerReferenceMenu
    {
        [MenuItem("Tools/Profiler Reference/Open Tool", false, 1)]
        private static void OpenProfilerReferenceTool()
        {
            ProfilerReferenceWindow.ShowWindow();
        }

        [MenuItem("Tools/Profiler Reference/Open Tool", true)]
        private static bool ValidateOpenProfilerReferenceTool()
        {
            return true; // Luôn khả dụng
        }

        [MenuItem("Tools/Profiler Reference/Check Profiler Status", false, 2)]
        private static void CheckProfilerStatus()
        {
            if (UnityEngine.Profiling.Profiler.enabled)
            {
                EditorUtility.DisplayDialog("Profiler Status",
                    "✅ Profiler đã được bật\n\n" +
                    "Tool có thể thu thập dữ liệu bình thường.",
                    "OK");
            }
            else
            {
                var result = EditorUtility.DisplayDialog("Profiler Status",
                    "⚠️ Profiler chưa được bật\n\n" +
                    "Bạn có muốn bật Profiler không?",
                    "Bật Profiler", "Để sau");

                if (result)
                {
                    UnityEngine.Profiling.Profiler.enabled = true;
                    EditorUtility.DisplayDialog("Profiler Status",
                        "✅ Profiler đã được bật thành công!",
                        "OK");
                }
            }
        }

        [MenuItem("Tools/Profiler Reference/Documentation", false, 20)]
        private static void OpenDocumentation()
        {
            var documentationPath = "Assets/ProfilerReference/README.md";

            if (System.IO.File.Exists(documentationPath))
            {
                System.Diagnostics.Process.Start(documentationPath);
            }
            else
            {
                EditorUtility.DisplayDialog("Documentation",
                    "File documentation chưa được tạo.\n\n" +
                    "Vui lòng tạo file README.md trong thư mục Assets/ProfilerReference/",
                    "OK");
            }
        }

        [MenuItem("Tools/Profiler Reference/About", false, 21)]
        private static void ShowAbout()
        {
            var aboutContent = "Profiler Reference Tool v1.0\n\n" +
                             "Công cụ phân tích performance cho Unity\n\n" +
                             "Tính năng:\n" +
                             "• Phân tích dữ liệu Profiler runtime\n" +
                             "• Phát hiện function/script chiếm nhiều resource\n" +
                             "• Gợi ý tối ưu hóa chi tiết\n" +
                             "• Giao diện Editor thân thiện\n\n" +
                             "Hỗ trợ: contact@profilerreference.com";

            EditorUtility.DisplayDialog("About Profiler Reference", aboutContent, "OK");
        }
    }
}
