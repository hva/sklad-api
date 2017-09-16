using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace SkladApi
{
    public class MySqlTraceListener : ConsoleTraceListener
    {
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format,
            params object[] args)
        {
            if ((MySqlTraceEventType)id == MySqlTraceEventType.QueryOpened)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                base.TraceEvent(eventCache, source, eventType, id, "{2}", args);
                Console.ForegroundColor = color;
            }

            //base.TraceEvent(eventCache, source, eventType, id, format, args);
        }
    }
}
