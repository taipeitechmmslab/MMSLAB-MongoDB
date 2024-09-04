//匯入函式庫
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Lab7
{
    class Program
    {
        static void Main(string[] args)
        {
            //Step1:連接MongoDB伺服器
            var client = new MongoClient("mongodb://localhost:27017");
            //Step2:取得MongoDB中，名為ntut的資料庫及名為accounts的集合
            var db = client.GetDatabase("ntut") as MongoDatabaseBase;
            //Step3:使用db.GetCollection取得後續會使用到的集合
            var colSchools = db.GetCollection<SchoolsDocument>("schools");
            //Step4:使用Builders建立後續會使用到的運算子
            var builderAccountsFilter = Builders<SchoolsDocument>.Filter;
            var builderAccountsUpdate = Builders<SchoolsDocument>.Update;
            //Step5:顯示執行範例的控制介面
            controlPanel();
            #region 控制介面
            void controlPanel()
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine("1.抓取網路資料");
                Console.WriteLine("2.查詢目前資料數量");
                Console.WriteLine("3.查詢校地面積前10大的學校資料");
                Console.WriteLine("4.查詢校地面積指定區間的學校資料");
                Console.WriteLine("\n請輸入編號1~4，選擇要執行的功能");
                try
                {
                    var num = int.Parse(Console.ReadLine()); //取得輸入的編號
                    Console.Clear(); //清除Console顯示的內容
                                     //使用switch判斷編號，選擇要執行的範例
                    switch (num)
                    {
                        case 1:
                            update();
                            break;
                        case 2:
                            count();
                            break;
                        case 3:
                            search_1();
                            break;
                        case 4:
                            search_2();
                            break;
                        default:
                            Console.WriteLine("\n請輸入正確內容"); //輸入錯誤的提示
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e); //輸入錯誤的提示
                }
                finally
                {
                    Console.WriteLine("\n\n");
                    controlPanel(); //結束後再次執行controlPanel()方法
                }
            }
            #endregion
            #region 1.抓取網路資料
            void update()
            {
                Console.WriteLine("1.抓取網路資料\n");
                //定義要請求的 URL
                string url = "https://stats.moe.gov.tw/files/others/opendata/112area.json";
                //建立 HttpClient
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        //發送 GET 請求
                        var response = client.GetAsync(url).Result;
                        //確認請求成功
                        response.EnsureSuccessStatusCode();
                        //讀取回應內容
                        var responseBody = response.Content.ReadAsStringAsync().Result;
                        //將 JSON 轉換成物件
                        var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SchoolResponse>>(responseBody);
                        //將回應結果轉換成Document物件
                        var schoolsData = responseData.Select(e =>
                        {
                            var a = int.Parse(e.學年度);
                            var output = new SchoolsDocument
                            {
                                schoolYear = int.Parse(e.學年度),
                                schoolCode = e.學校代號,
                                schoolName = e.學校名稱,
                                educationLevel = e.教育級別,
                                campusArea = int.Parse(e.校地總面積)
                            };
                            return output;
                        });
                        //刪除所有文件
                        colSchools.DeleteMany(builderAccountsFilter.Empty);
                        //插入多個文件
                        colSchools.InsertMany(schoolsData);
                        //輸出結果
                        Console.WriteLine($"已匯入共 {responseData.Count} 間學校資料");
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine($"Request error: {e.Message}");
                    }
                }
            }
            #endregion
            #region 2.查詢目前資料數量
            void count()
            {
                Console.WriteLine("2.查詢目前資料數量\n");
                // 使用集合的 CountDocuments 方法來計算集合中的文件數量
                var count = colSchools.CountDocuments(builderAccountsFilter.Empty);
                // 輸出目前資料數量
                Console.WriteLine($"目前資料數量: {count}");
            }
            #endregion
            #region 3.查詢校地面積前10大的學校資料
            void search_1()
            {
                Console.WriteLine("3.查詢校地面積前10大的學校資料\n");
                // 使用集合的 Find 方法來查詢所有文件，並使用 Sort 方法根據校地面積進行降序排序，最後使用 Limit 方法限制結果數量為 10
                var top10Schools = colSchools.Find(builderAccountsFilter.Empty)
                                             .Sort(Builders<SchoolsDocument>.Sort.Descending(s => s.campusArea))
                                             .Limit(10)
                                             .ToList();
                // 輸出查詢結果
                Console.WriteLine("校地面積前10大的學校資料:");
                foreach (var school in top10Schools)
                {
                    Console.WriteLine($"學校名稱: {school.schoolName}, 校地面積: {school.campusArea}");
                }
            }
            #endregion
            #region 4.查詢校地面積指定區間的學校資料
            void search_2()
            {
                Console.WriteLine("4.查詢校地面積指定區間的學校資料\n");
                // 提示使用者輸入校地面積的最小值
                Console.WriteLine("請輸入校地面積的最小值:");
                var minArea = int.Parse(Console.ReadLine());
                // 提示使用者輸入校地面積的最大值
                Console.WriteLine("請輸入校地面積的最大值:");
                var maxArea = int.Parse(Console.ReadLine());

                // 使用集合的 Find 方法來查詢校地面積在指定區間內的文件
                var schoolsInRange = colSchools.Find(builderAccountsFilter.And(
                                                        builderAccountsFilter.Gte(s => s.campusArea, minArea),
                                                        builderAccountsFilter.Lte(s => s.campusArea, maxArea)))
                                               .ToList();
                // 輸出查詢結果
                Console.WriteLine($"校地面積在 {minArea} 到 {maxArea} 之間的學校資料:");
                foreach (var school in schoolsInRange)
                {
                    Console.WriteLine($"學校名稱: {school.schoolName}, 校地面積: {school.campusArea}");
                }
            }
            #endregion
        }
    }
}
