using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TsuburayaTesting.Config;
using TsuburayaTesting.TsuburayaServices;

namespace TsuburayaTesting
{

    class MailingList
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

        public void clickMailingListButton()
        {
            Thread.Sleep(1000);
            IWebElement buttonPublishCreate = m_driver.FindElement(By.Id("mailingList")); // selecting the second element of the menu
            buttonPublishCreate.Click(); 
            Thread.Sleep(1000);
        }

        [Test]
        public void MailingListCreate()
        {
            login();
            clickMailingListButton();
            var textMailtoSend = "This a text mail";
            var htmlMailtoSend = "This an HTML mail";

            IWebElement mailMagazineRegistration = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[3]/div[1]/button"));
            mailMagazineRegistration.Click();

            var timeNow = DateTime.Now;
            var titleName = "_UITEST" + timeNow.ToString();
            IWebElement subject = m_driver.FindElement(By.Id("subject"));
            subject.SendKeys(titleName);

            IWebElement textMail = m_driver.FindElement(By.Id("textMail"));
            textMail.SendKeys(textMailtoSend);

            IWebElement htmlMail = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[3]/div[7]/div[2]/div/div[2]/div[2]"));
            htmlMail.SendKeys(htmlMailtoSend);

            IWebElement italic = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[3]/div[7]/div[2]/div/div[1]/ul/li[4]/a"));
            italic.Click();
            IWebElement bold = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[3]/div[7]/div[2]/div/div[1]/ul/li[3]/a"));
            bold.Click();
            IWebElement address = m_driver.FindElement(By.Id("sendToEmail"));
            address.SendKeys("jose_bergua@t-rnd.com");

            IWebElement date = m_driver.FindElement(By.Id("scheduled-date"));
            for (int i=0; i<7; i++)
            {
                date.SendKeys(Keys.Backspace);
            }             
            string newDate = "0210101";
            date.SendKeys(newDate);

            IWebElement time = m_driver.FindElement(By.Id("scheduled-time"));
            for (int i=0; i< 3; i++)
            {
                time.SendKeys(Keys.Backspace);
            }                
            string newTime = "212";
            time.SendKeys(newTime);

            IWebElement saveButton = m_driver.FindElement(By.Id("saveButton"));
            saveButton.Click();

            mailingListEditAndPreview(titleName);

            /******************* Testing ************************/

            subject = m_driver.FindElement(By.Id("subject"));
            Assert.AreEqual(subject.GetAttribute("value"), titleName); //Assert if title is same
            textMail = m_driver.FindElement(By.Id("textMail"));
            Assert.AreEqual(textMail.Text, textMailtoSend); //Assert if textmail saved the content
            IWebElement htmlMailCheck = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[3]/div[7]/div[2]/div/div[2]/div[2]"));
            Assert.AreEqual(htmlMailCheck.Text, htmlMailtoSend); //Assert if HTML mail text saved the content
            address = m_driver.FindElement(By.Id("sendToEmail"));
            Assert.AreEqual(address.GetAttribute("value"), "jose_bergua@t-rnd.com"); //Assert if address mail is saved
            date = m_driver.FindElement(By.Id("scheduled-date"));
            Assert.AreEqual(date.GetAttribute("value"), "2021/01/01"); //Assert if date is saved
            time = m_driver.FindElement(By.Id("scheduled-time"));
            Assert.AreEqual(time.GetAttribute("value").Substring(1), "2:12"); //Assert if time is saved

        }

        public void mailingListEditAndPreview(string titleName)
        {
            //Edit

            Thread.Sleep(3000);
            m_driver.Url = env + "admin/mailingList";
            Thread.Sleep(1000);

            var records = m_driver.FindElements(By.ClassName("itemTitle")); //get allrecord
            Assert.IsNotNull(records.Where(x => x.Text == titleName).FirstOrDefault()); // Assert if the record exists
            Thread.Sleep(1000);

            IWebElement savedElement = records.Where(x => x.Text == titleName).FirstOrDefault();
            //find in hierarchy the edit button
            IWebElement editButton = savedElement.FindElement(By.XPath("./../../../div[1]/div[2]/button[1]"));
            editButton.Click();

            Thread.Sleep(1000);
        }

        [TearDown]
        public void closeBrowser()
        {
            m_driver.Close();
        }

    }
}
