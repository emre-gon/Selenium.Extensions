using System;
using System.Collections.Generic;
using System.Text;

namespace Sl.Selenium
{
    public static class StringExtensions
    {
        public static IDictionary<String, String> GetQueryDictionary(this String query)
        {
            query = query.Substring(query.IndexOf('?') + 1);
            String[] splitted = query.Split('&');
            var map = new Dictionary<String, String>();
            foreach (String param in splitted)
            {
                String[] p = param.Split('=');
                String name = p[0];
                if (p.Length > 1)
                {
                    String value = p[1];
                    map[name] = value;
                }
            }
            return map;
        }
        public static String GetQueryString(this String query, String key)
        {
            return query.GetQueryDictionary()[key];
        }
    }
}
