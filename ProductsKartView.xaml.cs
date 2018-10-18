using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using oracon.Models;
using Xamarin.Forms;

namespace oracon.Views
{
    public partial class ProductsKartView : ContentPage
    {
        #region Product Data

        ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return this.products; }
            set 
            { 
                this.products = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Products));
            }
        }

        bool hasProducts;
        public bool HasProducts
        {
            get { return hasProducts; }
            set
            {
                hasProducts = value;
                OnPropertyChanged();
            }
        }

        Product selectedProduct;
        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set 
            {
                selectedProduct = value;
                //if(selectedProduct != null)
                //{
                       //DisplayAlert("Produto", "Clicou no produto: " + selectedProduct.Description+".", "OK");
                //}
                lvProducts.SelectedItem = null;
            }
        }

        public string SelectedProductOrderAmount
        {
            get
            {
                if (SelectedProduct != null)
                    return SelectedProduct.OrderAmount.ToString();
                return "0";
            }
            set
            {
                if (SelectedProduct != null)
                {
                    int aux;
                    if (value != string.Empty)
                    {
                        if (int.TryParse(value, out aux))
                        {
                            SelectedProduct.OrderAmount = aux;
                            OnPropertyChanged(nameof(SelectedProductSubTotal));
                        }
                        else
                            DisplayAlert("Pedido", "Valor de quantidade incorreto.", "OK");
                    }
                }
            }
        }

        public Double SelectedProductSubTotal
        {
            get
            {
                if (SelectedProduct != null)
                    return SelectedProduct.OrderAmount * SelectedProduct.NetAmount;
                return 0d;
            }
        }
        #endregion

        #region Order Settings

        OrderSettings orderSettings;

        public DateTime InitialDate
        {
            get { return this.orderSettings.InitialDate; }
            set { this.orderSettings.InitialDate = value; }
        }

        public DateTime FinalDate
        {
            get { return this.orderSettings.FinalDate; }
            set { this.orderSettings.FinalDate = value; }
        }

        //Valores do rodapé desta página
        int totalQuantity;
        public int TotalQuantity
        {
            get { return totalQuantity; }
            set
            {
                totalQuantity = value;
                OnPropertyChanged();
            }
        }

        double totalGrossAmount;
        public double TotalGrossAmount
        {
            get { return totalGrossAmount; }
            set
            {
                totalGrossAmount = value;
                OnPropertyChanged();
            }
        }

        double totalNetAmount;
        public double TotalNetAmount
        {
            get { return totalNetAmount; }
            set
            {
                totalNetAmount = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Client Data
        Client client;

        public string CPF_CNPJ
        {
            get { return this.client.CPF_CNPJ; }
        }

        public string SocialName
        {
            get { return this.client.SocialName; }
        }
        #endregion

        bool loading;
        public bool Loading
        {
            get { return loading; }
            set 
            { 
                loading = value;
                OnPropertyChanged();
            }
        }

        bool footerIn = true;

        public ProductsKartView()
        {
            InitializeComponent();
        }

        public ProductsKartView(ObservableCollection<Product> products, OrderSettings orderSettings)
        {
            InitializeComponent();

            this.products = products;
            this.orderSettings = orderSettings;
        }

        #region Cicles

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Loading = true;

            LoadPage();

            Loading = false;

            this.BindingContext = this;
        }

        #endregion

        #region Private Methods

        void LoadPage()
        {
            this.client = new Client()
            {
                SocialName = "Ataide Gomes Representações LTDA - ME",
                CPF_CNPJ = "04.306.837/0001-14"
            };

            HasProducts = products.Count > 0;

            TotalValuesCalculate();

            //Minimizar o Footer
            FooterIcon_Tapped(new object(), new EventArgs());
        }

        void TotalValuesCalculate()
        {
            TotalQuantity = 0;
            TotalGrossAmount = 0d;
            TotalNetAmount = 0d;

            HasProducts = products.Count > 0;

            if (!hasProducts)
                return;

            foreach (Product item in products)
            {
                if (item.OrderAmount > 0)
                {
                    TotalQuantity += item.OrderAmount;
                    TotalGrossAmount += item.OrderAmount * item.GrossAmount;
                    TotalNetAmount += item.OrderAmount * item.NetAmount;
                }
            }
        }

        void ProductsListRefresh()
        {
            lvProducts.IsVisible = false;
            lvProducts.ItemsSource = null;
            lvProducts.ItemsSource = products;
            lvProducts.IsVisible = true;
        }

        async void FooterAnimateOut()
        {
            uint duration = 250;

            await Task.WhenAll(
                footerIcon.RotateTo(180, duration),
                FooterBox.TranslateTo(0, (FooterBox.Height  - (FooterBox.Height * 0.2)), duration)
            );

            //var animation = new Animation(v => FooterBox.TranslationY = v, FooterBox.Y, (FooterBox.Y - 0.1));

            //animation.Commit(FooterBox, "SimpleAnimation", 0, 2000, Easing.Linear, (v, c) => FooterBox.Y = (FooterBox.Y - 0.1), () => false);
        }

        async void FooterAnimateIn()
        {
            uint duration = 250;

            await Task.WhenAll(
                footerIcon.RotateTo(0, duration),
                FooterBox.TranslateTo(0, 0, duration)
            );

            //var animation = new Animation(v => FooterBox.TranslationY = v, FooterBox.Y, (FooterBox.Y - 0.1));

            //animation.Commit(FooterBox, "SimpleAnimation", 0, 2000, Easing.Linear, (v, c) => FooterBox.Y = (FooterBox.Y - 0.1), () => false);
        }

        #endregion

        #region Events


        void ToolItemFinalizeOrder_Clicked(object sender, EventArgs args)
        {
            DisplayAlert("Finalizar", "Função em construção.", "OK");
        }

        void ButtonAddProduct_Clicked(object sender, EventArgs args)
        {
            Navigation.PopToRootAsync();
        }

        void FooterIcon_Tapped(object sender, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Footer Height: " + FooterBox.Height);

            if (footerIn)
            {
                footerIn = false;
                FooterAnimateOut();
            }
            else
            {
                footerIn = true;
                FooterAnimateIn();
            }
        }

        void Quantity_Clicked(object sender, EventArgs e)
        {
            Button btnQuantity = (Button)sender;
            Grid grid = (Grid)btnQuantity.Parent;
            Label lblID = (Label)grid.Children[0];

            SelectedProduct = products.Single(p => p.ID.ToString().Equals(lblID.Text));

            EntryQuantity.Text = SelectedProduct.OrderAmount.ToString();

            BoxesOverlayModal.IsVisible = true;
            BoxesModal.IsVisible = true;
        }

        void Exclude_Clicked(object sender, EventArgs e)
        {
            Button btnExclude = (Button)sender;
            Grid grid = (Grid)btnExclude.Parent;
            Label lblID = (Label)grid.Children[0];

            SelectedProduct = products.Single(p => p.ID.ToString().Equals(lblID.Text));

            BoxesOverlayModal.IsVisible = true;
            BoxeExclude.IsVisible = true;
        }

        void BoxesClose_Tapped(object sender, EventArgs e)
        {
            BoxesOverlayModal.IsVisible = false;
            BoxesModal.IsVisible = false;
            BoxeExclude.IsVisible = false;
        }

        void BoxesAlterQuantity_Clicked(object sender, EventArgs e)
        {
            int newOrderAmount;

            if (InitialDate > FinalDate)
            {
                DisplayAlert("Pedido", "Data inicial não pode ser maior que a data final", "OK");
                return;
            }

            if(EntryQuantity.Text != SelectedProduct.OrderAmount.ToString())
            {
                if (int.TryParse(EntryQuantity.Text, out newOrderAmount))
                    products.Single(p => p.ID == SelectedProduct.ID).OrderAmount = newOrderAmount;
            }

            TotalValuesCalculate();

            BoxesOverlayModal.IsVisible = false;
            BoxesModal.IsVisible = false;
        }

        void BoxeExcludeNo_Clicked(object sender, EventArgs e)
        {
            BoxesOverlayModal.IsVisible = false;
            BoxeExclude.IsVisible = false;
        }

        void BoxeExcludeYes_Clicked(object sender, EventArgs e)
        {
            Loading = true;

            Product target = products.Single(p => p.ID == SelectedProduct.ID);

            BoxesOverlayModal.IsVisible = false;
            BoxeExclude.IsVisible = false;

            products.Remove(target);

            //ProductsListRefresh();

            TotalValuesCalculate();

            Loading = false;
        }

        #endregion
    }
}
