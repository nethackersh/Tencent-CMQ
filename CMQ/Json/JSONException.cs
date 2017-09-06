using System;

namespace TencentCloud.CMQ.Json {

    /// <summary>
    /// The JSONException is thrown by the JSON.org classes when things are amiss.
    /// 
    /// @author JSON.org
    /// @version 2014-05-03
    /// </summary>
    public class JSONException : Exception {
        private const long serialVersionUID = 0;
        private Exception cause;

        /// <summary>
        /// Constructs a JSONException with an explanatory message.
        /// </summary>
        /// <param name="message">
        ///            Detail about the reason for the exception. </param>
        public JSONException(string message) : base(message) {
        }

        /// <summary>
        /// Constructs a new JSONException with the specified cause. </summary>
        /// <param name="cause"> The cause. </param>
        public JSONException(Exception cause) : base(cause.Message) {
            this.cause = cause;
        }

        /// <summary>
        /// Returns the cause of this exception or null if the cause is nonexistent
        /// or unknown.
        /// </summary>
        /// <returns> the cause of this exception or null if the cause is nonexistent
        ///          or unknown. </returns>
        public override Exception Cause {
            get {
                return this.cause;
            }
        }
    }

}