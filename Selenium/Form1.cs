using EAGetMail;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Data;
using System.Globalization;
using System.Text;

namespace Selenium
{
    public partial class Form1 : Form
    {
        IWebDriver driver;
        string chromeDriverPath = Application.StartupPath + @"lib";
        string emailTxt = Application.StartupPath + @"lib\email.txt";
        string hoTxt = Application.StartupPath + @"lib\ho.txt";
        string tenTxt = Application.StartupPath + @"lib\ten.txt";
        string proxy = Application.StartupPath + @"lib\proxy.txt";
        List<string> lstHo = new List<string>();
        List<string> lstTen = new List<string>();
        Dictionary<string, string> dicMail = new Dictionary<string, string>();
        List<string> lstProxy = new List<string>();
        public Form1()
        {
            InitializeComponent();

            LoadGrid();
            lstHo = System.IO.File.ReadLines(hoTxt).ToList();
            lstTen = System.IO.File.ReadLines(tenTxt).ToList();
            lstProxy = System.IO.File.ReadLines(proxy).ToList();
        }
        private void LoadGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("email");
            dt.Columns.Add("password");

            using (var streamReader = new StreamReader(emailTxt))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var values = line.Split('\t');
                    var rowIndex = dt.NewRow();
                    for (int i = 0; i < values.Length; i++)
                    {
                        rowIndex[i] = values[i];
                    }
                    dt.Rows.Add(rowIndex);
                }
            }
            dgvEmail.DataSource = dt;


            dt = new DataTable();
            dt.Columns.Add("ho");
            dt.Columns[0].Caption = "Họ";
            using (var streamReader = new StreamReader(hoTxt))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var values = line.Split('\t');
                    var rowIndex = dt.NewRow();
                    rowIndex[0] = line.ToString();
                    dt.Rows.Add(rowIndex);
                }
            }
            dgvHo.DataSource = dt;

            dt = new DataTable();
            dt.Columns.Add("ten");
            dt.Columns[0].Caption = "Tên";
            using (var streamReader = new StreamReader(hoTxt))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var values = line.Split('\t');
                    var rowIndex = dt.NewRow();
                    rowIndex[0] = line.ToString();
                    dt.Rows.Add(rowIndex);
                }
            }
            dgvTen.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //driver.Close();
            //driver.Quit();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lstTen.Count() == 0 || lstHo.Count() == 0)
            {
                MessageBox.Show("Chưa có danh sách họ tên đăng ký");
                return;
            }


            Random getrandom = new Random();

            driver = new ChromeDriver(chromeDriverPath/*, options, TimeSpan.FromDays(20)*/);
            driver.Navigate().GoToUrl(@"https://whoer.net/");

            DataTable dt = (DataTable)dgvEmail.DataSource;
            int i = 0;
            foreach (DataRow drow in dt.Rows)
            {
                try
                {
                    if (i >= 2)
                    {
                        i = 0;
                    }
                    ChromeOptions options = new ChromeOptions();
                    options.AddArgument(string.Format("--proxy-server={0}", lstProxy[i]));
                    //driver = new ChromeDriver(chromeDriverPath, options);
                    i++;
                    string email = drow["email"].ToString();
                    string pass = drow["password"].ToString();

                    #region ĐK Tiktok
                    driver = new ChromeDriver(chromeDriverPath, options);
                    driver.Manage().Window.Maximize();

                    //register tiktok
                    driver.Navigate().GoToUrl(@"https://www.tiktok.com/signup/phone-or-email/email");
                    Thread.Sleep(5000);
                    //Month
                    IWebElement month = driver.FindElement(By.CssSelector("div.container-1lSJp"));
                    month.Click();
                    Thread.Sleep(1000);
                    IWebElement select = driver.FindElement(By.ClassName("list-container-2f5zg"));
                    var liElement = select.FindElements(By.ClassName("list-item-MOAq4"));
                    liElement[getrandom.Next(0, 12)].Click();
                    Thread.Sleep(1000);

                    //Day
                    IWebElement day = driver.FindElements(By.CssSelector("div.container-1lSJp"))[1];
                    day.Click();
                    Thread.Sleep(1000);
                    select = driver.FindElements(By.ClassName("container-1lSJp"))[1];
                    liElement = select.FindElements(By.ClassName("list-item-MOAq4"));
                    liElement[getrandom.Next(1, 29)].Click();
                    Thread.Sleep(1000);

                    //Year
                    IWebElement year = driver.FindElements(By.CssSelector("div.container-1lSJp"))[2];
                    year.Click();
                    Thread.Sleep(1000);
                    select = driver.FindElements(By.ClassName("container-1lSJp"))[2];
                    liElement = select.FindElements(By.ClassName("list-item-MOAq4"));
                    liElement[getrandom.Next(17, 29)].Click();
                    Thread.Sleep(1000);

                    //Email
                    driver.FindElement(By.Name("email"), 5000).SendKeys(email);
                    //Pass
                    driver.FindElement(By.Name("password"), 5000).SendKeys(pass);
                    //Get Code
                    driver.FindElement(By.ClassName("login-button-31D24"), 5000).Click();
                    #endregion

                    #region Lấy mã Otp
                    //Get Otp
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    driver.Navigate().GoToUrl(@"https://outlook.live.com/owa/0/?state=1&redirectTo=aHR0cHM6Ly9vdXRsb29rLmxpdmUuY29tL21haWwvMC8&nlp=1");

                    Thread.Sleep(5000);
                    driver.FindElement(By.Id("i0116"), 5000).SendKeys(email);
                    driver.FindElement(By.Id("idSIButton9"), 5000).Click();
                    Thread.Sleep(1000);
                    driver.FindElement(By.Name("passwd"), 5000).SendKeys(pass);

                    driver.SwitchTo().DefaultContent();
                    driver.FindElement(By.Name("passwd"), 5000).Submit();
                    Thread.Sleep(1000);

                    //try
                    //{
                    //    IWebElement webElement = driver.FindElement(By.Name("passwd"), 5000);
                    //    webElement.SendKeys(pass);
                    //    webElement.Submit();
                    //}
                    //catch { }


                    driver.FindElement(By.Id("idBtn_Back")).Click();
                    Thread.Sleep(5000);
                    IWebElement searchElemnt = driver.FindElement(By.CssSelector("[aria-label='Search']"), 15000);
                    searchElemnt.SendKeys("tiktok");

                    searchElemnt.SendKeys(OpenQA.Selenium.Keys.Enter);
                    Thread.Sleep(5000);
                    IWebElement credId = null;
                    while (credId == null)
                    {
                        searchElemnt.SendKeys(OpenQA.Selenium.Keys.Enter);
                        Thread.Sleep(5000);
                        credId = driver.FindElement(By.CssSelector("[aria-label='Select a conversation']"));
                    }
                    credId.Click();

                    IWebElement textSelection = driver.FindElement(By.CssSelector("div.allowTextSelection>span"), 5000);
                    string otpAuthen = textSelection.Text.Split(" ").FirstOrDefault();

                    driver.SwitchTo().Window(driver.WindowHandles.First());
                    //Input Code
                    driver.FindElement(By.CssSelector("div.digit-code-container-GBZyT>div>input")).SendKeys(otpAuthen != "" ? otpAuthen : "123456");

                    string userName = "";

                    Random rdHo = new Random();
                    int index = rdHo.Next(0, lstHo.Count());
                    userName += lstHo[index].ToString();
                    Random rdTen = new Random();
                    index = rdHo.Next(0, lstTen.Count());
                    userName += lstTen[index].ToString();

                    //Input Name
                    driver.FindElement(By.CssSelector("div.input-field-3x_mo>input")).SendKeys(userName);
                    driver.FindElement(By.CssSelector("div.input-field-3x_mo>input")).Submit();

                    #endregion
                    //dgvEmail.Rows[dt.Rows.IndexOf(drow)].Cells["isDone"].Value = true;

                    driver.Close();
                    driver.Quit();
                    driver.Dispose();
                }
                catch (Exception)
                {

                }
                finally
                {
                }
            }
        }

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    Random rd = new Random();
        //    ChromeDriver driver = new ChromeDriver(chromeDriverPath/*, options, TimeSpan.FromDays(20)*/);
        //    driver.Manage().Window.Maximize();

        //    driver.Navigate().GoToUrl(@"https://signup.live.com/");

        //    driver.FindElement(By.Id("liveSwitch"), 5000).Click();
        //    var selectDomain = driver.FindElement(By.Name("LiveDomainBoxList"), 5000);
        //    //create select element object 
        //    var selectElement = new SelectElement(selectDomain);
        //    selectElement.SelectByValue("hotmail.com");

        //    string name = GenerateName(10);
        //    string pass = RandomPassword(10);
        //    dicMail.Add(name, pass);
        //    driver.FindElement(By.Name("MemberName"), 5000).SendKeys(name);
        //    driver.FindElement(By.Id("iSignupAction"), 5000).Click();

        //    IWebElement searchElemnt = driver.FindElement(By.Id("PasswordInput"), 15000);
        //    searchElemnt.SendKeys(pass);
        //    //driver.FindElement(By.CssSelector("[aria-label]='Create password'"),5000).SendKeys(pass);
        //    driver.FindElement(By.Id("iSignupAction"), 5000).Click();

        //    string FirstName = GenerateName(4);
        //    string LastName = GenerateName(rd.Next(1, 5));
        //    driver.FindElement(By.Name("FirstName"), 5000).SendKeys(FirstName);
        //    driver.FindElement(By.Name("LastName"), 5000).SendKeys(LastName);
        //    driver.FindElement(By.Name("LastName"), 5000).Submit();

        //    selectDomain = driver.FindElement(By.Name("BirthMonth"), 5000);
        //    //create select element object 
        //    selectElement = new SelectElement(selectDomain);
        //    selectElement.SelectByIndex(rd.Next(1, 12));

        //    selectDomain = driver.FindElement(By.Name("BirthDay"), 5000);
        //    //create select element object 
        //    selectElement = new SelectElement(selectDomain);
        //    selectElement.SelectByIndex(rd.Next(1, 28));

        //    selectDomain = driver.FindElement(By.Name("BirthYear"), 5000);
        //    //create select element object 
        //    selectElement = new SelectElement(selectDomain);
        //    selectElement.SelectByIndex(rd.Next(17, 29));

        //    driver.FindElement(By.Name("BirthYear"), 5000).Submit();
        //}
        //public string RandomPassword(int size = 0)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(RandomString(4, true));
        //    builder.Append(RandomNumber(1000, 9999));
        //    builder.Append(RandomString(2, false));
        //    return builder.ToString();
        //}
        //public int RandomNumber(int from, int to)
        //{
        //    Random random = new Random();
        //    return random.Next(from, to);
        //}
        //public string RandomString(int size, bool lowerCase)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    Random random = new Random();
        //    char ch;
        //    for (int i = 0; i < size; i++)
        //    {
        //        ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
        //        builder.Append(ch);
        //    }
        //    if (lowerCase)
        //        return builder.ToString().ToLower();
        //    return builder.ToString();
        //}
        //public string GenerateName(int len)
        //{
        //    Random r = new Random();
        //    string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
        //    string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        //    string Name = "";
        //    Name += consonants[r.Next(consonants.Length)].ToUpper();
        //    Name += vowels[r.Next(vowels.Length)];
        //    int b = 2;
        //    while (b < len)
        //    {
        //        Name += consonants[r.Next(consonants.Length)];
        //        b++;
        //        Name += vowels[r.Next(vowels.Length)];
        //        b++;
        //    }

        //    return Name;
        //}
    }
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }
    }
}