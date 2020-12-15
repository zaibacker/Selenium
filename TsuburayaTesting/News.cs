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

    class News
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
        public void publishNewsCreate()
        {
            login();
            clickPublishCreateButton();

            var time = DateTime.Now;
            var titleName = "_UITEST" + time.ToString();

            IWebElement newsButton = m_driver.FindElement(By.CssSelector("button[value='News']"));
            newsButton.Click();
            Thread.Sleep(1000);

            IWebElement title = m_driver.FindElement(By.Id("title"));
            title.SendKeys(titleName);

            //add endpoint url
            IWebElement endPointUrl = m_driver.FindElement(By.Id("endpointUrl"));
            endPointUrl.SendKeys("google");

            //add tag
            IWebElement tag = m_driver.FindElement(By.Id("tag"));
            tag.SendKeys("google");

            //select scope of disclosure
            IWebElement disclosureMemberClub = m_driver.FindElement(By.CssSelector("button[value='MEM']"));
            disclosureMemberClub.Click();
            IWebElement tsbAccount = m_driver.FindElement(By.CssSelector("button[value='TSB']"));
            tsbAccount.Click();

            //modifying label period
            IWebElement labelPeriod = m_driver.FindElement(By.CssSelector("input[value='7']"));
            labelPeriod.SendKeys(Keys.Backspace);
            labelPeriod.SendKeys("9");

            //TextBox
            IWebElement textBox = m_driver.FindElement(By.ClassName("jodit_wysiwyg"));
            string texBoxMessage = Faker.Lorem.Sentence();
            textBox.SendKeys(texBoxMessage);

            //release check box
            IWebElement ReleaseCheckBox = m_driver.FindElement(By.CssSelector("input[value='start']"));// Release CheckBox
            if (!ReleaseCheckBox.Selected)
                ReleaseCheckBox.Click();

            //Content Genre
            IWebElement contentGenre = m_driver.FindElement(By.CssSelector("button[value='event']"));
            contentGenre.Click();

            IWebElement buttonSubmit = m_driver.FindElement(By.Id("saveButton")); // button save
            buttonSubmit.Click();


            /******************************* Testing *************************/

            NewListChecking(titleName, texBoxMessage);

        }

        public void NewListChecking(string titleName, string texBoxMessage)
        {
            //Edit
            NewsListEdit(titleName);

            Thread.Sleep(1000);

            //check if endpoint url has been saved
            IWebElement endPointUrl = m_driver.FindElement(By.Id("endpointUrl"));
            Assert.AreEqual(endPointUrl.GetAttribute("value"), "google");
            //check if tag  url has been saved
            IWebElement tag = m_driver.FindElement(By.Id("endpointUrl"));
            Assert.AreEqual(tag.GetAttribute("value"), "google");

            //select scope of disclosure
            IWebElement disclosureMemberClub = m_driver.FindElement(By.CssSelector("button[value='MEM']"));
            Assert.AreEqual(disclosureMemberClub.GetAttribute("aria-pressed"), "true"); // Assert if option checked
            IWebElement tsbAccount = m_driver.FindElement(By.CssSelector("button[value='TSB']"));
            Assert.AreEqual(tsbAccount.GetAttribute("aria-pressed"), "true"); // Assert if option is checked

            //modifying label period
            var records = m_driver.FindElements(By.Id("title")); //get allrecord
            IWebElement labelPeriod = records.Where(x => x.GetAttribute("type") == "number").FirstOrDefault();
            Assert.AreEqual(labelPeriod.GetAttribute("value"),"9"); // check if number is 9

            //TextBox
            IWebElement textBox = m_driver.FindElement(By.ClassName("jodit_wysiwyg"));
            Assert.AreEqual(texBoxMessage,textBox.Text); // Assert is text was saved

            //release check box
            IWebElement ReleaseCheckBox = m_driver.FindElement(By.CssSelector("input[value='start']"));
            Assert.True(ReleaseCheckBox.Selected);//Assert if checlbox is checked

            //Content Genre
            IWebElement contentGenre = m_driver.FindElement(By.CssSelector("button[value='event']"));
            Assert.AreEqual(contentGenre.GetAttribute("aria-pressed"), "true"); // Assert if option is checked

            Thread.Sleep(1000);
            IWebElement buttonPreview = m_driver.FindElement(By.Id("previewButton")); // button preview
            buttonPreview.Click();

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

            Thread.Sleep(1000);

        }

        public void NewsListEdit(string titleName)
        {
            Thread.Sleep(4000);
            m_driver.Url = env + "admin/contentManageList";
            Thread.Sleep(1000);

            var records = m_driver.FindElements(By.ClassName("itemTitle")); //get allrecord
            Assert.IsNotNull(records.Where(x => x.Text == titleName).FirstOrDefault()); // Assert if the record exists
            Thread.Sleep(1000);

            IWebElement savedElement = records.Where(x => x.Text == titleName).FirstOrDefault();

            //Find in hierarchy the check box
            IWebElement ReleaseCheckBox = savedElement.FindElement(By.XPath("./../../../../..")).FindElement(By.CssSelector("input[value='start']"));
            Assert.True(ReleaseCheckBox.Selected);//Assert if checkbox is checked

            //find in hierarchy the menu button
            IWebElement menuButton = savedElement.FindElement(By.XPath("./../../../../../div[3]/button"));
            menuButton.Click();

            Thread.Sleep(500);
            IWebElement edit = m_driver.FindElement(By.CssSelector("ul[role='menu'] > li")); // button preview
            edit.Click();

        }

        [TearDown]
        public void closeBrowser()
        {
            m_driver.Close();
        }

    }
}
