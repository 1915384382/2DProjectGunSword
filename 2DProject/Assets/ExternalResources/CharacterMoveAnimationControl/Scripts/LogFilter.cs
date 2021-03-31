using System;
using System.Collections.Generic;

namespace LogFilter
{
    public enum ELogLevel : byte { DEBUG = 0, INFO, IMPORTANT_INFO, WARNING, ERROR, EXCEPTION, NONE }

    public class CLogFilter
    {
        private Dictionary<ELogLevel, Action<string>> _dicprinters = new Dictionary<ELogLevel, Action<string>>();

        public delegate ELogLevel DGetCurrentLogLevel();

        private DGetCurrentLogLevel _getloglevel;

        ELogLevel _LogLevel;
        public ELogLevel LogLevel
        {
            set { _LogLevel = value; }
            get
            {
                if (_getloglevel == null)
                    return _LogLevel;
                return _getloglevel();
            }
        }
        
        public CLogFilter(ELogLevel inLogLevel, Action<string> inNormalPrintMsg, Action<string> inErrorPrintMsg)
        {
            if (inNormalPrintMsg == null)
                throw new ArgumentException("inNormalPrintMsg must be defined!");

            if (inErrorPrintMsg == null)
                throw new ArgumentException("inErrorPrintMsg must be defined!");

            LogLevel = inLogLevel;
            _dicprinters.Add(ELogLevel.DEBUG, inNormalPrintMsg);
            _dicprinters.Add(ELogLevel.INFO, inNormalPrintMsg);
            _dicprinters.Add(ELogLevel.IMPORTANT_INFO, inNormalPrintMsg);
            _dicprinters.Add(ELogLevel.WARNING, inNormalPrintMsg);
            _dicprinters.Add(ELogLevel.ERROR, inErrorPrintMsg);
            _dicprinters.Add(ELogLevel.EXCEPTION, inErrorPrintMsg);
        }

        public CLogFilter(Action<string> inNormalPrintMsg, Action<string> inErrorPrintMsg) : this(ELogLevel.DEBUG, inNormalPrintMsg, inErrorPrintMsg) { }
        public CLogFilter(Action<string> inPrintMsg) : this(ELogLevel.DEBUG, inPrintMsg, inPrintMsg) { }
        
        public void SetPrinter(ELogLevel inLogLevel, Action<string> inPrintMsg)
        {
            if (inPrintMsg == null)
                throw new ArgumentException("inPrintMsg must be defined!");
            _dicprinters[inLogLevel] = inPrintMsg;
        }

        public void SetLogLevelTracker(DGetCurrentLogLevel inGetloglevel) { _getloglevel = inGetloglevel; }

        public void Log(string inText, ELogLevel inLogLevel)
        {
            if (inLogLevel < ELogLevel.NONE && inLogLevel >= LogLevel &&
                _dicprinters.ContainsKey(inLogLevel) && _dicprinters[inLogLevel] != null)
            {
                _dicprinters[inLogLevel](inText);
            }
        }

        public void LogDebug(string inText) { Log(inText, ELogLevel.DEBUG); }
        public void LogInfo(string inText) { Log(inText, ELogLevel.INFO); }
        public void LogImportantInfo(string inText) { Log(inText, ELogLevel.IMPORTANT_INFO); }
        public void LogWarning(string inText) { Log(inText, ELogLevel.WARNING); }
        public void LogError(string inText) { Log(inText, ELogLevel.ERROR); }
        public void LogException(string inText) { Log(inText, ELogLevel.EXCEPTION); }
        public void Log(Exception ex) { Log(ex.Message, ELogLevel.EXCEPTION); }
    }

    public class CLogFilter2<T> : CLogFilter
    {
        private Dictionary<T, ELogLevel> _dicthemes = new Dictionary<T, ELogLevel>();

        public CLogFilter2(ELogLevel inLogLevel, Action<string> inNormalPrintMsg, Action<string> inErrorPrintMsg) : base(inLogLevel, inNormalPrintMsg, inErrorPrintMsg) { }
        public CLogFilter2(Action<string> inNormalPrintMsg, Action<string> inErrorPrintMsg) : base(inNormalPrintMsg, inErrorPrintMsg) { }
        public CLogFilter2(Action<string> inPrintMsg) : base(inPrintMsg) { }

        public void Set(T inTheme, ELogLevel inLogLevel)
        {
            if (_dicthemes.ContainsKey(inTheme))
                _dicthemes[inTheme] = inLogLevel;
            else
                _dicthemes.Add(inTheme, inLogLevel);
        }

        public void Set(IEnumerable<T> inThemes, ELogLevel inLogLevel)
        {
            foreach (var t in inThemes)
                Set(t, inLogLevel);
        }

        public void Log(string inText, T inTheme)
        {
            ELogLevel level = ELogLevel.IMPORTANT_INFO;
            if (_dicthemes.ContainsKey(inTheme))
                level = _dicthemes[inTheme];
            Log(inText, level);
        }
    }
}