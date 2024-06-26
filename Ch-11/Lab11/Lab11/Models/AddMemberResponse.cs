namespace Lab11.Models
{
    public class AddMemberResponse
    {
        //新增會員資訊回應的成功結果ok欄位為布林值
        public bool ok { get; set; }
        //新增會員資訊回應的錯誤訊息errMsg欄位為字串
        public string errMsg { get; set; }
        //初始化後將ok欄位設為true、errMsg欄位設為空字串
        public AddMemberResponse()
        {
            this.ok = true;
            this.errMsg = "";
        }
    }
}
