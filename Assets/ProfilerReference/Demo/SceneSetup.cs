using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ProfilerReference.Demo
{
    /// <summary>
    /// Script để setup demo scene
    /// </summary>
    public static class SceneSetup
    {
        [MenuItem("Tools/Profiler Reference/Create Demo Scene")]
        private static void CreateDemoScene()
        {
            // Tạo scene mới
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Tạo GameObject chính
            var demoManager = new GameObject("Profiler Reference Demo Manager");
            var demoScript = demoManager.AddComponent<ProfilerReferenceDemoScene>();
            demoScript.enableDemoMode = true;
            demoScript.demoDuration = 30f;

            // Tạo performance test object
            var performanceObj = new GameObject("Performance Test Object");
            var performanceScript = performanceObj.AddComponent<ExamplePerformanceScript>();
            performanceScript.enableInefficientCode = true;
            performanceScript.objectCount = 200; // Giảm số lượng để tránh lag quá nhiều

            // Tạo data collector
            var collectorObj = new GameObject("Profiler Data Collector");
            var collectorScript = collectorObj.AddComponent<ProfilerDataCollector>();
            collectorScript.autoStart = true;
            collectorScript.collectionInterval = 0.5f;
            collectorScript.maxCollectionTime = 30f;

            // Link objects trong script
            demoScript.performanceTestObject = performanceObj;
            demoScript.dataCollectorObject = collectorObj;

            // Lưu scene
            var scenePath = "Assets/ProfilerReference/Demo/ProfilerReferenceDemoScene.unity";
            EditorSceneManager.SaveScene(scene, scenePath);

            Debug.Log("✅ Đã tạo demo scene tại: " + scenePath);
            Debug.Log("🎮 Mở scene và chạy để test Profiler Reference Tool!");
        }

        [MenuItem("Tools/Profiler Reference/Open Demo Scene")]
        private static void OpenDemoScene()
        {
            var scenePath = "Assets/ProfilerReference/Demo/ProfilerReferenceDemoScene.unity";

            if (System.IO.File.Exists(scenePath))
            {
                EditorSceneManager.OpenScene(scenePath);
                Debug.Log("✅ Đã mở demo scene");
            }
            else
            {
                Debug.LogError("❌ Demo scene chưa được tạo. Chạy 'Create Demo Scene' trước!");
            }
        }
    }
}
