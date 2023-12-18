using HashidsNet;
using StringEncoding.App.Helpers;
using StringEncoding.App.Models;
using System.Windows;
using System.Windows.Controls;

namespace StringEncoding.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private async Task InitializeData()
        {
            List<Profile> profiles = await LocalStorage.GetProfiles();
            comboProfile.ItemsSource = profiles;
            comboProfile.DisplayMemberPath = "Name";
            if (profiles.Count > 0)
            {
                comboProfile.SelectedIndex = 0;
                btnDelete.IsEnabled = true;
            }
        }

        private void btnEncodeClickHandler(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtMinLength.Text, out int minLength);
            if (minLength < 0)
            {
                MessageBox.Show(this, "Minimum length cannot be read");
                return;
            }

            try
            {
                Hashids hashids = new Hashids(txtSalt.Text, minLength);
                if (!checkUseLongId.IsChecked.Value)
                {
                    int.TryParse(txtPlainId.Text, out int plainId);
                    txtEncodedId.Text = hashids.Encode(plainId);
                }
                else
                {
                    long.TryParse(txtPlainId.Text, out long plainId);
                    txtEncodedId.Text = hashids.EncodeLong(plainId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btnDecodeClickHandler(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtMinLength.Text, out int minLength);
            if (minLength < 0)
            {
                MessageBox.Show(this, "Minimum length cannot be read");
                return;
            }

            try
            {
                Hashids hashids = new Hashids(txtSalt.Text, minLength);
                if (!checkUseLongId.IsChecked.Value)
                    txtPlainId.Text = hashids.DecodeSingle(txtEncodedId.Text).ToString();
                else
                    txtPlainId.Text = hashids.DecodeSingleLong(txtEncodedId.Text).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void btnSaveProfileClickHandler(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProfileName.Text))
            {
                MessageBox.Show(this, "Name cannot be empty");
                return;
            }
            try
            {
                int minLength = int.Parse(txtMinLength.Text);
                Profile profile = new Profile
                {
                    Name = txtProfileName.Text,
                    MinLenght = minLength,
                    Salt = txtSalt.Text
                };
                await LocalStorage.AddProfile(profile);
                await InitializeData();
                MessageBox.Show(this, "Profile saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void comboProfileChangeHandler(object sender, SelectionChangedEventArgs e)
        {
            Profile profile = comboProfile.SelectedItem as Profile;
            if (profile == null)
            {
                return;
            }
            txtMinLength.Text = profile.MinLenght.ToString();
            txtSalt.Text = profile.Salt;
            txtProfileName.Text = profile.Name;
        }

        private async void btnDeleteClickHandler(object sender, RoutedEventArgs e)
        {
            Profile profile = comboProfile.SelectedItem as Profile;
            if (profile == null)
            {
                return;
            }
            await LocalStorage.DeleteProfile(profile);
            await InitializeData();
            MessageBox.Show(this, "Profile deleted");
        }
    }
}