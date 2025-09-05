using System;

namespace ProfilerReference
{
    /// <summary>
    /// Loại gợi ý tối ưu
    /// </summary>
    public enum SuggestionType
    {
        Performance,
        Memory,
        General
    }

    /// <summary>
    /// Mức độ nghiêm trọng của vấn đề
    /// </summary>
    public enum SuggestionSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Gợi ý tối ưu hóa cho function/script
    /// </summary>
    [Serializable]
    public class OptimizationSuggestion
    {
        /// <summary>
        /// Loại gợi ý
        /// </summary>
        public SuggestionType Type { get; set; }

        /// <summary>
        /// Tên function cần tối ưu
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// Tên script chứa function
        /// </summary>
        public string ScriptName { get; set; }

        /// <summary>
        /// Mức độ nghiêm trọng
        /// </summary>
        public SuggestionSeverity Severity { get; set; }

        /// <summary>
        /// Mô tả vấn đề
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Chi tiết gợi ý tối ưu
        /// </summary>
        public string SuggestionText { get; set; }

        /// <summary>
        /// Thời gian tạo gợi ý
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Lấy màu sắc cho severity level
        /// </summary>
        public string GetSeverityColor()
        {
            switch (Severity)
            {
                case SuggestionSeverity.Low:
                    return "#00FF00"; // Green
                case SuggestionSeverity.Medium:
                    return "#FFFF00"; // Yellow
                case SuggestionSeverity.High:
                    return "#FF8000"; // Orange
                case SuggestionSeverity.Critical:
                    return "#FF0000"; // Red
                default:
                    return "#FFFFFF"; // White
            }
        }

        /// <summary>
        /// Lấy icon cho suggestion type
        /// </summary>
        public string GetTypeIcon()
        {
            switch (Type)
            {
                case SuggestionType.Performance:
                    return "⏱️";
                case SuggestionType.Memory:
                    return "💾";
                case SuggestionType.General:
                    return "⚙️";
                default:
                    return "❓";
            }
        }

        /// <summary>
        /// Lấy tiêu đề hiển thị
        /// </summary>
        public string GetDisplayTitle()
        {
            return $"{GetTypeIcon()} {FunctionName} ({ScriptName})";
        }

        /// <summary>
        /// Kiểm tra xem suggestion có hợp lệ không
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(FunctionName) &&
                   !string.IsNullOrEmpty(ScriptName) &&
                   !string.IsNullOrEmpty(Description);
        }
    }
}
