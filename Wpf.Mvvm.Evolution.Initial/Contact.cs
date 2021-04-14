using System;

namespace Wpf.Mvvm.Evolution.Initial
{
    internal sealed class Contact
    {

        private readonly string firstName;
        private readonly Guid id;
        private readonly string lastName;

        private string email = string.Empty;
        private string phone = string.Empty;
        private string phoneCode = string.Empty;

        public Contact(string firstName, string lastName) :
            this(firstName, lastName, Guid.NewGuid())
        {
        }

        public Contact(string firstName, string lastName, Guid id)
        {
            this.firstName = firstName;
            this.id = id;
            this.lastName = lastName;
        }

        public string Email
        {
            get => email;
            set
            {
                if (!email.Equals(value))
                {
                    email = value;
                    OnEmailChanged(EventArgs.Empty);
                }
            }
        }

        public string FirstName => firstName;

        public Guid Id => id;

        public string LastName => lastName;

        public string Phone
        {
            get => phone;
            set
            {
                if (!phone.Equals(value))
                {
                    phone = value;
                    OnPhoneChanged(EventArgs.Empty);
                }
            }
        }

        public string PhoneCode
        {
            get => phoneCode;
            set
            {
                if (!phoneCode.Equals(value))
                {
                    phoneCode = value;
                    OnPhoneCodeChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler<EventArgs> EmailChanged;
        public event EventHandler<EventArgs> PhoneChanged;
        public event EventHandler<EventArgs> PhoneCodeChanged;

        private void OnEmailChanged(EventArgs e)
        {
            EmailChanged?.Invoke(this, e);
        }

        private void OnPhoneChanged(EventArgs e)
        {
            PhoneChanged?.Invoke(this, e);
        }

        private void OnPhoneCodeChanged(EventArgs e)
        {
            PhoneCodeChanged?.Invoke(this, e);
        }
    }
}