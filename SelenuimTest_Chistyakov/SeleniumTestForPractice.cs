using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace SelenuimTest_Chistyakov;

public class TestsForPractice
{
    // объявляем драйвер
    public ChromeDriver driver;

    [SetUp]
    public void SetUp()
    {
        SetDriver();
        Authorization();
    }
    
    // ОБЩИЕ МЕТОДЫ СЛУЖЕБНЫЕ
    // СОЗДАТь ДРАЙВЕР. С неявным ожиданием для действий с элементами.
    public void SetDriver()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");

        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }
    
    // ПЕРЕХОД ПО УРЛУ. Для дальнейшего использования в тестах.
    
    public void GoToUrl(string url)
    {
       driver.Navigate().GoToUrl(url); 
    }

    // АВТОРИЗАЦИЯ. Просто процедура авторизации, c явным ожиданием перехода в конце, чтобы не падали тесты при переходе по урлу в другие разделы.
    public void Authorization()
    {
        GoToUrl("https://staff-testing.testkontur.ru");
        
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("vezdeborg@gmail.com");
        
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("105%241Staff");
        
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
    }
    
    // САМИ ТЕСТЫ
    
    // Проверяет успешную авторизацию с валидным логином и паролем.
    [Test]
    public void AuthorizationSuccessful()
    {
        driver.Url.Should().Be("https://staff-testing.testkontur.ru/news");
    }

    // Проверяет, что в разделе "Компания" заголовок называется "Тестовый холдинг".
    [Test]
    public void CompanyStructureTitle()
    {
        var companySection = driver.FindElement(By.CssSelector("[data-tid='Structure']"));
        companySection.Click();
        driver.FindElement(By.CssSelector("[data-tid='Title']")).Text.Should().Be("Тестовый холдинг");
    }

    // Проверяет, что в профиле можно вписать или изменить "Адрес рабочего места".
    [Test]
    public void ProfileAddressEdit()
    {
        GoToUrl("https://staff-testing.testkontur.ru/profile/settings/edit");
        // driver.FindElement(By.CssSelector("[data-tid='Title']")).Text.Should().Contain("Редактирование профиля");
        var addressField = driver.FindElements(By.CssSelector("[data-tid='Input']"))[2];
        addressField.SendKeys(Keys.Control+"a");
        addressField.SendKeys(Keys.Backspace);
        addressField.SendKeys("Заводская");
        var saveButton = driver.FindElements(By.TagName("button"))[3];
        saveButton.Click();
        var contactCard = driver.FindElement(By.CssSelector("[data-tid='ContactCard']"));
        contactCard.Text.Should().Contain("Заводская");
    }
    
    // Проверяет успешное написание коммента и его отображание. В Мероприятия -> Мои -> Обсуждение -> Коммент.
    // Длинный урл потому что я сразу захожу в обсуждение конкретного мероприятия. Урл стабильный.
    [Test]
    public void AddComment()
    {
        GoToUrl("https://staff-testing.testkontur.ru/events/81a80b01-bf62-42e9-b739-081ed100b7c2?tab=discussions&id=34462d65-9611-4a9e-95f1-bb206bac7cca");
        var commentField = driver.FindElement(By.CssSelector("[data-tid='AddComment']"));
        commentField.Click();
        
        // элемент AddComment меняется на CommentInput, лоцируем уже его.
        var commentInput = driver.FindElement(By.CssSelector("[data-tid='CommentInput']"));
        commentInput.SendKeys("Комментарий to check"); 
        var sendButton = driver.FindElement(By.CssSelector("[data-tid='SendComment']"));
        sendButton.Click();
        Thread.Sleep(2000); // с ним работает хорошо. 
        // implicit wait из сетапа не срабатывает, потому что элемент уже есть (код читает предыдущий коммент, который был\есть до теста)
        
        // а explicit не прописать, в методах нет подходящих условий.
        // var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        // wait.Until(ExpectedConditions.чтобы что? элемент есть, он видимый, с ним все ок, просто в нем старый коммент)); Прочитал все варианты, не подходят.
        
        var commentToCheck = driver.FindElement(By.CssSelector("[data-tid='TextComment']"));
        commentToCheck.Text.Should().Contain("Комментарий to check");
        
        // удаление тестового коммента
        driver.FindElement(By.CssSelector("[data-tid='RemoveComment']")).Click();
        driver.FindElement(By.CssSelector("[data-tid='DeleteButton']")).Click();
    }
    
    // Проверяет, что в разделе Файлы поиск показывает папки и файлы соотв. запросу
    [Test]
    public void FileAndFolderSearch()
    {
        GoToUrl("https://staff-testing.testkontur.ru/files");
        var searchButton = driver.FindElement(By.CssSelector("[data-tid='Search']"));
        searchButton.Click();
        var searchHeader = driver.FindElement(By.CssSelector("[data-tid='ModalPageHeader']"));
        // вводим поисковый запрос
        searchHeader.FindElement(By.CssSelector("[data-tid='Search']")).SendKeys("123");
        
        // находим именно в теле модального окна элементы *называются, заразы, так же, как и на странице*
        var modalWindowBody = driver.FindElement(By.CssSelector("[data-tid='ModalPageBody']"));
        
        // проверяем папки
        var folders = modalWindowBody.FindElements(By.CssSelector("[data-tid='ListItemWrapper']"));
        foreach (var folder in folders)
        {
            folder.Text.Should().Contain("123");
        }
        // проверяем файлы
        var files = modalWindowBody.FindElement(By.CssSelector("[data-tid='FilesTable']"));
        var fileNames = files.FindElements(By.CssSelector("[data-tid='FilesTable']"));
        foreach (var fileName in fileNames)
        {
            fileName.Text.Should().Contain("123");
        }
    }
    
    [TearDown]
    public void TearDown()
    {
        driver.Close();
        driver.Quit();
    }
}