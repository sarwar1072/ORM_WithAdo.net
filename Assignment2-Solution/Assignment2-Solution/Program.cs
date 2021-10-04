using ORM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TestSubject;

namespace Assignment2_Solution
{
    class Program
    {
        const string _connectionString = "Server=DESKTOP-48QHTC5\\SQLEXPRESS;Database=Demo;User Id = demo; Password=123456;";

        static void Main(string[] args)
        {
            //InsertData();
            GetAll();
        }

        private static void GetAll()
        {
            var dataOperation = new DataOperation<Organization>(_connectionString);
            var data = dataOperation.GetAll();
        }

        private static void InsertData()
        {
            var org = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "SMS4BD",
                WebAddress = "www.sms4bd.com",
                Branches = new List<Branch>
                {
                    new Branch
                    {
                        Id = Guid.NewGuid(),
                        Name = "Head Office",
                        PhoneNumber = "2323232",
                        Location = new Address
                        {
                            Id = Guid.NewGuid(),
                            AddressLine1 = "184, Begun Rokeya Soroni",
                            AddressLine2 = "Mirpur - 10",
                            City = "Dhaka",
                            Country = "Bangladesh",
                            Zipcode = "1216"
                        }
                    },
                    new Branch
                    {
                        Id = Guid.NewGuid(),
                        Name = "Branch Office",
                        PhoneNumber = "43242342",
                        Location = new Address
                        {
                            Id = Guid.NewGuid(),
                            AddressLine1 = "Sylhet, Bus stand road",
                            AddressLine2 = "Sylhet",
                            City = "Sylhet",
                            Country = "Bangladesh",
                            Zipcode = "323"
                        }
                    }
                }
            };

            var dataOperation = new DataOperation<Organization>(_connectionString);
            dataOperation.Insert(org);
        }
    }
}
