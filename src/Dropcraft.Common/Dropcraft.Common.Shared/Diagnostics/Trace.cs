using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dropcraft.Common.Diagnostics
{
    public class Trace
    {
        private readonly TraceSource _traceSource = new TraceSource("Dropcraft", SourceLevels.Information);

        public static Trace Current { get; } = new Trace();

        private Trace()
        {
        }

        public SourceLevels Level
        {
            get { return _traceSource.Switch.Level; }
            set { _traceSource.Switch.Level = value; }
        }

        public void AddListener(TraceListener listener)
        {
            _traceSource.Listeners.Add(listener);
        }

        public void RemoveListener(TraceListener listener)
        {
            _traceSource.Listeners.Remove(listener);
        }

        public IEnumerable<TraceListener> Listeners =>
            _traceSource.Listeners.OfType<TraceListener>();

        public void Critical(string messageOrFormat, params object[] args) =>
            TraceEvent(TraceEventType.Critical, messageOrFormat, args);

        public void Error(string messageOrFormat, params object[] args) =>
            TraceEvent(TraceEventType.Error, messageOrFormat, args);

        public void Warning(string messageOrFormat, params object[] args) =>
            TraceEvent(TraceEventType.Warning, messageOrFormat, args);

        public void Information(string messageOrFormat, params object[] args) =>
            TraceEvent(TraceEventType.Information, messageOrFormat, args);

        public void Verbose(string messageOrFormat, params object[] args) =>
            TraceEvent(TraceEventType.Verbose, messageOrFormat, args);

        public void TraceEvent(TraceEventType eventType, string messageOrFormat, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                _traceSource.TraceEvent(eventType, 0, messageOrFormat);
            }
            else
            {
                _traceSource.TraceEvent(eventType, 0, messageOrFormat, args);
            }
        }
    }
}
