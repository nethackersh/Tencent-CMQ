using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TencentCloud.CMQ.Msg {
    /// <summary>
    /// 队列清单
    /// </summary>
    public class ListQueue : Base {
        /// <summary>
        /// 队列总个数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 队列清单
        /// </summary>
        public Queue[] QueueList { get; set; }
    }
    public class Queue {
        /// <summary>
        /// 队列Id
        /// </summary>
        public string QueueId { get; set; }
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }
    }
}
