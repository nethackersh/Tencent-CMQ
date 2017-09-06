using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TencentCloud.CMQ.Json {

    /*
	 Copyright (c) 2002 JSON.org
	
	 Permission is hereby granted, free of charge, to any person obtaining a copy
	 of this software and associated documentation files (the "Software"), to deal
	 in the Software without restriction, including without limitation the rights
	 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	 copies of the Software, and to permit persons to whom the Software is
	 furnished to do so, subject to the following conditions:
	
	 The above copyright notice and this permission notice shall be included in all
	 copies or substantial portions of the Software.
	
	 The Software shall be used for Good, not Evil.
	
	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	 SOFTWARE.
	 */


    /// <summary>
    /// A JSONObject is an unordered collection of name/value pairs. Its external
    /// form is a string wrapped in curly braces with colons between the names and
    /// values, and commas between the values and names. The internal form is an
    /// object having <code>get</code> and <code>opt</code> methods for accessing
    /// the values by name, and <code>put</code> methods for adding or replacing
    /// values by name. The values can be any of these types: <code>Boolean</code>,
    /// <code>JSONArray</code>, <code>JSONObject</code>, <code>Number</code>,
    /// <code>String</code>, or the <code>JSONObject.NULL</code> object. A
    /// JSONObject constructor can be used to convert an external form JSON text
    /// into an internal form whose values can be retrieved with the
    /// <code>get</code> and <code>opt</code> methods, or to convert values into a
    /// JSON text using the <code>put</code> and <code>toString</code> methods. A
    /// <code>get</code> method returns a value if one can be found, and throws an
    /// exception if one cannot be found. An <code>opt</code> method returns a
    /// default value instead of throwing an exception, and so is useful for
    /// obtaining optional values.
    /// <p>
    /// The generic <code>get()</code> and <code>opt()</code> methods return an
    /// object, which you can cast or query for type. There are also typed
    /// <code>get</code> and <code>opt</code> methods that do type checking and type
    /// coercion for you. The opt methods differ from the get methods in that they
    /// do not throw. Instead, they return a specified value, such as null.
    /// <p>
    /// The <code>put</code> methods add or replace values in an object. For
    /// example,
    /// 
    /// <pre>
    /// myString = new JSONObject()
    ///         .put(&quot;JSON&quot;, &quot;Hello, World!&quot;).toString();
    /// </pre>
    /// 
    /// produces the string <code>{"JSON": "Hello, World"}</code>.
    /// <p>
    /// The texts produced by the <code>toString</code> methods strictly conform to
    /// the JSON syntax rules. The constructors are more forgiving in the texts they
    /// will accept:
    /// <ul>
    /// <li>An extra <code>,</code>&nbsp;<small>(comma)</small> may appear just
    /// before the closing brace.</li>
    /// <li>Strings may be quoted with <code>'</code>&nbsp;<small>(single
    /// quote)</small>.</li>
    /// <li>Strings do not need to be quoted at all if they do not begin with a
    /// quote or single quote, and if they do not contain leading or trailing
    /// spaces, and if they do not contain any of these characters:
    /// <code>{ } [ ] / \ : , #</code> and if they do not look like numbers and
    /// if they are not the reserved words <code>true</code>, <code>false</code>,
    /// or <code>null</code>.</li>
    /// </ul>
    /// 
    /// @author JSON.org
    /// @version 2015-05-05
    /// </summary>
    public class JSONObject {
        /// <summary>
        /// JSONObject.NULL is equivalent to the value that JavaScript calls null,
        /// whilst Java's null is equivalent to the value that JavaScript calls
        /// undefined.
        /// </summary>
        private sealed class Null {

            /// <summary>
            /// There is only intended to be a single instance of the NULL object,
            /// so the clone method returns itself.
            /// </summary>
            /// <returns> NULL. </returns>
            protected internal override object clone() {
                return this;
            }

            /// <summary>
            /// A Null object is equal to the null value and to itself.
            /// </summary>
            /// <param name="object">
            ///            An object to test for nullness. </param>
            /// <returns> true if the object parameter is the JSONObject.NULL object or
            ///         null. </returns>
            public override bool Equals(object @object) {
                return @object == null || @object == this;
            }

            /// <summary>
            /// Get the "null" string value.
            /// </summary>
            /// <returns> The string "null". </returns>
            public string ToString() {
                return "null";
            }
        }

        /// <summary>
        /// The map where the JSONObject's properties are kept.
        /// </summary>
        private readonly IDictionary<string, object> map;

        /// <summary>
        /// It is sometimes more convenient and less ambiguous to have a
        /// <code>NULL</code> object than to use Java's <code>null</code> value.
        /// <code>JSONObject.NULL.equals(null)</code> returns <code>true</code>.
        /// <code>JSONObject.NULL.toString()</code> returns <code>"null"</code>.
        /// </summary>
        public static readonly object NULL = new Null();

        /// <summary>
        /// Construct an empty JSONObject.
        /// </summary>
        public JSONObject() {
            this.map = new Dictionary<string, object>();
        }

        /// <summary>
        /// Construct a JSONObject from a subset of another JSONObject. An array of
        /// strings is used to identify the keys that should be copied. Missing keys
        /// are ignored.
        /// </summary>
        /// <param name="jo">
        ///            A JSONObject. </param>
        /// <param name="names">
        ///            An array of strings. </param>
        /// <exception cref="JSONException"> </exception>
        /// <exception cref="JSONException">
        ///                If a value is a non-finite number or if a name is
        ///                duplicated. </exception>
        public JSONObject(JSONObject jo, string[] names) : this() {
            for (int i = 0; i < names.Length; i += 1) {
                try {
                    this.putOnce(names[i], jo.opt(names[i]));
                } catch (Exception ignore) {
                }
            }
        }

        /// <summary>
        /// Construct a JSONObject from a JSONTokener.
        /// </summary>
        /// <param name="x">
        ///            A JSONTokener object containing the source string. </param>
        /// <exception cref="JSONException">
        ///             If there is a syntax error in the source string or a
        ///             duplicated key. </exception>
        
        //ORIGINAL LINE: public JSONObject(JSONTokener x) throws JSONException
        public JSONObject(JSONTokener x) : this() {
            char c;
            string key;

            if (x.nextClean() != '{') {
                throw x.syntaxError("A JSONObject text must begin with '{'");
            }
            for (;;) {
                c = x.nextClean();
                switch (c) {
                    case 0:
                        throw x.syntaxError("A JSONObject text must end with '}'");
                    case '}':
                        return;
                    default:
                        x.back();
                        key = x.nextValue().ToString();
                        break;
                }

                // The key is followed by ':'.

                c = x.nextClean();
                if (c != ':') {
                    throw x.syntaxError("Expected a ':' after a key");
                }
                this.putOnce(key, x.nextValue());

                // Pairs are separated by ','.

                switch (x.nextClean()) {
                    case ';':
                    case ',':
                        if (x.nextClean() == '}') {
                            return;
                        }
                        x.back();
                        break;
                    case '}':
                        return;
                    default:
                        throw x.syntaxError("Expected a ',' or '}'");
                }
            }
        }

        /// <summary>
        /// Construct a JSONObject from a Map.
        /// </summary>
        /// <param name="map">
        ///            A map object that can be used to initialize the contents of
        ///            the JSONObject. </param>
        /// <exception cref="JSONException"> </exception>
        public JSONObject(IDictionary<string, object> map) {
            this.map = new Dictionary<string, object>();
            if (map != null) {
                IEnumerator<KeyValuePair<string, object>> i = map.GetEnumerator();
                while (i.MoveNext()) {
                    KeyValuePair<string, object> entry = i.Current;
                    object value = entry.Value;
                    if (value != null) {
                        this.map[entry.Key] = wrap(value);
                    }
                }
            }
        }

        /// <summary>
        /// Construct a JSONObject from an Object using bean getters. It reflects on
        /// all of the public methods of the object. For each of the methods with no
        /// parameters and a name starting with <code>"get"</code> or
        /// <code>"is"</code> followed by an uppercase letter, the method is invoked,
        /// and a key and the value returned from the getter method are put into the
        /// new JSONObject.
        /// 
        /// The key is formed by removing the <code>"get"</code> or <code>"is"</code>
        /// prefix. If the second remaining character is not upper case, then the
        /// first character is converted to lower case.
        /// 
        /// For example, if an object has a method named <code>"getName"</code>, and
        /// if the result of calling <code>object.getName()</code> is
        /// <code>"Larry Fine"</code>, then the JSONObject will contain
        /// <code>"name": "Larry Fine"</code>.
        /// </summary>
        /// <param name="bean">
        ///            An object that has getter methods that should be used to make
        ///            a JSONObject. </param>
        public JSONObject(object bean) : this() {
            this.populateMap(bean);
        }

        /// <summary>
        /// Construct a JSONObject from an Object, using reflection to find the
        /// public members. The resulting JSONObject's keys will be the strings from
        /// the names array, and the values will be the field values associated with
        /// those keys in the object. If a key is not found or not visible, then it
        /// will not be copied into the new JSONObject.
        /// </summary>
        /// <param name="object">
        ///            An object that has fields that should be used to make a
        ///            JSONObject. </param>
        /// <param name="names">
        ///            An array of strings, the names of the fields to be obtained
        ///            from the object. </param>
        public JSONObject(object @object, string[] names) : this() {
            Type c = @object.GetType();
            for (int i = 0; i < names.Length; i += 1) {
                string name = names[i];
                try {
                    this.putOpt(name, c.GetField(name).get(@object));
                } catch (Exception ignore) {
                }
            }
        }

        /// <summary>
        /// Construct a JSONObject from a source JSON text string. This is the most
        /// commonly used JSONObject constructor.
        /// </summary>
        /// <param name="source">
        ///            A string beginning with <code>{</code>&nbsp;<small>(left
        ///            brace)</small> and ending with <code>}</code>
        ///            &nbsp;<small>(right brace)</small>. </param>
        /// <exception cref="JSONException">
        ///                If there is a syntax error in the source string or a
        ///                duplicated key. </exception>
        
        //ORIGINAL LINE: public JSONObject(String source) throws JSONException
        public JSONObject(string source) : this(new JSONTokener(source)) {
        }

        /// <summary>
        /// Construct a JSONObject from a ResourceBundle.
        /// </summary>
        /// <param name="baseName">
        ///            The ResourceBundle base name. </param>
        /// <param name="locale">
        ///            The Locale to load the ResourceBundle for. </param>
        /// <exception cref="JSONException">
        ///             If any JSONExceptions are detected. </exception>
        
        //ORIGINAL LINE: public JSONObject(String baseName, java.util.Locale locale) throws JSONException
        public JSONObject(string baseName, java.util.Locale locale) : this() {
            java.util.ResourceBundle bundle = java.util.ResourceBundle.getBundle(baseName, locale, Thread.CurrentThread.ContextClassLoader);

            // Iterate through the keys in the bundle.

            System.Collections.IEnumerator<string> keys = bundle.Keys;
            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
            while (keys.hasMoreElements()) {
                //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                object key = keys.nextElement();
                if (key != null) {

                    // Go through the path, ensuring that there is a nested JSONObject for each
                    // segment except the last. Add the value using the last segment's name into
                    // the deepest nested JSONObject.

                    string[] path = StringHelperClass.StringSplit(((string)key), "\\.", true);
                    int last = path.Length - 1;
                    JSONObject target = this;
                    for (int i = 0; i < last; i += 1) {
                        string segment = path[i];
                        JSONObject nextTarget = target.optJSONObject(segment);
                        if (nextTarget == null) {
                            nextTarget = new JSONObject();
                            target.put(segment, nextTarget);
                        }
                        target = nextTarget;
                    }
                    target.put(path[last], bundle.getString((string)key));
                }
            }
        }

        /// <summary>
        /// Accumulate values under a key. It is similar to the put method except
        /// that if there is already an object stored under the key then a JSONArray
        /// is stored under the key to hold all of the accumulated values. If there
        /// is already a JSONArray, then the new value is appended to it. In
        /// contrast, the put method replaces the previous value.
        /// 
        /// If only one value is accumulated that is not a JSONArray, then the result
        /// will be the same as using put. But if multiple values are accumulated,
        /// then the result will be like append.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            An object to be accumulated under the key. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If the value is an invalid number or if the key is null. </exception>
        
        //ORIGINAL LINE: public JSONObject accumulate(String key, Object value) throws JSONException
        public virtual JSONObject accumulate(string key, object value) {
            testValidity(value);
            object @object = this.opt(key);
            if (@object == null) {
                this.put(key, value is JSONArray ? (new JSONArray()).put(value) : value);
            } else if (@object is JSONArray) {
                ((JSONArray)@object).put(value);
            } else {
                this.put(key, (new JSONArray()).put(@object).put(value));
            }
            return this;
        }

        /// <summary>
        /// Append values to the array under a key. If the key does not exist in the
        /// JSONObject, then the key is put in the JSONObject with its value being a
        /// JSONArray containing the value parameter. If the key was already
        /// associated with a JSONArray, then the value parameter is appended to it.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            An object to be accumulated under the key. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If the key is null or if the current value associated with
        ///             the key is not a JSONArray. </exception>
        
        //ORIGINAL LINE: public JSONObject append(String key, Object value) throws JSONException
        public virtual JSONObject append(string key, object value) {
            testValidity(value);
            object @object = this.opt(key);
            if (@object == null) {
                this.put(key, (new JSONArray()).put(value));
            } else if (@object is JSONArray) {
                this.put(key, ((JSONArray)@object).put(value));
            } else {
                throw new JSONException("JSONObject[" + key + "] is not a JSONArray.");
            }
            return this;
        }

        /// <summary>
        /// Produce a string from a double. The string "null" will be returned if the
        /// number is not finite.
        /// </summary>
        /// <param name="d">
        ///            A double. </param>
        /// <returns> A String. </returns>
        public static string doubleToString(double d) {
            if (double.IsInfinity(d) || double.IsNaN(d)) {
                return "null";
            }

            // Shave off trailing zeros and decimal point, if possible.

            string @string = Convert.ToString(d);
            if (@string.IndexOf('.') > 0 && @string.IndexOf('e') < 0 && @string.IndexOf('E') < 0) {
                while (@string.EndsWith("0")) {
                    @string = @string.Substring(0, @string.Length - 1);
                }
                if (@string.EndsWith(".")) {
                    @string = @string.Substring(0, @string.Length - 1);
                }
            }
            return @string;
        }

        /// <summary>
        /// Get the value object associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> The object associated with the key. </returns>
        /// <exception cref="JSONException">
        ///             if the key is not found. </exception>
        
        //ORIGINAL LINE: public Object get(String key) throws JSONException
        public virtual object get(string key) {
            if (key == null) {
                throw new JSONException("Null key.");
            }
            object @object = this.opt(key);
            if (@object == null) {
                throw new JSONException("JSONObject[" + quote(key) + "] not found.");
            }
            return @object;
        }

        /// <summary>
        /// Get the boolean value associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> The truth. </returns>
        /// <exception cref="JSONException">
        ///             if the value is not a Boolean or the String "true" or
        ///             "false". </exception>
        
        //ORIGINAL LINE: public boolean getBoolean(String key) throws JSONException
        public virtual bool getBoolean(string key) {
            object @object = this.get(key);
            if (@object.Equals(false) || (@object is string && ((string)@object).ToUpper() == "false".ToUpper())) {
                return false;
            } else if (@object.Equals(true) || (@object is string && ((string)@object).ToUpper() == "true".ToUpper())) {
                return true;
            }
            throw new JSONException("JSONObject[" + quote(key) + "] is not a Boolean.");
        }

        /// <summary>
        /// Get the double value associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> The numeric value. </returns>
        /// <exception cref="JSONException">
        ///             if the key is not found or if the value is not a Number
        ///             object and cannot be converted to a number. </exception>
        
        //ORIGINAL LINE: public double getDouble(String key) throws JSONException
        public virtual double getDouble(string key) {
            object @object = this.get(key);
            try {
                return @object is Number ? (double)((Number)@object) : Convert.ToDouble((string)@object);
            } catch (Exception e) {
                throw new JSONException("JSONObject[" + quote(key) + "] is not a number.");
            }
        }

        /// <summary>
        /// Get the int value associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> The integer value. </returns>
        /// <exception cref="JSONException">
        ///             if the key is not found or if the value cannot be converted
        ///             to an integer. </exception>
        
        //ORIGINAL LINE: public int getInt(String key) throws JSONException
        public virtual int getInt(string key) {
            object @object = this.get(key);
            try {
                return @object is Number ? (int)((Number)@object) : Convert.ToInt32((string)@object);
            } catch (Exception e) {
                throw new JSONException("JSONObject[" + quote(key) + "] is not an int.");
            }
        }

        /// <summary>
        /// Get the JSONArray value associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> A JSONArray which is the value. </returns>
        /// <exception cref="JSONException">
        ///             if the key is not found or if the value is not a JSONArray. </exception>
        
        //ORIGINAL LINE: public JSONArray getJSONArray(String key) throws JSONException
        public virtual JSONArray getJSONArray(string key) {
            object @object = this.get(key);
            if (@object is JSONArray) {
                return (JSONArray)@object;
            }
            throw new JSONException("JSONObject[" + quote(key) + "] is not a JSONArray.");
        }

        /// <summary>
        /// Get the JSONObject value associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> A JSONObject which is the value. </returns>
        /// <exception cref="JSONException">
        ///             if the key is not found or if the value is not a JSONObject. </exception>
        
        //ORIGINAL LINE: public JSONObject getJSONObject(String key) throws JSONException
        public virtual JSONObject getJSONObject(string key) {
            object @object = this.get(key);
            if (@object is JSONObject) {
                return (JSONObject)@object;
            }
            throw new JSONException("JSONObject[" + quote(key) + "] is not a JSONObject.");
        }

        /// <summary>
        /// Get the long value associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> The long value. </returns>
        /// <exception cref="JSONException">
        ///             if the key is not found or if the value cannot be converted
        ///             to a long. </exception>
        
        //ORIGINAL LINE: public long getLong(String key) throws JSONException
        public virtual long getLong(string key) {
            object @object = this.get(key);
            try {
                return @object is Number ? (long)((Number)@object) : Convert.ToInt64((string)@object);
            } catch (Exception e) {
                throw new JSONException("JSONObject[" + quote(key) + "] is not a long.");
            }
        }

        /// <summary>
        /// Get an array of field names from a JSONObject.
        /// </summary>
        /// <returns> An array of field names, or null if there are no names. </returns>
        public static string[] getNames(JSONObject jo) {
            int length = jo.length();
            if (length == 0) {
                return null;
            }
            IEnumerator<string> iterator = jo.keys();
            string[] names = new string[length];
            int i = 0;
            while (iterator.MoveNext()) {
                names[i] = iterator.Current;
                i += 1;
            }
            return names;
        }

        /// <summary>
        /// Get an array of field names from an Object.
        /// </summary>
        /// <returns> An array of field names, or null if there are no names. </returns>
        public static string[] getNames(object @object) {
            if (@object == null) {
                return null;
            }
            Type klass = @object.GetType();
            Field[] fields = klass.GetFields();
            int length = fields.Length;
            if (length == 0) {
                return null;
            }
            string[] names = new string[length];
            for (int i = 0; i < length; i += 1) {
                names[i] = fields[i].Name;
            }
            return names;
        }

        /// <summary>
        /// Get the string associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> A string which is the value. </returns>
        /// <exception cref="JSONException">
        ///             if there is no string value for the key. </exception>
        
        //ORIGINAL LINE: public String getString(String key) throws JSONException
        public virtual string getString(string key) {
            object @object = this.get(key);
            if (@object is string) {
                return (string)@object;
            }
            throw new JSONException("JSONObject[" + quote(key) + "] not a string.");
        }

        /// <summary>
        /// Determine if the JSONObject contains a specific key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> true if the key exists in the JSONObject. </returns>
        public virtual bool has(string key) {
            return this.map.ContainsKey(key);
        }

        /// <summary>
        /// Increment a property of a JSONObject. If there is no such property,
        /// create one with a value of 1. If there is such a property, and if it is
        /// an Integer, Long, Double, or Float, then add one to it.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If there is already a property with this name that is not an
        ///             Integer, Long, Double, or Float. </exception>
        
        //ORIGINAL LINE: public JSONObject increment(String key) throws JSONException
        public virtual JSONObject increment(string key) {
            object value = this.opt(key);
            if (value == null) {
                this.put(key, 1);
            } else if (value is int?) {
                this.put(key, (int?)value + 1);
            } else if (value is long?) {
                this.put(key, (long?)value + 1);
            } else if (value is double?) {
                this.put(key, (double?)value + 1);
            } else if (value is float?) {
                this.put(key, (float?)value + 1);
            } else {
                throw new JSONException("Unable to increment [" + quote(key) + "].");
            }
            return this;
        }

        /// <summary>
        /// Determine if the value associated with the key is null or if there is no
        /// value.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> true if there is no value associated with the key or if the value
        ///         is the JSONObject.NULL object. </returns>
        public virtual bool isNull(string key) {
            return JSONObject.NULL.Equals(this.opt(key));
        }

        /// <summary>
        /// Get an enumeration of the keys of the JSONObject.
        /// </summary>
        /// <returns> An iterator of the keys. </returns>
        public virtual IEnumerator<string> keys() {
            return this.Keys.GetEnumerator();
        }

        /// <summary>
        /// Get a set of keys of the JSONObject.
        /// </summary>
        /// <returns> A keySet. </returns>
        public virtual java.util.Set<string> keySet() {
            return this.map.Keys;
        }

        /// <summary>
        /// Get the number of keys stored in the JSONObject.
        /// </summary>
        /// <returns> The number of keys in the JSONObject. </returns>
        public virtual int length() {
            return this.map.Count;
        }

        /// <summary>
        /// Produce a JSONArray containing the names of the elements of this
        /// JSONObject.
        /// </summary>
        /// <returns> A JSONArray containing the key strings, or null if the JSONObject
        ///         is empty. </returns>
        public virtual JSONArray names() {
            JSONArray ja = new JSONArray();
            IEnumerator<string> keys = this.keys();
            while (keys.MoveNext()) {
                ja.put(keys.Current);
            }
            return ja.length() == 0 ? null : ja;
        }

        /// <summary>
        /// Produce a string from a Number.
        /// </summary>
        /// <param name="number">
        ///            A Number </param>
        /// <returns> A String. </returns>
        /// <exception cref="JSONException">
        ///             If n is a non-finite number. </exception>
        
        //ORIGINAL LINE: public static String numberToString(Number number) throws JSONException
        public static string numberToString(Number number) {
            if (number == null) {
                throw new JSONException("Null pointer");
            }
            testValidity(number);

            // Shave off trailing zeros and decimal point, if possible.

            string @string = number.ToString();
            if (@string.IndexOf('.') > 0 && @string.IndexOf('e') < 0 && @string.IndexOf('E') < 0) {
                while (@string.EndsWith("0")) {
                    @string = @string.Substring(0, @string.Length - 1);
                }
                if (@string.EndsWith(".")) {
                    @string = @string.Substring(0, @string.Length - 1);
                }
            }
            return @string;
        }

        /// <summary>
        /// Get an optional value associated with a key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> An object which is the value, or null if there is no value. </returns>
        public virtual object opt(string key) {
            return key == null ? null : this.map[key];
        }

        /// <summary>
        /// Get an optional boolean associated with a key. It returns false if there
        /// is no such key, or if the value is not Boolean.TRUE or the String "true".
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> The truth. </returns>
        public virtual bool optBoolean(string key) {
            return this.optBoolean(key, false);
        }

        /// <summary>
        /// Get an optional boolean associated with a key. It returns the
        /// defaultValue if there is no such key, or if it is not a Boolean or the
        /// String "true" or "false" (case insensitive).
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="defaultValue">
        ///            The default. </param>
        /// <returns> The truth. </returns>
        public virtual bool optBoolean(string key, bool defaultValue) {
            try {
                return this.getBoolean(key);
            } catch (Exception e) {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get an optional double associated with a key, or NaN if there is no such
        /// key or if its value is not a number. If the value is a string, an attempt
        /// will be made to evaluate it as a number.
        /// </summary>
        /// <param name="key">
        ///            A string which is the key. </param>
        /// <returns> An object which is the value. </returns>
        public virtual double optDouble(string key) {
            return this.optDouble(key, double.NaN);
        }

        /// <summary>
        /// Get an optional double associated with a key, or the defaultValue if
        /// there is no such key or if its value is not a number. If the value is a
        /// string, an attempt will be made to evaluate it as a number.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="defaultValue">
        ///            The default. </param>
        /// <returns> An object which is the value. </returns>
        public virtual double optDouble(string key, double defaultValue) {
            try {
                return this.getDouble(key);
            } catch (Exception e) {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get an optional int value associated with a key, or zero if there is no
        /// such key or if the value is not a number. If the value is a string, an
        /// attempt will be made to evaluate it as a number.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> An object which is the value. </returns>
        public virtual int optInt(string key) {
            return this.optInt(key, 0);
        }

        /// <summary>
        /// Get an optional int value associated with a key, or the default if there
        /// is no such key or if the value is not a number. If the value is a string,
        /// an attempt will be made to evaluate it as a number.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="defaultValue">
        ///            The default. </param>
        /// <returns> An object which is the value. </returns>
        public virtual int optInt(string key, int defaultValue) {
            try {
                return this.getInt(key);
            } catch (Exception e) {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get an optional JSONArray associated with a key. It returns null if there
        /// is no such key, or if its value is not a JSONArray.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> A JSONArray which is the value. </returns>
        public virtual JSONArray optJSONArray(string key) {
            object o = this.opt(key);
            return o is JSONArray ? (JSONArray)o : null;
        }

        /// <summary>
        /// Get an optional JSONObject associated with a key. It returns null if
        /// there is no such key, or if its value is not a JSONObject.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> A JSONObject which is the value. </returns>
        public virtual JSONObject optJSONObject(string key) {
            object @object = this.opt(key);
            return @object is JSONObject ? (JSONObject)@object : null;
        }

        /// <summary>
        /// Get an optional long value associated with a key, or zero if there is no
        /// such key or if the value is not a number. If the value is a string, an
        /// attempt will be made to evaluate it as a number.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> An object which is the value. </returns>
        public virtual long optLong(string key) {
            return this.optLong(key, 0);
        }

        /// <summary>
        /// Get an optional long value associated with a key, or the default if there
        /// is no such key or if the value is not a number. If the value is a string,
        /// an attempt will be made to evaluate it as a number.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="defaultValue">
        ///            The default. </param>
        /// <returns> An object which is the value. </returns>
        public virtual long optLong(string key, long defaultValue) {
            try {
                return this.getLong(key);
            } catch (Exception e) {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get an optional string associated with a key. It returns an empty string
        /// if there is no such key. If the value is not a string and is not null,
        /// then it is converted to a string.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <returns> A string which is the value. </returns>
        public virtual string optString(string key) {
            return this.optString(key, "");
        }

        /// <summary>
        /// Get an optional string associated with a key. It returns the defaultValue
        /// if there is no such key.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="defaultValue">
        ///            The default. </param>
        /// <returns> A string which is the value. </returns>
        public virtual string optString(string key, string defaultValue) {
            object @object = this.opt(key);
            return NULL.Equals(@object) ? defaultValue : @object.ToString();
        }

        private void populateMap(object bean) {
            Type klass = bean.GetType();

            // If klass is a System class then set includeSuperClass to false.

            bool includeSuperClass = klass.ClassLoader != null;

            Method[] methods = includeSuperClass ? klass.GetMethods() : klass.DeclaredMethods;
            for (int i = 0; i < methods.Length; i += 1) {
                try {
                    Method method = methods[i];
                    if (Modifier.isPublic(method.Modifiers)) {
                        string name = method.Name;
                        string key = "";
                        if (name.StartsWith("get")) {
                            if ("getClass".Equals(name) || "getDeclaringClass".Equals(name)) {
                                key = "";
                            } else {
                                key = name.Substring(3);
                            }
                        } else if (name.StartsWith("is")) {
                            key = name.Substring(2);
                        }
                        if (key.Length > 0 && char.IsUpper(key[0]) && method.ParameterTypes.length == 0) {
                            if (key.Length == 1) {
                                key = key.ToLower();
                            } else if (!char.IsUpper(key[1])) {
                                key = key.Substring(0, 1).ToLower() + key.Substring(1);
                            }

                            object result = method.invoke(bean, (object[])null);
                            if (result != null) {
                                this.map[key] = wrap(result);
                            }
                        }
                    }
                } catch (Exception ignore) {
                }
            }
        }

        /// <summary>
        /// Put a key/boolean pair in the JSONObject.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            A boolean which is the value. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If the key is null. </exception>
        
        //ORIGINAL LINE: public JSONObject put(String key, boolean value) throws JSONException
        public virtual JSONObject put(string key, bool value) {
            this.put(key, value ? true : false);
            return this;
        }

        /// <summary>
        /// Put a key/value pair in the JSONObject, where the value will be a
        /// JSONArray which is produced from a Collection.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            A Collection value. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException"> </exception>
        
        //ORIGINAL LINE: public JSONObject put(String key, java.util.Collection<Object> value) throws JSONException
        public virtual JSONObject put(string key, ICollection<object> value) {
            this.put(key, new JSONArray(value));
            return this;
        }

        /// <summary>
        /// Put a key/double pair in the JSONObject.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            A double which is the value. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If the key is null or if the number is invalid. </exception>
        
        //ORIGINAL LINE: public JSONObject put(String key, double value) throws JSONException
        public virtual JSONObject put(string key, double value) {
            this.put(key, new double?(value));
            return this;
        }

        /// <summary>
        /// Put a key/int pair in the JSONObject.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            An int which is the value. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If the key is null. </exception>
        
        //ORIGINAL LINE: public JSONObject put(String key, int value) throws JSONException
        public virtual JSONObject put(string key, int value) {
            this.put(key, new int?(value));
            return this;
        }

        /// <summary>
        /// Put a key/long pair in the JSONObject.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            A long which is the value. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If the key is null. </exception>
        
        //ORIGINAL LINE: public JSONObject put(String key, long value) throws JSONException
        public virtual JSONObject put(string key, long value) {
            this.put(key, new long?(value));
            return this;
        }

        /// <summary>
        /// Put a key/value pair in the JSONObject, where the value will be a
        /// JSONObject which is produced from a Map.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            A Map value. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException"> </exception>
        
        //ORIGINAL LINE: public JSONObject put(String key, java.util.Map<String, Object> value) throws JSONException
        public virtual JSONObject put(string key, IDictionary<string, object> value) {
            this.put(key, new JSONObject(value));
            return this;
        }

        /// <summary>
        /// Put a key/value pair in the JSONObject. If the value is null, then the
        /// key will be removed from the JSONObject if it is present.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            An object which is the value. It should be of one of these
        ///            types: Boolean, Double, Integer, JSONArray, JSONObject, Long,
        ///            String, or the JSONObject.NULL object. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If the value is non-finite number or if the key is null. </exception>
        
        //ORIGINAL LINE: public JSONObject put(String key, Object value) throws JSONException
        public virtual JSONObject put(string key, object value) {
            if (key == null) {
                throw new NullPointerException("Null key.");
            }
            if (value != null) {
                testValidity(value);
                this.map[key] = value;
            } else {
                this.remove(key);
            }
            return this;
        }

        /// <summary>
        /// Put a key/value pair in the JSONObject, but only if the key and the value
        /// are both non-null, and only if there is not already a member with that
        /// name.
        /// </summary>
        /// <param name="key"> string </param>
        /// <param name="value"> object </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             if the key is a duplicate </exception>
        
        //ORIGINAL LINE: public JSONObject putOnce(String key, Object value) throws JSONException
        public virtual JSONObject putOnce(string key, object value) {
            if (key != null && value != null) {
                if (this.opt(key) != null) {
                    throw new JSONException("Duplicate key \"" + key + "\"");
                }
                this.put(key, value);
            }
            return this;
        }

        /// <summary>
        /// Put a key/value pair in the JSONObject, but only if the key and the value
        /// are both non-null.
        /// </summary>
        /// <param name="key">
        ///            A key string. </param>
        /// <param name="value">
        ///            An object which is the value. It should be of one of these
        ///            types: Boolean, Double, Integer, JSONArray, JSONObject, Long,
        ///            String, or the JSONObject.NULL object. </param>
        /// <returns> this. </returns>
        /// <exception cref="JSONException">
        ///             If the value is a non-finite number. </exception>
        
        //ORIGINAL LINE: public JSONObject putOpt(String key, Object value) throws JSONException
        public virtual JSONObject putOpt(string key, object value) {
            if (key != null && value != null) {
                this.put(key, value);
            }
            return this;
        }

        /// <summary>
        /// Produce a string in double quotes with backslash sequences in all the
        /// right places. A backslash will be inserted within </, producing <\/,
        /// allowing JSON text to be delivered in HTML. In JSON text, a string cannot
        /// contain a control character or an unescaped quote or backslash.
        /// </summary>
        /// <param name="string">
        ///            A String </param>
        /// <returns> A String correctly formatted for insertion in a JSON text. </returns>
        public static string quote(string @string) {
            TextWriter sw = new java.io.StringWriter();
            lock (sw.Buffer) {
                try {
                    return quote(@string, sw).ToString();
                } catch (IOException ignored) {
                    // will never happen - we are writing to a string writer
                    return "";
                }
            }
        }

        
        //ORIGINAL LINE: public static java.io.Writer quote(String string, java.io.Writer w) throws java.io.IOException
        public static TextWriter quote(string @string, TextWriter w) {
            if (@string == null || @string.Length == 0) {
                w.write("\"\"");
                return w;
            }

            char b;
            char c = 0;
            string hhhh;
            int i;
            int len = @string.Length;

            w.Write('"');
            for (i = 0; i < len; i += 1) {
                b = c;
                c = @string[i];
                switch (c) {
                    case '\\':
                    case '"':
                        w.Write('\\');
                        w.Write(c);
                        break;
                    case '/':
                        if (b == '<') {
                            w.Write('\\');
                        }
                        w.Write(c);
                        break;
                    case '\b':
                        w.Write("\\b");
                        break;
                    case '\t':
                        w.Write("\\t");
                        break;
                    case '\n':
                        w.Write("\\n");
                        break;
                    case '\f':
                        w.Write("\\f");
                        break;
                    case '\r':
                        w.Write("\\r");
                        break;
                    default:
                        if (c < ' ' || (c >= '\u0080' && c < '\u00a0') || (c >= '\u2000' && c < '\u2100')) {
                            w.Write("\\u");
                            hhhh = int.toHexString(c);
                            w.Write("0000", 0, 4 - hhhh.Length);
                            w.Write(hhhh);
                        } else {
                            w.Write(c);
                        }
                        break;
                }
            }
            w.Write('"');
            return w;
        }

        /// <summary>
        /// Remove a name and its value, if present.
        /// </summary>
        /// <param name="key">
        ///            The name to be removed. </param>
        /// <returns> The value that was associated with the name, or null if there was
        ///         no value. </returns>
        public virtual object remove(string key) {
            return this.map.Remove(key);
        }

        /// <summary>
        /// Determine if two JSONObjects are similar.
        /// They must contain the same set of names which must be associated with
        /// similar values.
        /// </summary>
        /// <param name="other"> The other JSONObject </param>
        /// <returns> true if they are equal </returns>
        public virtual bool similar(object other) {
            try {
                if (!(other is JSONObject)) {
                    return false;
                }
                java.util.Set<string> set = this.Keys;
                if (!set.Equals(((JSONObject)other).Keys)) {
                    return false;
                }
                IEnumerator<string> iterator = set.GetEnumerator();
                while (iterator.MoveNext()) {
                    string name = iterator.Current;
                    object valueThis = this.get(name);
                    object valueOther = ((JSONObject)other).get(name);
                    if (valueThis is JSONObject) {
                        if (!((JSONObject)valueThis).similar(valueOther)) {
                            return false;
                        }
                    } else if (valueThis is JSONArray) {
                        if (!((JSONArray)valueThis).similar(valueOther)) {
                            return false;
                        }
                    } else if (!valueThis.Equals(valueOther)) {
                        return false;
                    }
                }
                return true;
            } catch (Exception exception) {
                return false;
            }
        }

        /// <summary>
        /// Try to convert a string into a number, boolean, or null. If the string
        /// can't be converted, return the string.
        /// </summary>
        /// <param name="string">
        ///            A String. </param>
        /// <returns> A simple JSON value. </returns>
        public static object stringToValue(string @string) {
            double? d;
            if (@string.Equals("")) {
                return @string;
            }
            if (@string.ToUpper() == "true".ToUpper()) {
                return true;
            }
            if (@string.ToUpper() == "false".ToUpper()) {
                return false;
            }
            if (@string.ToUpper() == "null".ToUpper()) {
                return JSONObject.NULL;
            }

            /*
			 * If it might be a number, try converting it. If a number cannot be
			 * produced, then the value will just be a string.
			 */

            char b = @string[0];
            if ((b >= '0' && b <= '9') || b == '-') {
                try {
                    if (@string.IndexOf('.') > -1 || @string.IndexOf('e') > -1 || @string.IndexOf('E') > -1) {
                        d = Convert.ToDouble(@string);
                        if (!d.Infinite && !d.NaN) {
                            return d;
                        }
                    } else {
                        long? myLong = new long?(@string);
                        if (@string.Equals(myLong.ToString())) {
                            if (myLong == (int)myLong) {
                                return (int)myLong;
                            } else {
                                return myLong;
                            }
                        }
                    }
                } catch (Exception ignore) {
                }
            }
            return @string;
        }

        /// <summary>
        /// Throw an exception if the object is a NaN or infinite number.
        /// </summary>
        /// <param name="o">
        ///            The object to test. </param>
        /// <exception cref="JSONException">
        ///             If o is a non-finite number. </exception>
        
        //ORIGINAL LINE: public static void testValidity(Object o) throws JSONException
        public static void testValidity(object o) {
            if (o != null) {
                if (o is double?) {
                    if (((double?)o).Infinite || ((double?)o).NaN) {
                        throw new JSONException("JSON does not allow non-finite numbers.");
                    }
                } else if (o is float?) {
                    if (((float?)o).Infinite || ((float?)o).NaN) {
                        throw new JSONException("JSON does not allow non-finite numbers.");
                    }
                }
            }
        }

        /// <summary>
        /// Produce a JSONArray containing the values of the members of this
        /// JSONObject.
        /// </summary>
        /// <param name="names">
        ///            A JSONArray containing a list of key strings. This determines
        ///            the sequence of the values in the result. </param>
        /// <returns> A JSONArray of values. </returns>
        /// <exception cref="JSONException">
        ///             If any of the values are non-finite numbers. </exception>
        
        //ORIGINAL LINE: public JSONArray toJSONArray(JSONArray names) throws JSONException
        public virtual JSONArray toJSONArray(JSONArray names) {
            if (names == null || names.length() == 0) {
                return null;
            }
            JSONArray ja = new JSONArray();
            for (int i = 0; i < names.length(); i += 1) {
                ja.put(this.opt(names.getString(i)));
            }
            return ja;
        }

        /// <summary>
        /// Make a JSON text of this JSONObject. For compactness, no whitespace is
        /// added. If this would not result in a syntactically correct JSON text,
        /// then null will be returned instead.
        /// <p>
        /// Warning: This method assumes that the data structure is acyclical.
        /// </summary>
        /// <returns> a printable, displayable, portable, transmittable representation
        ///         of the object, beginning with <code>{</code>&nbsp;<small>(left
        ///         brace)</small> and ending with <code>}</code>&nbsp;<small>(right
        ///         brace)</small>. </returns>
        public virtual string ToString() {
            try {
                return this.ToString(0);
            } catch (Exception e) {
                return null;
            }
        }

        /// <summary>
        /// Make a prettyprinted JSON text of this JSONObject.
        /// <p>
        /// Warning: This method assumes that the data structure is acyclical.
        /// </summary>
        /// <param name="indentFactor">
        ///            The number of spaces to add to each level of indentation. </param>
        /// <returns> a printable, displayable, portable, transmittable representation
        ///         of the object, beginning with <code>{</code>&nbsp;<small>(left
        ///         brace)</small> and ending with <code>}</code>&nbsp;<small>(right
        ///         brace)</small>. </returns>
        /// <exception cref="JSONException">
        ///             If the object contains an invalid number. </exception>
        
        //ORIGINAL LINE: public String toString(int indentFactor) throws JSONException
        public virtual string ToString(int indentFactor) {
            java.io.StringWriter w = new java.io.StringWriter();
            lock (w.Buffer) {
                return this.write(w, indentFactor, 0).ToString();
            }
        }

        /// <summary>
        /// Make a JSON text of an Object value. If the object has an
        /// value.toJSONString() method, then that method will be used to produce the
        /// JSON text. The method is required to produce a strictly conforming text.
        /// If the object does not contain a toJSONString method (which is the most
        /// common case), then a text will be produced by other means. If the value
        /// is an array or Collection, then a JSONArray will be made from it and its
        /// toJSONString method will be called. If the value is a MAP, then a
        /// JSONObject will be made from it and its toJSONString method will be
        /// called. Otherwise, the value's toString method will be called, and the
        /// result will be quoted.
        /// 
        /// <p>
        /// Warning: This method assumes that the data structure is acyclical.
        /// </summary>
        /// <param name="value">
        ///            The value to be serialized. </param>
        /// <returns> a printable, displayable, transmittable representation of the
        ///         object, beginning with <code>{</code>&nbsp;<small>(left
        ///         brace)</small> and ending with <code>}</code>&nbsp;<small>(right
        ///         brace)</small>. </returns>
        /// <exception cref="JSONException">
        ///             If the value is or contains an invalid number. </exception>
        
        //ORIGINAL LINE: public static String valueToString(Object value) throws JSONException
        public static string valueToString(object value) {
            if (value == null || value.Equals(null)) {
                return "null";
            }
            if (value is JSONString) {
                object @object;
                try {
                    @object = ((JSONString)value).toJSONString();
                } catch (Exception e) {
                    throw new JSONException(e);
                }
                if (@object is string) {
                    return (string)@object;
                }
                throw new JSONException("Bad value from toJSONString: " + @object);
            }
            if (value is Number) {
                return numberToString((Number)value);
            }
            if (value is bool? || value is JSONObject || value is JSONArray) {
                return value.ToString();
            }
            if (value is IDictionary) {
                
                
                IDictionary<string, object> map = (IDictionary<string, object>)value;
                return (new JSONObject(map)).ToString();
            }
            if (value is ICollection) {
                
                //ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Collection<Object> coll = (java.util.Collection<Object>) value;
                ICollection<object> coll = (ICollection<object>)value;
                return (new JSONArray(coll)).ToString();
            }
            if (value.GetType().IsArray) {
                return (new JSONArray(value)).ToString();
            }
            return quote(value.ToString());
        }

        /// <summary>
        /// Wrap an object, if necessary. If the object is null, return the NULL
        /// object. If it is an array or collection, wrap it in a JSONArray. If it is
        /// a map, wrap it in a JSONObject. If it is a standard property (Double,
        /// String, et al) then it is already wrapped. Otherwise, if it comes from
        /// one of the java packages, turn it into a string. And if it doesn't, try
        /// to wrap it in a JSONObject. If the wrapping fails, then null is returned.
        /// </summary>
        /// <param name="object">
        ///            The object to wrap </param>
        /// <returns> The wrapped value </returns>
        public static object wrap(object @object) {
            try {
                if (@object == null) {
                    return NULL;
                }
                if (@object is JSONObject || @object is JSONArray || NULL.Equals(@object) || @object is JSONString || @object is sbyte? || @object is char? || @object is short? || @object is int? || @object is long? || @object is bool? || @object is float? || @object is double? || @object is string) {
                    return @object;
                }

                if (@object is ICollection) {
                    
                    //ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Collection<Object> coll = (java.util.Collection<Object>) object;
                    ICollection<object> coll = (ICollection<object>)@object;
                    return new JSONArray(coll);
                }
                if (@object.GetType().IsArray) {
                    return new JSONArray(@object);
                }
                if (@object is IDictionary) {
                    
                    //ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Map<String, Object> map = (java.util.Map<String, Object>) object;
                    IDictionary<string, object> map = (IDictionary<string, object>)@object;
                    return new JSONObject(map);
                }
                Package objectPackage = @object.GetType().Assembly;
                string objectPackageName = objectPackage != null ? objectPackage.Name : "";
                if (objectPackageName.StartsWith("java.") || objectPackageName.StartsWith("javax.") || @object.GetType().ClassLoader == null) {
                    return @object.ToString();
                }
                return new JSONObject(@object);
            } catch (Exception exception) {
                return null;
            }
        }

        /// <summary>
        /// Write the contents of the JSONObject as JSON text to a writer. For
        /// compactness, no whitespace is added.
        /// <p>
        /// Warning: This method assumes that the data structure is acyclical.
        /// </summary>
        /// <returns> The writer. </returns>
        /// <exception cref="JSONException"> </exception>
        
        
        public virtual TextWriter write(TextWriter writer) {
            return this.write(writer, 0, 0);
        }

        
        
        internal static TextWriter writeValue(TextWriter writer, object value, int indentFactor, int indent) {
            if (value == null || value.Equals(null)) {
                writer.Write("null");
            } else if (value is JSONObject) {
                ((JSONObject)value).write(writer, indentFactor, indent);
            } else if (value is JSONArray) {
                ((JSONArray)value).write(writer, indentFactor, indent);
            } else if (value is IDictionary) {
                
                
                IDictionary<string, object> map = (IDictionary<string, object>)value;
                (new JSONObject(map)).write(writer, indentFactor, indent);
            } else if (value is ICollection) {
                
                //ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.Collection<Object> coll = (java.util.Collection<Object>) value;
                ICollection<object> coll = (ICollection<object>)value;
                (new JSONArray(coll)).write(writer, indentFactor, indent);
            } else if (value.GetType().IsArray) {
                (new JSONArray(value)).write(writer, indentFactor, indent);
            } else if (value is Number) {
                writer.Write(numberToString((Number)value));
            } else if (value is bool?) {
                writer.Write(value.ToString());
            } else if (value is JSONString) {
                object o;
                try {
                    o = ((JSONString)value).toJSONString();
                } catch (Exception e) {
                    throw new JSONException(e);
                }
                writer.Write(o != null ? o.ToString() : quote(value.ToString()));
            } else {
                quote(value.ToString(), writer);
            }
            return writer;
        }

        
        
        internal static void indent(TextWriter writer, int indent) {
            for (int i = 0; i < indent; i += 1) {
                writer.Write(' ');
            }
        }

        /// <summary>
        /// Write the contents of the JSONObject as JSON text to a writer. For
        /// compactness, no whitespace is added.
        /// <p>
        /// Warning: This method assumes that the data structure is acyclical.
        /// </summary>
        /// <returns> The writer. </returns>
        /// <exception cref="JSONException"> </exception>
        internal virtual TextWriter write(TextWriter writer, int indentFactor, int indent) {
            try {
                bool commanate = false;
                int length = this.length();
                IEnumerator<string> keys = this.keys();
                writer.Write('{');

                if (length == 1) {
                    string key = keys.Current;
                    writer.Write(quote(key.ToString()));
                    writer.Write(':');
                    if (indentFactor > 0) {
                        writer.Write(' ');
                    }
                    writeValue(writer, this.map[key], indentFactor, indent);
                } else if (length != 0) {
                    int newindent = indent + indentFactor;
                    while (keys.MoveNext()) {
                        string key = keys.Current;
                        if (commanate) {
                            writer.Write(',');
                        }
                        if (indentFactor > 0) {
                            writer.Write('\n');
                        }
                        indent(writer, newindent);
                        writer.Write(quote(key.ToString()));
                        writer.Write(':');
                        if (indentFactor > 0) {
                            writer.Write(' ');
                        }
                        writeValue(writer, this.map[key], indentFactor, newindent);
                        commanate = true;
                    }
                    if (indentFactor > 0) {
                        writer.Write('\n');
                    }
                    indent(writer, indent);
                }
                writer.Write('}');
                return writer;
            } catch (IOException exception) {
                throw new JSONException(exception);
            }
        }
    }

}