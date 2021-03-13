//匯入函式庫
using MongoDB.Bson;
using MongoDB.Driver;
using Lab11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lab11.Controllers
{
    public class MemberController : ApiController
    {
        // [指令1] 「新增」會員資訊
        // POST api/member
        // 使用Route Attributes 指定路由為api/member 且方法為POST
        [Route("api/member")]
        [HttpPost]
        public AddMemberResponse Post(AddMemberRequest request)
        {
            /* 宣告指令的輸出結果 */
            var response = new AddMemberResponse();
            /* Step1 連接MongoDB伺服器 */
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            /* Step2 取得MongoDB資料庫(Database)和集合(Collection) */
            /* Step2-1 取得ntut資料庫(Database) */
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            /* Step2-2 取得members 集合(Collection) */
            var colMembers = db.GetCollection<MembersDocument>("members");

            /* Step3 新增ㄧ筆會員資訊 */
            /* Step3-1 設定查詢式 */
            var query = Builders<MembersDocument>.Filter.Eq(e => e.uid, request.uid);
            /* Step3-2 進行查詢的操作，並取得會員資訊 */
            var doc = colMembers.Find(query).ToListAsync().Result.FirstOrDefault();
            if (doc == null)
            {
                /* Step3-3-1 當資料庫中沒有該會員時，進行新增會員資訊的操作 */
                colMembers.InsertOne(new MembersDocument()
                {
                    _id = ObjectId.GenerateNewId(),
                    uid = request.uid,
                    name = request.name,
                    phone = request.phone
                });
            }
            else
            {
                /* Step3-3-2 當資料庫中存在該會員時，設定Response 的ok 欄位與errMsg 欄
                位 */
                response.ok = false;
                response.errMsg = "編號為" + request.uid + "的會員已存在，請重新輸入別組會員編號。";
            }
            return response;
        }
        // [指令2] 「修改」會員資訊
        // PUT api/member
        // 使用Route Attributes 指定路由為api/member 且方法為PUT
        [Route("api/member")]
        [HttpPut]
        public EditMemberResponse Put(EditMemberRequest request)
        {
            /* 宣告指令的輸出結果 */
            var response = new EditMemberResponse();
            /* Step1 連接MongoDB伺服器 */
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            /* Step2 取得MongoDB資料庫(Database)和集合(Collection) */
            /* Step2-1 取得ntut資料庫(Database) */
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            /* Step2-2 取得members 集合(Collection) */
            var colMembers = db.GetCollection<MembersDocument>("members");
            /* Step3 修改會員資訊 */
            /* Step3-1 設定查詢式 */
            var query = Builders<MembersDocument>.Filter.Eq(e => e.uid, request.uid);
            /* Step3-2 進行查詢的操作，並取得會員資訊 */
            var doc = colMembers.Find(query).ToListAsync().Result.FirstOrDefault();
            if (doc != null)
            {
                /* Step3-3-1 當資料庫中存在該會員時，進行修改會員資訊的操作 */
                var update = Builders<MembersDocument>.Update
                .Set("name", request.name)
                .Set("phone", request.phone);
                colMembers.UpdateOne(query, update);
            }
            else
            {
                /* Step3-3-2 當資料庫中沒有該會員時，設定Response 的ok 欄位與errMsg 欄
                位 */
                response.ok = false;
                response.errMsg = "編號為" + request.uid + "的會員不存在，請確認會員編號。";
            }
            return response;
        }
        // [指令3] 「刪除」會員資訊
        // DELETE api/member/<會員編號>
        // 使用Route Attributes 指定路由為api/member/{id}且方法為DELETE
        [Route("api/member/{id}")]
        [HttpDelete]
        public DeleteMemberResponse Delete(string id)
        {
            /* 宣告指令的輸出結果 */
            var response = new DeleteMemberResponse();
            /* Step1 連接MongoDB伺服器 */
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            /* Step2 取得MongoDB資料庫(Database)和集合(Collection) */
            /* Step2-1 取得ntut資料庫(Database) */
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            /* Step2-2 取得members 集合(Collection) */
            var colMembers = db.GetCollection<MembersDocument>("members");
            /* Step3 刪除會員資訊 */
            /* Step3-1 設定查詢式 */
            var query = Builders<MembersDocument>.Filter.Eq(e => e.uid, id);
            /* Step3-2 進行刪除會員資訊的操作 */
            var result = colMembers.DeleteOne(query);
            if (result.DeletedCount != 0)
            {
                /* Step3-3-1 當刪除會員資訊成功時，直接回傳response */
                return response;
            }
            else
            {
                /* Step3-3-2 當刪除會員資訊失敗時，設定Response的ok 欄位與errMsg欄位 */
                response.ok = false;
                response.errMsg = "編號為" + id + "的會員不存在，請確認會員編號。";
                return response;
            }
        }
        // [指令4] 「取得」全部的會員資訊
        // GET api/member
        // 使用Route Attributes 指定路由為api/member 且方法為Get
        [Route("api/member")]
        [HttpGet]
        public GetMemberListResponse Get()
        {
            /* 宣告指令的輸出結果 */
            var response = new GetMemberListResponse();
            /* Step1 連接MongoDB伺服器 */
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            /* Step2 取得MongoDB資料庫(Database)和集合(Collection) */
            /* Step2-1 取得ntut資料庫(Database) */
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            /* Step2-2 取得members 集合(Collection) */
            var colMembers = db.GetCollection<MembersDocument>("members");
            /* Step3 取得全部會員的資訊 */
            /* Step3-1 設定空的查詢式，即查詢全部的資料 */
            var query = new BsonDocument();
            /* Step3-2 進行查詢的操作，並取得結果集合 */
            var cursor = colMembers.Find(query).ToListAsync().Result;
            /* Step4 設定指令的輸出結果 */
            foreach (var doc in cursor)
            {
                response.list.Add(new MemberInfo()
                {
                    uid = doc.uid,
                    name = doc.name,
                    phone = doc.phone
                });
            }
            return response;
        }
        // [指令5] 「取得」指定的會員資訊
        // GET api/member/<會員編號>
        // 使用Route Attributes 指定路由為api/member/{id}且方法為Get
        [Route("api/member/{id}")]
        [HttpGet]
        public GetMemberResponse Get(string id)
        {
            /* 宣告指令的輸出結果 */
            var response = new GetMemberResponse();
            /* Step1 連接MongoDB伺服器 */
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            /* Step2 取得MongoDB資料庫(Database)和集合(Collection) */
            /* Step2-1 取得ntut資料庫(Database) */
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            /* Step2-2 取得members 集合(Collection) */
            var colMembers = db.GetCollection<MembersDocument>("members");
            /* Step3 取得指定會員的資訊 */
            /* Step3-1 設定查詢式 */
            var query = Builders<MembersDocument>.Filter.Eq(e => e.uid, id);
            /* Step3-2 進行查詢的操作，並取得會員資訊 */
            var doc = colMembers.Find(query).ToListAsync().Result.FirstOrDefault();
            /* Step4 設定指令的輸出結果 */
            if (doc != null)
            {
                /* Step4-1 當資料庫中存在該會員時，設定Response的data欄位 */
                response.data.uid = doc.uid;
                response.data.name = doc.name;
                response.data.phone = doc.phone;
            }
            else
            {
                /* Step4-2 當資料庫中沒有該會員時，設定Response 的ok 欄位與errMsg 欄位 */
                response.ok = false;
                response.errMsg = "沒有此會員";
            }
            return response;
        }
        // [指令6] 狀態檢查
        // GET /
        // 使用Route Attributes 指定路由為/且方法為Get
        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetHealth()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent($"{System.Configuration.ConfigurationManager.AppSettings["EnvInfo"]} Web API is running!");
            return response;
        }
    }
}
