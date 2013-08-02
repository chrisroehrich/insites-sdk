using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace IntelligentInSites.Api.Rest
{
    public class APIParams : IEnumerable
    {
        private Dictionary<String, List<Object>> Params;

        public APIParams()
        {
            this.Params = new Dictionary<String, List<Object>>();
        }

        /// <summary>
        /// Adds a String parameter to the collection.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="value">the value of the parameter</param>
        public void Add(string name, string value)
        {
            AddObject(name, value);
        }

        /// <summary>
        /// Adds a byte[] parameter to the collection. This would typically be used in an HTTP multipart/form-data POST request.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="value">the value of the parameter</param>
        public void Add(string name, byte[] value)
        {
            AddObject(name, value);
        }

        /// <summary>
        /// Adds an int parameter to the collection.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="value">the value of the parameter</param>
        public void Add(string name, int value)
        {
            AddObject(name, value);
        }

        /// <summary>
        /// Adds a bool parameter to the collection.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="value">the value of the parameter</param>
        public void Add(string name, bool value)
        {
            AddObject(name, value);
        }

        /// <summary>
        /// Adds a decimal parameter to the collection.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="value">the value of the parameter</param>
        public void Add(string name, decimal value)
        {
            AddObject(name, value);
        }

        /// <summary>
        /// Removes all parameters from the collection.
        /// </summary>
        public void Clear()
        {
            Params.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<String, List<object>> pair in Params)
            {
                foreach (object value in pair.Value)
                {
                    if (value is byte[])
                    {
                        yield return new KeyValuePair<string, object>(pair.Key, (byte[])value);
                    }
                    else
                    {
                        yield return new KeyValuePair<string, object>(pair.Key, value.ToString());
                    }
                }
            }
        }

        
        /// <summary>
        /// Returns the names of all parameters in the collection.
        /// </summary>
        /// <returns>A KeyCollection containing all parameter names in the collection.</returns>
        public Dictionary<string, List<object>>.KeyCollection GetNames()
        {
            return Params.Keys;
        }

        /// <summary>
        /// Returns the value of a parameter in the collection.
        /// If the parameter has multiple values, this returns a List of those values.
        /// </summary>
        /// <param name="name">The name of a parameter.</param>
        /// <returns>The value of the given parameter. If the parameter has multiple values, this returns a List of those values.</returns>
        public object GetValue(string name)
        {
            List<object> value = Params[name];
            if (value != null)
            {
                if (value.Count == 1)
                {
                    return value[0];
                }
            }
            return value;
        }

        /// <summary>
        /// Returns whether the given parameter name has been assigned a value in this instance.
        /// </summary>
        /// <param name="name">The name of a parameter.</param>
        /// <returns>True if the given parameter name has been assigned a value.</returns>
        public bool IsParameterSet(string name)
        {
            return Params.ContainsKey(name);
        }

        private void AddObject(string name, object value)
        {
            if (Params.ContainsKey(name))
            {
                Params[name].Add(value);
            }
            else
            {
                List<object> valueList = new List<object>();
                valueList.Add(value);
                Params.Add(name, valueList);
            }
        }
    }
}
