using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CTimeCounters<N>
{
    private Dictionary<N, float> _times = new Dictionary<N, float>();

    internal void Reset(N inName)
    {
        SetTime(inName, 0);
    }

    internal void SetTime(N inName, float inTime)
    {
        if (_times.ContainsKey(inName))
            _times[inName] = inTime;
        else
            _times.Add(inName, inTime);
    }

    internal bool Update(N inName, float inPeriod, float deltaTime)
    {
        Update(inName, deltaTime);
        return IsPassed(inName, inPeriod);
    }

    internal void Update(N inName, float deltaTime)
    {
        if (_times.ContainsKey(inName))
            _times[inName] = _times[inName] + deltaTime;
        else
            _times.Add(inName, deltaTime);
    }

    internal float GetPassedTime(N inName)
    {
        if (!_times.ContainsKey(inName))
            return 0;
        return _times[inName];
    }

    internal bool IsPassed(N inName, float inPeriod)
    {
        return GetPassedTime(inName) >= inPeriod;
    }
}
