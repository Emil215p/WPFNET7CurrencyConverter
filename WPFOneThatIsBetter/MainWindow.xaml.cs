﻿// various usings that the software depends on
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace WPFOneThatIsBetter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            BindCurrency();
        }

        private void BindCurrency()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load("https://nationalbanken.dk/api/currencyratesxml?lang=da");
            }
            catch (Exception e)
            {
                MessageBox.Show("Converter cannot access the webpage for the national banks exchange rates.");
                lblCurrency.Content = "Error: converter cannot access the url which has the exchange rates.";
            }
            XmlNodeList codeNodes = doc.SelectNodes("//currency/@code");
            XmlNodeList rateNodes = doc.SelectNodes("//currency/@rate");

            // add them to selection options
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");

            // default value = Select
            dtCurrency.Rows.Add("Select", "Select");
            // manually inserted the Danish Krone as thats what the calculations are based on and will always have a value of 100.
            dtCurrency.Rows.Add("DKK", "100");
            // loop through the rates to add to the software
            for (int i = 0; i < codeNodes.Count; i++)
            {
                string code = codeNodes[i].Value;
                double rate = double.Parse(rateNodes[i].Value);

                dtCurrency.Rows.Add(code, rate);
            }

            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }
        private void ClearControls()
        { // will clear selected and converted currencies
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }
        Regex regex = new Regex(@"^\d+[,|\.]{0,1}\d{0,}$");

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        { // makes sure that only numbers are inserted into the input boxes

            var text = ((TextBox)sender).Text + e.Text;

            if (!regex.IsMatch(text))
            {
                // if not match deny new char
                e.Handled = true;
            }
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        { // clicking on it clears the converter
            ClearControls();
        }
        private void Convert_Click(object sender, RoutedEventArgs e)
        { // clicking on it converts the currency
            double ConvertedValue;

            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                try
                {
                    ConvertedValue = double.Parse(txtCurrency.Text);
                    lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
                }
                catch (Exception g) 
                {
                    MessageBox.Show("Invalid input.");
                }
            }
            else
            {
                try
                {
                    ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());
                    lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
                } catch (Exception d)
                {
                    MessageBox.Show("Invalid input.");
                }

            }
        }
    }
}