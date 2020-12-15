using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using TsuburayaTesting.TsuburayaServices;

namespace TsuburayaTesting
{

    class Links
    {
        IWebDriver m_driver;
        string env = "";

        [SetUp]
        public void startBrowser()
        {
            m_driver = new ChromeDriver(Services.FullDirectoryPath("ChromeDriverV78"));
            env = Services.Environement();
        }

        public void login()
        {
            m_driver.Url = env + "admin/";
            m_driver.Manage().Window.Maximize();

            // Store locator values of email text box and sign up button				
            IWebElement emailTextBox = m_driver.FindElement(By.Id("email"));
            IWebElement pswd  = m_driver.FindElement(By.Id("password"));

            emailTextBox.SendKeys("admin");
            pswd.SendKeys("123654");

            IWebElement signUpButton = m_driver.FindElement(By.ClassName("MuiButton-fullWidth")); //button sign in
            signUpButton.Click();
            Thread.Sleep(1000);
            m_driver.Navigate().Forward();
        }

        public void clickLinksButton()
        {
            Thread.Sleep(1000);
            IWebElement buttonPublishCreate = m_driver.FindElement(By.Id("LinkList")); // selecting the second element of the menu
            buttonPublishCreate.Click(); 
            Thread.Sleep(1000);
        }

        [Test]
        public void links()
        {
            login();
            clickLinksButton();

            IWebElement popupButton = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[3]/div/button"));
            popupButton.Click();
            Thread.Sleep(1000);

            var timeNow = DateTime.Now;
            var titleName = "_UITEST" + timeNow.ToString();
            IWebElement title = m_driver.FindElement(By.Id("title"));
            title.SendKeys(titleName);

            //end point URL
            IWebElement endPointUrl = m_driver.FindElement(By.Id("endpointUrl"));
            endPointUrl.SendKeys("google");

            //Populating URL
            IWebElement linkUrl = m_driver.FindElement(By.Id("link"));
            linkUrl.SendKeys("tsuburaya-membership-dev.azurewebsites.net");        

            //select membership
            IWebElement memButton = m_driver.FindElement(By.CssSelector("button[value='MEM']"));
            memButton.Click();

            //select tsuburaya account
            IWebElement tsbButton = m_driver.FindElement(By.CssSelector("button[value='TSB']"));
            tsbButton.Click();

            //Save
            Thread.Sleep(500);
            IWebElement buttonSubmit = m_driver.FindElement(By.Id("saveButton")); // button save
            buttonSubmit.Click();
            Thread.Sleep(1000);

            checkInfoSaved(titleName);
        }


        public void checkInfoSaved(string titleName)
        {
            clickLinksButton();
            Thread.Sleep(500);
            PopupEdit(titleName);

            IWebElement title = m_driver.FindElement(By.Id("title"));
            Assert.AreEqual(title.GetAttribute("value"), titleName); //Assert if the name is correct

            //check if end point URL is populated
            IWebElement endPointUrl = m_driver.FindElement(By.Id("endpointUrl"));
            Assert.AreEqual(endPointUrl.GetAttribute("value"), "google");//Assert if the name is correct;

            //check if URL is populated
            IWebElement linkUrl = m_driver.FindElement(By.Id("link"));
            Assert.AreEqual(linkUrl.GetAttribute("value"), "tsuburaya-membership-dev.azurewebsites.net");//Assert if the name is correct;

            //select membership
            IWebElement memButton = m_driver.FindElement(By.CssSelector("button[value='MEM']"));
            Assert.AreEqual(memButton.GetAttribute("aria-pressed"), "true"); // Assert if option checked

            //select tsuburaya account
            IWebElement tsbButton = m_driver.FindElement(By.CssSelector("button[value='TSB']"));
            Assert.AreEqual(tsbButton.GetAttribute("aria-pressed"), "true"); // Assert if option checked
        }

        public void PopupEdit(string titleName)
        {

            var records = m_driver.FindElements(By.ClassName("MuiTypography-h6")); //get allrecord
            Assert.IsNotNull(records.Where(x => x.Text == titleName).FirstOrDefault()); // Assert if the record exists
            Thread.Sleep(1000);

            IWebElement savedElement = records.Where(x => x.Text == titleName).FirstOrDefault();

            //find in hierarchy the button edit
            IWebElement editButton = savedElement.FindElement(By.XPath("./../../../../div[1]/div/div/button"));
            editButton.Click();
            Thread.Sleep(500);

        }

        const string JS_DROP_FILE = "for(var b=arguments[0],k=arguments[1],l=arguments[2],c=b.ownerDocument,m=0;;){var e=b.getBoundingClientRect(),g=e.left+(k||e.width/2),h=e.top+(l||e.height/2),f=c.elementFromPoint(g,h);if(f&&b.contains(f))break;if(1<++m)throw b=Error('Element not interractable'),b.code=15,b;b.scrollIntoView({behavior:'instant',block:'center',inline:'center'})}var a=c.createElement('INPUT');a.setAttribute('type','file');a.setAttribute('style','position:fixed;z-index:2147483647;left:0;top:0;');a.onchange=function(){var b={effectAllowed:'all',dropEffect:'none',types:['Files'],files:this.files,setData:function(){},getData:function(){},clearData:function(){},setDragImage:function(){}};window.DataTransferItemList&&(b.items=Object.setPrototypeOf([Object.setPrototypeOf({kind:'file',type:this.files[0].type,file:this.files[0],getAsFile:function(){return this.file},getAsString:function(b){var a=new FileReader;a.onload=function(a){b(a.target.result)};a.readAsText(this.file)}},DataTransferItem.prototype)],DataTransferItemList.prototype));Object.setPrototypeOf(b,DataTransfer.prototype);['dragenter','dragover','drop'].forEach(function(a){var d=c.createEvent('DragEvent');d.initMouseEvent(a,!0,!0,c.defaultView,0,0,0,g,h,!1,!1,!1,!1,0,null);Object.setPrototypeOf(d,null);d.dataTransfer=b;Object.setPrototypeOf(d,DragEvent.prototype);f.dispatchEvent(d)});a.parentElement.removeChild(a)};c.documentElement.appendChild(a);a.getBoundingClientRect();return a;";
        static void DropFile(IWebElement target, string filePath, double offsetX = 0, double offsetY = 0)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            IWebDriver driver = ((RemoteWebElement)target).WrappedDriver;
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;

            IWebElement input = (IWebElement)jse.ExecuteScript(JS_DROP_FILE, target, offsetX, offsetY);
            input.SendKeys(filePath);
        }

        [TearDown]
        public void closeBrowser()
        {
            m_driver.Close();
        }

    }
}
