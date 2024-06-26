//匯入函式庫
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

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
            var colAccounts = db.GetCollection<AccountsDocument>("accounts");
            //Step4:使用Builders建立後續會使用到的運算子
            var builderAccountsFilter = Builders<AccountsDocument>.Filter;
            var builderAccountsUpdate = Builders<AccountsDocument>.Update;
            //Step5:顯示執行範例的控制介面
            controlPanel();
            #region 控制介面
            void controlPanel()
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine("1.開戶");
                Console.WriteLine("2.存款");
                Console.WriteLine("3.提款");
                Console.WriteLine("4.銷戶");
                Console.WriteLine("5.查詢存款");
                Console.WriteLine("\n請輸入編號1~5，選擇要執行的功能");
                try
                {
                    var num = int.Parse(Console.ReadLine()); //取得輸入的編號
                    Console.Clear(); //清除Console顯示的內容
                                     //使用switch判斷編號，選擇要執行的範例
                    switch (num)
                    {
                        case 1:
                            open();
                            break;
                        case 2:
                            deposit();
                            break;
                        case 3:
                            withdrawal();
                            break;
                        case 4:
                            eliminate();
                            break;
                        case 5:
                            query();
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
                    controlPanel(); //結束後再次執行controlPanel()方法
                }
            }
            #endregion
            #region 1.開戶
            void open()
            {
                Console.WriteLine("1.開戶\n");
                Console.WriteLine("請輸入帳戶編號");
                var id = Console.ReadLine(); //取得輸入的帳戶編號

                Console.WriteLine("請輸入帳戶持有人");
                var name = Console.ReadLine(); //取得輸入的帳戶持有人

                Console.WriteLine("請輸入帳戶類型");
                Console.WriteLine("1.台幣帳戶");
                Console.WriteLine("2.美金帳戶");
                Console.WriteLine("3.台幣與美金帳戶");
                var num = int.Parse(Console.ReadLine()); //取得輸入的帳戶類型

                AccountsDocument account = null;
                switch (num)
                {
                    case 1:
                        Console.WriteLine("請輸入存入金額");

                        account = new AccountsDocument
                        {
                            _id = id,
                            name = name,
                            currency = new[]
                            {
                                new AccountsDocument.Currency
                                {
                                    type = "TWD",
                                    cash = Double.Parse(Console.ReadLine()), //取得輸入的存入金額
                                    lastModified = DateTime.UtcNow //現在的UTC時間
                                }
                            }
                        };
                        break;
                    case 2:
                        Console.WriteLine("請輸入存入金額");

                        account = new AccountsDocument
                        {
                            _id = id,
                            name = name,
                            currency = new[]
                            {
                                new AccountsDocument.Currency
                                {
                                    type = "USD",
                                    cash = double.Parse(Console.ReadLine()), //取得輸入的存入金額
                                    lastModified = DateTime.UtcNow //現在的UTC時間
                                }
                            }
                        };
                        break;
                    case 3:
                        Console.WriteLine("請輸入存入的台幣金額");
                        var twd = double.Parse(Console.ReadLine()); //取得輸入的台幣金額
                        Console.WriteLine("請輸入存入的美金金額");
                        var usd = double.Parse(Console.ReadLine()); //取得輸入的美金金額

                        account = new AccountsDocument
                        {
                            _id = id,
                            name = name,
                            currency = new[]
                            {
                                new AccountsDocument.Currency
                                {
                                    type = "TWD",
                                    cash = twd,
                                    lastModified = DateTime.UtcNow //現在的UTC時間
                                },
                                new AccountsDocument.Currency
                                {
                                    type = "USD",
                                    cash = usd,
                                    lastModified = DateTime.UtcNow //現在的UTC時間
                                }
                            }
                        };
                        break;
                    default:
                        Console.WriteLine("輸入錯誤，開戶失敗");
                        return;
                }

                try
                {
                    colAccounts.InsertOne(account); //新增AccountsDocument至資料庫
                }
                catch (Exception e)
                {
                    Console.WriteLine("帳戶編號已存在");
                    return;
                }
                Console.WriteLine("開戶成功");
            }
            #endregion
            #region 2.存款
            void deposit()
            {
                Console.WriteLine("2.存款\n");
                Console.WriteLine("請輸入帳戶編號");
                var id = Console.ReadLine(); //取得輸入的帳戶編號

                Console.WriteLine("請輸入帳戶類型");
                Console.WriteLine("1.台幣帳戶");
                Console.WriteLine("2.美金帳戶");
                var num = int.Parse(Console.ReadLine()); //取得輸入的帳戶類型
                var type = "";
                switch (num)
                {
                    case 1:
                        type = "TWD";
                        break;
                    case 2:
                        type = "USD";
                        break;
                    default:
                        Console.WriteLine("輸入錯誤，存款失敗");
                        return;
                }

                Console.WriteLine("請輸入存入金額");
                var cash = double.Parse(Console.ReadLine()); //取得輸入的存入金額
                if (cash <= 0)
                {
                    Console.WriteLine("輸入金額小於等於0，存款失敗");
                    return;
                }

                //建立查詢條件為id欄位等於帳戶編號
                var idFilter = builderAccountsFilter.Eq(e => e._id, id);
                //建立查詢條件為currency欄位的type欄位為帳戶類型
                var typeFilter = builderAccountsFilter.ElemMatch(e => e.currency, e => e.type == type);
                //建立查詢條件為符合上述所有條件
                var filter = builderAccountsFilter.And(idFilter, typeFilter);
                /* 建立更新方式為currency欄位的cash欄位增加存入金額、
                * currency欄位的lastModified欄位改為現在時間 */
                var update = builderAccountsUpdate
                    .Inc(e => e.currency[num - 1].cash, cash)
                    .CurrentDate(e => e.currency[num - 1].lastModified);
                //進行條件篩選並更新
                var result = colAccounts.UpdateMany(filter, update);
                //依據更新數量判斷是否有成功更新，並顯示結果
                var msg = result.ModifiedCount != 0 ? "存款成功" : "查無帳號";
                Console.WriteLine(msg);
            }
            #endregion
            #region 3.提款
            void withdrawal()
            {
                Console.WriteLine("3.提款\n");
                Console.WriteLine("請輸入帳戶編號");
                var id = Console.ReadLine(); //取得輸入的帳戶編號

                Console.WriteLine("請輸入帳戶類型");
                Console.WriteLine("1.台幣帳戶");
                Console.WriteLine("2.美金帳戶");
                var num = int.Parse(Console.ReadLine()); //取得輸入的帳戶類型
                var type = "";
                switch (num)
                {
                    case 1:
                        type = "TWD";
                        break;
                    case 2:
                        type = "USD";
                        break;
                    default:
                        Console.WriteLine("輸入錯誤，提款失敗");
                        return;
                }

                Console.WriteLine("請輸入提領金額");
                var cash = double.Parse(Console.ReadLine()); //取得輸入的提領金額
                if (cash <= 0)
                {
                    Console.WriteLine("輸入金額小於等於0，提款失敗");
                    return;
                }

                //建立查詢條件為id欄位等於帳戶編號
                var idFilter = builderAccountsFilter.Eq(e => e._id, id);
                //建立查詢條件為currency欄位的type欄位為帳戶類型，且cash欄位大於等於提領金額
                var typeFilter = builderAccountsFilter.ElemMatch(
                    e => e.currency, 
                    e => e.type == type && e.cash >= cash
                );
                //建立查詢條件為符合上述所有條件
                var filter = builderAccountsFilter.And(idFilter, typeFilter);
                /* 建立更新方式為currency欄位的cash欄位減少提領金額、
                * currency欄位的lastModified欄位改為現在時間 */
                var update = builderAccountsUpdate
                    .Inc(e => e.currency[num - 1].cash, -cash)
                    .CurrentDate(e => e.currency[num - 1].lastModified);
                //進行條件篩選並更新
                var result = colAccounts.UpdateMany(filter, update);
                //依據更新數量判斷是否有成功更新，並顯示結果
                var msg = result.ModifiedCount != 0 ? "提款成功" : "查無帳號或餘額不足";
                Console.WriteLine(msg);
            }
            #endregion
            #region 4.銷戶
            void eliminate()
            {
                Console.WriteLine("4.銷戶\n");
                Console.WriteLine("請輸入帳戶編號");
                var id = Console.ReadLine(); //取得輸入的帳戶編號

                Console.WriteLine("請輸入帳戶類型");
                Console.WriteLine("1.台幣帳戶");
                Console.WriteLine("2.美金帳戶");
                var num = int.Parse(Console.ReadLine()); //取得輸入的帳戶類型
                var type = "";
                switch (num)
                {
                    case 1:
                        type = "TWD";
                        break;
                    case 2:
                        type = "USD";
                        break;
                    default:
                        Console.WriteLine("輸入錯誤，銷戶失敗");
                        return;
                }

                //建立查詢條件為id欄位等於帳戶編號
                var idFilter = builderAccountsFilter.Eq(e => e._id, id);
                //建立查詢條件為currency欄位的type欄位為帳戶類型
                var typeFilter = builderAccountsFilter.ElemMatch(e => e.currency, e => e.type == type);
                //建立查詢條件為符合上述所有條件
                var filter = builderAccountsFilter.And(idFilter, typeFilter);
                /* 建立更新方式為currency欄位的cash欄位減少提領金額、
                * currency欄位的lastModified欄位改為現在時間 */
                var update = builderAccountsUpdate
                    .PullFilter(e => e.currency, e => e.type == type);
                //進行條件篩選並更新
                var result = colAccounts.UpdateMany(filter, update);
                //依據更新數量判斷是否有成功更新，並顯示結果
                var msg = result.ModifiedCount != 0 ? "銷戶成功" : "查無帳號";
                Console.WriteLine(msg);
            }
            #endregion
            #region 5.查詢存款
            void query()
            {
                Console.WriteLine("5.查詢存款\n");
                Console.WriteLine("請輸入帳戶編號");
                var id = Console.ReadLine(); //取得輸入的帳戶編號

                //建立查詢條件為id欄位為帳戶編號
                var filter = builderAccountsFilter.Eq(e => e._id, id);
                //進行查詢並取得結果
                var result = colAccounts.Find(filter).ToListAsync().Result;
                //判斷有無結果
                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查詢結果");
                    //使用foreach遍歷查詢結果，將帳戶顯示於Console
                    foreach (AccountsDocument accounts in result)
                    {
                        foreach (AccountsDocument.Currency currency in accounts.currency)
                        {
                            Console.WriteLine($"幣別:{currency.type}, 餘額:{currency.cash}");
                        }
                    }
                }
            }
            #endregion
        }
    }
}
