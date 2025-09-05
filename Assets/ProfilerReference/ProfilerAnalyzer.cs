using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;

namespace ProfilerReference
{
    /// <summary>
    /// Phân tích dữ liệu Profiler để tìm các function/script chiếm nhiều resource
    /// </summary>
    public class ProfilerAnalyzer
    {
        private readonly Dictionary<string, ProfilerData> _functionData = new Dictionary<string, ProfilerData>();
        private readonly List<OptimizationSuggestion> _suggestions = new List<OptimizationSuggestion>();

        /// <summary>
        /// Dữ liệu profiler cho một function
        /// </summary>
        public class ProfilerData
        {
            public string FunctionName { get; set; }
            public string ScriptName { get; set; }
            public float TotalTime { get; set; }
            public float TotalMemory { get; set; }
            public int CallCount { get; set; }
            public float AverageTime => CallCount > 0 ? TotalTime / CallCount : 0;
            public float AverageMemory => CallCount > 0 ? TotalMemory / CallCount : 0;
            public DateTime LastSampleTime { get; set; }
        }

        /// <summary>
        /// Thu thập dữ liệu từ Profiler
        /// </summary>
        public void CollectProfilerData()
        {
            if (!Profiler.enabled)
            {
                Debug.LogWarning("Profiler chưa được bật. Vui lòng bật Profiler trong Window > Analysis > Profiler");
                return;
            }

            // Thu thập dữ liệu profiler cơ bản
            // Sử dụng các metrics có sẵn từ Unity Profiler
            var totalMemory = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f; // MB
            var monoMemory = Profiler.GetMonoHeapSizeLong() / 1024f / 1024f; // MB
            var fps = 1f / Time.deltaTime;

            // Tạo sample data cơ bản cho demo
            var sampleNames = new string[] {
                "Update()",
                "FixedUpdate()",
                "LateUpdate()",
                "OnGUI()",
                "ExpensiveMethod()"
            };

            var sampleTimes = new float[sampleNames.Length];
            var sampleCounts = new int[sampleNames.Length];

            // Giả lập dữ liệu dựa trên performance hiện tại
            for (int i = 0; i < sampleNames.Length; i++)
            {
                sampleTimes[i] = UnityEngine.Random.Range(0.1f, 10f); // Random time for demo
                sampleCounts[i] = UnityEngine.Random.Range(1, 100);

                var sampleName = sampleNames[i];
                var functionInfo = ParseFunctionName(sampleName);

                if (!_functionData.ContainsKey(sampleName))
                {
                    _functionData[sampleName] = new ProfilerData
                    {
                        FunctionName = functionInfo.functionName,
                        ScriptName = functionInfo.scriptName,
                        LastSampleTime = DateTime.Now
                    };
                }

                var data = _functionData[sampleName];
                data.TotalTime += sampleTimes[i];
                data.TotalMemory += GetMemoryUsageForSample(sampleName);
                data.CallCount += sampleCounts[i];
            }

            AnalyzeAndGenerateSuggestions();
        }

        /// <summary>
        /// Phân tích tên function để xác định script
        /// </summary>
        private (string functionName, string scriptName) ParseFunctionName(string sampleName)
        {
            // Ví dụ: "MyScript.Update()" -> functionName: "Update", scriptName: "MyScript"
            var parts = sampleName.Split('.');
            if (parts.Length >= 2)
            {
                var functionPart = parts[parts.Length - 1];
                var scriptPart = string.Join(".", parts.Take(parts.Length - 1));

                // Loại bỏ dấu ngoặc đơn nếu có
                if (functionPart.Contains("("))
                {
                    functionPart = functionPart.Substring(0, functionPart.IndexOf("("));
                }

                return (functionPart, scriptPart);
            }

            return (sampleName, "Unknown");
        }

        /// <summary>
        /// Lấy thông tin memory usage cho sample (ước tính)
        /// </summary>
        private float GetMemoryUsageForSample(string sampleName)
        {
            // Trong thực tế, có thể cần sử dụng Profiler API để lấy memory data chính xác
            // Đây là ước tính đơn giản
            return Profiler.GetMonoHeapSizeLong() / 1024f / 1024f; // MB
        }

        /// <summary>
        /// Phân tích dữ liệu và tạo gợi ý tối ưu
        /// </summary>
        private void AnalyzeAndGenerateSuggestions()
        {
            _suggestions.Clear();

            var sortedByTime = _functionData.Values
                .OrderByDescending(d => d.TotalTime)
                .Take(10);

            var sortedByMemory = _functionData.Values
                .OrderByDescending(d => d.TotalMemory)
                .Take(10);

            // Phát hiện function chiếm nhiều thời gian
            foreach (var data in sortedByTime)
            {
                if (data.TotalTime > 100f) // Threshold có thể điều chỉnh
                {
                    var suggestion = new OptimizationSuggestion
                    {
                        Type = SuggestionType.Performance,
                        FunctionName = data.FunctionName,
                        ScriptName = data.ScriptName,
                        Severity = data.TotalTime > 500f ? SuggestionSeverity.High :
                                 data.TotalTime > 200f ? SuggestionSeverity.Medium : SuggestionSeverity.Low,
                        Description = $"Function '{data.FunctionName}' chiếm {data.TotalTime:F2}ms tổng thời gian",
                        SuggestionText = GenerateTimeOptimizationSuggestion(data)
                    };
                    _suggestions.Add(suggestion);
                }
            }

            // Phát hiện function chiếm nhiều bộ nhớ
            foreach (var data in sortedByMemory)
            {
                if (data.TotalMemory > 50f) // Threshold có thể điều chỉnh
                {
                    var suggestion = new OptimizationSuggestion
                    {
                        Type = SuggestionType.Memory,
                        FunctionName = data.FunctionName,
                        ScriptName = data.ScriptName,
                        Severity = data.TotalMemory > 200f ? SuggestionSeverity.High :
                                 data.TotalMemory > 100f ? SuggestionSeverity.Medium : SuggestionSeverity.Low,
                        Description = $"Function '{data.FunctionName}' sử dụng {data.TotalMemory:F2}MB bộ nhớ",
                        SuggestionText = GenerateMemoryOptimizationSuggestion(data)
                    };
                    _suggestions.Add(suggestion);
                }
            }
        }

        /// <summary>
        /// Tạo gợi ý tối ưu cho vấn đề thời gian
        /// </summary>
        private string GenerateTimeOptimizationSuggestion(ProfilerData data)
        {
            var suggestions = new List<string>();

            if (data.FunctionName.Contains("Update"))
            {
                suggestions.Add("- Cache references thay vì tìm kiếm mỗi frame");
                suggestions.Add("- Sử dụng object pooling cho instantiation");
                suggestions.Add("- Tối ưu hóa vòng lặp và điều kiện");
            }
            else if (data.FunctionName.Contains("FixedUpdate"))
            {
                suggestions.Add("- Giảm tần suất FixedUpdate nếu có thể");
                suggestions.Add("- Tối ưu hóa physics calculations");
            }
            else if (data.FunctionName.Contains("LateUpdate"))
            {
                suggestions.Add("- Tránh camera calculations phức tạp");
                suggestions.Add("- Cache transform positions");
            }

            suggestions.Add("- Sử dụng Profiler để measure impact của từng thay đổi");

            return string.Join("\n", suggestions);
        }

        /// <summary>
        /// Tạo gợi ý tối ưu cho vấn đề bộ nhớ
        /// </summary>
        private string GenerateMemoryOptimizationSuggestion(ProfilerData data)
        {
            var suggestions = new List<string>();

            suggestions.Add("- Kiểm tra memory leaks bằng Memory Profiler");
            suggestions.Add("- Sử dụng object pooling thay vì Instantiate/Destroy");
            suggestions.Add("- Dispose unmanaged resources properly");
            suggestions.Add("- Cache strings và textures khi có thể");
            suggestions.Add("- Sử dụng AssetBundle cho loading resources");

            return string.Join("\n", suggestions);
        }

        /// <summary>
        /// Lấy danh sách gợi ý tối ưu
        /// </summary>
        public List<OptimizationSuggestion> GetSuggestions()
        {
            return _suggestions.OrderByDescending(s => (int)s.Severity).ToList();
        }

        /// <summary>
        /// Lấy dữ liệu profiler
        /// </summary>
        public Dictionary<string, ProfilerData> GetProfilerData()
        {
            return _functionData;
        }

        /// <summary>
        /// Xóa dữ liệu đã thu thập
        /// </summary>
        public void ClearData()
        {
            _functionData.Clear();
            _suggestions.Clear();
        }
    }
}
