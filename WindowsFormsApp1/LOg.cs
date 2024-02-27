using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class LOg : Form
    {
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private Label usernameLabel;
        private Label passwordLabel;
        private Button loginButton;
        private Button exitButton;
        private CheckBox showPasswordCheckBox;
        private readonly WarehouseDbContext _dbContext;
        public LOg()
        {
            _dbContext = new WarehouseDbContext();
            EnsureDatabaseCreated();
            InitializeComponent();
            InitializeComponents();
            FillTablesIfNeeded();
            FillPurchaseOrderItems();
            CreateOperatorUserIfNotExists();
        }
        private void EnsureDatabaseCreated()
        {
            if (!_dbContext.Database.CanConnect())
            {
                _dbContext.Database.EnsureCreated();
                MessageBox.Show("База данных успешно создана!");

                CreateAdminUserIfNotExists(); 
                FillTablesIfNeeded();
            }
            }


        private void CreateAdminUserIfNotExists()
        {
            var adminUsername = "admin";
            var adminPassword = "adminpassword";

            // Проверяем, существует ли уже пользователь с именем "admin"
            if (!_dbContext.Users.Any(u => u.Username == adminUsername))
            {
                // Если нет, создаем администратора
                var admin = new User
                {
                    Username = adminUsername,
                    Password = adminPassword
                };

                var adminRole = new Role
                {
                    Name = "Admin"
                };

                var userRole = new UserRole
                {
                    User = admin,
                    Role = adminRole
                };

                _dbContext.Users.Add(admin);
                _dbContext.Roles.Add(adminRole);
                _dbContext.UserRoles.Add(userRole);
                _dbContext.SaveChanges();

                MessageBox.Show("Администратор успешно создан!");
            }
            else
            {
                MessageBox.Show("Администратор уже существует!");
            }
        }
        private void CreateOperatorUserIfNotExists()
        {
            var operatorUsername = "operator";
            var operatorPassword = "operatorpassword";

            // Проверяем, существует ли уже пользователь с именем "operator"
            if (!_dbContext.Users.Any(u => u.Username == operatorUsername))
            {
                // Если нет, создаем оператора
                var operatorUser = new User
                {
                    Username = operatorUsername,
                    Password = operatorPassword
                };

                var operatorRole = new Role
                {
                    Name = "Operator"
                };

                var userRole = new UserRole
                {
                    User = operatorUser,
                    Role = operatorRole
                };

                _dbContext.Users.Add(operatorUser);
                _dbContext.Roles.Add(operatorRole);
                _dbContext.UserRoles.Add(userRole);
                _dbContext.SaveChanges();

                MessageBox.Show("Оператор успешно создан!");
            }
            else
            {
                MessageBox.Show("Оператор уже существует!");
            }
        }
        private void InitializeComponents()
        {
            // TextBox для ввода имени пользователя
            usernameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(150, 50),
                Size = new System.Drawing.Size(200, 20)
            };

            // TextBox для ввода пароля
            passwordTextBox = new TextBox
            {
                Location = new System.Drawing.Point(150, 80),
                Size = new System.Drawing.Size(200, 20),
                PasswordChar = '*'
            };

            // Label для отображения "Имя пользователя"
            usernameLabel = new Label
            {
                Text = "Имя пользователя:",
                Location = new System.Drawing.Point(50, 50),
                AutoSize = true
            };

            // Label для отображения "Пароль"
            passwordLabel = new Label
            {
                Text = "Пароль:",
                Location = new System.Drawing.Point(50, 80),
                AutoSize = true
            };

            // Кнопка "Вход"
            loginButton = new Button
            {
                Text = "Вход",
                Location = new System.Drawing.Point(150, 120),
                Size = new System.Drawing.Size(100, 30)
            };
            loginButton.Click += btnLogin_Click;

            // Кнопка "Выход"
            exitButton = new Button
            {
                Text = "Выход",
                Location = new System.Drawing.Point(270, 120),
                Size = new System.Drawing.Size(100, 30)
            };
            exitButton.Click += btnExit_Click;
            showPasswordCheckBox = new CheckBox
            {
                Text = "Показать пароль",
                Location = new System.Drawing.Point(150, 100),
                AutoSize = true
            };
            showPasswordCheckBox.CheckedChanged += ShowPasswordCheckBox_CheckedChanged;
            Controls.Add(showPasswordCheckBox);

            // Добавляем элементы на форму
            Controls.Add(usernameTextBox);
            Controls.Add(passwordTextBox);
            Controls.Add(usernameLabel);
            Controls.Add(passwordLabel);
            Controls.Add(loginButton);
            Controls.Add(exitButton);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string enteredUsername = usernameTextBox.Text;
            string enteredPassword = passwordTextBox.Text;

            // Проверяем, есть ли пользователь с введенными учетными данными в базе данных
            var user = _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Username == enteredUsername && u.Password == enteredPassword);

            if (user != null)
            {
                MessageBox.Show("Вход выполнен успешно!");

                // Проверяем роль пользователя
                if (user.UserRoles.Any(ur => ur.Role.Name == "Admin"))
                {
                    // Пользователь - администратор
                    AdminForms adminForms = new AdminForms();
                    adminForms.Show();
                }
                else if (user.UserRoles.Any(ur => ur.Role.Name == "Operator"))
                {
                    // Пользователь - оператор
                    OperatorForm operatorForms = new OperatorForm();
                    operatorForms.Show();
                }

                // Закрываем текущую форму входа
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.");
            }
        }


        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Изменяем свойство PasswordChar в зависимости от состояния CheckBox
            passwordTextBox.PasswordChar = showPasswordCheckBox.Checked ? '\0' : '*';
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Добавьте код для обработки нажатия кнопки "Выход"
            Application.Exit();
        }

        private void FillPurchaseOrderItems()
        {
            var newSupplier = new Supplier { Name = "New Supplier", ContactPerson = "Contact Person" };
            _dbContext.Suppliers.Add(newSupplier);
            _dbContext.SaveChanges();
        }
        private void FillTablesIfNeeded()
        {
            // Проверяем, есть ли продукты в базе данных
            if (!_dbContext.Products.Any())
            {
                // Добавляем продукты
                var product1 = new Product
                {
                    Name = "Product1",
                    Price = 19.99m
                };

                var product2 = new Product
                {
                    Name = "Product2",
                    Price = 29.99m
                };

                _dbContext.Products.AddRange(product1, product2);
            }

            // Проверяем, есть ли поставщики в базе данных
            if (!_dbContext.Suppliers.Any())
            {
                CreateNewSupplier();
            }

            // Проверяем, есть ли заказы в базе данных
            if (!_dbContext.PurchaseOrders.Any())
            {
                var defaultSupplier = _dbContext.Suppliers.FirstOrDefault();

                var newPurchaseOrder = new PurchaseOrder
                {
                    OrderDate = DateTime.Now,
                    SupplierId = defaultSupplier?.Id ?? CreateNewSupplier().Id
                };

                var purchaseOrderItem1 = new PurchaseOrderItem
                {
                    Quantity = 10,
                    ProductId = _dbContext.Products.First().Id,
                };

                var purchaseOrderItem2 = new PurchaseOrderItem
                {
                    Quantity = 5,
                    ProductId = _dbContext.Products.Skip(1).First().Id,
                };

                newPurchaseOrder.OrderItems.Add(purchaseOrderItem1);
                newPurchaseOrder.OrderItems.Add(purchaseOrderItem2);

                _dbContext.PurchaseOrders.Add(newPurchaseOrder);
                _dbContext.SaveChanges();
            }

            // Проверяем, есть ли записи о запасах в базе данных
            if (!_dbContext.StockItems.Any())
            {
                var stockItem1 = new StockItem
                {
                    Quantity = 15,
                    ProductId = _dbContext.Products.First().Id,
                    DateAdded = DateTime.Now
                };

                var stockItem2 = new StockItem
                {
                    Quantity = 8,
                    ProductId = _dbContext.Products.Skip(1).First().Id,
                    DateAdded = DateTime.Now
                };

                _dbContext.StockItems.AddRange(stockItem1, stockItem2);
            }

            _dbContext.SaveChanges();
        }

        private Supplier CreateNewSupplier()
        {
            var newSupplier = new Supplier { Name = "New Supplier", ContactPerson = "Contact Person" };
            _dbContext.Suppliers.Add(newSupplier);
            _dbContext.SaveChanges();

            return newSupplier;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            EnsureDatabaseCreated();
            FillTablesIfNeeded();
            CreateAdminUserIfNotExists();
        }
    }
}  


