namespace Microsoft.AspNetCore.Mvc
{
    internal class CustomRouteAttribute : RouteAttribute
    {
        public CustomRouteAttribute(string template):base(RouteCustomizer.GetPrefixedRoute(template))
        { }
    }

    internal static class RouteCustomizer
    {
        private static string _prefix ="";
        public static string Prefix {
            get { return _prefix; }
            set
            {
                var prefix = value;
                if (string.IsNullOrWhiteSpace(prefix))
                    _prefix = "";
                else
                _prefix = prefix.EndsWith("/")? prefix : prefix + "/";
            }
        }
        public static string GetPrefixedRoute(string template)
        {
            return _prefix+template;
        }

    }
}
