using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TencentCloud.CMQ.Msg {
    /// <summary>
    /// 调用 创建消息队列的API后，返回的消息
    /// </summary>
    public class CreateQueue : Base {
        /// <summary>
        /// 队列的唯一标识Id。
        /// </summary>
        public string QueueId { get; set; }
    }

}
