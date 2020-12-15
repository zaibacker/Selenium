using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using TsuburayaTesting.Config;
using TsuburayaTesting.TsuburayaServices;

namespace TsuburayaTesting
{

    class Popup
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

        public void clickPopupButton()
        {
            Thread.Sleep(1000);
            IWebElement buttonPublishCreate = m_driver.FindElement(By.Id("popupList")); // selecting the second element of the menu
            buttonPublishCreate.Click(); 
            Thread.Sleep(1000);
        }

        [Test]
        public void popup()
        {
            login();
            clickPopupButton();

            IWebElement popupButton = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[3]/div[1]/button"));
            popupButton.Click();
            Thread.Sleep(1000);

            var timeNow = DateTime.Now;
            var titleName = "_UITEST" + timeNow.ToString();
            IWebElement title = m_driver.FindElement(By.Id("title"));
            title.SendKeys(titleName);

            //Populating URL
            IWebElement linkUrl = m_driver.FindElement(By.Id("link"));
            linkUrl.SendKeys("tsuburaya-membership-dev.azurewebsites.net");        

            // locate the drop area
            IWebElement droparea = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[4]/div[3]/div"));
            Services.DropFile(droparea, Services.FullDirectoryPath("Images", "download.jpg"));

            //release check box
            IWebElement ReleaseCheckBox = m_driver.FindElement(By.CssSelector("input[value='start']"));// Release CheckBox
            if (!ReleaseCheckBox.Selected)
                ReleaseCheckBox.Click();

            //Save
            Thread.Sleep(500);
            IWebElement buttonSubmit = m_driver.FindElement(By.Id("saveButton")); // button save
            buttonSubmit.Click();
            Thread.Sleep(1000);

            checkInfoSaved(titleName);
        }


        public void checkInfoSaved(string titleName)
        {
            clickPopupButton();
            Thread.Sleep(500);
            PopupEdit(titleName);

            IWebElement title = m_driver.FindElement(By.Id("title"));
            Assert.AreEqual(title.GetAttribute("value"), titleName); //Assert if the name is correct

            //check if URL is populated
            IWebElement linkUrl = m_driver.FindElement(By.Id("link"));
            Assert.AreEqual(linkUrl.GetAttribute("value"), "tsuburaya-membership-dev.azurewebsites.net");//Assert if the name is correct;

            //checking if the pic has been saved
            Assert.IsNotNull(m_driver.FindElement(By.ClassName("MuiPaper-elevation1")));

            // check if checkbox is selected  
            IWebElement ReleaseCheckBox = m_driver.FindElement(By.CssSelector("input[value='start']"));
            Assert.True(ReleaseCheckBox.Selected);//Assert if checlbox is checked
        }

        public void PopupEdit(string titleName)
        {

            var records = m_driver.FindElements(By.ClassName("MuiTypography-h6")); //get allrecord
            Assert.IsNotNull(records.Where(x => x.Text == titleName).FirstOrDefault()); // Assert if the record exists
            Thread.Sleep(1000);

            IWebElement savedElement = records.Where(x => x.Text == titleName).FirstOrDefault();

            //Find in hierarchy the check box
            IWebElement ReleaseCheckBox = savedElement.FindElement(By.XPath("./../../../..")).FindElement(By.CssSelector("input[value='start']"));
            Assert.True(ReleaseCheckBox.Selected);//Assert if checkbox is checked

            //find in hierarchy the button edit
            IWebElement editButton = savedElement.FindElement(By.XPath("./../../../../div[1]/div/div/button"));
            editButton.Click();
            Thread.Sleep(500);

        }

        [TearDown]
        public void closeBrowser()
        {
            m_driver.Close();
        }

    }
}
