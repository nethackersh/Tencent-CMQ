using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TencentCloud.CMQ {
    /// <summary>
    /// ����
    /// </summary>
    public class Queue {
        protected  string queueName;
        protected  CMQClient client;
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="client"></param>
        internal Queue(string queueName, CMQClient client) {
            this.queueName = queueName;
            this.client = client;
        }
        /// <summary>
        /// ��ȡ�����ö��е�����/Ԫ����
        /// </summary>
        public virtual QueueMeta QueueAttributes {
            set {
                SortedDictionary<string, string> param = new SortedDictionary<string, string>();

                param["queueName"] = this.queueName;

                if (value.MaxMsgHeapNum > 0) {
                    param["maxMsgHeapNum"] = Convert.ToString(value.MaxMsgHeapNum);
                }
                if (value.PollingWaitSeconds > 0) {
                    param["pollingWaitSeconds"] = Convert.ToString(value.PollingWaitSeconds);
                }
                if (value.VisibilityTimeout > 0) {
                    param["visibilityTimeout"] = Convert.ToString(value.VisibilityTimeout);
                }
                if (value.MaxMsgSize > 0) {
                    param["maxMsgSize"] = Convert.ToString(value.MaxMsgSize);
                }
                if (value.MsgRetentionSeconds > 0) {
                    param["msgRetentionSeconds"] = Convert.ToString(value.MsgRetentionSeconds);
                }
                if (value.RewindSeconds > 0) {
                    param["rewindSeconds"] = Convert.ToString(value.RewindSeconds);
                }

                string result = this.client.Call("SetQueueAttributes", param);
                QueueMeta jsonObj = JsonConvert.DeserializeObject<QueueMeta>(result);
                if (jsonObj.Code != 0) {
                    throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
                }
            }
            get {
                SortedDictionary<string, string> param = new SortedDictionary<string, string>();
                param["queueName"] = this.queueName;
                string result = this.client.Call("GetQueueAttributes", param);
                QueueMeta jsonObj = JsonConvert.DeserializeObject<QueueMeta>(result);
                if (jsonObj.Code != 0) {
                    throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
                }
                return jsonObj;
            }
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="msgBody">��Ϣ����</param>
        /// <returns>������Ϣ��MsgId</returns>
        public virtual string SendMessage(string msgBody) {
            return SendMessage(msgBody, 0);
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="msgBody">��Ϣ����</param>
        /// <param name="delayTime">��λΪ�룬��ʾ����Ϣ���͵����к���Ҫ��ʱ����û��ſɼ�����Ϣ</param>
        /// <returns>������Ϣ��MsgId</returns>
        public virtual string SendMessage(string msgBody, int delayTime) {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();

            param["queueName"] = this.queueName;
            param["msgBody"] = msgBody;
            param["delaySeconds"] = Convert.ToString(delayTime);

            string result = this.client.Call("SendMessage", param);
            Msg.SendMessage jsonObj = JsonConvert.DeserializeObject<Msg.SendMessage>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }
            return jsonObj.MsgId;
        }
        /// <summary>
        /// ����������Ϣ
        /// </summary>
        /// <param name="vtMsgBody">��Ϣ����</param>
        /// <returns>��Ϣ��MsgId����</returns>
        public virtual List<string> BatchSendMessage(IList<string> vtMsgBody) {
            return BatchSendMessage(vtMsgBody, 0);
        }
        /// <summary>
        /// ���������������Ϣ
        /// </summary>
        /// <param name="vtMsgBody">��Ϣ����</param>
        /// <param name="delayTime">��λΪ�룬��ʾ����Ϣ���͵����к���Ҫ��ʱ����û��ſɼ�����Ϣ</param>
        /// <returns></returns>
        public virtual List<string> BatchSendMessage(IList<string> vtMsgBody, int delayTime) {
            if (vtMsgBody.Count == 0 || vtMsgBody.Count > 16) {
                throw new CMQClientException("Error: message size is empty or more than 16");
            }

            SortedDictionary<string, string> param = new SortedDictionary<string, string>();

            param["queueName"] = this.queueName;
            for (int i = 0; i < vtMsgBody.Count; i++) {
                string k = "msgBody." + Convert.ToString(i + 1);
                param[k] = vtMsgBody[i];
            }
            param["delaySeconds"] = Convert.ToString(delayTime);

            string result = this.client.Call("BatchSendMessage", param);
            Msg.BatchSendMessage jsonObj = JsonConvert.DeserializeObject<Msg.BatchSendMessage>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }
            return jsonObj.MsgList;
        }

        /// <summary>
        /// ���ӿ� (ReceiveMessage) �������Ѷ����е�һ����Ϣ��
        /// ReceiveMessage �����Ὣȡ�õ���Ϣ״̬��� inactive��inactive ��ʱ�䳤���ɶ������� visibilityTimeout ָ�������CreateQueue�ӿڣ��� 
        /// ������������ visibilityTimeout ʱ�������ѳɹ�����Ҫ���� (batch)DeleteMessage �ӿ�ɾ������Ϣ���������Ϣ�������±��Ϊ active ״̬��
        /// ����Ϣ�ֿɱ��������������ѡ�
        /// </summary>
        /// <param name="pollingWaitSeconds">��������ĳ���ѯ�ȴ�ʱ�䡣ȡֵ��Χ 0-30 �룬��������øò�������Ĭ��ʹ�ö��������е�pollingWaitSecondsֵ��</param>
        /// <returns></returns>
        public virtual QueueMessage ReceiveMessage(int pollingWaitSeconds) {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();

            param["queueName"] = this.queueName;
            if (pollingWaitSeconds > 0) {
                param["UserpollingWaitSeconds"] = Convert.ToString(pollingWaitSeconds * 1000);
                param["pollingWaitSeconds"] = Convert.ToString(pollingWaitSeconds);
            } else {
                param["UserpollingWaitSeconds"] = Convert.ToString(30000);
            }

            string result = this.client.Call("ReceiveMessage", param);
            QueueMessage jsonObj = JsonConvert.DeserializeObject<QueueMessage>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }
            return jsonObj;
        }

        /// <summary>
        /// ����������Ϣ
        /// </summary>
        /// <param name="numOfMsg">�������ѵ���Ϣ������ȡֵ��Χ 1-16��</param>
        /// <param name="pollingWaitSeconds">��������ĳ���ѯ�ȴ�ʱ�䡣ȡֵ��Χ 0-30 ��,��������øò�������Ĭ��ʹ�ö��������е�pollingWaitSecondsֵ��</param>
        /// <returns>message��Ϣ�б�ÿ��Ԫ����һ����Ϣ�ľ�����Ϣ��</returns>
        public virtual List<QueueMessage> BatchReceiveMessage(int numOfMsg, int pollingWaitSeconds) {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();

            param["queueName"] = this.queueName;
            param["numOfMsg"] = Convert.ToString(numOfMsg);
            if (pollingWaitSeconds > 0) {
                param["UserpollingWaitSeconds"] = Convert.ToString(pollingWaitSeconds * 1000);
                param["pollingWaitSeconds"] = Convert.ToString(pollingWaitSeconds);
            } else {
                param["UserpollingWaitSeconds"] = Convert.ToString(30000);
            }
            string result = this.client.Call("BatchReceiveMessage", param);
            BatchQueueMessage jsonObj = JsonConvert.DeserializeObject<BatchQueueMessage>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }

            List<QueueMessage> vtMessage = new List<QueueMessage>();
            foreach (var model in jsonObj.MsgInfoList) {
                QueueMessage msg = model;
                msg.Code = jsonObj.Code;
                msg.Message = jsonObj.Message;
                msg.RequestId = jsonObj.RequestId;
                vtMessage.Add(msg);
            }
            return vtMessage;
        }

        /// <summary>
        /// ɾ��һ����Ϣ
        /// </summary>
        /// <param name="receiptHandle"></param>
        public virtual void DeleteMessage(string receiptHandle) {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();

            param["queueName"] = this.queueName;
            param["receiptHandle"] = receiptHandle;

            string result = this.client.Call("DeleteMessage", param);
            Msg.Base jsonObj = JsonConvert.DeserializeObject<Msg.Base>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }
        }

        /// <summary>
        /// ����ɾ����Ϣ
        /// </summary>
        /// <param name="vtReceiptHandle"></param>
        public virtual void BatchDeleteMessage(IList<string> vtReceiptHandle) {
            if (vtReceiptHandle.Count == 0) {
                return;
            }

            SortedDictionary<string, string> param = new SortedDictionary<string, string>();

            param["queueName"] = this.queueName;
            for (int i = 0; i < vtReceiptHandle.Count; i++) {
                string k = "receiptHandle." + Convert.ToString(i + 1);
                param[k] = vtReceiptHandle[i];
            }

            string result = this.client.Call("BatchDeleteMessage", param);
            Msg.BatchDeleteMessage jsonObj = JsonConvert.DeserializeObject<Msg.BatchDeleteMessage>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }
        }

    }

}