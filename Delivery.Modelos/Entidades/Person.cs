using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery.Modelos.Entidades
{
    public abstract class Person
    {
        [NotMapped]
        public virtual string Dni { get; set; } = string.Empty;

        [NotMapped]
        public virtual string Name { get; set; } = string.Empty;

        [NotMapped]
        public virtual Fecha DateBirth { get; set; } = new Fecha();

        [NotMapped]
        public virtual string Address { get; set; } = string.Empty;

        [NotMapped]
        public virtual string Mail { get; set; } = string.Empty;

        [NotMapped]
        public virtual string Phone { get; set; } = string.Empty;

        [NotMapped]
        public virtual string Password { get; set; } = string.Empty;

        public Person() { }

        // Copy constructor
        public Person(Person oldPerson)
        {
            if (oldPerson != null)
            {
                this.Dni = oldPerson.Dni;
                this.Name = oldPerson.Name;
                this.DateBirth = new Fecha(oldPerson.DateBirth);
                this.Address = oldPerson.Address;
                this.Mail = oldPerson.Mail;
                this.Phone = oldPerson.Phone;
                this.Password = oldPerson.Password;
            }
        }

        public string getDni() { return Dni; }
        public void setDni(string dni) { Dni = dni; }

        public string getName() { return Name; }
        public void setName(string name) { Name = name; }

        public Fecha getDateBirth() { return DateBirth; }
        public void setDateBirth(Fecha dateBirth) { DateBirth = dateBirth; }

        public string getAddress() { return Address; }
        public void setAddress(string address) { Address = address; }

        public string getMail() { return Mail; }
        public void setMail(string mail) { Mail = mail; }

        public string getPhone() { return Phone; }
        public void setPhone(string phone) { Phone = phone; }

        public string getPassword() { return Password; }
        public void setPassword(string password) { Password = password; }

        public int CalculateAge(Fecha dateBirth)
        {
            if (dateBirth == null) return 0;
            var today = DateTime.Today;
            var age = today.Year - dateBirth.Year;
            if (dateBirth.Month > today.Month || (dateBirth.Month == today.Month && dateBirth.Day > today.Day))
            {
                age--;
            }
            return age;
        }
    }
}
