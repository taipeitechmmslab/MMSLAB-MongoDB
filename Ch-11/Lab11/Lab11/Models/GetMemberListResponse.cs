//匯入函式庫
using System.Collections.Generic;

namespace Lab11.Models
{
    public class GetMemberListResponse
    {
        //取得全部會員資訊回應的成功結果ok欄位為布林值
        public bool ok { get; set; }
        //取得全部會員資訊回應的錯誤訊息errMsg欄位為字串
        public string errMsg { get; set; }
        //取得全部會員資訊回應的資料list欄位為MemberInfo類型的串列
        public List<MemberInfo> list { get; set; }
        //初始化後將ok欄位設為true、errMsg欄位設為空字串、list欄位初始化
        public GetMemberListResponse()
        {
            this.ok = true;
            this.errMsg = "";
            this.list = new List<MemberInfo>();
        }
    }
    public class MemberInfo
    {
        //取得的會員資訊的會員編號uid欄位為字串
        public string uid { get; set; }
        //取得的會員資訊的會員姓名name欄位為字串
        public string name { get; set; }
        //取得的會員資訊的會員電話phone欄位為字串
        public string phone { get; set; }
    }
}