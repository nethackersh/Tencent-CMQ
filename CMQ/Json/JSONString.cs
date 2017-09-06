namespace TencentCloud.CMQ.Json
{
	/// <summary>
	/// The <code>JSONString</code> interface allows a <code>toJSONString()</code>
	/// method so that a class can change the behavior of
	/// <code>JSONObject.toString()</code>, <code>JSONArray.toString()</code>,
	/// and <code>JSONWriter.value(</code>Object<code>)</code>. The
	/// <code>toJSONString</code> method will be used instead of the default behavior
	/// of using the Object's <code>toString()</code> method and quoting the result.
	/// </summary>
	public interface JSONString
	{
		/// <summary>
		/// The <code>toJSONString</code> method allows a class to produce its own JSON
		/// serialization.
		/// </summary>
		/// <returns> A strictly syntactically correct JSON text. </returns>
		string toJSONString();
	}

}