using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProfilerReference
{
    /// <summary>
    /// Editor Window chính cho Profiler Reference Tool
    /// </summary>
    public class ProfilerReferenceWindow : EditorWindow
    {
        private ProfilerAnalyzer _analyzer;
        private Vector2 _scrollPosition;
        private Vector2 _dataScrollPosition;
        private bool _showPerformanceSuggestions = true;
        private bool _showMemorySuggestions = true;
        private bool _showGeneralSuggestions = true;
        private SuggestionSeverity _minSeverity = SuggestionSeverity.Low;
        private int _selectedTab = 0;
        private string[] _tabNames = { "Gợi ý tối ưu", "Dữ liệu Profiler", "Cài đặt" };

        // Cài đặt
        private float _timeThreshold = 100f;
        private float _memoryThreshold = 50f;
        private bool _autoRefresh = false;
        private float _refreshInterval = 5f;
        private double _lastRefreshTime;

        [MenuItem("Tools/Profiler Reference/Analyze", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<ProfilerReferenceWindow>("Profiler Reference");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        private void OnEnable()
        {
            _analyzer = new ProfilerAnalyzer();
            _lastRefreshTime = EditorApplication.timeSinceStartup;
        }

        private void OnDisable()
        {
            if (_analyzer != null)
            {
                _analyzer.ClearData();
            }
        }

        private void Update()
        {
            if (_autoRefresh && EditorApplication.timeSinceStartup - _lastRefreshTime > _refreshInterval)
            {
                RefreshData();
                _lastRefreshTime = EditorApplication.timeSinceStartup;
                Repaint();
            }
        }

        private void OnGUI()
        {
            DrawToolbar();

            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames);

            switch (_selectedTab)
            {
                case 0:
                    DrawSuggestionsTab();
                    break;
                case 1:
                    DrawProfilerDataTab();
                    break;
                case 2:
                    DrawSettingsTab();
                    break;
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Thu thập dữ liệu", EditorStyles.toolbarButton))
                {
                    RefreshData();
                }

                if (GUILayout.Button("Xóa dữ liệu", EditorStyles.toolbarButton))
                {
                    _analyzer.ClearData();
                }

                GUILayout.FlexibleSpace();

                _autoRefresh = GUILayout.Toggle(_autoRefresh, "Tự động refresh", EditorStyles.toolbarButton);

                if (GUILayout.Button("Help", EditorStyles.toolbarButton))
                {
                    ShowHelp();
                }
            }
        }

        private void DrawSuggestionsTab()
        {
            var suggestions = _analyzer.GetSuggestions();

            // Bộ lọc
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Bộ lọc:", GUILayout.Width(50));
                _showPerformanceSuggestions = EditorGUILayout.ToggleLeft("Performance", _showPerformanceSuggestions, GUILayout.Width(100));
                _showMemorySuggestions = EditorGUILayout.ToggleLeft("Memory", _showMemorySuggestions, GUILayout.Width(80));
                _showGeneralSuggestions = EditorGUILayout.ToggleLeft("General", _showGeneralSuggestions, GUILayout.Width(80));

                EditorGUILayout.LabelField("Severity tối thiểu:", GUILayout.Width(120));
                _minSeverity = (SuggestionSeverity)EditorGUILayout.EnumPopup(_minSeverity, GUILayout.Width(80));
            }

            // Thống kê
            var filteredSuggestions = FilterSuggestions(suggestions);
            EditorGUILayout.HelpBox($"Tìm thấy {filteredSuggestions.Count} gợi ý tối ưu", MessageType.Info);

            // Danh sách gợi ý
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var suggestion in filteredSuggestions)
            {
                DrawSuggestionBox(suggestion);
                GUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawProfilerDataTab()
        {
            var profilerData = _analyzer.GetProfilerData();

            EditorGUILayout.HelpBox($"Thu thập dữ liệu từ {profilerData.Count} function", MessageType.Info);

            // Header
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Function", EditorStyles.boldLabel, GUILayout.Width(200));
                EditorGUILayout.LabelField("Script", EditorStyles.boldLabel, GUILayout.Width(150));
                EditorGUILayout.LabelField("Total Time (ms)", EditorStyles.boldLabel, GUILayout.Width(100));
                EditorGUILayout.LabelField("Memory (MB)", EditorStyles.boldLabel, GUILayout.Width(100));
                EditorGUILayout.LabelField("Calls", EditorStyles.boldLabel, GUILayout.Width(60));
            }

            // Data rows
            _dataScrollPosition = EditorGUILayout.BeginScrollView(_dataScrollPosition);

            var sortedData = profilerData.Values
                .OrderByDescending(d => d.TotalTime)
                .Take(50); // Hiển thị top 50

            foreach (var data in sortedData)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(data.FunctionName, GUILayout.Width(200));
                    EditorGUILayout.LabelField(data.ScriptName, GUILayout.Width(150));
                    EditorGUILayout.LabelField(data.TotalTime.ToString("F2"), GUILayout.Width(100));
                    EditorGUILayout.LabelField(data.TotalMemory.ToString("F2"), GUILayout.Width(100));
                    EditorGUILayout.LabelField(data.CallCount.ToString(), GUILayout.Width(60));
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSettingsTab()
        {
            EditorGUILayout.LabelField("Ngưỡng phát hiện", EditorStyles.boldLabel);

            _timeThreshold = EditorGUILayout.FloatField("Ngưỡng thời gian (ms):", _timeThreshold);
            _memoryThreshold = EditorGUILayout.FloatField("Ngưỡng bộ nhớ (MB):", _memoryThreshold);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tự động refresh", EditorStyles.boldLabel);
            _refreshInterval = EditorGUILayout.FloatField("Khoảng thời gian (giây):", _refreshInterval);

            EditorGUILayout.Space();

            if (GUILayout.Button("Khôi phục cài đặt mặc định"))
            {
                ResetSettings();
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Hướng dẫn sử dụng:\n" +
                "1. Bật Profiler trong Window > Analysis > Profiler\n" +
                "2. Chạy game và thực hiện các action cần phân tích\n" +
                "3. Nhấn 'Thu thập dữ liệu' để phân tích\n" +
                "4. Xem gợi ý tối ưu trong tab đầu tiên",
                MessageType.Info
            );
        }

        private void DrawSuggestionBox(OptimizationSuggestion suggestion)
        {
            var backgroundColor = GetSeverityColor(suggestion.Severity);

            using (new EditorGUILayout.VerticalScope(GetColoredBoxStyle(backgroundColor)))
            {
                // Header
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(suggestion.GetDisplayTitle(), EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(suggestion.Severity.ToString(), GetSeverityStyle(suggestion.Severity));
                }

                // Description
                EditorGUILayout.LabelField(suggestion.Description, EditorStyles.wordWrappedLabel);

                // Suggestions
                if (!string.IsNullOrEmpty(suggestion.SuggestionText))
                {
                    EditorGUILayout.LabelField("Gợi ý tối ưu:", EditorStyles.boldLabel);
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        var lines = suggestion.SuggestionText.Split('\n');
                        foreach (var line in lines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                EditorGUILayout.LabelField($"• {line.Trim()}", EditorStyles.wordWrappedLabel);
                            }
                        }
                    }
                }
            }
        }

        private List<OptimizationSuggestion> FilterSuggestions(List<OptimizationSuggestion> suggestions)
        {
            return suggestions.Where(s =>
                (s.Type == SuggestionType.Performance && _showPerformanceSuggestions) ||
                (s.Type == SuggestionType.Memory && _showMemorySuggestions) ||
                (s.Type == SuggestionType.General && _showGeneralSuggestions) &&
                (int)s.Severity >= (int)_minSeverity
            ).ToList();
        }

        private void RefreshData()
        {
            if (_analyzer != null)
            {
                _analyzer.ClearData();
                _analyzer.CollectProfilerData();
                Debug.Log("Đã thu thập dữ liệu profiler mới");
            }
        }

        private void ResetSettings()
        {
            _timeThreshold = 100f;
            _memoryThreshold = 50f;
            _refreshInterval = 5f;
        }

        private void ShowHelp()
        {
            var helpContent = "Profiler Reference Tool\n\n" +
                            "Công cụ này giúp phân tích performance của Unity project bằng cách:\n" +
                            "• Thu thập dữ liệu từ Unity Profiler\n" +
                            "• Phát hiện function/script chiếm nhiều resource\n" +
                            "• Gợi ý cách tối ưu hóa\n\n" +
                            "Cách sử dụng:\n" +
                            "1. Mở Profiler (Window > Analysis > Profiler)\n" +
                            "2. Chạy game và thực hiện gameplay\n" +
                            "3. Quay lại tool này và nhấn 'Thu thập dữ liệu'\n" +
                            "4. Xem kết quả và áp dụng gợi ý tối ưu";

            EditorUtility.DisplayDialog("Help", helpContent, "OK");
        }

        private GUIStyle GetColoredBoxStyle(string colorHex)
        {
            var style = new GUIStyle(EditorStyles.helpBox);
            var color = HexToColor(colorHex);
            color.a = 0.1f; // Làm mờ màu nền
            style.normal.background = MakeTex(1, 1, color);
            return style;
        }

        private GUIStyle GetSeverityStyle(SuggestionSeverity severity)
        {
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = HexToColor(GetSeverityColor(severity));
            return style;
        }

        private string GetSeverityColor(SuggestionSeverity severity)
        {
            switch (severity)
            {
                case SuggestionSeverity.Low: return "#00FF00";
                case SuggestionSeverity.Medium: return "#FFFF00";
                case SuggestionSeverity.High: return "#FF8000";
                case SuggestionSeverity.Critical: return "#FF0000";
                default: return "#FFFFFF";
            }
        }

        private Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            return Color.white;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
