using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace ProfilerReference
{
    /// <summary>
    /// Component thu thập dữ liệu profiler trong runtime
    /// </summary>
    public class ProfilerDataCollector : MonoBehaviour
    {
        [Header("Cài đặt thu thập")]
        [Tooltip("Tần suất thu thập dữ liệu (giây)")]
        public float collectionInterval = 1f;

        [Tooltip("Có tự động thu thập khi start không")]
        public bool autoStart = true;

        [Tooltip("Thời gian tối đa thu thập (giây)")]
        public float maxCollectionTime = 60f;

        [Header("Thông tin debug")]
        [Tooltip("Tổng số samples đã thu thập (chỉ đọc)")] public int totalSamples = 0;
        [Tooltip("Tổng thời gian thu thập (giây) (chỉ đọc)")] public float totalTime = 0f;
        [Tooltip("FPS trung bình (chỉ đọc)")] public float averageFPS = 0f;

        private ProfilerAnalyzer _analyzer;
        private float _lastCollectionTime;
        private float _startTime;
        private bool _isCollecting = false;
        private List<float> _fpsSamples = new List<float>();

        private void Awake()
        {
            _analyzer = new ProfilerAnalyzer();
        }

        private void Start()
        {
            if (autoStart)
            {
                StartCollecting();
            }
        }

        private void Update()
        {
            if (_isCollecting)
            {
                // Thu thập FPS
                _fpsSamples.Add(1f / Time.deltaTime);
                if (_fpsSamples.Count > 100) // Giữ 100 sample gần nhất
                {
                    _fpsSamples.RemoveAt(0);
                }

                // Tính average FPS
                averageFPS = CalculateAverage(_fpsSamples);

                // Thu thập dữ liệu profiler theo interval
                if (Time.time - _lastCollectionTime >= collectionInterval)
                {
                    CollectData();
                    _lastCollectionTime = Time.time;
                }

                // Dừng nếu quá thời gian
                if (Time.time - _startTime >= maxCollectionTime)
                {
                    StopCollecting();
                    Debug.Log($"Dừng thu thập sau {maxCollectionTime} giây. Tổng samples: {totalSamples}");
                }
            }
        }

        /// <summary>
        /// Bắt đầu thu thập dữ liệu
        /// </summary>
        public void StartCollecting()
        {
            if (_isCollecting) return;

            _isCollecting = true;
            _startTime = Time.time;
            _lastCollectionTime = Time.time;
            _fpsSamples.Clear();
            totalSamples = 0;
            totalTime = 0f;

            // Đảm bảo Profiler được bật
            if (!Profiler.enabled)
            {
                Profiler.enabled = true;
                Debug.Log("Đã bật Profiler tự động");
            }

            Debug.Log("Bắt đầu thu thập dữ liệu profiler");
        }

        /// <summary>
        /// Dừng thu thập dữ liệu
        /// </summary>
        public void StopCollecting()
        {
            if (!_isCollecting) return;

            _isCollecting = false;
            Debug.Log($"Dừng thu thập dữ liệu. Tổng samples: {totalSamples}, Thời gian: {totalTime:F2}s");
        }

        /// <summary>
        /// Thu thập dữ liệu profiler
        /// </summary>
        private void CollectData()
        {
            if (_analyzer == null) return;

            _analyzer.CollectProfilerData();

            var data = _analyzer.GetProfilerData();
            totalSamples = data.Count;
            totalTime = Time.time - _startTime;
        }

        /// <summary>
        /// Lấy analyzer để truy cập dữ liệu
        /// </summary>
        public ProfilerAnalyzer GetAnalyzer()
        {
            return _analyzer;
        }

        /// <summary>
        /// Export dữ liệu ra file
        /// </summary>
        public void ExportData(string filePath)
        {
            if (_analyzer == null) return;

            var data = _analyzer.GetProfilerData();
            var suggestions = _analyzer.GetSuggestions();

            var content = "PROFILER REFERENCE DATA EXPORT\n";
            content += $"Thời gian thu thập: {totalTime:F2}s\n";
            content += $"Tổng samples: {totalSamples}\n";
            content += $"Average FPS: {averageFPS:F2}\n\n";

            content += "=== PROFILER DATA ===\n";
            foreach (var item in data)
            {
                content += $"{item.Value.ScriptName}.{item.Value.FunctionName}: " +
                          $"{item.Value.TotalTime:F2}ms, {item.Value.TotalMemory:F2}MB, {item.Value.CallCount} calls\n";
            }

            content += "\n=== OPTIMIZATION SUGGESTIONS ===\n";
            foreach (var suggestion in suggestions)
            {
                content += $"[{suggestion.Severity}] {suggestion.GetDisplayTitle()}\n";
                content += $"{suggestion.Description}\n";
                content += $"{suggestion.SuggestionText}\n\n";
            }

            System.IO.File.WriteAllText(filePath, content);
            Debug.Log($"Đã export dữ liệu ra file: {filePath}");
        }

        /// <summary>
        /// Tính trung bình của list float
        /// </summary>
        private float CalculateAverage(List<float> values)
        {
            if (values.Count == 0) return 0f;

            float sum = 0f;
            foreach (var value in values)
            {
                sum += value;
            }
            return sum / values.Count;
        }

        private void OnDestroy()
        {
            StopCollecting();
        }

        #region Menu Items for Runtime Testing

        [ContextMenu("Start Collecting")]
        private void ContextStartCollecting()
        {
            StartCollecting();
        }

        [ContextMenu("Stop Collecting")]
        private void ContextStopCollecting()
        {
            StopCollecting();
        }

        [ContextMenu("Export Data to Desktop")]
        private void ContextExportData()
        {
            var desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            var fileName = $"ProfilerData_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var filePath = System.IO.Path.Combine(desktopPath, fileName);
            ExportData(filePath);
        }

        #endregion
    }
}
