using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace InCube.Core.Web
{
    /// <summary>
    /// Owin middleware to log all API requests to the DB.
    /// </summary>
    /// <remarks>
    /// Usage: app.Use&lt;LoggingMiddleware&gt;((logContent) => Console.WriteLine(logContent.ToString()));
    /// </remarks>
    public class LoggingMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Gets or sets the maximum length of a body that is logged, in bytes.
        /// </summary>
        /// <remarks>
        /// I.e. the request body is not logged if the body exceeds this length.
        /// </remarks>
        public int MaxRequestBodyLength { get; set; } = 30000;

        private readonly Action<RequestLogInfo> loggingCallback;

        private readonly HashSet<string> ignoredLocalPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="loggingCallback">The callback used when </param>
        public LoggingMiddleware(OwinMiddleware next, Action<RequestLogInfo> loggingCallback, IEnumerable<string> ignoredLocalPaths) : base(next)
        {
            this.loggingCallback = loggingCallback;
            this.ignoredLocalPaths = new HashSet<string>(ignoredLocalPaths);
        }

        /// <inheritdoc />
        public override async Task Invoke(IOwinContext context)
        {
            var requestStart = DateTime.Now;

            // read out body (wait for all bytes)
            using (var streamCopy = new MemoryStream())
            {
                context.Request.Body.CopyTo(streamCopy);
                streamCopy.Position = 0;

                string body;
                if (streamCopy.Length < MaxRequestBodyLength)
                {
                    body = new StreamReader(streamCopy).ReadToEnd();
                }
                else
                {
                    body = "[request body is too large for logging]";
                }

                // rewind and put back in place
                streamCopy.Position = 0;
                context.Request.Body = streamCopy;

                // hand over to child middlewares
                await this.Next.Invoke(context);

                this.WriteLogEntry(context, body, requestStart, DateTime.Now - requestStart);
            }
        }

        private void WriteLogEntry(IOwinContext context, string body, DateTime requestStart, TimeSpan duration)
        {
            var httpMethod = context.Request.Method;
            var localPath = context.Request.Uri.LocalPath;

            // early return if the api call is not relevant for the audit-log.
            if (httpMethod == HttpMethod.Options.Method || this.IsIgnoredLocalPath(localPath))
            {
                return;
            }

            var username = context?.Authentication?.User?.Identity?.Name;
            var remoteIpAddress = context.Request.RemoteIpAddress;

            var logContext = new RequestLogInfo()
            {
                OriginalRequest = context.Request,
                Username = username,
                RemoteIpAddress = remoteIpAddress,
                HttpMethod = httpMethod,
                LocalPath = localPath,
                RequestBody = body,
                RequestStart = requestStart,
                RequestDuration = duration
            };

            this.loggingCallback.BeginInvoke(logContext, Callback, null);
        }

        private static void Callback(IAsyncResult ar)
        {
            // TODO: Require error handler
        }

        private bool IsIgnoredLocalPath(string localPath)
        {
            return this.ignoredLocalPaths.Any(localUriSubstring => localPath.StartsWith(localUriSubstring, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// A class containing relevant information about a HTTP request to be logged.
        /// </summary>
        public class RequestLogInfo
        {
            /// <summary>
            /// Gets or sets the HTTP method the request uses.
            /// </summary>
            public string HttpMethod { get; set; }

            /// <summary>
            /// Gets or sets the IP address from where the request was sent.
            /// </summary>
            public string RemoteIpAddress { get; set; }

            /// <summary>
            /// Gets or sets the username (OWIN Prinicpal UPN, if any)
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the local path (the URL without the host name).
            /// </summary>
            public string LocalPath { get; set; }

            /// <summary>
            /// Gets or sets the request body. IMPORTANT: This is be null if the request body length exceeds <see cref=""/>.
            /// </summary>
            public string RequestBody { get; set; }

            /// <summary>
            /// Gets or sets time the request was started.
            /// </summary>
            public DateTime RequestStart { get; set; }

            /// <summary>
            /// Gets or sets the duration the request took to process.
            /// </summary>
            public TimeSpan RequestDuration { get; set; }

            /// <summary>
            /// Gets or sets the HTTP request istself. 
            /// Use this to as fallback to access any request properties not extracted already.
            /// </summary>
            public IOwinRequest OriginalRequest { get; set; }
        }
    }

    public static class LoggingMiddlewareExtensionMethod
    {
        
    }
}
