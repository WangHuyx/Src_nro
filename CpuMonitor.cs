using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class CpuMonitor
{
    private TimeSpan _lastCpuTime = TimeSpan.Zero;
    private DateTime _lastCheckTime = DateTime.MinValue;

    public CpuMonitor()
    {
    }
    public double GetCurrentProcessCpuUsage()
    {
        var currentProcess = Process.GetCurrentProcess();

        if (_lastCheckTime == DateTime.MinValue)
        {
            _lastCpuTime = currentProcess.TotalProcessorTime;
            _lastCheckTime = DateTime.Now;
            return 0;
        }

        var currentTime = DateTime.Now;
        var currentCpuTime = currentProcess.TotalProcessorTime;

        // Tính độ lệch thời gian
        double elapsedMs = (currentTime - _lastCheckTime).TotalMilliseconds;
        double cpuUsedMs = (currentCpuTime - _lastCpuTime).TotalMilliseconds;

        // Tính % (Chia cho số nhân để ra con số chính xác trên Task Manager)
        double cpuUsage = (cpuUsedMs / (elapsedMs * Environment.ProcessorCount)) * 100;

        // Lưu lại cho lần sau
        _lastCpuTime = currentCpuTime;
        _lastCheckTime = currentTime;

        return cpuUsage;
    }
}