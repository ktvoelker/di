using System;
using System.Collections.Generic;
namespace Di.Controller
{
    public class MenuMode : WindowMode
    {
        public string Title { get; set; }

        private IDictionary<KeyInput, string> descriptions = new Dictionary<KeyInput, string>();

        public void AddDescription(KeyInput key, string desc)
        {
            descriptions[key] = desc;
        }

        public string GetDescription(KeyInput key)
        {
            if (descriptions.ContainsKey(key))
            {
                return descriptions[key];
            }
            return null;
        }
    }
}

