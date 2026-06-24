namespace CleanerService
{
    public class CleanService : BackgroundService
    {
        private readonly ILogger<CleanService> _logger;
        private readonly string CleanPathFolder1 = @"C:\Windows\Temp";
        public CleanService(ILogger<CleanService> logger)
        {
            _logger = logger;
        }
        private List<string> GetPaths()
        {
            List<string> paths = new List<string>()
            {
                CleanPathFolder1
            };
            string userroot = @"C:\Users";
            if (Directory.Exists(userroot))
            {
                foreach (string path in Directory.GetDirectories(userroot))
                {
                    string folderName= Path.GetFileName(path);
                    if(folderName is "Public" or "Default" or "Default User" or "All Users")
                    {
                        continue;
                    }
                    string usertemp = Path.Combine(path, @"AppData\Local\Temp");
                    if (Directory.Exists(usertemp))
                    {
                        paths.Add(usertemp);
                    }
                }

            }
            return paths;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service Starting at: {time}", DateTimeOffset.Now);
            await base.StartAsync(cancellationToken);
        }
        private void CleanDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                _logger.LogWarning("Path not found: {path}", path);
                return;
            }
            foreach(var file in Directory.GetFiles(path))
            {
                TryDeleteFile(file);
            }
            foreach(var dir in Directory.GetDirectories(path))
            {
                TryDeleteDirectory(dir);
            }
        }
        private void TryDeleteFile(string file)
        {
            try
            {
                File.Delete(file);
                _logger.LogInformation("Deleted file: {file}", file);
            }
            catch (IOException)
            {
                _logger.LogWarning($"Skipped (in use): {file}");
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning($"Skipped (access denied): {file}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete {file}: {msg}", file, ex.Message);
            }
        }
        private void TryDeleteDirectory(string dir)
        {
            foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories)){
                TryDeleteFile(file);
            }
            try
            {
                Directory.Delete(dir,true);
                _logger.LogInformation("Deleted folder: {folder}", dir);
            }
            catch (IOException)
            {
                _logger.LogWarning($"Skipped folder (in use): {dir}");
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning($"Skipped folder (access denied): {dir}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete {dir}: {msg}", dir, ex.Message);
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Cleaning started at: {time}", DateTimeOffset.Now);
                foreach(var path in GetPaths())
                {
                    CleanDirectory(path);
                }

                _logger.LogInformation("Cleaning done. Next run in 3 days.");
                await Task.Delay(TimeSpan.FromDays(3), stoppingToken);
            }
        }
    }
}
