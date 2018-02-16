using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CScraper.Models;
using HtmlAgilityPack;
using Windows.Media.SpeechRecognition;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CScraper
{
    
    public sealed partial class MainPage : Page
    {
        private List<Product> _products;
        private static uint HResultPrivacyStatementDeclined = 0x80045509;
        private string _website;
        private string _category;
        public MainPage()
        {
            _products = new List<Product>();
            this.InitializeComponent();
        }

        

        public async Task GetSmartphonesAsync(string website, string category)
        {
            if (website.Equals("scrooge"))
            {
                await GetSkroutzSmartPhonesAsync(category);
            }
            else if (website.Equals("ebay"))
            {
                await GetEbaySmartPhonesAsync(category);
            }
        }

        private async Task GetSkroutzSmartPhonesAsync(string category)
        {
            string url = null;
            if (category.Equals("smartphones"))
            {
                url = "https://www.skroutz.gr/c/40/kinhta-thlefwna.html";
            }
            else if (category.Equals("laptops"))
            {
                url = "https://www.skroutz.gr/c/25/laptop.html?";
            }
           

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductListHtml = htmlDocument.DocumentNode.Descendants("ol")
                .Where(x => x.GetAttributeValue("id", "").Equals("sku-list")).ToList();
            
            var ProductListItems = ProductListHtml[0].Descendants("li").ToList();

            ProgressRing.IsActive = true;
            await Task.Delay(5000);
            ProgressRing.IsActive = false;
            foreach (var i in ProductListItems)
            {

                try
                {
                    var name = i.Descendants("a")
                        .First(x => x.GetAttributeValue("class", "").Contains("sku-link js-sku-link"))
                        .GetAttributeValue("title", "");
                    var price = i.Descendants("a").First(x => x.GetAttributeValue("class", "").Contains("sku-link js-sku-link"))
                        .InnerText.Trim('€');
                    string temp = price;
                    if (price.Contains('.'))
                    {
                        temp = price.Replace(".", string.Empty);
                    }
                    var newprice = temp.Replace(',', '.');
                    var manufacturerName = name.Split(' ')[0];



                    Product product = new Product();
                    product.Id = Guid.NewGuid().ToString("N");
                    product.Name = name;
                    product.Price = Double.Parse(newprice);
                    _products.Add(product);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + " ");
                }

            }

            ProductListView.ItemsSource = _products;
        }

        private async Task GetEbaySmartPhonesAsync(string category)
        {
            string url = null;
            if (category.Equals("smartphones"))
            {
                url = "https://www.skroutz.gr/c/40/kinhta-thlefwna.html";
            }
            else if (category.Equals("laptops"))
            {
                url = "https://www.ebay.co.uk/b/PC-Laptops-Netbooks/177/bn_450756";
            }


            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductListHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(x => x.GetAttributeValue("class", "").Equals("b-list__items_nofooter")).ToList();

            var ProductListItems = ProductListHtml[0].Descendants("li").ToList();

            ProgressRing.IsActive = true;
            await Task.Delay(5000);
            ProgressRing.IsActive = false;
            foreach (var i in ProductListItems)
            {

                try
                {
                    var name = i.Descendants("h3")
                        .First(x => x.GetAttributeValue("class", "").Contains("s-item__title")).InnerText.Trim('\t');
                    var price = i.Descendants("span").First(x => x.GetAttributeValue("class", "").Contains("s-item__price"))
                        .InnerText.Trim('£');

                    var newprice = Double.Parse(price);



                    Product product = new Product();
                    product.Id = Guid.NewGuid().ToString("N");
                    product.Name = name;
                    product.Price = Double.Parse(price);
                    _products.Add(product);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + " ");
                }

            }

            ProductListView.ItemsSource = _products;
        }

        private async void StartVoiceRecognition_OnClick(object sender, RoutedEventArgs e)
        {
            if (VoiceStackPanel.Visibility == Visibility.Visible)
            {
                VoiceStackPanel.Visibility = Visibility.Collapsed;
                ProductListView.Visibility = Visibility.Visible;
            }
            else
            {
                _products = new List<Product>();
            }
           
           

            var speechRecognizer = new SpeechRecognizer();
            
            await speechRecognizer.CompileConstraintsAsync();
            
            try
            {
                SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeWithUIAsync();

                var message = speechRecognitionResult.Text.Split();
                _website = message[0];

                if (message.Length < 3)
                {
                    _category = message[1];
                }
                else if(message.Length == 3)
                {
                    _category = message[1] + message[2];
                }

                await GetSmartphonesAsync(_website,_category);
                
            }
            catch (Exception exception)
            {
                if ((uint)exception.HResult == HResultPrivacyStatementDeclined)
                {
                   
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-accounts"));
                }
                else
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
                    await messageDialog.ShowAsync();
                }
            }
            
           
        }
    }
}
