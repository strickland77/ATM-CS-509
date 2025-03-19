using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using MySql;

namespace ATM
{
    class Program
    {
        abstract class User
        {
            private string login;
            private int pin;
            private string name;
            private double balance;
            private int account_number;
            
            public User(string input_login, int input_pin, string input_name, double input_balance, int input_account_number)
            {
                this.login = input_login;
                this.pin = input_pin;
                this.name = input_name;
                this.balance = input_balance;
                this.account_number = input_account_number;
            }

            public abstract void DisplayMenu();

            public int GetAccountNumber()
            {
                return this.account_number;
            }

            public string GetAccountName()
            {
                return this.name;
            }
        }
        class Admin : User
        {
            public Admin(string input_login, int input_pin, string input_name, double input_balance, int input_account_number) : 
            base(input_login, input_pin, input_name, input_balance, input_account_number){}
            public override void DisplayMenu()
            {
                Console.WriteLine("1----Create New Account");
                Console.WriteLine("2----Delete Existing Account");
                Console.WriteLine("3----Update Account Information");
                Console.WriteLine("4----Search for Account");
                Console.WriteLine("5----Exit");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        CreateAccount();
                        break;
                    case "2":
                        DeleteAccount();
                        break;
                    case "3":
                        UpdateAccount();
                        break;
                    case "4":
                        SearchAccount();
                        break;
                    case "5":
                        Exit();
                        break;
                    default:
                        Console.WriteLine("Invalid input...");
                        DisplayMenu();
                        break;
                }
            }

            public void CreateAccount()
            {
                Console.Write("Input new account login: ");
                var input_login = Console.ReadLine();
                Console.Write("Input new account pin: ");
                var input_pin = Console.ReadLine();
                Console.Write("Input new account name: ");
                var input_name = Console.ReadLine();
                Console.Write("Input new account balance: ");
                var input_balance = Console.ReadLine();
                Console.Write("Input new account status: ");
                var input_status = Console.ReadLine();

                var conn = Connect();

                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "insert into atm.users(login, pin, name, balance, status) values(@login, @pin, @name, @balance, @status)";
                cmd.Parameters.AddWithValue("@login", input_login);
                cmd.Parameters.AddWithValue("@pin", input_pin);
                cmd.Parameters.AddWithValue("@name", input_name);
                cmd.Parameters.AddWithValue("@balance", input_balance);
                cmd.Parameters.AddWithValue("@status", input_status);

                var rows = cmd.ExecuteNonQuery();
                if(rows > 0)
                {
                    User new_account = RetrieveAccountByLogin(input_login, Convert.ToInt32(input_pin));
                    Console.Write("Account Successfully Created - the account number assigned is: " + new_account.GetAccountNumber());
                }
                else
                {
                    Console.Write("New account creation failed...");
                }
            }

            public void DeleteAccount()
            {
                Console.Write("Enter the account number to which you want to delete: ");
                var input_account_number = Convert.ToInt32(Console.ReadLine());
                User user = RetrieveAccountByNumber(input_account_number);

                if(user == null)
                {
                    Console.Write("Account with that number not found...");
                    DisplayMenu();
                }
                else
                {
                    Console.Write("You wish to delete the account held by " + user.GetAccountName() + 
                        ". If this information is correct, please re-enter the account number: ");
                    var confirmed_account_number = Convert.ToInt32(Console.ReadLine());

                    if(confirmed_account_number == input_account_number)
                    {
                        var conn = Connect();

                        var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;

                        cmd.CommandText = "delete from atm.users where account_number = @account_number";
                        cmd.Parameters.AddWithValue("@account_number", confirmed_account_number);

                        var rows = cmd.ExecuteNonQuery();
                        Console.Write(rows);
                        Console.Write("Account Deleted Successfully");
                    }
                    else
                    {
                        Console.Write("Re-entered account number did not match...");
                        DisplayMenu();
                    }
                }
            }

            public void UpdateAccount()
            {
                
            }

            public void SearchAccount()
            {
                
            }

            public void Exit()
            {
                
            }

            public User RetrieveAccountByLogin(string login, int pin)
            {
                return Login(login, pin);
            }

            public User RetrieveAccountByNumber(int account_number)
            {
                var conn = Connect();

                var cmd = new MySql.Data.MySqlClient.MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from atm.users where account_number = @account_number";
                cmd.Parameters.AddWithValue("@account_number", account_number);
    
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    string db_login = reader["login"].ToString();
                    int db_pin = Convert.ToInt32(reader["pin"]);
                    string db_name = reader["name"].ToString();
                    double db_balance = Convert.ToDouble(reader["balance"]);
                    int db_account_number = Convert.ToInt32(reader["account_number"]);
    
                    if(db_name == "Admin")
                    {
                        Admin user = new Admin(db_login, db_pin, db_name, db_balance, db_account_number);
                        return user;
                    }
                    else
                    {
                        Customer user = new Customer(db_login, db_pin, db_name, db_balance, db_account_number);
                        return user;
                    }
    
                    Console.WriteLine(db_login);
                    Console.WriteLine(db_pin);
                    Console.WriteLine(db_name);
                    Console.WriteLine(db_balance);
                    Console.WriteLine(db_account_number);
                }
    
                Console.WriteLine("Found no account matching those credentials...");
                return null;
            }
        }

        class Customer : User
        {
            public Customer(string input_login, int input_pin, string input_name, double input_balance, int input_account_number) : 
            base(input_login, input_pin, input_name, input_balance, input_account_number){}
            public override void DisplayMenu()
            {
                Console.WriteLine("1----Withdraw Cash");
                Console.WriteLine("2----Deposit Cash");
                Console.WriteLine("3----Display Balance");
                Console.WriteLine("4----Exit");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        WithdrawCash();
                        break;
                    case "2":
                        DepositCash();
                        break;
                    case "3":
                        DisplayBalance();
                        break;
                    case "4":
                        Exit();
                        break;
                    default:
                        Console.WriteLine("Invalid input...");
                        DisplayMenu();
                        break;
                }
            }

            public void WithdrawCash()
            {

            }

            public void DepositCash()
            {

            }

            public void DisplayBalance()
            {

            }

            public void Exit()
            {

            }

            
        }

        private static void Main()
        {
            Console.WriteLine("Welcome to the ATM");
            var login = "";
            var input_pin = "";
            int pin;
            User user = null;
            while(user == null)
            {
                Console.Write("Input login: ");
                login = Console.ReadLine();
                Console.Write("Input pin: ");
                input_pin = Console.ReadLine();
                if(!Int32.TryParse(input_pin, out pin))
                {
                    Console.WriteLine("Input pin was not a number...");
                }
                user = Login(login, pin);
            }

            while(true)
            {
                user.DisplayMenu();
            }
        }

        private static MySql.Data.MySqlClient.MySqlConnection Connect()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=127.0.0.1;port=3306;uid=root;pwd=password;database=ATM";

            conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

            return conn;
        }

        private static User Login(string login, int pin)
        {
            var conn = Connect();

            var cmd = new MySql.Data.MySqlClient.MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from atm.users where login = @login and pin = @pin";
            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@pin", pin);

            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                string db_login = reader["login"].ToString();
                int db_pin = Convert.ToInt32(reader["pin"]);
                string db_name = reader["name"].ToString();
                double db_balance = Convert.ToDouble(reader["balance"]);
                int db_account_number = Convert.ToInt32(reader["account_number"]);

                if(db_name == "Admin")
                {
                    Admin user = new Admin(db_login, db_pin, db_name, db_balance, db_account_number);
                    return user;
                }
                else
                {
                    Customer user = new Customer(db_login, db_pin, db_name, db_balance, db_account_number);
                    return user;
                }

                Console.WriteLine(db_login);
                Console.WriteLine(db_pin);
                Console.WriteLine(db_name);
                Console.WriteLine(db_balance);
                Console.WriteLine(db_account_number);
            }

            Console.WriteLine("Found no account matching those credentials...");
            return null;
        }
    }
}