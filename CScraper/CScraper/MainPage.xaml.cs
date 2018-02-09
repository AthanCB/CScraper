using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CScraper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Product> _products;
        public MainPage()
        {
            _products = new List<Product>();
            this.InitializeComponent();
            GetSmartphonesAsync(550);
        }

        

        public async void GetSmartphonesAsync(double minprice)
        {
            string url = "https://www.skroutz.gr/c/40/kinhta-thlefwna.html?price_min=" + minprice;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductListHtml = htmlDocument.DocumentNode.Descendants("ol")
                .Where(x => x.GetAttributeValue("id", "").Equals("sku-list")).ToList();

            var ProductListItems = ProductListHtml[0].Descendants("li").ToList();


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
    }
}
