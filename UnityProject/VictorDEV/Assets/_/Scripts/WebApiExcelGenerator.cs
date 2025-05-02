using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using VictorDev.Common;
using Debug = UnityEngine.Debug;

namespace VictorDev.Net.WebAPI.TCIT
{
    public class WebApiExcelGenerator : SingletonMonoBehaviour<WebApiExcelGenerator>
    {
       
        public static void ExcelPerpare(string deviceJsonString, int? chunkSize = null)
        {
            Instance.jsonChunkSize = chunkSize ?? Instance.jsonChunkSize;
            
        }
        [Button("ExcelStart")]
        public  void ExcelStart()
        {
            WebAPI_Caller.CallWebAPI(requestExcelExportStart, onSuccessHandler);
            Debug.Log("ExcelStart");
        }

        [Button("ExcelExport")]
        public void ExcelExport()
        {
            WebAPI_Caller.CallWebAPI(requestExcelExport, onSuccessHandler);
            Debug.Log("ExcelExport");
        }

        private void onSuccessHandler(long arg1, Dictionary<string, string> arg2)
        {
            Debug.Log("dfasdf");
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
    }
}