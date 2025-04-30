using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using VictorDev.Common;
using Debug = VictorDev.Common.Debug;

namespace VictorDev.Net.WebAPI.TCIT
{
    /// 後端第二代 - 動態生成機房設備Excel報表
    public class WebApiExcelGenerator : SingletonMonoBehaviour<WebApiExcelGenerator>
    {
        private List<string> jsonChunkList;
        private int jsonChunkCounter = 0;


        [Button("ExcelPerpare")]
        private void ExcelPerpare() => ExcelPerpare("AAA");
        
        
        /// 將JSON字串分段批次，傳給Prepare做緩存
        public static void ExcelPerpare(string deviceJsonString, int? chunkSize = null)
        {
            Instance.jsonChunkSize = chunkSize ?? Instance.jsonChunkSize;
            Instance.jsonChunkList = StringHelper.SplitString(deviceJsonString, Instance.jsonChunkSize);
            Instance.jsonChunkCounter = 0;
            Debug.Log($"JonChunkList Count: {Instance.jsonChunkList.Count} / Each ChunkSize: {Instance.jsonChunkSize}",
                Instance, EmojiEnum.DataBox);
            Instance.SendExcelPrepareDataRecursive();
        }
        /// 遞回呼叫Prepare
        private void SendExcelPrepareDataRecursive()
        {
            PrepareBodyRawData prepareBodyRawData = new PrepareBodyRawData()
            {
                batchString = jsonChunkList[jsonChunkCounter],
                index = jsonChunkCounter,
                isFinalBatch = (jsonChunkCounter == jsonChunkList.Count - 1),
            };
            Debug.Log(prepareBodyRawData, this, EmojiEnum.Robot);
            requestExcelExportPrepare.SetRawJsonData(JsonConvert.SerializeObject(prepareBodyRawData, Formatting.Indented));
            WebAPI_Caller.CallWebAPI(requestExcelExportPrepare, OnPrepareSuccessHandler);

            void OnPrepareSuccessHandler(long responseCode, Dictionary<string, string> arg2)
            {
                Debug.Log($"onPrepareSuccessHandler[{responseCode}]", this, EmojiEnum.Done);
                if (++jsonChunkCounter < jsonChunkList.Count) SendExcelPrepareDataRecursive();
                else ExcelStart();
            }
        }

        /// 開始生成Excel報表
        [Button("ExcelStart")]
        private void ExcelStart()
        {
            Debug.Log("ExcelStart", this, EmojiEnum.Robot);
            WebAPI_Caller.CallWebAPI(requestExcelExportStart, OnStartSuccessHandler);

            void OnStartSuccessHandler(long responseCode, Dictionary<string, string> arg2)
            {
                Debug.Log($"OnStartSuccessHandler[{responseCode}]", this, EmojiEnum.Done);
                ExcelExport();
            }
        }

        /// 下載Excel報表
        [Button("ExcelExport")]
        public void ExcelExport()
        {
            Debug.Log("ExcelExport", this, EmojiEnum.Robot);
            WebAPI_Caller.CallWebAPI(requestExcelExport, OnExportSuccessHandler);

            void OnExportSuccessHandler(long responseCode, Dictionary<string, string> arg2)
            {
                Debug.Log($"OnExportSuccessHandler[{responseCode}]", this, EmojiEnum.Done);
            }
        }

        #region Variables

        [Header("[設定] - JSON分割大小")] [SerializeField]
        private int jsonChunkSize = 500000;

        [Foldout("[WebAPI Request]")] [SerializeField]
        private WebAPI_Request requestExcelExportPrepare;

        [Foldout("[WebAPI Request]")] [SerializeField]
        private WebAPI_Request requestExcelExportStart;

        [Foldout("[WebAPI Request]")] [SerializeField]
        private WebAPI_Request requestExcelExport;

        #endregion

        /// 呼叫Prepare的Body Raw格式
        [SerializeField]
        public struct PrepareBodyRawData
        {
            /// 批次JSON字串
            public string batchString;
            /// 第幾批(從0起算)
            public int index;
            /// 最後一批要為true
            public bool isFinalBatch;
            public override string ToString()
                => $"SendJsonChunkToPrepare[{index}]:\n{batchString}\nisFinalBatch: {isFinalBatch}";
        }
    }
}