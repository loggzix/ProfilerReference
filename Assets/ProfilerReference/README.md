# Profiler Reference Tool

Công cụ phân tích performance cho Unity projects, giúp developer tối ưu hóa code bằng cách phân tích dữ liệu từ Unity Profiler.

## Tính năng chính

- 📊 **Phân tích Profiler Data**: Thu thập và phân tích dữ liệu runtime từ Unity Profiler
- 🔍 **Phát hiện vấn đề**: Tự động phát hiện function/script chiếm nhiều CPU và Memory
- 💡 **Gợi ý tối ưu**: Cung cấp gợi ý chi tiết để tối ưu hóa performance
- 🎛️ **Giao diện Editor**: Editor Window thân thiện với bộ lọc và sắp xếp
- 📈 **Real-time Monitoring**: Theo dõi performance trong runtime
- 📄 **Export Data**: Xuất báo cáo chi tiết ra file

## Cài đặt và sử dụng

### 1. Chuẩn bị

1. Mở Unity project
2. Import package hoặc copy thư mục `Assets/ProfilerReference` vào project
3. Đảm bảo Unity Profiler khả dụng (Window > Analysis > Profiler)

### 2. Sử dụng cơ bản

#### Từ Editor:
1. Mở tool: **Tools > Profiler Reference > Open Tool**
2. Kiểm tra trạng thái Profiler: **Tools > Profiler Reference > Check Profiler Status**
3. Bật Profiler nếu chưa bật
4. Chạy game và thực hiện gameplay cần phân tích
5. Quay lại tool và nhấn **"Thu thập dữ liệu"**
6. Xem kết quả trong tab "Gợi ý tối ưu"

#### Từ Runtime:
1. Thêm component `ProfilerDataCollector` vào GameObject trong scene
2. Cài đặt các tham số thu thập
3. Chạy game để thu thập dữ liệu tự động
4. Dừng game để xem kết quả

### 3. Các tab trong tool

#### Tab "Gợi ý tối ưu"
- Hiển thị danh sách các vấn đề được phát hiện
- Bộ lọc theo loại (Performance, Memory, General)
- Bộ lọc theo mức độ nghiêm trọng
- Mỗi gợi ý có mô tả chi tiết và hướng dẫn tối ưu

#### Tab "Dữ liệu Profiler"
- Hiển thị raw data từ Profiler
- Sắp xếp theo thời gian thực thi
- Thông tin chi tiết về mỗi function

#### Tab "Cài đặt"
- Điều chỉnh ngưỡng phát hiện
- Cài đặt auto-refresh
- Hướng dẫn sử dụng

## Kiến trúc code

```
Assets/ProfilerReference/
├── ProfilerAnalyzer.cs          # Logic phân tích dữ liệu
├── OptimizationSuggestion.cs    # Class gợi ý tối ưu
├── ProfilerReferenceWindow.cs   # Editor Window chính
├── ProfilerReferenceMenu.cs     # Menu integration
├── ProfilerDataCollector.cs     # Runtime data collector
└── README.md                    # Documentation này
```

## API chính

### ProfilerAnalyzer
```csharp
var analyzer = new ProfilerAnalyzer();
analyzer.CollectProfilerData();           // Thu thập dữ liệu
var suggestions = analyzer.GetSuggestions(); // Lấy gợi ý
var data = analyzer.GetProfilerData();    // Lấy raw data
```

### ProfilerDataCollector
```csharp
var collector = gameObject.AddComponent<ProfilerDataCollector>();
collector.StartCollecting();              // Bắt đầu thu thập
collector.StopCollecting();               // Dừng thu thập
collector.ExportData(filePath);           // Export dữ liệu
```

## Các loại gợi ý tối ưu

### Performance Issues
- Function Update() được gọi quá nhiều
- Vòng lặp phức tạp
- Cache miss trong Update/FixedUpdate
- Physics calculations không hiệu quả

### Memory Issues
- Memory leaks
- Object instantiation/destruction thường xuyên
- Texture/String không được cache
- Unmanaged resources không được dispose

## Mẹo sử dụng hiệu quả

1. **Chạy Profiler trước**: Luôn bật Profiler trước khi thu thập dữ liệu
2. **Thu thập trong thời gian dài**: Chạy game ít nhất 30-60 giây để có dữ liệu chính xác
3. **Focus vào High severity**: Ưu tiên tối ưu các vấn đề có severity cao
4. **Test sau khi tối ưu**: Thu thập dữ liệu lại để verify improvement
5. **Combine với Unity Profiler**: Sử dụng tool này kết hợp với Unity Profiler gốc

## Troubleshooting

### Tool không thu thập được dữ liệu
- Đảm bảo Profiler được bật
- Kiểm tra có lỗi trong Console không
- Thử restart Unity

### Gợi ý không chính xác
- Thu thập dữ liệu trong thời gian dài hơn
- Kiểm tra ngưỡng trong tab Settings
- Verify kết quả với Unity Profiler

### Performance issue khi sử dụng tool
- Tắt auto-refresh khi không cần thiết
- Giảm collection interval
- Chỉ chạy tool khi cần phân tích

## Version History

### v1.0.0
- Phân tích basic profiler data
- Gợi ý tối ưu cơ bản
- Editor Window với 3 tabs
- Runtime data collector
- Export functionality

## Contributing

Để contribute cho project này:
1. Fork repository
2. Tạo feature branch
3. Implement changes
4. Test thoroughly
5. Submit pull request

## License

MIT License - sử dụng tự do cho mục đích phi thương mại.

## Support

Nếu gặp vấn đề hoặc cần hỗ trợ:
- Kiểm tra documentation trước
- Tạo issue trên repository
- Contact: profilerreference@unity.com
