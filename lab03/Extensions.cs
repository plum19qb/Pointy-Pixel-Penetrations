using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab03
{
    public static class Extensions
    {
        //Dictionary RemoveAll Extension Method Received from https://www.codeproject.com/Tips/494499/Implementing-Dictionary-RemoveAll
        public static void RemoveAll<K, V>(this IDictionary<K,V> dict, Func<K,V,bool> match)
        {
            foreach (var key in dict.Keys.ToArray()
            .Where(key => match(key, dict[key])))
                dict.Remove(key);
        }
    }
}
