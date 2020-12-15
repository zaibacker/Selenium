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

    class WallPaperDL
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

        public void clickPublishCreateButton()
        {
            Thread.Sleep(1000);
            IWebElement buttonPublishCreate = m_driver.FindElement(By.Id("contentManageNew")); // selecting the second element of the menu
            buttonPublishCreate.Click(); 
            Thread.Sleep(1000);
        }

        [Test]
        public void wallPaperDL()
        {
            login();
            clickPublishCreateButton();

            IWebElement popupButton = m_driver.FindElement(By.CssSelector("button[value='Poster']"));
            popupButton.Click();
            Thread.Sleep(1000);

            var timeNow = DateTime.Now;
            var titleName = "_UITEST" + timeNow.ToString();
            IWebElement title = m_driver.FindElement(By.Id("title"));
            title.SendKeys(titleName);

            IWebElement textBox = m_driver.FindElement(By.ClassName("jodit_wysiwyg"));
            string texBoxMessage = Faker.Lorem.Sentence();
            textBox.SendKeys(texBoxMessage);

            // locate the drop area
            IWebElement droparea = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[23]/div[2]/div"));
            Services.DropFile(droparea, Services.FullDirectoryPath("Images", "download.jpg"));

            //Save
            Thread.Sleep(500);
            IWebElement buttonSubmit = m_driver.FindElement(By.Id("saveButton")); // button save
            buttonSubmit.Click();

            // Test if at least one textBox, radioButton , checkBox and pullDownButton exist
            checkInfoSavedInWalpaper(titleName, texBoxMessage);
        }

        public void checkInfoSavedInWalpaper(string titleName, string texBoxMessage)
        {
            //Edit + Preview
            wallPaperEditAndPreview(titleName);

            Thread.Sleep(1000);
            var priorHandles = m_driver.WindowHandles;
            m_driver.SwitchTo().Window(priorHandles[priorHandles.Count - 1]);
            m_driver.Navigate().Refresh();
            Thread.Sleep(500);

            IWebElement titleTest = m_driver.FindElement(By.ClassName("content-title"));
            Assert.AreEqual(titleTest.Text.Trim(), titleName); // Assert title appears

            IWebElement textTest = m_driver.FindElement(By.ClassName("ultra-content-container")); // button preview
            Assert.AreEqual(textTest.FindElement(By.XPath("./../div[2]")).Text.Trim(), texBoxMessage); // Assert message appears

            m_driver.Close();
            m_driver.SwitchTo().Window(priorHandles[0]);

        }

        public void wallPaperEditAndPreview(string titleName)
        {
            //Edit + Preview

            Thread.Sleep(4000);
            m_driver.Url = env + "admin/wallpaperDownloadList/Poster";
            Thread.Sleep(1000);

            var records = m_driver.FindElements(By.ClassName("itemTitle")); //get allrecord
            Assert.IsNotNull(records.Where(x => x.Text == titleName).FirstOrDefault()); // Assert if the record exists
            Thread.Sleep(1000);

            IWebElement savedElement = records.Where(x => x.Text == titleName).FirstOrDefault();
            //find in hierarchy the edit button
            IWebElement editButton = savedElement.FindElement(By.XPath("./../../../../div[1]/div[2]/button[1]"));
            editButton.Click();

            Thread.Sleep(1000);
            IWebElement buttonPreview = m_driver.FindElement(By.Id("previewButton")); // button preview
            buttonPreview.Click();

            Thread.Sleep(1000);
        }


        [TearDown]
        public void closeBrowser()
        {
            m_driver.Close();
        }

    }
}
