namespace Lab7
{
    //定義網路資料回應的結構，並命名為SchoolsResponse
    class SchoolResponse
    {
        public string 學年度 { get; set; }
        public string 學校代號 { get; set; }
        public string 學校名稱 { get; set; }
        public string 教育級別 { get; set; }
        public string 校地總面積 { get; set; }
    }
}