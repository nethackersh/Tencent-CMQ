using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
namespace TencentCloud.CMQ {
    /// <summary>
    /// CMQ -Account 
    /// </summary>
    public class Account {
        protected internal CMQClient client;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="secretId">密钥Id</param>
        /// <param name="secretKey">密钥Key</param>
        /// <param name="endpoint">请求域名，例如：北京访问点->http://cmq-queue-bj.api.tencentyun.com</param>
        /// <param name="path">例如：/v2/index.php</param>
        /// <param name="method">POST/GET,默认为POST</param>
        public Account(string secretId, string secretKey, string endpoint = "http://cmq-queue-bj.api.tencentyun.com", string path = "/v2/index.php", string method = "POST") {
            this.client = new CMQClient(endpoint, path, secretId, secretKey, method);
        }

        public virtual string SignMethod {
            set {
                this.client.SignMethod = value;
            }
        }
        /// <summary>
        /// 创建一个队列
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="meta">        QueueMeta class object </param>
        /// <exception cref="Exception"> </exception>
        /// <exception cref="CMQClientException"> </exception>
        /// <exception cref="CMQServerException"> </exception>
        public virtual void CreateQueue(string queueName, QueueMeta meta) {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (queueName.Equals("")) {
                throw new CMQClientException("Invalid parameter:queueName is empty");
            } else {
                param["queueName"] = queueName;
            }

            if (meta.MaxMsgHeapNum > 0) {
                param["maxMsgHeapNum"] = Convert.ToString(meta.MaxMsgHeapNum);
            }
            if (meta.PollingWaitSeconds > 0) {
                param["pollingWaitSeconds"] = Convert.ToString(meta.PollingWaitSeconds);
            }
            if (meta.VisibilityTimeout > 0) {
                param["visibilityTimeout"] = Convert.ToString(meta.VisibilityTimeout);
            }
            if (meta.MaxMsgSize > 0) {
                param["maxMsgSize"] = Convert.ToString(meta.MaxMsgSize);
            }
            if (meta.MsgRetentionSeconds > 0) {
                param["msgRetentionSeconds"] = Convert.ToString(meta.MsgRetentionSeconds);
            }
            if (meta.RewindSeconds > 0) {
                param["rewindSeconds"] = Convert.ToString(meta.RewindSeconds);
            }

            string result = this.client.Call("CreateQueue", param);
            Msg.CreateQueue jsonObj = JsonConvert.DeserializeObject<Msg.CreateQueue>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }
        }

        /// <summary>
        /// 删除一个队列
        /// </summary>
        /// <param name="queueName">   String queue name </param>
        /// <exception cref="CMQClientException"> </exception>
        /// <exception cref="CMQServerException"> </exception>
        public virtual void DeleteQueue(string queueName) {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (queueName.Equals("")) {
                throw new CMQClientException("Invalid parameter:queueName is empty");
            } else {
                param["queueName"] = queueName;
            }
            string result = this.client.Call("DeleteQueue", param);
            Msg.Base jsonObj = JsonConvert.DeserializeObject<Msg.Base>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }
        }

        /// <summary>
        ///  列出队列
        /// </summary>
        /// <param name="searchWord"> String </param>
        /// <param name="offset">     int </param>
        /// <param name="limit">      int </param>
        /// <param name="queueList">  List<String> </param>
        /// <returns> totalCount int </returns>
        /// <exception cref="Exception"> </exception>
        /// <exception cref="CMQClientException"> </exception>
        /// <exception cref="CMQServerException"> </exception>
        public virtual int ListQueue(string searchWord, int offset, int limit, List<Msg.Queue> queueList) {
            SortedDictionary<string, string> param = new SortedDictionary<string, string>();
            if (!searchWord.Equals("")) {
                param["searchWord"] = searchWord;
            }
            if (offset >= 0) {
                param["offset"] = Convert.ToString(offset);
            }
            if (limit > 0) {
                param["limit"] = Convert.ToString(limit);
            }

            string result = this.client.Call("ListQueue", param);
            Msg.ListQueue jsonObj = JsonConvert.DeserializeObject<Msg.ListQueue>(result);
            if (jsonObj.Code != 0) {
                throw new CMQServerException(jsonObj.Code, jsonObj.Message, jsonObj.RequestId);
            }
            if (queueList == null) { queueList = new List<Msg.Queue>(); }
            queueList.Clear();
            foreach (var model in jsonObj.QueueList) {
                queueList.Add(model);
            }
            return jsonObj.TotalCount;
        }

        /// <summary>
        /// 获取一个队列 
        /// </summary>
        /// <param name="queueName">  String </param>
        /// <returns> Queue object
        ///  </returns>
        public virtual Queue GetQueue(string queueName) {
            return new Queue(queueName, this.client);
        }
    }

}