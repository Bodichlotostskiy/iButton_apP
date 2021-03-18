using iButton_apP.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace iButton_apP.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}