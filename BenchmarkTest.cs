using System;
using System.Diagnostics;

namespace NRO;
public static class BenchmarkTest
{
    // --- FixedUpdate (Tick) ---
    private static int _fixedCount = 0;
    private static long _fixedTicksTotal = 0;
    private static long _fixedTicksMax = 0;
    private static long _fixedStart = 0;

    // --- OnGUI (Render) ---
    private static int _guiCount = 0;
    private static long _guiTicksTotal = 0;
    private static long _guiTicksMax = 0;
    private static long _guiStart = 0;

    // --- Interval logging ---
    private static long _lastLogTime = Stopwatch.GetTimestamp();
    private static readonly double TicksToMs = 1000.0 / Stopwatch.Frequency;

    public static void BeginFixedUpdate()
    {
        _fixedStart = Stopwatch.GetTimestamp();
    }

    public static void EndFixedUpdate()
    {
        long elapsed = Stopwatch.GetTimestamp() - _fixedStart;
        _fixedTicksTotal += elapsed;
        if (elapsed > _fixedTicksMax) _fixedTicksMax = elapsed;
        _fixedCount++;
        CheckLog();
    }

    public static void BeginOnGUI()
    {
        _guiStart = Stopwatch.GetTimestamp();
    }

    public static void EndOnGUI()
    {
        long elapsed = Stopwatch.GetTimestamp() - _guiStart;
        _guiTicksTotal += elapsed;
        if (elapsed > _guiTicksMax) _guiTicksMax = elapsed;
        _guiCount++;
        CheckLog();
    }

    private static void CheckLog()
    {
        long now = Stopwatch.GetTimestamp();
        double elapsedSec = (now - _lastLogTime) / (double)Stopwatch.Frequency;

        // In báo cáo mỗi 1.0 giây
        if (elapsedSec >= 1.0)
        {
            double fixedTps = _fixedCount / elapsedSec;
            double fixedAvgMs = _fixedCount > 0 ? (_fixedTicksTotal * TicksToMs / _fixedCount) : 0;
            double fixedMaxMs = _fixedTicksMax * TicksToMs;

            double guiFps = _guiCount / elapsedSec;
            double guiAvgMs = _guiCount > 0 ? (_guiTicksTotal * TicksToMs / _guiCount) : 0;
            double guiMaxMs = _guiTicksMax * TicksToMs;

            string log = $"[PERF] FixedUpdate: {fixedTps,5:F1} TPS (avg: {fixedAvgMs:F2}ms, max: {fixedMaxMs:F2}ms) | OnGUI: {guiFps,5:F1} FPS (avg: {guiAvgMs:F2}ms, max: {guiMaxMs:F2}ms)";

            GameScr.info1.addInfo(log,0);
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.Log(log);
#endif

            // Reset chu kỳ đo mới
            _fixedCount = 0;
            _fixedTicksTotal = 0;
            _fixedTicksMax = 0;

            _guiCount = 0;
            _guiTicksTotal = 0;
            _guiTicksMax = 0;

            _lastLogTime = now;
        }
    }
}