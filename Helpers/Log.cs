using Newtonsoft.Json;
using System.ComponentModel;

namespace ShiftPro.Helpers
{

    public class Log
    {
        public ApiResultStatus Status { get; set; }
        public string Message { get; set; }
        public Object? Data { get; set; }
        public DateTime Logtime { get; set; } = DateTime.Now;
    }

    public enum ApiResultStatus
    {
        [Description("成功")]
        Success = 1,

        [Description("業務錯誤")]
        Failed = -1,

        [Description("系統錯誤")]
        Error = -2,

        [Description("未登入錯誤")]
        Unauthorized = -3,

        [Description("沒權限")]
        Forbidden = -4
    }

    public class FileLogger
    {
        private readonly IConfiguration _config;
        public static readonly object _lock = new();

        public FileLogger(IConfiguration config)
        {
            _config = config;
        }

        public void Write(Log log)
        {
            string logFolder = _config["LogSettings:Folder"];

            string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string filePath = Path.Combine(logFolder, fileName);

            if (string.IsNullOrWhiteSpace(logFolder))
            {
                logFolder = Path.Combine(AppContext.BaseDirectory, "Logs");
            }
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            var json = JsonConvert.SerializeObject(log);

            lock (_lock)
            {
                File.AppendAllText(filePath, json + Environment.NewLine);
            }
        }
    }
}
