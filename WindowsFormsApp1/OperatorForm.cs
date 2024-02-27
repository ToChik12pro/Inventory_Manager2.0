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
using System.Xml.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;


namespace WindowsFormsApp1
{
    public partial class OperatorForm : Form
    {
        private WarehouseDbContext dbContext;

        private DataGridView dataGridViewProducts;
        private DataGridView dataGridViewStockItems;
        private DataGridView dataGridViewSuppliers;
        private DataGridView dataGridViewPurchaseOrders;
        private DataGridView dataGridViewPurchaseOrderItems;
        private DataGridView dataGridViewUsers;
        private DataGridView dataGridViewRoles;
        private DataGridView dataGridViewUserRoles;

        private Button btnSaveChanges;
        private Button btnAdd;
        private Button btnDelete;

        private TabControl tabControl;
        private TabPage tabProducts;
        private TabPage tabStockItems;
        private TabPage tabSuppliers;
        private TabPage tabPurchaseOrders;
        private TabPage tabPurchaseOrderItems;
        private TabPage tabUsers;
        private TabPage tabRoles;
        private TabPage tabUserRoles;
        private Button btnSaveAsPdf;
        private FlowLayoutPanel sidePanel;
        private Button currentButton;

        public OperatorForm()
        {
            InitializeComponent();
            InitializeComponents();

            dbContext = new WarehouseDbContext();
            LoadData();
            Application.ThreadException += Application_ThreadException;
            dataGridViewProducts.DataError += DataGridView_DataError;


        }

        private void btnSaveAsPdf_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.Title = "Сохранить как PDF";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                using (var writer = new PdfWriter(filePath))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);

                        // Вызывайте метод для добавления данных каждой таблицы
                        AddDataToPdf(document, dataGridViewProducts, "Продукты");
                        AddDataToPdf(document, dataGridViewStockItems, "Товары на складе");
                        AddDataToPdf(document, dataGridViewSuppliers, "Поставщики");
                        AddDataToPdf(document, dataGridViewPurchaseOrders, "Заказы на покупку");
                        AddDataToPdf(document, dataGridViewPurchaseOrderItems, "Элементы заказа на покупку");
                    }
                }

                MessageBox.Show("Данные сохранены в PDF.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AddDataToPdf(Document document, DataGridView dataGridView, string tableName)
        {
            // Добавьте заголовок таблицы
            document.Add(new Paragraph(tableName).SetFont(PdfFontFactory.CreateFont("c:/windows/fonts/arial.ttf", "CP1251")));

            // Создайте таблицу в PDF
            var pdfTable = new iText.Layout.Element.Table(dataGridView.ColumnCount);

            // Добавьте заголовки столбцов
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                pdfTable.AddHeaderCell(new Cell().Add(new Paragraph(column.HeaderText).SetFont(PdfFontFactory.CreateFont("c:/windows/fonts/arial.ttf", "CP1251"))));
            }

            // Добавьте данные из DataGridView
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    string cellValue = cell.Value?.ToString() ?? "";

                    // Обработка разных типов данных
                    if (cell.Value is DateTime)
                    {
                        pdfTable.AddCell(new Cell().Add(new Paragraph(((DateTime)cell.Value).ToShortDateString()).SetFont(PdfFontFactory.CreateFont("c:/windows/fonts/arial.ttf", "CP1251"))));
                    }
                    else if (cell.Value is bool)
                    {
                        pdfTable.AddCell(new Cell().Add(new Paragraph((bool)cell.Value ? "Да" : "Нет").SetFont(PdfFontFactory.CreateFont("c:/windows/fonts/arial.ttf", "CP1251"))));
                    }
                    else if (cell.Value is double || cell.Value is float || cell.Value is decimal)
                    {
                        pdfTable.AddCell(new Cell().Add(new Paragraph(Convert.ToDecimal(cell.Value).ToString("F2")).SetFont(PdfFontFactory.CreateFont("c:/windows/fonts/arial.ttf", "CP1251"))));
                    }
                    else
                    {
                        pdfTable.AddCell(new Cell().Add(new Paragraph(cellValue).SetFont(PdfFontFactory.CreateFont("c:/windows/fonts/arial.ttf", "CP1251"))));
                    }
                }
            }

            // Добавьте таблицу в PDF
            document.Add(pdfTable);
            document.Add(new Paragraph("\n")); // Разделитель между таблицами
        }


        // Добавьте таблицу в PDF


        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Обработка ошибок данных в DataGridView
            MessageBox.Show($"Произошла ошибка в DataGridView: {e.Exception.Message}", "Ошибка данных", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Отмечаем, что ошибка обработана, чтобы избежать краха приложения
            e.Cancel = true;
        }
        private void LoadData()

        {
            dbContext.Products.Load();
            dataGridViewProducts.DataSource = dbContext.Products.Local.ToBindingList();

            dbContext.StockItems.Load();
            dataGridViewStockItems.DataSource = dbContext.StockItems.Local.ToBindingList();

            dbContext.Suppliers.Load();
            dataGridViewSuppliers.DataSource = dbContext.Suppliers.Local.ToBindingList();

            dbContext.PurchaseOrders.Load();
            dataGridViewPurchaseOrders.DataSource = dbContext.PurchaseOrders.Local.ToBindingList();

            dbContext.PurchaseOrderItems.Load();
            dataGridViewPurchaseOrderItems.DataSource = dbContext.PurchaseOrderItems.Local.ToBindingList();
            dbContext.PurchaseOrderItems
        .Include(item => item.PurchaseOrder)
        .Include(item => item.Product)
        .Load();

            dataGridViewPurchaseOrderItems.DataSource = dbContext.PurchaseOrderItems.Local.ToBindingList();





        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            dbContext.SaveChanges();
            MessageBox.Show("Изменения сохранены.");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Добавление новой записи в зависимости от выбранной вкладки
            
            var selectedTab = tabControl.SelectedTab;

            if (selectedTab == tabProducts)
            {
                var newProduct = new Product();
                dbContext.Products.Add(newProduct);
            }
            else if (selectedTab == tabStockItems)
            {
                var newStockItem = new StockItem();

                // Установите ProductId для StockItem на основе существующего Product или любой логики для получения допустимого ProductId
                newStockItem.ProductId = GetValidProductId();

                dbContext.StockItems.Add(newStockItem);
            }
            else if (selectedTab == tabSuppliers)
            {
                var newSupplier = new Supplier();
                dbContext.Suppliers.Add(newSupplier);

            }
            if (selectedTab == tabPurchaseOrders)
            {
                var newPurchaseOrder = new PurchaseOrder();
                int purchaseOrderId = GetValidPurchaseOrderId();
                // Задайте SupplierId в соответствии с вашей логикой
                newPurchaseOrder.SupplierId = GetValidSupplierId();

                dbContext.PurchaseOrders.Add(newPurchaseOrder);
            }

            if (selectedTab == tabPurchaseOrderItems)
            {
                var newPurchaseOrderItem = new PurchaseOrderItem();

                // Задайте PurchaseOrderId в соответствии с вашей логикой
                int purchaseOrderId = GetValidPurchaseOrderId();

                // Проверка существования заказа с указанным PurchaseOrderId
                var existingPurchaseOrder = dbContext.PurchaseOrders.Find(purchaseOrderId);
                if (existingPurchaseOrder != null)
                {
                    newPurchaseOrderItem.PurchaseOrderId = purchaseOrderId;

                    // Задайте ProductId в соответствии с вашей логикой
                    newPurchaseOrderItem.ProductId = GetValidProductId();

                    dbContext.PurchaseOrderItems.Add(newPurchaseOrderItem);
                }
                else
                {
                    // Обработка случая, когда заказ не найден
                    MessageBox.Show("Заказ с указанным PurchaseOrderId не найден.");
                }
            }
            else if (selectedTab == tabUsers)
            {
                var newUser = new User();
                dbContext.Users.Add(newUser);
            }
            else if (selectedTab == tabRoles)
            {
                var newRole = new Role();
                dbContext.Roles.Add(newRole);
            }
            else if (selectedTab == tabUserRoles)
            {
                var newUserRole = new UserRole();
                dbContext.UserRoles.Add(newUserRole);
            }

            dbContext.SaveChanges();
            LoadData();
        }
        private int GetValidSupplierId()
        {
            // Реализуйте логику для получения допустимого SupplierId,
            // например, выбрав существующий Supplier или другим способом
            var existingSupplier = dbContext.Suppliers.FirstOrDefault();
            return existingSupplier?.Id ?? 1; // Возвращаем 1, если существующий поставщик не найден
        }
        private int GetValidPurchaseOrderId()
        {
            // Реализуйте логику для получения допустимого PurchaseOrderId
            // например, выбрав существующий PurchaseOrder или другим способом
            var existingPurchaseOrder = dbContext.PurchaseOrders.FirstOrDefault();
            return existingPurchaseOrder?.Id ?? 1; // Возвращаем 1, если существующий заказ не найден
        }
        private int GetValidProductId()
        {
            // Необходимо реализовать логику для получения допустимого ProductId
            // Это может быть из существующего Product или из любого другого источника
            // Для демонстрационных целей вы можете заменить это своей фактической логикой
            var existingProduct = dbContext.Products.FirstOrDefault();
            return existingProduct?.Id ?? 1; // Возвращаем 1, если существующий продукт не найден
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Удаление выбранной записи в зависимости от выбранной вкладки
            var selectedTab = tabControl.SelectedTab;

            if (selectedTab == tabProducts)
            {
                var selectedProduct = dataGridViewProducts.SelectedRows[0].DataBoundItem as Product;
                if (selectedProduct != null)
                {
                    dbContext.Products.Remove(selectedProduct);
                }
            }
            else if (selectedTab == tabStockItems)
            {
                var selectedStockItem = dataGridViewStockItems.SelectedRows[0].DataBoundItem as StockItem;
                if (selectedStockItem != null)
                {
                    dbContext.StockItems.Remove(selectedStockItem);
                }
            }
            else if (selectedTab == tabSuppliers)
            {
                var selectedSupplier = dataGridViewSuppliers.SelectedRows[0].DataBoundItem as Supplier;
                if (selectedSupplier != null)
                {
                    dbContext.Suppliers.Remove(selectedSupplier);
                }
            }
            else if (selectedTab == tabPurchaseOrders)
            {
                var selectedPurchaseOrder = dataGridViewPurchaseOrders.SelectedRows[0].DataBoundItem as PurchaseOrder;
                if (selectedPurchaseOrder != null)
                {
                    dbContext.PurchaseOrders.Remove(selectedPurchaseOrder);
                }
            }
            else if (selectedTab == tabPurchaseOrderItems)
            {
                var selectedPurchaseOrderItem = dataGridViewPurchaseOrderItems.SelectedRows[0].DataBoundItem as PurchaseOrderItem;
                if (selectedPurchaseOrderItem != null)
                {
                    dbContext.PurchaseOrderItems.Remove(selectedPurchaseOrderItem);
                }
            }
            else if (selectedTab == tabUsers)
            {
                var selectedUser = dataGridViewUsers.SelectedRows[0].DataBoundItem as User;
                if (selectedUser != null)
                {
                    dbContext.Users.Remove(selectedUser);
                }
            }
            else if (selectedTab == tabRoles)
            {
                var selectedRole = dataGridViewRoles.SelectedRows[0].DataBoundItem as Role;
                if (selectedRole != null)
                {
                    dbContext.Roles.Remove(selectedRole);
                }
            }
            else if (selectedTab == tabUserRoles)
            {
                var selectedUserRole = dataGridViewUserRoles.SelectedRows[0].DataBoundItem as UserRole;
                if (selectedUserRole != null)
                {
                    dbContext.UserRoles.Remove(selectedUserRole);
                }
            }

            dbContext.SaveChanges();
            LoadData();
        }

        private void InitializeComponents()
        {
            // Создание FlowLayoutPanel для боковой панели
            sidePanel = new FlowLayoutPanel();
            sidePanel.Dock = DockStyle.Left;
            sidePanel.Width = 150;

            // Создание TabControl
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // Создание вкладок для каждой таблицы
            tabProducts = CreateTabPage("Продукты");
            tabStockItems = CreateTabPage("Товары на складе");
            tabSuppliers = CreateTabPage("Поставщики");
            tabPurchaseOrders = CreateTabPage("Заказы на покупку");
            tabPurchaseOrderItems = CreateTabPage("Элементы заказа на покупку");
          

            // Создание DataGridView для каждой таблицы
            dataGridViewProducts = CreateDataGridView();
            dataGridViewStockItems = CreateDataGridView();
            dataGridViewSuppliers = CreateDataGridView();
            dataGridViewPurchaseOrders = CreateDataGridView();
            dataGridViewPurchaseOrderItems = CreateDataGridView();
       

            // Создание кнопок "Сохранить изменения", "Добавить" и "Удалить"
            btnSaveChanges = CreateButton("Сохранить изменения", btnSaveChanges_Click);
            btnAdd = CreateButton("Добавить", btnAdd_Click);
            btnDelete = CreateButton("Удалить", btnDelete_Click);

            // Добавление элементов на вкладки
            tabProducts.Controls.Add(dataGridViewProducts);
            tabStockItems.Controls.Add(dataGridViewStockItems);
            tabSuppliers.Controls.Add(dataGridViewSuppliers);
            tabPurchaseOrders.Controls.Add(dataGridViewPurchaseOrders);
            tabPurchaseOrderItems.Controls.Add(dataGridViewPurchaseOrderItems);
            

            // Добавление кнопок на боковую панель
            sidePanel.Controls.Add(btnSaveChanges);
            sidePanel.Controls.Add(btnAdd);
            sidePanel.Controls.Add(btnDelete);

            // Добавление вкладок на TabControl
            tabControl.TabPages.Add(tabProducts);
            tabControl.TabPages.Add(tabStockItems);
            tabControl.TabPages.Add(tabSuppliers);
            tabControl.TabPages.Add(tabPurchaseOrders);
            tabControl.TabPages.Add(tabPurchaseOrderItems);
            btnSaveAsPdf = CreateButton("Сохранить в PDF", btnSaveAsPdf_Click);


            // Добавление TabControl и боковой панели на форму
            Controls.Add(tabControl);
            Controls.Add(sidePanel);

            // Задание свойств формы
            Text = "Оператор склада";
            Size = new System.Drawing.Size(800, 400);
        }

        private TabPage CreateTabPage(string text)
        {
            var tabPage = new TabPage(text);
            tabPage.AutoScroll = true;
            return tabPage;
        }
       
        private DataGridView CreateDataGridView()
        {
            var dataGridView = new DataGridView();
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Dock = DockStyle.Fill;
            return dataGridView;
        }

        private Button CreateButton(string text, EventHandler clickHandler)
        {
            var button = new Button();
            button.Text = text;
            button.Width = sidePanel.Width - 10;
            button.Margin = new Padding(5);
            button.Click += clickHandler;

            sidePanel.Controls.Add(button);
            return button;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Удаляем предыдущую кнопку из боковой панели
            if (currentButton != null)
            {
                sidePanel.Controls.Remove(currentButton);
            }

            // Создаем новую кнопку для текущей вкладки
            var selectedTab = tabControl.SelectedTab;
            currentButton = CreateButton($"Действие для {selectedTab.Text}", ButtonForTab_Click);
        }

        private void ButtonForTab_Click(object sender, EventArgs e)
        {
            // Обработчик события для кнопки текущей вкладки
            MessageBox.Show($"Вызвано действие для {tabControl.SelectedTab.Text}");
        }
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // Обработка глобальных ошибок
            MessageBox.Show($"Произошла ошибка: {e.Exception.Message}", "Глобальная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

