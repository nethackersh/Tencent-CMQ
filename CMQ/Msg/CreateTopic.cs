using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TencentCloud.CMQ.Msg {
    /// <summary>
    /// 创建一个主题后返回的消息
    /// </summary>
    public class CreateTopic : Base {
        /// <summary>
        /// 主题的Id。
        /// </summary>
        public string TopicId { get; set; }
    }
}
