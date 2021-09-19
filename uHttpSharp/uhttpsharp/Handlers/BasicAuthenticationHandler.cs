using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uhttpsharp.Headers;

namespace uhttpsharp.Handlers
{
   
    public class BasicAuthenticationHandler : IHttpRequestHandler
    {
        private static readonly string BasicPrefix = "Basic ";
        private static readonly int BasicPrefixLength = BasicPrefix.Length;

        private readonly string _username;
        private readonly string _password;
        private readonly string _authenticationKey;
        private readonly ListHttpHeaders _headers;

        public BasicAuthenticationHandler(string realm, string username, string password)
        {
            _username = username;
            _password = password;
            _authenticationKey = "Authenticated." + realm;
            _headers = new ListHttpHeaders(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("WWW-Authenticate", string.Format(@"Basic realm=""{0}""", realm))
            });
        }

        public Task Handle(IHttpContext context, Func<Task> next)
        {
            IDictionary<string, dynamic> session = context.State.Session;
            dynamic ipAddress;
            
            if (!session.TryGetValue(_authenticationKey, out ipAddress) || ipAddress != context.RemoteEndPoint)
            {
                if (TryAuthenticate(context, session))
                {
                    return next();
                }

                context.Response =
                    StringHttpResponse.Create("Not Authenticated", HttpResponseCode.Unauthorized,
                    headers:
                            _headers);

                return Task.Factory.GetCompleted();
            }

            return next();
        }

        private bool TryAuthenticate(IHttpContext context, IDictionary<string, dynamic> session)
        {
            string credentials;
            if (context.Request.Headers.TryGetByName("Authorization", out credentials))
            {

                if (TryAuthenticate(credentials))
                {
                    session[_authenticationKey] = context.RemoteEndPoint;
                    {

                        return true;
                    }
                }
            }
            return false;
        }


        private bool TryAuthenticate(string credentials)
        {
            if (!credentials.StartsWith(BasicPrefix))
            {
                return false;
            }

            var basicCredentials = credentials.Substring(BasicPrefixLength);

            var usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(basicCredentials));
            var nekudataimIndex = usernameAndPassword.IndexOf(':');
            if (nekudataimIndex != -1)
            {
                var username = usernameAndPassword.Substring(0, nekudataimIndex);
                var password = usernameAndPassword.Substring(nekudataimIndex + 1);

                if (username == _username && password == _password)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
