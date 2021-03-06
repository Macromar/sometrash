﻿using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TenBet
{
    extern alias ble;
    public class MainPage
    {
        
        public static Configuration conf = new Configuration();
        public static BetrallyElements betrally = new BetrallyElements();
        public Random rnd = new Random();
        public MainPage() {
            conf.LoadJson();
        }

        static void Main(string[] args)
        {
                       
        }

        public static void Login(IWebDriver driver)
        {
            conf.LoadJson();
            PageFactory.InitElements(driver, betrally);
            betrally.login.Clear();
            betrally.login.SendKeys(conf.items["Login"]);
            betrally.password.SendKeys(conf.items["Password"]);
            betrally.buttonLogin.Click();
        }
        public IWebDriver driverInit()
        {
            IWebDriver driver = new ChromeDriver(@"C:\chromedriver");
            return driver;
        }
        public void LoginPopUp(IWebDriver driver, WebDriverWait wait)
        {
            if (conf.items["LoginPopUp"]=="Yes")
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("popUpCloseButton")));
                driver.FindElement(By.ClassName("popUpCloseButton")).Click();
            }
        }
        public void Maximize(IWebDriver driver)
        {
            string value = conf.items["Maximize"];
            if (value == "Yes")
                driver.Manage().Window.Maximize();
        }
        public void NavigateToURL(IWebDriver driver)
        {
            driver.Url = "https://" + conf.items["URL"] + "/?langid=168";
        }
        public decimal GetBalance(IWebDriver driver, OpenQA.Selenium.Interactions.Actions action, WebDriverWait wait)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".serviceArea.loggedin>.btn.withArrow[data-dropdown='balance']")));
            do
            {
                try
                {
                    driver.FindElement(By.CssSelector(".serviceArea.loggedin>.btn.withArrow[data-dropdown='balance']")).Click();
                }
                catch { }
            }
            while (betrally.headerSportsBalanceDrop.Text.Length == 0);
            return decimal.Parse(Regex.Replace(betrally.headerSportsBalanceDrop.Text, "[^0-9.]", ""));
        }
        public void AddRandomHighlightEvent(IWebDriver driver, WebDriverWait wait)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(betrally.highlightFirst));
            String s;
            for (int i = 10; i >=0; i--)
            {
                try
                {
                    s = "#_me_highlights_" + rnd.Next(0, i) + ">td.bet_name>a:nth-child(" + rnd.Next(1, 2).ToString() + ")>span.num_right";
                    driver.FindElement(By.CssSelector(s)).Click();
                    break;
                }
                catch
                {}
            }
        }

        public void AddRandomLiveEvent(IWebDriver driver, WebDriverWait wait)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".live_betting_table tr[id^=me_]")));
            String s;

            ///CHANGE IT!!!!
            for (int i = 50; i >= 0; i--)
            {
                try
                {
                    s = "#me_" + rnd.Next(0, i) + ">td.bet_name>a:nth-child(" + rnd.Next(1, 2).ToString() + ")>span.num_right";
                    driver.FindElement(By.CssSelector(s)).Click();
                    break;
                }
                catch
                { }
            }
            //////
        }

        public void PlaceStake(IWebDriver driver, WebDriverWait wait, decimal stake) {
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id(betrally.editStake)));
            betrally.editBoxStake.SendKeys(stake.ToString());
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id(betrally.placeBetButton)));
            betrally.placeBetButtonElement.Click();
        }
        public string WaitForStakeStatus(IWebDriver driver, WebDriverWait wait)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(betrally.betSlipStatus)));
            while (betrally.betSlipStatusElement.Text.Contains("aiting"));
            return betrally.betSlipStatusElement.Text;
        }
        public string IsStakeAccepted(IWebDriver driver, WebDriverWait wait)
        {
            if (betrally.betSlipStatusElement.Text.Contains("accepted"))
                return "accepted";
            else return "rejected";

        }
        public decimal ChangedBalance(IWebDriver driver, WebDriverWait wait, OpenQA.Selenium.Interactions.Actions action, decimal balance, decimal stake)
        {
            wait.Until(d => GetBalance(driver,action,wait)!=balance);
            return GetBalance(driver, action, wait);
        }
        public bool IsElementPresentBy(IWebDriver driver ,By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void WaitForElementBy(IWebDriver driver, WebDriverWait wait, By element)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(element));
        }
      

    }
    public class Configuration {

        public Dictionary<string, string> items { get; private set; }

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader(@"D:\Auto\TenBet\TenBet\config.json"))
            {
                string json = r.ReadToEnd();
                items = ble::Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }
        public string GetConfigValue(string s)
        {
            return MainPage.conf.items[s];
          
        }
    }

}
