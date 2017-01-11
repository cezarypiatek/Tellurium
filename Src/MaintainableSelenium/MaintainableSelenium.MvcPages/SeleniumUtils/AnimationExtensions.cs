using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.SeleniumUtils
{
    public static class AnimationExtensions
    {
        public static void DisableAnimations(this RemoteWebDriver driver)
        {
            driver.WaitUntilPageLoad();
            driver.DisableJQueryAnimations();
            driver.DisableCssAnimations();
        }

        public static void EnableAnimations(this RemoteWebDriver driver)
        {
            driver.WaitUntilPageLoad();
            driver.EnableJQueryAnimations();
            driver.EnableCssAnimations();
        }

        private static void DisableJQueryAnimations(this RemoteWebDriver driver)
        {
            driver.ExecuteScript(@"(function(){
            if(typeof $ !== 'undefined' && typeof $.fx != 'undefined')
            { 
                 $.fx.off = true;
            }
            })();");
        }

        private static void EnableJQueryAnimations(this RemoteWebDriver driver)
        {
            driver.ExecuteScript(@"(function(){
            if(typeof $ !== 'undefined' && typeof $.fx != 'undefined')
            { 
                $.fx.off = false;
            }
            })();");
        }

        private static void DisableCssAnimations(this RemoteWebDriver driver)
        {
            if (IsCssAnimationDisabled(driver) == false)
            {
                var body = driver.FindElementByTagName("body");
                driver.AppendHtml(body, @"<style id=""__SELENIUM_ANIMATION_OFF__"">
                * {
                    -webkit-transition-duration: 0ms !important;
                    -moz-transition-duration: 0ms !important;
                    -o-transition-duration: 0ms !important;
                    transition-duration: 0ms !important;
                    -webkit-animation-duration: 0ms !important;
                    -moz-animation-duration: 0ms !important;
                    -o-animation-duration: 0ms !important;
                    animation-duration: 0ms !important;
                }
                </style>");
            }
        }

        private static bool IsCssAnimationDisabled(RemoteWebDriver driver)
        {
            return (bool)driver.ExecuteScript("return document.getElementById('__SELENIUM_ANIMATION_OFF__') != null");
        }

        private static void EnableCssAnimations(this RemoteWebDriver driver)
        {
            driver.ExecuteScript(@"(function(){
                var cssOff = document.getElementById('__SELENIUM_ANIMATION_OFF__');
                if(cssOff != null)
                {
                    cssOff.remove();
                }
            })()");
        }
    }
}