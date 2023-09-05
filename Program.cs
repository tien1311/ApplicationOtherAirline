using ApplicationOtherAirline.Helpers;
using Dapper;
using EasyInvoice.Client;
using EasyInvoice.Client.Services;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationOtherAirline
{
    internal class Program
    {
        private static string Server = "Data Source=42.117.7.18,1344;Initial Catalog=INHOPDONG;User ID=sa;Password=EnViet@123;";
        private static string Token = "";
        private static List<string> List_Token = new List<string>();

        static async Task Main(string[] args)
        {
            try
            {
                var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
                if (exitCode == 0)
                {
                    Console.WriteLine("Playwright installed");
                }

                // Tạo tiến trình riêng cho hàm 1
                Thread t1 = new Thread(() =>
                {
                    while (true)
                    {
                        // Thực hiện công việc cho hàm 1 ở đây
                        bool result = SaveDataAirlineOther("Auto Bot");
                        if (result == true)
                        {
                            Console.WriteLine("Get data airline other success " + DateTime.Now);
                        }
                        else
                        {
                            Console.WriteLine("Get data airline other fail " + DateTime.Now);
                        }

                        Thread.Sleep(TimeSpan.FromMinutes(5)); // Thời gian chờ 5 phút
                    }
                });
                t1.Start();

                // Tạo tiến trình riêng cho hàm 2
                Thread t2 = new Thread(() =>
                {
                    while (true)
                    {
                        // Thực hiện công việc cho hàm 2 ở đây
                        List<ChiTietCongNoNhanVien> chiTietCongNoNhanViens = new List<ChiTietCongNoNhanVien>();
                        chiTietCongNoNhanViens = CongNoAllNhanVien();
                        bool result = SaveCongNo(chiTietCongNoNhanViens);
                        if (result == true)
                        {
                            Console.WriteLine("Get data debt success " + DateTime.Now);
                        }
                        else
                        {
                            Console.WriteLine("Get data debt fail " + DateTime.Now);
                        }
                        Thread.Sleep(TimeSpan.FromHours(3)); // 6 Tiếng
                    }
                });
                t2.Start();
                // Tạo tiến trình riêng cho hàm 2
                Thread t3 = new Thread(() =>
                {
                    while (true)
                    {
                        // Thực hiện công việc cho hàm 3 ở đây
                        bool result = SaveDataVNA("Auto Bot");
                        if (result == true)
                        {
                            Console.WriteLine("Get data VNA success " + DateTime.Now);
                        }
                        else
                        {
                            Console.WriteLine("Get data VNA fail " + DateTime.Now);
                        }
                        Thread.Sleep(TimeSpan.FromHours(2)); // 3 Tiếng
                    }
                });
                t3.Start();
                // Tạo tiến trình riêng cho hàm 4
                Thread t4 = new Thread(() =>
                {
                    while (true)
                    {
                        // Thực hiện công việc cho hàm 3 ở đây
                        bool result = UpdateInvoiceNumber();
                        if (result == true)
                        {
                            Console.WriteLine("Update invoice success " + DateTime.Now);
                        }
                        else
                        {
                            Console.WriteLine("Update invoice fail " + DateTime.Now);
                        }
                        Thread.Sleep(TimeSpan.FromDays(1)); // 1 ngày
                    }
                });
                t4.Start();
                //Tạo tiến trình riêng cho hàm 5
                Thread t5 = new Thread(() =>
                {
                    while (true)
                    {
                        // Thực hiện công việc cho hàm 1 ở đây
                        bool result = SaveOtherAirlineInternational("Auto Bot");
                        if (result == true)
                        {
                            Console.WriteLine("Get data airline International success " + DateTime.Now);
                        }
                        else
                        {
                            Console.WriteLine("Get data airline International fail " + DateTime.Now);
                        }

                        Thread.Sleep(TimeSpan.FromHours(2)); // Thời gian chờ 5 phút
                    }
                });
                t5.Start();

                // Chờ cho các tiến trình kết thúc (ở đây ta không cần sử dụng WaitForExit)
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error call api");
                throw;
            }


        }

        //Get token bsp api
        public class AuthenticateModel
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string client_id { get; set; }
            public string username { get; set; }
        }
        public class RequestAirlineSystem
        {
            public string AirlineSystem { get; set; }
        }
        public class RequestAirlineSystem_MT
        {
            public string AirlineSystem { get; set; }
            public string BookingRegion { get; set; } = "";
        }
        public class ResponseRetrieveAgencyCredit
        {
            public decimal AgencyCreditAmount { get; set; }
            public string Result { get; set; }

            public string[] Message { get; set; }

        }
        //Get token BamBoo MT api
        public static async Task<string> Authenticate_MT()
        {
            try
            {
                string endPoint = "http://api2.bsp.onlineairticket.vn/token";
                var client = new HttpClient();

                var data = new[]
                {
                    new KeyValuePair<string, string>("username", "2110212"),
                    new KeyValuePair<string, string>("password", "Enviet@2345@"),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", "api"),
                 };
                var response = await client.PostAsync(endPoint, new FormUrlEncodedContent(data));
                var responseContent = await response.Content.ReadAsStringAsync();

                AuthenticateModel result = JsonConvert.DeserializeObject<AuthenticateModel>(responseContent);
                return result.access_token;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static async Task<string> Authenticate()
        {
            try
            {
                string endPoint = "http://api.bsp.onlineairticket.vn/token";
                var client = new HttpClient();

                var data = new[]
                {
                    new KeyValuePair<string, string>("username", "2110212"),
                    new KeyValuePair<string, string>("password", "Enviet@2345@"),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", "api"),
                 };
                var response = await client.PostAsync(endPoint, new FormUrlEncodedContent(data));
                var responseContent = await response.Content.ReadAsStringAsync();

                AuthenticateModel result = JsonConvert.DeserializeObject<AuthenticateModel>(responseContent);
                return result.access_token;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public static async Task<ResponseRetrieveAgencyCredit> RetrieveAgencyCredit(string airlineSystem, string Airline)
        {
            RequestAirlineSystem request = new RequestAirlineSystem();
            RequestAirlineSystem_MT request_MT = new RequestAirlineSystem_MT();
            string accessToken = "";
            string strUrl = "";
            string jsonContent = "";
            if (Airline == "BAMBOO_MT")
            {
                strUrl = String.Format("http://api2.bsp.onlineairticket.vn/api/Category/RetrieveAgencyCredit");
                request_MT.AirlineSystem = airlineSystem;
                request_MT.BookingRegion = "CENTRAL";
                jsonContent = JsonConvert.SerializeObject(request_MT);
                accessToken = Authenticate_MT().Result;
            }
            else
            {
                strUrl = String.Format("http://api.bsp.onlineairticket.vn/api/Category/RetrieveAgencyCredit");
                request.AirlineSystem = airlineSystem;
                jsonContent = JsonConvert.SerializeObject(request);
                accessToken = Authenticate().Result;
            }
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(strUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            ResponseRetrieveAgencyCredit result = JsonConvert.DeserializeObject<ResponseRetrieveAgencyCredit>(responseContent);
            return result;
        }

        public static bool SaveDataAirlineOther(string NguoiLap)
        {
            try
            {
                bool result = true;
                string sql_soduhang = "";
                Task<ResponseRetrieveAgencyCredit> VietTravel = RetrieveAgencyCredit("VIETRAVEL", "VIETRAVEL");
                Task<ResponseRetrieveAgencyCredit> BamBoo = RetrieveAgencyCredit("BAMBOO", "BAMBOO");
                Task<ResponseRetrieveAgencyCredit> Vietjet = RetrieveAgencyCredit("VIETJET", "VIETJET");
                Task<ResponseRetrieveAgencyCredit> BamBoo_MT = RetrieveAgencyCredit("BAMBOO", "BAMBOO_MT");
                if (VietTravel.Result.Result != "true")
                {
                    result = false;
                    return result;
                }
                if (BamBoo.Result.Result != "true")
                {
                    result = false;
                    return result;
                }
                if (Vietjet.Result.Result != "true")
                {
                    result = false;
                    return result;
                }
                if (BamBoo_MT.Result.Result != "true")
                {
                    result = false;
                    return result;
                }
                sql_soduhang = "SP_INSERT_SODUHANG";
                int ID = 0;
                using (var conn = new SqlConnection(Server))
                {
                    var param = new
                    {
                        NguoiLap = NguoiLap,
                        Status = 1
                    };
                    ID = conn.QueryFirst<int>(sql_soduhang, param, null, commandTimeout: 30, commandType: System.Data.CommandType.StoredProcedure);
                }
                if (ID > 0)
                {
                    int result_chitiet = 0;
                    string sql_chitiet = "";
                    string VIETTRAVEL = "VIETRAVEL AIRLINES (37100047)";
                    string BAMBOO_MN = "BAMBOO AIRWAYS MN (3780285)";
                    string BAMBOO_MT = "BAMBOO AIRWAYS MT (3760345)";
                    string VIETJET = "VIETJET AIR (37384686)";
                    sql_chitiet = "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'" + BAMBOO_MN + "'," + BamBoo.Result.AgencyCreditAmount + ")";
                    sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'" + BAMBOO_MT + "'," + BamBoo_MT.Result.AgencyCreditAmount + ")";
                    sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'" + VIETJET + "'," + Vietjet.Result.AgencyCreditAmount + ")";
                    sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'" + VIETTRAVEL + "'," + VietTravel.Result.AgencyCreditAmount + ")";

                    using (var conn = new SqlConnection(Server))
                    {
                        result_chitiet = conn.Execute(sql_chitiet, null, null, commandTimeout: 30, commandType: System.Data.CommandType.Text);
                    }
                    if (result_chitiet > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static decimal SoDuCuoiNgayDapper(string maKH)
        {
            decimal result = 0;
            string sql = "";
            try
            {
                string server_KT = "Data Source=ketoan.enviet-group.com,8134; Initial Catalog = DATATEMP_VE; User ID = ELV_TEMP; Password = tkt@123$456;";
                sql = $@"SELECT top 1 isnull(SoDu,0) as sodu  FROM [DATATEMP_VE].[dbo].[_DUCUOI_NEW] WITH (NOLOCK) where  ID_Khachhang = '" + maKH + "'";
                using (var conn = new SqlConnection(server_KT))
                {
                    result = conn.QueryFirst<decimal>(sql, null, null, commandType: System.Data.CommandType.Text, commandTimeout: 30);
                    conn.Dispose();
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public class ChiTietCongNoNhanVien
        {
            public string MaNV { get; set; }
            public decimal SoTienNo { get; set; }
            public string TenNV { get; set; }
        }
        public static List<ChiTietCongNoNhanVien> CongNoAllNhanVien()
        {
            List<ChiTietCongNoNhanVien> result = new List<ChiTietCongNoNhanVien>();
            try
            {
                DateTime s = DateTime.Now;
                string server_EV = "Data Source=42.117.7.18,1344;Initial Catalog=INHOPDONG;User ID=sa;Password=EnViet@123";
                string sql_All_NV = "select yahoo as MaNV, 0 as SoTienNo, Ten as TENNV  from DM_NV where isnull(TinhTrang,0) = 1";
                using (var conn = new SqlConnection(server_EV))
                {
                    result = (List<ChiTietCongNoNhanVien>)conn.Query<ChiTietCongNoNhanVien>(sql_All_NV, null, commandType: System.Data.CommandType.Text, commandTimeout: 30);
                    conn.Dispose();
                }
                if (result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        decimal SoTienNo = -1 * SoDuCuoiNgayDapper(result[i].MaNV);
                        result[i].SoTienNo = SoTienNo;
                    }

                }
                DateTime e = DateTime.Now;
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public static int GetNumberCode()
        {
            int result = 1;
            try
            {
                string server_EV2 = "Data Source=42.117.7.18,1344;Initial Catalog=INHOPDONG_V2;User ID=sa;Password=EnViet@123";
                string sql = "select top 1 NUMBERCODE from CONGNO_NHANVIEN order by ID desc";
                using (var conn = new SqlConnection(server_EV2))
                {
                    result = conn.QueryFirst<int>(sql, null, commandType: System.Data.CommandType.Text, commandTimeout: 30);
                }
                if (result > 0)
                {
                    result++;
                }
            }
            catch (Exception)
            {
                return result;
            }


            return result;
        }
        public static bool SaveCongNo(List<ChiTietCongNoNhanVien> ListCongNo)
        {
            int result_Insert = 0;
            bool result = false;
            try
            {
                string server_EV2 = "Data Source=42.117.7.18,1344;Initial Catalog=INHOPDONG_V2;User ID=sa;Password=EnViet@123";

                int count = 0;
                string sql_Insert = "";
                int NumberCode = GetNumberCode();
                for (int i = 0; i < ListCongNo.Count; i++)
                {
                    sql_Insert += "Insert into CONGNO_NHANVIEN(MANV, SOTIENNO, TENNV, CREATEDATE, NUMBERCODE) VALUES('" + ListCongNo[i].MaNV + "'," + ListCongNo[i].SoTienNo + ",N'" + ListCongNo[i].TenNV + "',GETDATE(), " + NumberCode + ")";
                    count++;
                    if (count > 80)
                    {
                        using (var conn = new SqlConnection(server_EV2))
                        {
                            result_Insert = conn.Execute(sql_Insert, null, null, commandTimeout: 30, commandType: System.Data.CommandType.Text);
                            conn.Close();
                        }
                        count = 0;
                        sql_Insert = "";
                    }
                }
                if (sql_Insert != "")
                {
                    using (var conn = new SqlConnection(server_EV2))
                    {
                        result_Insert = conn.Execute(sql_Insert, null, null, commandTimeout: 30, commandType: System.Data.CommandType.Text);
                        conn.Close();
                    }
                }
                if (result_Insert > 0)
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }
        #region
        public class ResponseRetrieveAgencyCreditVNA
        {
            public string IataCode { get; set; }
            public string creditCode { get; set; }
            public bool success { get; set; }
            public int active { get; set; }
            public decimal balance { get; set; }
            public decimal creditLimit { get; set; }
            public string Currency { get; set; }
        }
        public class RequestVNA
        {
            public string iata { get; set; }
        }
        public static async Task<ResponseRetrieveAgencyCreditVNA> RetrieveAgencyCreditVNA(string airlineSystem, int ID)
        {
            RequestVNA request = new RequestVNA();
            string strUrl = String.Format("https://selfservice.vietnamairlines.com/api/getBalance");
            request.iata = airlineSystem;
            string jsonContent = JsonConvert.SerializeObject(request);
            string accessToken = GetTokenVNA(ID);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(strUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            ResponseRetrieveAgencyCreditVNA result = JsonConvert.DeserializeObject<ResponseRetrieveAgencyCreditVNA>(responseContent);
            return result;
        }

        public static bool SaveDataVNA(string NguoiLap)
        {
            try
            {
                string UserName_Airasia = "EVAKVND0114";
                string Password_Airasia = "Enviet4444@4444";
                Task<ResponseRetrieveAgencyCreditVNA> UJU = RetrieveAgencyCreditVNA("37971102", 1);
                Task<ResponseRetrieveAgencyCreditVNA> JPQ = RetrieveAgencyCreditVNA("37960705", 2);
                Task<ResponseRetrieveAgencyCreditVNA> FHQ = RetrieveAgencyCreditVNA("37981414", 3);
                Task<ResponseRetrieveAgencyCreditVNA> LXQ = RetrieveAgencyCreditVNA("37983094", 4);
                //Task<decimal> Airasia = RetrieveAgencyCreditAirasia(UserName_Airasia, Password_Airasia);

                if (UJU.Result.success != true)
                {
                    return false;
                }
                if (JPQ.Result.success != true)
                {
                    return false;
                }
                if (FHQ.Result.success != true)
                {
                    return false;
                }
                if (LXQ.Result.success != true)
                {
                    return false;
                }
                string sql = "SP_INSERT_SODUHANG";
                int ID = 0;
                using (var conn = new SqlConnection(Server))
                {
                    var param = new
                    {
                        NguoiLap = NguoiLap,
                        Status = 2
                    };
                    ID = conn.QueryFirst<int>(sql, param, null, commandTimeout: 30, commandType: System.Data.CommandType.StoredProcedure);
                }
                if (ID > 0)
                {
                    string sql_chitiet = "";
                    int result_chitiet = 0;
                    string FHQ_text = "FHQ (37981414)";
                    string UJU_text = "UJU (37971102)";
                    string JPQ_text = "JPQ  (37960705)";
                    string LXQ_text = "LXQ  (37983094)";
                    sql_chitiet = "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'" + FHQ_text + "'," + FHQ.Result.balance + ")";
                    sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'" + UJU_text + "'," + UJU.Result.balance + ")";
                    sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'" + JPQ_text + "'," + JPQ.Result.balance + ")";
                    sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'" + LXQ_text + "'," + LXQ.Result.balance + ")";
                    //sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'AIRASIA'," + Airasia.Result + ")";
                    using (var conn = new SqlConnection(Server))
                    {
                        result_chitiet = conn.Execute(sql_chitiet, null, null, commandTimeout: 30, commandType: System.Data.CommandType.Text);
                    }
                    if (result_chitiet > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string GetTokenVNA(int ID)
        {
            try
            {
                string Token = "";
                string sql = "select TokenVNA from TOKEN_VNA where ID = " + ID;
                using (var conn = new SqlConnection(Server))
                {
                    Token = conn.QueryFirst<string>(sql, null, null, commandTimeout: 30, commandType: System.Data.CommandType.Text);
                }
                string result = Token;
                string[] split = result.Split(" ");
                result = split[1].Trim().ToString();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region Cập nhật số dư
        public static bool UpdateInvoiceNumber()
        {
            try
            {
                int result = 0;
                List<string> Ikeys = new List<string>();
                string server_EV = "Data Source=42.117.7.18,1344;Initial Catalog=INHOPDONG;User ID=sa;Password=EnViet@123";
                DataTable tb_hoadon = new DataTable();

                string sql = @"select  IKey from HDDT_TONGQUAT 
                where (NgayChungTu is null or (NgayChungTu is not null and isnull(NumberHD,0) = 0)) and DaXoa = 0 and MONTH(NgayLap) >= Month(dateadd(DD,-1,getdate())) and YEAR(NGAYLAP) >= Year(dateadd(DD,-1,getdate()))";
                using (var conn = new SqlConnection(server_EV))
                {
                    Ikeys = (List<string>)conn.Query<string>(sql, null, commandType: System.Data.CommandType.Text, commandTimeout: 30);
                    conn.Dispose();
                }
                Response res = HoaDonHelper.GetInvoicesKey(Ikeys.ToArray());
                for (int y = 0; y < Ikeys.Count; y++)
                {
                    for (int z = 0; z < res.Data.Invoices.Count; z++)
                    {
                        if (Ikeys[y].Trim() == res.Data.Invoices[z].Ikey.Trim())
                        {
                            if (res.Data.Invoices[z].ArisingDate != "")
                            {
                                string update = @"Update HDDT_TONGQUAT set NumberHD = '" + res.Data.Invoices[z].No.ToString() + "' , NgayChungTu = '" + DateTime.ParseExact(res.Data.Invoices[z].ArisingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                               .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) + "' where ikey = '" + Ikeys[y].Trim() + "'";
                                using (var conn = new SqlConnection(server_EV))
                                {
                                    result = conn.Execute(update, null, null, commandTimeout: 30, commandType: System.Data.CommandType.Text);
                                    conn.Close();
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
        #region AirAsia
        public static async Task<decimal> RetrieveAgencyCreditAirasia(string userName, string password)
        {
            decimal result = 0;
            try
            {
                var pw = await Playwright.CreateAsync();
                var browser = await pw.Chromium.LaunchAsync();
                var page = await browser.NewPageAsync();
                await page.GotoAsync("https://www.airasia.com/agent/login/vi/vn");
                var title = await page.TitleAsync();
                if (title.Contains("Login"))
                {
                    await page.Locator("input[name='username']").FillAsync(userName);
                    await page.Locator("input[name='password']").FillAsync(password);
                    await page.Locator("button.LoginLayout__LoginButton-kxowu0-3").ClickAsync();
                    // Wait for navigation to the post-login page
                    await page.WaitForNavigationAsync();
                    await page.WaitForTimeoutAsync(10000);
                    // Evaluate JavaScript code to retrieve data from sessionStorage
                    var sessionValue = await page.EvaluateAsync<string>(@"() => {
                        return sessionStorage.getItem('agentCompanyInfo');
                    }");
                    var jsonObject = System.Text.Json.JsonSerializer.Deserialize<AirlineAirasia>(sessionValue);
                    result = jsonObject.AvailableCredits;
                    title = await page.TitleAsync();
                    await browser.CloseAsync();
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public class AirlineAirasia
        {
            public string AgentID { get; set; }
            public decimal AvailableCredits { get; set; }
        }

        #endregion

        #region Flyone
        public static async Task<decimal> RetrieveAgencyCreditFlyone(string userName, string password)
        {
            decimal result = 0;
            try
            {
                var pw = await Playwright.CreateAsync();
                var browser = await pw.Chromium.LaunchAsync();
                var page = await browser.NewPageAsync();
                await page.GotoAsync("https://book.flyone.com.vn/");
                var title = await page.TitleAsync();
                if (title.Contains("Đăng"))
                {
                    await page.Locator("input[name='username']").FillAsync(userName);
                    await page.Locator("input[name='password']").FillAsync(password);
                    await page.Locator("button[type=submit]").ClickAsync();
                    // Wait for navigation to the post-login page
                    //await page.WaitForNavigationAsync();
                    await page.WaitForTimeoutAsync(40000);
                    // Evaluate JavaScript code to retrieve data from sessionStorage
                    string SoDu = await page.Locator("div.header-right span b").TextContentAsync();
                    string[] Split = SoDu.Replace(",", "").Split(" ");
                    if (Split[0].Contains("+"))
                    {
                        result = decimal.Parse(Split[0].Substring(1, Split[0].Length - 1));
                    }
                    else
                    {
                        result = decimal.Parse(Split[0]);
                    }
                    await browser.CloseAsync();
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        #endregion

        #region Save airline international
        public static bool SaveOtherAirlineInternational(string NguoiLap)
        {
            try
            {
             
                string UserName_Flyone = "G11104-3";
                string Password_Flyone = "Enviet4444@4444";
                string UserName_Airasia = "EVAKVND0114";
                string Password_Airasia = "Enviet4444@4444";
                bool result = true;
                string sql_soduhang = "";
                Task<decimal> Airasia = RetrieveAgencyCreditAirasia(UserName_Airasia, Password_Airasia);
                Task<decimal> Flyone = RetrieveAgencyCreditFlyone(UserName_Flyone, Password_Flyone);
                sql_soduhang = "SP_INSERT_SODUHANG";
                int ID = 0;
                using (var conn = new SqlConnection(Server))
                {
                    var param = new
                    {
                        NguoiLap = NguoiLap,
                        Status = 3
                    };
                    ID = conn.QueryFirst<int>(sql_soduhang, param, null, commandTimeout: 30, commandType: System.Data.CommandType.StoredProcedure);
                }
                if (ID > 0)
                {
                    int result_chitiet = 0;
                    string sql_chitiet = "";
                    sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'AIRASIA'," + Airasia.Result + ")";
                    sql_chitiet += "Insert into SODUHANG_DETAIL(IDSODUHANG, HANG, SOTIEN) VALUES(" + ID + ",'FLYONE'," + Flyone.Result + ")";
                    using (var conn = new SqlConnection(Server))
                    {
                        result_chitiet = conn.Execute(sql_chitiet, null, null, commandTimeout: 30, commandType: System.Data.CommandType.Text);
                    }
                    if (result_chitiet > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion 
        //#region IndiGo
        //public static async Task<decimal> RetrieveAgencyCreditIndiGo(string userName, string password)
        //{
        //    decimal result = 0;
        //    try
        //    {
        //        var pw = await Playwright.CreateAsync();
        //        var browser = await pw.Chromium.LaunchAsync();
        //        var page = await browser.NewPageAsync();
        //        await page.GotoAsync("https://www.goindigo.in");

        //        //await page.WaitForSelectorAsync("body");
        //        //// Extract the data using the DOM API
        //        //var data = await page.EvaluateAsync(@"() => {
        //        //    const element = document.querySelector('body');
        //        //    return element.innerText;
        //        //}");
        //        await page.WaitForSelectorAsync("#ymPluginDivContainerInitial");

        //        // Wait for the page to fully load

        //        // Get the HTML content of the page
        //        var htmlContent = await page.ContentAsync();
        //        var title = await page.TitleAsync();
        //        //var loginPopup = await page.WaitForSelectorAsync(".popup-modal-with-content-login-form");
        //        //await page.WaitForTimeoutAsync(40000);
        //        string s = await page.InnerHTMLAsync("div.skyplus6e-header__container");


        //            await page.Locator("input.input-text-field__input--filled").FillAsync(userName);
        //            await page.Locator("input[type=password]").FillAsync(password);
        //            await page.Locator("button[type=submit]").ClickAsync();
        //            // Wait for navigation to the post-login page
        //            //await page.WaitForNavigationAsync();
        //            await page.WaitForTimeoutAsync(40000);
        //            // Evaluate JavaScript code to retrieve data from sessionStorage

        //            var sessionValue = await page.EvaluateAsync<string>(@"() => {
        //                return sessionStorage.getItem('u_a_d');
        //            }");

        //            var jsonObject = System.Text.Json.JsonSerializer.Deserialize<AirlineIndiGo>(sessionValue);
        //            result = jsonObject.totalAvailable;

        //            await browser.CloseAsync();

        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        return result;
        //    }
        //}
        //public class AirlineIndiGo
        //{
        //    public string accountKey { get; set; }
        //    public decimal totalAvailable { get; set; }
        //}
        //#endregion
    }
}
