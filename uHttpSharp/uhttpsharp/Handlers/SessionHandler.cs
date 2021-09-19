using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uhttpsharp.Handlers
{
    public class SessionHandler<TSession> : IHttpRequestHandler
    {
        private readonly Func<TSession> _sessionFactory;
        private readonly TimeSpan _expiration;

        private static readonly Random RandomGenerator = new Random();

        private class SessionHolder
        {
            private readonly TSession _session;
            private DateTime _lastAccessTime = DateTime.Now;

            public TSession Session
            {
                get
                {
                    _lastAccessTime = DateTime.Now;
                    return _session;
                }
            }

            public DateTime LastAccessTime
            {
                get
                {
                    return _lastAccessTime;
                }
            }

            public SessionHolder(TSession session)
            {
                _session = session;
            }
        }

        private readonly ConcurrentDictionary<string, SessionHolder> _sessions = new ConcurrentDictionary<string, SessionHolder>();

        public SessionHandler(Func<TSession> sessionFactory, TimeSpan expiration)
        {
            _sessionFactory = sessionFactory;
            _expiration = expiration;
        }

        public Task Handle(IHttpContext context, Func<Task> next)
        {

            string sessId;
            if (!context.Cookies.TryGetByName("SESSID", out sessId))
            {
                sessId = RandomGenerator.Next().ToString(CultureInfo.InvariantCulture);
                context.Cookies.Upsert("SESSID", sessId);
            }

            var sessionHolder = _sessions.GetOrAdd(sessId, CreateSession);

            if (DateTime.Now - sessionHolder.LastAccessTime > _expiration)
            {
                sessionHolder = CreateSession(sessId);
                _sessions.AddOrUpdate(sessId, sessionHolder, (sessionId, oldSession) => sessionHolder);
            }

            context.State.Session = sessionHolder.Session;

            return next();
        }
        private SessionHolder CreateSession(string sessionId)
        {
            return new SessionHolder(_sessionFactory());
        }
    }
}
