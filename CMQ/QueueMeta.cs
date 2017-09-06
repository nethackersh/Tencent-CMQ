namespace TencentCloud.CMQ {
    /// <summary>
    /// ����Ԫ���ݣ����л�������
    /// </summary>
    public class QueueMeta:Msg.Base {
        public QueueMeta() {
            this.PollingWaitSeconds = 0;
            this.VisibilityTimeout = 30;
            this.MaxMsgSize = 65536;
            this.MsgRetentionSeconds = 345600;
            this.MaxMsgHeapNum = -1;
            this.CreateTime = -1;
            this.LastModifyTime = -1;
            this.ActiveMsgNum = -1;
            this.InactiveMsgNum = -1;
        }
        /// <summary>
        /// ���ѻ���Ϣ����ȡֵ��Χ�ڹ����ڼ�Ϊ 1,000,000 - 10,000,000����ʽ���ߺ�Χ�ɴﵽ 1000,000-1000,000,000��Ĭ��ȡֵ�ڹ����ڼ�Ϊ 10,000,000����ʽ���ߺ�Ϊ 100,000,000��
        /// </summary>
        public int MaxMsgHeapNum { get; set; }
        /// <summary>
        /// ��Ϣ���ճ���ѯ�ȴ�ʱ�䡣ȡֵ��Χ0-30�룬Ĭ��ֵ0��
        /// </summary>
        public int PollingWaitSeconds { get; set; }
        /// <summary>
        /// ��Ϣ�ɼ��Գ�ʱ��ȡֵ��Χ1-43200�루��12Сʱ�ڣ���Ĭ��ֵ30��
        /// </summary>
        public int VisibilityTimeout { get; set; }
        /// <summary>
        /// ��Ϣ��󳤶ȡ�ȡֵ��Χ1024-65536 Byte����1-64K����Ĭ��ֵ65536��
        /// </summary>
        public int MaxMsgSize { get; set; }
        /// <summary>
        /// ��Ϣ�������ڡ�ȡֵ��Χ60-1296000�루1min-15�죩��Ĭ��ֵ345600 (4 ��)��
        /// </summary>
        public int MsgRetentionSeconds { get; set; }
        /// <summary>
        /// ���еĴ���ʱ�䡣����Unixʱ�������ȷ���롣
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// ���һ���޸Ķ������Ե�ʱ�䡣����Unixʱ�������ȷ���롣
        /// </summary>
        public int LastModifyTime { get; set; }
        /// <summary>
        /// �ڶ����д��� Active ״̬�������ڱ�����״̬������Ϣ������Ϊ����ֵ��
        /// </summary>
        public int ActiveMsgNum { get; set; }
        /// <summary>
        /// �ڶ����д��� Inactive ״̬�������ڱ�����״̬������Ϣ������Ϊ����ֵ��
        /// </summary>
        public int InactiveMsgNum { get; set; }
        /// <summary>
        /// �ѵ���DelMsg�ӿ�ɾ���������ڻ��ݱ���ʱ���ڵ���Ϣ������
        /// </summary>
        public int RewindmsgNum { get; set; }
        /// <summary>
        /// ��Ϣ��Сδ����ʱ�䣬��λΪ��
        /// </summary>
        public int MinMsgTime { get; set; }
        /// <summary>
        /// ��ʱ��Ϣ����
        /// </summary>
        public int DelayMsgNum { get; set; }
        /// <summary>
        /// ���Ϣ����ʱ��,��λ�� 
        /// </summary>
        public int RewindSeconds { get; set; }

    }

}