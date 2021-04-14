using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Wpf.Mvvm.Evolution.Initial
{
    internal sealed partial class MainWindow : Window
    {
        private Contact editContact;

        public MainWindow()
        {
            InitializeComponent();
        }

        private string Email
        {
            get => emailTextBox.Text;
            set => emailTextBox.Text = value;
        }

        private string FirstName
        {
            get => firstNameTextBox.Text;
            set => firstNameTextBox.Text = value;
        }

        private string LastName
        {
            get => lastNameTextBox.Text;
            set => lastNameTextBox.Text = value;
        }

        private string Phone
        {
            get => phoneTextBox.Text;
            set => phoneTextBox.Text = value;
        }

        private string PhoneCode
        {
            get => (string)phoneCodeComboBox.SelectedItem;
            set => phoneCodeComboBox.SelectedItem = value;
        }

        private Contact SelectedContact => (Contact)((ListBoxItem)contactsListBox.SelectedItem)?.Tag;

        private void AddContact(Contact contact)
        {
            AddContactToFile(contact);
            AddContactToListBox(contact);
        }

        private void AddContactToFile(Contact contact)
        {
            XDocument document = XDocument.Load(ContactsFile.Path);
            document.Root.Add(
                new XElement(
                    ContactsFile.Root.Contact.ElementName,
                    new XAttribute(ContactsFile.Root.Contact.Attributes.Email, contact.Email),
                    new XAttribute(ContactsFile.Root.Contact.Attributes.FirstName, contact.FirstName),
                    new XAttribute(ContactsFile.Root.Contact.Attributes.Id, contact.Id),
                    new XAttribute(ContactsFile.Root.Contact.Attributes.LastName, contact.LastName),
                    new XAttribute(ContactsFile.Root.Contact.Attributes.Phone, contact.Phone),
                    new XAttribute(ContactsFile.Root.Contact.Attributes.PhoneCode, contact.PhoneCode)
                )
            );
            document.Save(ContactsFile.Path);
        }

        private void AddContactToListBox(Contact contact)
        {
            ListBoxItem listBoxItem = CreateListBoxItem(contact);

            contactsListBox.Items.Add(listBoxItem);
        }

        private void AddPhoneCodeToComboBox(string phoneCode)
        {
            phoneCodeComboBox.Items.Add(phoneCode);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
            EnableInputs(isEnabled: true);

            editContact = null;
        }

        private void ClearInputs()
        {
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Phone = string.Empty;
            PhoneCode = null;
        }

        private void ClearContactToView()
        {
            selectedAreaTextBlock.Text = string.Empty;
            selectedEmailTextBlock.Text = string.Empty;
            selectedNameTextBlock.Text = string.Empty;
            selectedPhoneTextBlock.Text = string.Empty;
        }

        private void ContactsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contactsListBox.SelectedItem is null)
            {
                ClearContactToView();
            }
            else
            {
                SelectContactToView(SelectedContact);
            }
        }

        private ListBoxItem CreateListBoxItem(Contact contact)
        {
            var nameTextBlock = new TextBlock { FontWeight = FontWeights.Bold, Text = FormatContactName(contact) };

            var emailTextBlock = new TextBlock { Text = FormatContactEmail(contact) };
            contact.EmailChanged += (sender, e) => emailTextBlock.Text = FormatContactEmail(contact);

            var phoneTextBlock = new TextBlock { Text = FormatContactPhone(contact) };
            contact.PhoneChanged += (sender, e) => phoneTextBlock.Text = FormatContactPhone(contact);
            contact.PhoneCodeChanged += (sender, e) => phoneTextBlock.Text = FormatContactPhone(contact);

            var editButton = new Button { Content = "Edit" };
            editButton.Click += (sender, e) => SelectContactToEdit(contact);

            var deleteButton = new Button { Content = "Delete" };
            deleteButton.Click += (sender, e) => DeleteContact(contact);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { SharedSizeGroup = "Name" });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5.0) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { SharedSizeGroup = "Email" });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5.0) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { SharedSizeGroup = "Phone" });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5.0) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50.0) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5.0) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50.0) });
            grid.Children.Add(nameTextBlock);
            grid.Children.Add(emailTextBlock);
            grid.Children.Add(phoneTextBlock);
            grid.Children.Add(editButton);
            grid.Children.Add(deleteButton);

            Grid.SetColumn(nameTextBlock, 0);
            Grid.SetColumn(emailTextBlock, 2);
            Grid.SetColumn(phoneTextBlock, 4);
            Grid.SetColumn(editButton, 6);
            Grid.SetColumn(deleteButton, 8);

            return new ListBoxItem { Content = grid, Tag = contact };
        }

        private void DeleteContact(Contact contact)
        {
            DeleteContactFromFile(contact);
            DeleteContactFromListBox(contact);
        }

        private void DeleteContactFromFile(Contact contact)
        {
            XDocument document = XDocument.Load(ContactsFile.Path);

            IEnumerable<XElement> contactElements = document
                .Element(ContactsFile.Root.ElementName)
                .Elements(ContactsFile.Root.Contact.ElementName);
            foreach (XElement contactElement in contactElements)
            {
                XAttribute idAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.Id);
                var id = new Guid(idAttribute.Value);
                if (id.Equals(contact.Id))
                {
                    contactElement.Remove();
                    document.Save(ContactsFile.Path);
                    break;
                }
            }
        }

        private void DeleteContactFromListBox(Contact contact)
        {
            foreach (ListBoxItem listBoxItem in contactsListBox.Items)
            {
                if (listBoxItem.Tag == contact)
                {
                    contactsListBox.Items.Remove(listBoxItem);
                    break;
                }
            }
        }

        private void EditContact(Contact contact)
        {
            EditContactInFile(contact);
            EditContactInSelection(contact);
        }

        private void EditContactInFile(Contact contact)
        {
            XDocument document = XDocument.Load(ContactsFile.Path);

            IEnumerable<XElement> contactElements = document
                .Element(ContactsFile.Root.ElementName)
                .Elements(ContactsFile.Root.Contact.ElementName);
            foreach (XElement contactElement in contactElements)
            {
                XAttribute idAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.Id);
                var id = new Guid(idAttribute.Value);
                if (id.Equals(contact.Id))
                {
                    XAttribute phoneAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.Phone);
                    phoneAttribute.Value = contact.Phone;

                    XAttribute phoneCodeAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.PhoneCode);
                    phoneCodeAttribute.Value = contact.PhoneCode;

                    XAttribute emailAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.Email);
                    emailAttribute.Value = contact.Email;

                    document.Save(ContactsFile.Path);
                    break;
                }
            }
        }

        private void EditContactInSelection(Contact contact)
        {
            if (SelectedContact == contact)
            {
                SelectContactToView(contact);
            }
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSave();
        }

        private void EnableInputs(bool isEnabled)
        {
            firstNameTextBox.IsEnabled = isEnabled;
            lastNameTextBox.IsEnabled = isEnabled;
        }

        private void EnableSave()
        {
            saveButton.IsEnabled = FirstName.Length > 0 &&
                LastName.Length > 0 &&
                Email.Length > 0 &&
                Phone.Length == 7 &&
                PhoneCode != null;
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSave();
        }

        private string FormatContactArea(Contact contact)
        {
            XDocument document = XDocument.Load(PhoneCodesFile.Path);

            IEnumerable<XElement> areaElements = document
                .Element(PhoneCodesFile.Root.ElementName)
                .Elements(PhoneCodesFile.Root.Area.ElementName);
            foreach (XElement areaElement in areaElements)
            {
                IEnumerable<XElement> codeElements = areaElement.Elements(PhoneCodesFile.Root.Area.PhoneCode.ElementName);
                foreach (XElement codeElement in codeElements)
                {
                    XAttribute valueAttribute = codeElement.Attribute(PhoneCodesFile.Root.Area.PhoneCode.Attributes.Value);
                    if (valueAttribute.Value.Equals(contact.PhoneCode))
                    {
                        return areaElement.Attribute(PhoneCodesFile.Root.Area.Attributes.Name).Value;
                    }
                }
            }

            throw new Exception();
        }

        private string FormatContactEmail(Contact contact)
        {
            return contact.Email.ToLower();
        }

        private string FormatContactName(Contact contact)
        {
            return $"{contact.FirstName} {contact.LastName}";
        }

        private string FormatContactPhone(Contact contact)
        {
            return $"({contact.PhoneCode}) {contact.Phone.Substring(0, 3)}-{contact.Phone.Substring(3, 4)}";
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSave();
        }

        private void LoadContacts()
        {
            XDocument document = XDocument.Load(ContactsFile.Path);

            IEnumerable<XElement> contactElements = document
                .Element(ContactsFile.Root.ElementName)
                .Elements(ContactsFile.Root.Contact.ElementName);
            foreach (XElement contactElement in contactElements)
            {
                XAttribute idAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.Id);
                XAttribute firstNameAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.FirstName);
                XAttribute lastNameAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.LastName);
                XAttribute phoneAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.Phone);
                XAttribute phoneCodeAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.PhoneCode);
                XAttribute emailAttribute = contactElement.Attribute(ContactsFile.Root.Contact.Attributes.Email);

                var contact = new Contact(firstNameAttribute.Value, lastNameAttribute.Value, new Guid(idAttribute.Value))
                {
                    Email = emailAttribute.Value,
                    Phone = phoneAttribute.Value,
                    PhoneCode = phoneCodeAttribute.Value
                };

                AddContactToListBox(contact);
            }
        }

        private void LoadPhoneCodes()
        {
            var phoneCodes = new List<string>();

            XDocument document = XDocument.Load(PhoneCodesFile.Path);

            IEnumerable<XElement> areaElements = document
                .Element(PhoneCodesFile.Root.ElementName)
                .Elements(PhoneCodesFile.Root.Area.ElementName);
            foreach (XElement areaElement in areaElements)
            {
                IEnumerable<XElement> codeElements = areaElement.Elements(PhoneCodesFile.Root.Area.PhoneCode.ElementName);
                foreach (XElement codeElement in codeElements)
                {
                    XAttribute valueAttribute = codeElement.Attribute(PhoneCodesFile.Root.Area.PhoneCode.Attributes.Value);

                    phoneCodes.Add(valueAttribute.Value);
                }
            }

            phoneCodes.Sort();

            foreach (string phoneCode in phoneCodes)
            {
                AddPhoneCodeToComboBox(phoneCode);
            }
        }

        private void PhoneCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableSave();
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSave();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Contact contact = editContact ?? new Contact(FirstName, LastName);

            contact.Email = Email;
            contact.Phone = Phone;
            contact.PhoneCode = PhoneCode;

            if (editContact is null)
            {
                AddContact(contact);
            }
            else
            {
                EditContact(contact);
            }
        }

        private void SelectContactToEdit(Contact contact)
        {
            EnableInputs(isEnabled: false);
            UpdateInputs(contact);

            editContact = contact;
        }

        private void SelectContactToView(Contact contact)
        {
            selectedAreaTextBlock.Text = FormatContactArea(contact);
            selectedEmailTextBlock.Text = FormatContactEmail(contact);
            selectedNameTextBlock.Text = FormatContactName(contact);
            selectedPhoneTextBlock.Text = FormatContactPhone(contact);
        }

        private void UpdateInputs(Contact contact)
        {
            Email = contact.Email;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            Phone = contact.Phone;
            PhoneCode = contact.PhoneCode;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPhoneCodes();
            LoadContacts();
        }
    }
}