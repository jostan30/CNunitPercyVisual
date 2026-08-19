using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PercyIO.Appium;

namespace WikipediaPercyTests;

[TestFixture]
public class WikipediaTests
{
    private AndroidDriver driver = null!;
    private AppPercy percy = null!;

    [SetUp]
    public void Setup()
    {
        // BrowserStack SDK injects all capabilities from browserstack.yml automatically.
        var options = new AppiumOptions();

        // Percy options — ignoreErrors:true so Percy failures don't break functional tests
        var percyOptions = new System.Collections.Generic.Dictionary<string, string>
        {
            { "ignoreErrors", "true" },
            { "enabled", "true" }
        };
        options.AddAdditionalAppiumOption("appium:percyOptions", percyOptions);

        driver = new AndroidDriver(
            new Uri("https://hub-cloud.browserstack.com/wd/hub"),
            options
        );

        percy = new AppPercy(driver);
    }

    /// <summary>
    /// Search for "BrowserStack" and verify the results list appears.
    /// Selectors confirmed via live session interactions.
    /// Percy captures: Home Screen, Search Results.
    /// </summary>
    [Test]
    public void SearchWikipedia_ShouldShowResults()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

        percy.Screenshot("Wikipedia Home Screen");

        // search_container confirmed via click in session
        wait.Until(d => d.FindElement(By.Id("org.wikipedia.alpha:id/search_container"))).Click();

        // search_src_text confirmed via type action in session
        var searchInput = wait.Until(d => d.FindElement(By.Id("org.wikipedia.alpha:id/search_src_text")));
        searchInput.SendKeys("BrowserStack");

        // Wait for results to load — page_list_item_container confirmed via click in session
        wait.Until(d => d.FindElements(By.Id("org.wikipedia.alpha:id/page_list_item_container")).Count > 0);

        percy.Screenshot("Search Results List");

        var results = driver.FindElements(By.Id("org.wikipedia.alpha:id/page_list_item_container"));
        Assert.That(results.Count, Is.GreaterThan(0),
            "Search results list should contain at least one result for 'BrowserStack'");

        var firstTitle = driver.FindElement(By.Id("org.wikipedia.alpha:id/page_list_item_title"));
        Assert.That(firstTitle.Displayed, Is.True,
            "First search result title should be visible");
    }

    /// <summary>
    /// Verify the Explore feed loads with the search bar visible.
    /// Percy captures: Explore Feed.
    /// </summary>
    [Test]
    public void ExploreFeed_ShouldDisplaySearchBar()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

        // search_container confirmed via click action in session
        var searchContainer = wait.Until(d => d.FindElement(By.Id("org.wikipedia.alpha:id/search_container")));

        percy.Screenshot("Explore Feed");

        Assert.That(searchContainer.Displayed, Is.True,
            "Search container should be visible on the Explore feed");
    }

    /// <summary>
    /// Open the toolbar overflow menu and verify Settings item appears.
    /// Percy captures: Overflow Menu Open.
    /// </summary>
    [Test]
    public void OverflowMenu_ShouldOpen()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

        percy.Screenshot("Home Before Overflow Menu");

        // menu_overflow_button confirmed via click in session
        wait.Until(d => d.FindElement(By.Id("org.wikipedia.alpha:id/menu_overflow_button"))).Click();

        percy.Screenshot("Overflow Menu Open");

        // "Settings" text confirmed via click action in session
        var settingsItem = wait.Until(d => d.FindElement(
            MobileBy.AndroidUIAutomator("new UiSelector().text(\"Settings\")")));
        Assert.That(settingsItem.Displayed, Is.True,
            "Settings menu item should be visible in the overflow menu");
    }

    /// <summary>
    /// Navigate to the My Lists bottom-nav tab.
    /// content-desc "My lists" confirmed via click action in session.
    /// Percy captures: My Lists Tab.
    /// </summary>
    [Test]
    public void MyListsTab_ShouldBeAccessible()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

        // "My lists" content-desc confirmed via click action in session
        wait.Until(d => d.FindElement(MobileBy.AccessibilityId("My lists"))).Click();

        percy.Screenshot("My Lists Tab");

        var myListsTab = wait.Until(d => d.FindElement(MobileBy.AccessibilityId("My lists")));
        Assert.That(myListsTab.Displayed, Is.True,
            "My Lists tab should remain visible after tapping");
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Quit();
        driver?.Dispose();
    }
}