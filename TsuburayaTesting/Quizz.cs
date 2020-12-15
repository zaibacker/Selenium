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

    class Quizz
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
            IWebElement pswd = m_driver.FindElement(By.Id("password"));

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
        public void QuizCreateAllCases()
        {
            login();
            clickPublishCreateButton();

            //Already selected by default
            //IWebElement newsButton = m_driver.FindElement(By.CssSelector("button[value='Quiz']"));
            //newsButton.Click();
            //Thread.Sleep(1000);

            var timeNow = DateTime.Now;
            var titleName = "_UITEST" + timeNow.ToString();

            IWebElement title = m_driver.FindElement(By.Id("title"));
            title.SendKeys(titleName);
            IWebElement ReleaseCheckBox = m_driver.FindElement(By.CssSelector("input[value='start']")); // Release CheckBox
            if (!ReleaseCheckBox.Selected)
                ReleaseCheckBox.Click();

            //7 types of form to be tested
            string[] questions = new string[7];
            for(int i= 0; i<7; i++)
            {
                //int j = 2 + i;
                //string xpath = "//*[@id='root']/div[1]/main/div[23]/div[" + j + "]/div/button[1]";
                //IWebElement buttonAddForm = m_driver.FindElement(By.XPath(xpath));
                IWebElement buttonAddForm = m_driver.FindElement(By.Id("addQuestion"));
                buttonAddForm.Click();
                questions[i] = Faker.Lorem.Sentence();
            }

            var questionTitles = m_driver.FindElements(By.XPath("//*[@id='question']"));
            var listDropdown = m_driver.FindElements(By.CssSelector("[class*='MuiCard-root'] > * > * > *> [role='button']"));
            Actions actions = new Actions(m_driver);
            IWebElement selection;
            string newNumber = Faker.RandomNumber.Next().ToString();

            /*------------------------------------------------------------*/

            IWebElement paragraphBox = m_driver.FindElement(By.ClassName("jodit_wysiwyg"));
            var paragraphSentence = Faker.Lorem.Sentence();
            paragraphBox.SendKeys(paragraphSentence);

            /*------------------------------------------------------------*/

            //text Box One Line
            questionTitles[0].SendKeys(questions[0]);
            var span = m_driver.FindElement(By.XPath("//*[@id='q-0']/span/p"));
            Assert.AreEqual(span.Text, questions[0]); // testing if question is correctly displayed 

            /*------------------------------------------------------------*/

            //text box paragraph
            questionTitles[1].SendKeys(questions[1]);
            actions.MoveToElement(listDropdown[1]).Click().Perform();
            Thread.Sleep(500);
            selection = m_driver.FindElement(By.XPath("//*[@id='menu-']/div[3]/ul/li[2]"));
            selection.Click();

            /*------------------------------------------------------------*/

            //radio Button
            questionTitles[2].SendKeys(questions[2]);
            listDropdown[2].Click();
            Thread.Sleep(500);
            selection = m_driver.FindElement(By.XPath("//*[@id='menu-']/div[3]/ul/li[3]"));
            selection.Click();

            Thread.Sleep(500);
            IWebElement addRadioButton = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[22]/div[3]/div[1]/div[2]/div/button")); // clicking add button
            //actions.MoveToElement(addRadioButton).Click().Perform();
            addRadioButton.Click();

            IWebElement newEntry = m_driver.FindElement(By.XPath("/html/body/main/nav/div/div[1]/main/div[22]/div[3]/div[1]/div[2]/div/div[4]/div[1]/label/div/div/input")); // creating a new entry
            newEntry.SendKeys(newNumber);

            IWebElement radioPreview = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[22]/div[3]/div[1]/div[3]/div[1]/button")); // preview for radio button
            radioPreview.Click();

            IWebElement parentLabel = m_driver.FindElement(By.XPath("//*[@id='q-2']")); // get Parent
            var childrens = parentLabel.FindElements(By.XPath(".//*[@id='q-2']")); // get children
            Assert.AreEqual(4, childrens.Count); // testing , it should have 4 children 

            var spanRadio = m_driver.FindElement(By.XPath("//*[@id='q-2']/span/p"));
            Assert.AreEqual(spanRadio.Text, questions[2]); // testing if question is correctly displayed 

            IWebElement radioTest = m_driver.FindElement(By.XPath("//*[@id='q-2']/span[1]/span[1]/input"));
            string typeOfElement = radioTest.GetAttribute("type");
            Assert.AreEqual("radio",typeOfElement); // testing that the type of the element is radio button

            /*------------------------------------------------------------*/

            //Pull Down
            questionTitles[3].SendKeys(questions[3]);
            listDropdown[3].Click();
            Thread.Sleep(500);
            selection = m_driver.FindElement(By.XPath("//*[@id='menu-']/div[3]/ul/li[4]"));
            selection.Click();

            try
            {
                IWebElement addPullButton = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[22]/div[4]/div[1]/div[2]/div/button")); // clicking add button
                addPullButton.Click();
            }
            catch(Exception e)
            {
                IWebElement addPullButton = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[22]/div[4]/div[1]/div[2]/div/button")); // clicking add button
                actions.MoveToElement(addPullButton).Click().Perform();
            }


            IWebElement newPullEntry = m_driver.FindElement(By.XPath("/html/body/main/nav/div/div[1]/main/div[22]/div[4]/div[1]/div[2]/div/div[4]/div[1]/label/div/div/input")); // creating a new entry
            newPullEntry.SendKeys(newNumber);

            IWebElement pullPreview = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[22]/div[4]/div[1]/div[3]/div[1]/button")); // preview for pull down button
            pullPreview.Click();

            IWebElement SelectOptionButton = m_driver.FindElement(By.XPath("//*[@id='q-3']/div/div")); // Select option button from dropdownlist
            SelectOptionButton.Click();

            IWebElement parentListDropDown = m_driver.FindElement(By.XPath("//*[@id='menu-']/div[3]/ul")); // get Parent of the list (ul is parent tag)
            //var childrensListDropDown = parentListDropDown.FindElements(By.XPath(".//*")); // get children
            var childrensListDropDown = parentListDropDown.FindElements(By.TagName("li"));// get children
            childrensListDropDown[2].Click();
            Assert.AreEqual(5, childrensListDropDown.Count); // testing , it should have 5 elements,  4 children + element "select an option"

            /*------------------------------------------------------------*/

            //Check Box
            questionTitles[4].SendKeys(questions[4]);
            listDropdown[4].Click();
            Thread.Sleep(500);
            selection = m_driver.FindElement(By.XPath("//*[@id='menu-']/div[3]/ul/li[5]"));
            selection.Click();
            Thread.Sleep(500);


            IWebElement addCheckButton = m_driver.FindElement(By.XPath("/html/body/main/nav/div/div[1]/main/div[22]/div[5]/div[1]/div[2]/div/button")); // clicking add Check button
            addCheckButton.Click();

            IWebElement newCheckBoxEntry = m_driver.FindElement(By.XPath("/html/body/main/nav/div/div[1]/main/div[22]/div[5]/div[1]/div[2]/div/div[4]/div[1]/label/div/div/input")); // creating a new entry
            newCheckBoxEntry.SendKeys(newNumber);

            IWebElement chekBoxPreview = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[22]/div[5]/div[1]/div[3]/div[1]/button")); // preview for pull down button
            chekBoxPreview.Click();

            var getAllBoxes = m_driver.FindElements(By.XPath("//*[@id='q-4']")); // get all boxes
            Assert.AreEqual(4, getAllBoxes.Count); // check if we have 4 records
            Assert.AreEqual("checkbox", getAllBoxes[0].FindElement(By.XPath("./span[1]/span[1]/input")).GetProperty("type")); //check if we have type = checkbox

            /*------------------------------------------------------------*/

            // Date 
            questionTitles[5].SendKeys(questions[5]);
            listDropdown[5].Click();
            Thread.Sleep(500);
            selection = m_driver.FindElement(By.XPath("//*[@id='menu-']/div[3]/ul/li[6]"));
            selection.Click();

            IWebElement todayDate = m_driver.FindElement(By.XPath("//*[@id='q-5']/div/div/input")); // date displayed
            Assert.IsEmpty(todayDate.Text);

            /*------------------------------------------------------------*/

            // Time
            questionTitles[6].SendKeys(questions[6]);
            listDropdown[6].Click();
            Thread.Sleep(500);
            selection = m_driver.FindElement(By.XPath("//*[@id='menu-']/div[3]/ul/li[7]"));
            selection.Click();

            IWebElement time = m_driver.FindElement(By.XPath("//*[@id='q-6']/div/div/input")); // date displayed
            Assert.IsEmpty(time.Text);

            /*------------------------------------------------------------*/

            //Save + Edit + Preview
            Thread.Sleep(500);
             IWebElement buttonSubmit = m_driver.FindElement(By.Id("saveButton")); // button save
             buttonSubmit.Click();

            quizListEditAndPreview(titleName);

            /*******************************************************/

            //Testing
            Thread.Sleep(1000);
            var priorHandles = m_driver.WindowHandles;
             m_driver.SwitchTo().Window(priorHandles[priorHandles.Count - 1]);

            m_driver.Navigate().Refresh();
            Thread.Sleep(2000);

            var paragraphBoxTest = m_driver.FindElement(By.XPath("/html/body/main/div/div/div[2]"));
            Assert.IsTrue(paragraphBoxTest.Text.Contains(paragraphSentence)); // Assert if same text introduced in paragraph

            var quizQuestionTitle = m_driver.FindElements(By.ClassName("quizQuestionTitle"));

            Assert.AreEqual(questions[0], quizQuestionTitle[0].Text); // check first title
            Assert.AreEqual(quizQuestionTitle[0].FindElement(By.XPath("..//input")).GetProperty("type"), "text"); // Assert if textbox

            Assert.AreEqual(questions[1], quizQuestionTitle[1].Text); // check second title
            Assert.AreEqual(quizQuestionTitle[1].FindElement(By.XPath("..//input")).GetProperty("type"), "text"); // Assert if textbox

            Assert.AreEqual(questions[2], quizQuestionTitle[2].Text); // check third title
            var quizQuestionAnswerRadiobox = quizQuestionTitle[2].FindElement(By.XPath("..")).FindElements(By.ClassName("quizQuestionAnswerRadiobox"));
            Assert.AreEqual(4, quizQuestionAnswerRadiobox.Count); // Assert if there are 4 radio buttons
            Assert.AreEqual(quizQuestionAnswerRadiobox[0].GetProperty("type"),"radio"); // Assert if the first button is radio button

            Assert.AreEqual(questions[3], quizQuestionTitle[3].Text); // check fourth title
            IWebElement course = m_driver.FindElement(By.ClassName("quizQuestionAnswerSelect"));
            var selectTest = new SelectElement(course); // this is only for dropdownlist
            Assert.AreEqual(4, selectTest.Options.Count); // assert if there are 4 options for dropdownlist

            Assert.AreEqual(questions[4], quizQuestionTitle[4].Text); // check fifth title
            var checkBoxes = quizQuestionTitle[4].FindElement(By.XPath("..")).FindElements(By.ClassName("quizQuestionAnswerOptions"));
            Assert.AreEqual(4, checkBoxes.Count); // assert if there are 4 options for check boxes
            Assert.AreEqual(checkBoxes[0].FindElement(By.XPath("./input")).GetProperty("type"), "checkbox"); // Assert if the first button is checkbox type

            Assert.AreEqual(questions[5], quizQuestionTitle[5].Text); // check sixth title
            Assert.AreEqual(quizQuestionTitle[5].FindElement(By.XPath("..//input")).GetProperty("type"), "date"); // Assert if date type

            Assert.AreEqual(questions[6], quizQuestionTitle[6].Text); // check seventh title
            Assert.AreEqual(quizQuestionTitle[6].FindElement(By.XPath("..//input")).GetProperty("type"), "time"); // Assert if time type

            m_driver.Close();
            m_driver.SwitchTo().Window(priorHandles[0]);

        }

        [Test]
        public void QuizCreateRandom()
        {
            login();
            clickPublishCreateButton();
            var timeNow = DateTime.Now;
            var titleName = "_UITEST" + timeNow.ToString();
            IWebElement title = m_driver.FindElement(By.Id("title"));
            title.SendKeys(titleName);

            textBox();
            radioButton();
            checkBox();
            pullDownButton();

            //Save
            Thread.Sleep(500);
            IWebElement buttonSubmit = m_driver.FindElement(By.Id("saveButton")); // button save
            buttonSubmit.Click();

            // Test if at least one textBox, radioButton , checkBox and pullDownButton exist
            checkInfoSaved(titleName);

        }

        public void textBox()
        {
            Random r = new Random();
            int rInt = r.Next(1, 10); //random new records for text box creation , between 1 and 10

            int formNumber = m_driver.FindElements(By.XPath("//*[@id='question']")).Count;
            //creating 10 question forms
            for (int i = formNumber; i < formNumber+ rInt; i++)
            {
                int j = 2 + i;
                string xpath = "//*[@id='root']/div[1]/main/div[22]/div[" + j + "]/div/button[1]";
                IWebElement buttonAddForm = m_driver.FindElement(By.XPath(xpath));
                buttonAddForm.Click();

                Thread.Sleep(500);
                var question = Faker.Lorem.Sentence();
                var questionTitles = m_driver.FindElements(By.XPath("//*[@id='question']"));
                questionTitles[i].SendKeys(question);
                var spanString = "//*[@id='q-"+ i + "']/span/p";
                var span = m_driver.FindElement(By.XPath(spanString));
                Assert.AreEqual(span.Text, question); // testing if question is correctly displayed 
            }

        }

        public void checkInfoSaved(string titleName)
        {
            //Edit + Preview
            quizListEditAndPreview(titleName);

            var priorHandles = m_driver.WindowHandles;
            m_driver.SwitchTo().Window(priorHandles[priorHandles.Count - 1]);

            m_driver.Navigate().Refresh();
            Thread.Sleep(1000);

            Assert.AreEqual(m_driver.FindElement(By.ClassName("content-title")).Text.Trim(), titleName); // The Title must be the correct one

            var radioTest = m_driver.FindElements(By.ClassName("quizQuestionAnswerString"));
            Assert.Greater(radioTest.Count, 0); // Assert if at least one radioBox exists

            var textBoxTest = m_driver.FindElements(By.ClassName("quizQuestionAnswerRadiobox"));
            Assert.Greater(textBoxTest.Count, 0); // Assert if at least one text box exists

            var checkBoxTest = m_driver.FindElements(By.ClassName("quizQuestionAnswerCheckbox"));
            Assert.Greater(checkBoxTest.Count, 0); // Assert if at least one check box exists

            var pullDownTest = m_driver.FindElements(By.ClassName("quizQuestionAnswerSelect"));
            Assert.Greater(pullDownTest.Count, 0); // Assert if at least one pulld down button exists

            m_driver.Close();
            m_driver.SwitchTo().Window(priorHandles[0]);
        }

        public void pullDownButton()
        {
            functionButton(4);
        }
        public void radioButton()
        {
            functionButton(3);
        }
        public void checkBox()
        {
            functionButton(5);
        }

        public void functionButton(int buttonType)
        {
            Random r = new Random();
            int rInt = r.Next(1, 10); // random value between 1 and 10 new records for each category
            r = new Random();
            int rNewRecords = r.Next(1, 5); // for each category , random between 1 and 5 new records

            int formNumber = m_driver.FindElements(By.XPath("//*[@id='question']")).Count;
            //creating 10 question forms
            for (int i = formNumber; i < formNumber+ rInt; i++)
            {
                int j = 2 + i;
                string xpath = "//*[@id='root']/div[1]/main/div[22]/div[" + j + "]/div/button[1]";
                IWebElement buttonAddForm = m_driver.FindElement(By.XPath(xpath));
                buttonAddForm.Click();

                Thread.Sleep(500);
                var question = Faker.Lorem.Sentence();
                var questionTitles = m_driver.FindElements(By.XPath("//*[@id='question']"));
                questionTitles[i].SendKeys(question);

                //selecting radio button
                var listDropdown = m_driver.FindElements(By.CssSelector("[class*='MuiCard-root'] > * > * > *> [role='button']"));
                listDropdown[i].Click();
                Thread.Sleep(500);
                string selectionButton = "//*[@id='menu-']/div[3]/ul/li[" + buttonType + "]";
                IWebElement selection = m_driver.FindElement(By.XPath(selectionButton));
                selection.Click();

                Thread.Sleep(1000);
                int h = i + 1;
                string radiobuttonPath = "//*[@id='root']/div[1]/main/div[22]/div[" + h + "]/div[1]/div[2]/div/button";
                IWebElement addRadioButton = m_driver.FindElement(By.XPath(radiobuttonPath)); // clicking add button

                for ( int input = 0; input < rNewRecords; input++)
                {
                    Thread.Sleep(500);
                    addRadioButton.Click();
                    var inputString = "//*[@id='q-" + i + "']/div/div/input";
                    var inputs = m_driver.FindElements(By.XPath(inputString));
                    inputs[inputs.Count -1].SendKeys(Faker.RandomNumber.Next().ToString());
                }
            }
        }

        [Test]
        public void QuizCreateAddPersonalInformation()
        {
            login();
            clickPublishCreateButton();
            Actions actions = new Actions(m_driver);

            var timeNow = DateTime.Now;
            var titleName = "_UITEST" + timeNow.ToString();

            IWebElement title = m_driver.FindElement(By.Id("title"));
            title.SendKeys(titleName);

            IWebElement buttonAddPersonalForm = m_driver.FindElement(By.Id("addPrivate"));
            buttonAddPersonalForm.Click();

            var questionList = m_driver.FindElements(By.XPath("//*[@id='question']"));
            Assert.AreEqual(9, questionList.Count); // when creating Personal information, there are 9 questions

            /************************************************/

            //test randomly entering a name           
            Random r = new Random();
            int randomTextBoxnumber = 0;
            do
            {
                randomTextBoxnumber = r.Next(0, 8);
            } while (randomTextBoxnumber == 6);
            while (questionList[randomTextBoxnumber].GetAttribute("value").Length != 0)
            {
                questionList[randomTextBoxnumber].SendKeys(Keys.Backspace);
            }
            string textBoxMessage = Faker.Lorem.Sentence();
            questionList[randomTextBoxnumber].SendKeys(textBoxMessage);

            var textBoxString = "//*[@id='q-" + randomTextBoxnumber + "']/span/p";
            IWebElement textBox = m_driver.FindElement(By.XPath(textBoxString));
            Assert.AreEqual(textBox.Text, textBoxMessage);

            /************************************************/

            //Prefectures testing
            var prefectures = m_driver.FindElements(By.XPath("//*[@id='q-6']"));
            Assert.AreEqual(prefectures.Count, 47); // Assert 47 prefectures must appears

            Thread.Sleep(500);
            IWebElement deleteButton = prefectures[0].FindElement(By.XPath("../..//button"));
            deleteButton.Click(); // deleting the first prefecture

            IWebElement previewButton = m_driver.FindElement(By.XPath("//*[@id='root']/div[1]/main/div[22]/div[7]/div[1]/div[3]/div[1]/button"));
            previewButton.Click(); // clicking on preview

            Thread.Sleep(3000);
            try
            {
                IWebElement selectOptionButton = m_driver.FindElement(By.XPath("//*[@id='q-6']/div/div"));
                selectOptionButton.Click();
            }
            catch (Exception e)
            {
                IWebElement emailBox = m_driver.FindElement(By.XPath("//*[@id='q-4']"));
                IWebElement selectOptionButton = m_driver.FindElement(By.XPath("//*[@id='q-6']/div/div"));
                actions.MoveToElement(emailBox).Perform();
                selectOptionButton.Click();
            }

            Thread.Sleep(500);
            IWebElement allOptions = m_driver.FindElement(By.XPath("//*[@id='menu-']/div[3]/ul"));
            var childrensListDropDown = allOptions.FindElements(By.TagName("li"));// get children
            childrensListDropDown[2].Click(); //click on the 3rd option
            Assert.AreEqual(47, childrensListDropDown.Count); // testing , it should have 47 elements,  46 children + element "select an option"


            //Save
            Thread.Sleep(500);
            IWebElement buttonSubmit = m_driver.FindElement(By.Id("saveButton")); // button save
            buttonSubmit.Click();

            checkInfoSavedForPersonalInfo(titleName, randomTextBoxnumber, textBoxMessage);

        }

        public void checkInfoSavedForPersonalInfo(string titleName,int checkValueIndex, string questionToBeTested)
        {
            //Edit + Preview

            quizListEditAndPreview(titleName);

            var priorHandles = m_driver.WindowHandles;
            m_driver.SwitchTo().Window(priorHandles[priorHandles.Count - 1]);

            m_driver.Navigate().Refresh();
            Thread.Sleep(1000);

            Assert.AreEqual(m_driver.FindElement(By.ClassName("content-title")).Text.Trim(), titleName); // The Title must be the correct one

            IWebElement TextBoxesList = m_driver.FindElements(By.ClassName("quizQuestionTitle"))[checkValueIndex];
            Assert.AreEqual(TextBoxesList.Text, questionToBeTested); // Assert if question is the correct one

            IWebElement course = m_driver.FindElement(By.ClassName("quizQuestionAnswerSelect"));
            var selectTest = new SelectElement(course); // this is only for dropdownlist
            Assert.AreEqual(46, selectTest.Options.Count); // assert if there are 46 options for dropdownlist

            m_driver.Close();
            m_driver.SwitchTo().Window(priorHandles[0]);
        }


        public void quizListEditAndPreview(string titleName)
        {
            //Edit + Preview

            Thread.Sleep(4000);
            m_driver.Url = env + "admin/quizList";
            Thread.Sleep(1000);

            var records = m_driver.FindElements(By.ClassName("itemTitle")); //get allrecord
            Assert.IsNotNull(records.Where(x => x.Text == titleName).FirstOrDefault()); // Assert if the record exists
            Thread.Sleep(1000);

            IWebElement savedElement = records.Where(x => x.Text == titleName).FirstOrDefault();
            //find in hierarchy the edit button
            IWebElement editButton = savedElement.FindElement(By.XPath("./../../../../div[1]/div[2]/button"));
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
