using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace iyates
{
    public static class Nosj
    {
        public static readonly JsonSerializerSettings JSON_SETTINGS_FULL = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include
            };
        private static readonly JsonSerializerSettings NOSJ_SETTINGS = new JsonSerializerSettings
            {
                //DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

        /// <summary>
        /// Puts object fields in an array first and returns objects as arrays of values.
        /// Supports object instance, but designed really for IEnumerables
        /// </summary>
        /// <param name="nullMissing">
        /// true to use 'null' for missing values, false to use "__UNDEF__" string
        /// </param>
        /// <param name="stripDefaults">
        /// If true, strips fields where all instances are default. May result in field reordering.
        /// </param>
        public static string Serialize(object data, bool nullMissing = false, bool stripDefaults = true)
        {
            var str = JsonConvert.SerializeObject(data, NOSJ_SETTINGS);

            var fields = new List<string>();
            var objs = new List<Dictionary<string, object>>();

            using (var sr = new StringReader(str))
            using (var tr = new Newtonsoft.Json.JsonTextReader(sr))
            {
                tr.Read();
                if (tr.TokenType == JsonToken.StartArray)
                {
                    tr.Read();
                }

                while (tr.TokenType == JsonToken.StartObject && tr.Read())
                {
                    var obj = new Dictionary<string, object>();

                    while (tr.TokenType == JsonToken.PropertyName)
                    {
                        string propName = tr.Value.ToString();
                        tr.Read();

                        if (!fields.Contains(propName))
                        {
                            var typ = tr.ValueType ?? typeof(object);
                            if (stripDefaults && typ.IsPrimitive)
                            {
                                var def = Activator.CreateInstance(typ);
                                if (!tr.Value.Equals(def))
                                {
                                    fields.Add(propName);
                                }
                            }
                            else if (!stripDefaults || tr.TokenType != JsonToken.Null)
                            {
                                fields.Add(propName);
                            }
                        }

                        if (tr.TokenType == JsonToken.StartArray || tr.TokenType == JsonToken.StartObject)
                        {
                            var d = tr.Depth;
                            var sti = tr.LinePosition - 1;
                            while (tr.Read() && tr.Depth > d)
                                ;
                            obj[propName] = Newtonsoft.Json.Linq.JToken.Parse(str.Substring(sti, tr.LinePosition - sti));
                            tr.Read();
                        }
                        else
                        {
                            obj[propName] = tr.Value;
                            tr.Read();
                        }
                    }

                    objs.Add(obj);
                    tr.Read();
                }
            }

            var body = new List<object[]>();
            body.Add(fields.ToArray());
            foreach (var obj in objs)
            {
                var i = 0;
                for (var j = 0; j < fields.Count; ++j)
                {
                    if (obj.ContainsKey(fields[j]))
                    {
                        i = j;
                    }
                }

                var dt = new object[i + 1];
                for (var j = 0; j <= i; ++j)
                {
                    if (!obj.ContainsKey(fields[j]))
                    {
                        dt[j] = nullMissing ? null : "__UNDEF__";
                    }
                    else
                    {
                        dt[j] = obj[fields[j]];
                    }
                }
                body.Add(dt);
            }

            str = JsonConvert.SerializeObject(body, JSON_SETTINGS_FULL);
            return str;
        }
    }
}