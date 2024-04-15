using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SelenuimTest_Chistyakov;

public class SeleniumTestForPractice 
{
    [Test]
    public void Authorization()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        
        // - зайти в хром (с помощью вебдрайвера)
        var driver = new ChromeDriver(options);
        
        //     - перейти по урлу https://staff-testing.testkontur.ru
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        Thread.Sleep(2500);
        
        // - ввести логин и пароль
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("vezdeborg@gmail.com");

        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("105%241Staff");
        
        Thread.Sleep(3000);
        
        //     - нажать на кнопку "войти"
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        
        Thread.Sleep(3000);
        
        //     - проверяем что мы находимся на нужной странице
        var currentUrl = driver.Url;
        Assert.That(currentUrl == "https://staff-testing.testkontur.ru/news");
        
        //     - закрываем браузер и убиваем процесс драйвера
        driver.Quit();
    }
}