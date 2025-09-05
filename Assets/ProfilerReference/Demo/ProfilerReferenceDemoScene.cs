using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProfilerReference.Demo
{
    /// <summary>
    /// Script quản lý demo scene cho Profiler Reference Tool
    /// </summary>
    public class ProfilerReferenceDemoScene : MonoBehaviour
    {
        [Header("Demo Objects")]
        public GameObject performanceTestObject;
        public GameObject dataCollectorObject;

        [Header("Demo Settings")]
        public bool enableDemoMode = true;
        public float demoDuration = 30f;

        private float _startTime;
        private bool _demoRunning = false;

        private void Start()
        {
            if (enableDemoMode)
            {
                StartDemo();
            }
        }

        private void Update()
        {
            if (_demoRunning && Time.time - _startTime >= demoDuration)
            {
                EndDemo();
            }
        }

        /// <summary>
        /// Bắt đầu demo
        /// </summary>
        public void StartDemo()
        {
            _demoRunning = true;
            _startTime = Time.time;

            // Đảm bảo các components được setup
            SetupDemoObjects();

            Debug.Log("🚀 Bắt đầu Profiler Reference Demo");
            Debug.Log($"Demo sẽ chạy trong {demoDuration} giây");
            Debug.Log("Mở Profiler Reference Tool (Tools > Profiler Reference > Open Tool) để xem kết quả");
        }

        /// <summary>
        /// Kết thúc demo
        /// </summary>
        public void EndDemo()
        {
            _demoRunning = false;

            Debug.Log("✅ Kết thúc Profiler Reference Demo");
            Debug.Log("Kiểm tra Profiler Reference Tool để xem kết quả phân tích!");
        }

        /// <summary>
        /// Setup các objects cho demo
        /// </summary>
        private void SetupDemoObjects()
        {
            // Tạo performance test object nếu chưa có
            if (performanceTestObject == null)
            {
                performanceTestObject = new GameObject("PerformanceTestObject");
                var performanceScript = performanceTestObject.AddComponent<ExamplePerformanceScript>();
                performanceScript.enableInefficientCode = true;
                performanceScript.objectCount = 500; // Giảm số lượng để tránh crash
            }

            // Tạo data collector object nếu chưa có
            if (dataCollectorObject == null)
            {
                dataCollectorObject = new GameObject("DataCollector");
                var collector = dataCollectorObject.AddComponent<ProfilerDataCollector>();
                collector.autoStart = true;
                collector.collectionInterval = 0.5f;
                collector.maxCollectionTime = demoDuration;
            }

            // Đảm bảo Profiler được bật
            UnityEngine.Profiling.Profiler.enabled = true;

            Debug.Log("📊 Đã setup demo objects");
        }

        private void OnGUI()
        {
            if (!_demoRunning) return;

            // Hiển thị thông tin demo
            GUI.Label(new Rect(10, 10, 400, 20), $"🚀 Profiler Reference Demo - Thời gian: {Time.time - _startTime:F1}s / {demoDuration:F1}s");

            if (GUI.Button(new Rect(10, 35, 150, 30), "Dừng Demo"))
            {
                EndDemo();
            }

            if (GUI.Button(new Rect(170, 35, 150, 30), "Mở Tool"))
            {
                // Mở Profiler Reference Tool
                var windowType = System.Type.GetType("ProfilerReference.ProfilerReferenceWindow, Assembly-CSharp-Editor");
                if (windowType != null)
                {
                    var method = windowType.GetMethod("ShowWindow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (method != null)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        #region Context Menu Items

        [ContextMenu("Start Demo")]
        private void ContextStartDemo()
        {
            StartDemo();
        }

        [ContextMenu("End Demo")]
        private void ContextEndDemo()
        {
            EndDemo();
        }

        [ContextMenu("Setup Demo Objects")]
        private void ContextSetupDemoObjects()
        {
            SetupDemoObjects();
        }

        #endregion
    }
}
