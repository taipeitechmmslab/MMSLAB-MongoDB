using MongoDB.Bson;

namespace Lab11.Models
{
    public class MembersDocument
    {
        /// <summary>
        /// 系統自動產生的唯一識別欄位
        /// </summary>
        public ObjectId _id { get; set; }

        /// <summary>
        /// 會員編號
        /// </summary>
        public string uid { get; set; }

        /// <summary>
        /// 會員姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 會員電話
        /// </summary>
        public string phone { get; set; }

    }
}
