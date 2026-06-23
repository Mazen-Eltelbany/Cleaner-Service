namespace CleanerService
{
    public class CleanService : BackgroundService
    {
        private readonly ILogger<CleanService> _logger;
        private readonly string CleanPathFolder1 = @"C:\Windows\Temp";
        private readonly string CleanPathFolder2 = Path.GetTempPath();
        public CleanService(ILogger<CleanService> logger)
        {
            _logger = logger;
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
                Console.WriteLine($"Deleted file: {file}");
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
                Console.WriteLine($"Failed to delete {file}: {ex.Message}");
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

                CleanDirectory(CleanPathFolder1);
                CleanDirectory(CleanPathFolder2);

                _logger.LogInformation("Cleaning done. Next run in 3 days.");
                await Task.Delay(TimeSpan.FromDays(3), stoppingToken);
            }
        }
    }
}
