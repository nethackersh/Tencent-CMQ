using System.Collections.Generic;

namespace TencentCloud.CMQ {
    /// <summary>
    /// �����е�һ����Ϣ
    /// </summary>
    public class QueueMessage:Msg.Base {
        public QueueMessage() {

        }
        /// <summary>
        /// �������ѵ���ϢΨһ��ʶ Id��
        /// </summary>
        public string MsgId;
        /// <summary>
        /// ÿ�����ѷ���Ψһ����Ϣ���������ɾ������Ϣ������һ������ʱ��������Ϣ���������ɾ����Ϣ��
        /// </summary>
        public string ReceiptHandle;
        /// <summary>
        /// �������ѵ���Ϣ���ġ�
        /// </summary>
        public string MsgBody;
        /// <summary>
        /// ���ѱ�����������������е�ʱ�䡣����Unixʱ�������ȷ���롣
        /// </summary>
        public long EnqueueTime;
        /// <summary>
        /// ��Ϣ���´οɼ������ٴα����ѣ�ʱ�䡣����Unixʱ�������ȷ���롣
        /// </summary>
        public long NextVisibleTime;
        /// <summary>
        /// ��һ�����Ѹ���Ϣ��ʱ�䡣����Unixʱ�������ȷ���롣
        /// </summary>
        public long FirstDequeueTime;
        /// <summary>
        /// ��Ϣ�����ѵĴ�����
        /// </summary>
        public int DequeueCount;
    }
    public class BatchQueueMessage:Msg.Base{

        /// <summary>
        /// message��Ϣ�б�ÿ��Ԫ����һ����Ϣ�ľ�����Ϣ��
        /// </summary>
        public QueueMessage[] MsgInfoList { get; set; }
    }
}