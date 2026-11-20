using System;
using System.Collections.Generic;
using System.Linq;

namespace GapAndIslandSessionization
{
    public static class Sessionizer
    {
        private static readonly TimeSpan SessionGap = TimeSpan.FromMinutes(30);

        // Single-pass solution using Aggregate
        public static List<List<DateTime>> BuildSessions(List<DateTime> events)
        {
            if (events == null || events.Count == 0)
                return new List<List<DateTime>>();

            return events.Aggregate(
                new List<List<DateTime>>(),
                (sessions, currentEvent) =>
                {
                    if (sessions.Count == 0)
                    {
                        sessions.Add(new List<DateTime> { currentEvent });
                        return sessions;
                    }

                    var currentSession = sessions[^1];
                    var previousEvent = currentSession[^1];

                    if (currentEvent - previousEvent > SessionGap)
                    {
                        sessions.Add(new List<DateTime> { currentEvent });
                    }
                    else
                    {
                        currentSession.Add(currentEvent);
                    }

                    return sessions;
                });
        }

        // Example
        public static void Main()
        {
            var events = new List<DateTime>
            {
                new DateTime(2026, 1, 1, 10, 0, 0),
                new DateTime(2026, 1, 1, 10, 12, 0),
                new DateTime(2026, 1, 1, 10, 25, 0),
                new DateTime(2026, 1, 1, 11, 5, 0),
                new DateTime(2026, 1, 1, 11, 20, 0)
            };

            var sessions = BuildSessions(events);

            Console.WriteLine($"Sessions: {sessions.Count}");
        }
    }
}
