using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TencentCloud.CMQ.Msg {
    /// <summary>
    /// 向队列发送消息
    /// </summary>
    public class SendMessage : Base {
        /// <summary>
        /// 服务器生成消息的唯一标识 Id。
        /// </summary>
        public string MsgId { get; set; }
    }

}
