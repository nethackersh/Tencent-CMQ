using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TencentCloud.CMQ.Msg {
    /// <summary>
    /// 主题清单
    /// </summary>
    public class ListTopic : Base {
        /// <summary>
        /// 主题总个数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 主题清单
        /// </summary>
        public Topic[] TopicList { get; set; }
    }
    public class Topic {
        /// <summary>
        /// 主题Id
        /// </summary>
        public string TopicId { get; set; }
        /// <summary>
        /// 主题名称
        /// </summary>
        public string TopicName { get; set; }
    }
}
