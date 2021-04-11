using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

//SeleniumExtras.WaitHelpers.ExpectedConditions -> pacote que deve ser instalado para poder usar o Wait
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace NerdStore.BDD.Tests.Config
{
    //Classe criada para configurar o Selenium
    public class SeleniumHelper : IDisposable
    {
        public IWebDriver WebDriver;
        public readonly ConfigurationHelper Configuration;

        //Define um tempo de espera para aguardar a tela carregar
        public WebDriverWait Wait;

        //Browser -> indica qual browser deseja que o selenium execute
        //bool headless = true -> é poder abrir o browser e poder navegar nele sem enxergar a janela
        public SeleniumHelper(Browser browser, ConfigurationHelper configuration, bool headless = true)
        {
            Configuration = configuration;

            //Método responsáver por retornar uma instância do WebDriver
            WebDriver = WebDriverFactory.CreateWebDriver(browser, Configuration.WebDrivers, headless);
            WebDriver.Manage().Window.Maximize();

            //Caso a página ainda esteja renderizando, definirá um timeout de 30 segs para aguardar o carregamento do elemento/página
            Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(30));
        }

        public string ObterUrl()
        {
            return WebDriver.Url;
        }

        //Navegando para uma determinada URL
        public void IrParaUrl(string url)
        {
            WebDriver.Navigate().GoToUrl(url);
        }

        //Validando se a URL atual do browser contém uma determinada string
        public bool ValidarConteudoUrl(string conteudo)
        {
            return Wait.Until(ExpectedConditions.UrlContains(conteudo));
        }

        public void ClicarLinkPorTexto(string linkText)
        {
            //Aguardando que um elemento esteja visível na tela e clicando nele assim que estiver visível
            //Caso o elemento não seja encontrado, será lançada uma exceção e o Selenium irá interromper a execução
            var link = Wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(linkText)));
            link.Click();
        }

        //Clicando em um botão
        public void ClicarBotaoPorId(string botaoId)
        {
            var botao = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(botaoId)));
            botao.Click();
        }

        public void ClicarPorXPath(string xPath)
        {
            var elemento = Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath)));
            elemento.Click();
        }

        public IWebElement ObterElementoPorClasse(string classeCss)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(classeCss)));
        }

        public IWebElement ObterElementoPorXPath(string xPath)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath)));
        }

        public void PreencherTextBoxPorId(string idCampo, string valorCampo)
        {
            var campo = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(idCampo)));
            //SendKeys -> Ele simula a digitação em um campo, não apenas cola o valor no textbox
            campo.SendKeys(valorCampo);
        }

        public void PreencherDropDownPorId(string idCampo, string valorCampo)
        {
            var campo = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(idCampo)));
            var selectElement = new SelectElement(campo);
            selectElement.SelectByValue(valorCampo);
        }

        public string ObterTextoElementoPorClasseCss(string className)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(className))).Text;
        }

        public string ObterTextoElementoPorId(string id)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id))).Text;
        }

        public string ObterValorTextBoxPorId(string id)
        {
            return Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)))
                .GetAttribute("value");
        }

        //Obtendo uma lista de todos os elementos que contenham uma determinada classe assim que eles estejam disponíveis na página
        public IEnumerable<IWebElement> ObterListaPorClasse(string className)
        {
            return Wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName(className)));
        }

        public bool ValidarSeElementoExistePorIr(string id)
        {
            return ElementoExistente(By.Id(id));
        }

        public void VoltarNavegacao(int vezes = 1)
        {
            for (var i = 0; i < vezes; i++)
            {
                WebDriver.Navigate().Back();
            }
        }

        public void ObterScreenShot(string nome)
        {
            SalvarScreenShot(WebDriver.TakeScreenshot(), string.Format("{0}_" + nome + ".png", DateTime.Now.ToFileTime()));
        }

        private void SalvarScreenShot(Screenshot screenshot, string fileName)
        {
            screenshot.SaveAsFile($"{Configuration.FolderPicture}{fileName}", ScreenshotImageFormat.Png);
        }

        //Validando se um elemento existe na pagina
        private bool ElementoExistente(By by)
        {
            try
            {
                //caso exista retorna true
                WebDriver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                //Caso não exista retorna falso sem lançar exceção
                return false;
            }
        }

        public void Dispose()
        {
            //WebDriver.Quit() -> Fecha o browser de fato
            WebDriver.Quit();

            //Libera a alocação de memória para o garbage colector
            WebDriver.Dispose();
        }
    }
}

