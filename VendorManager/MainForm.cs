using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VendorManager.Data;
using VendorManager.Data.Models;

namespace VendorManager.Forms
{
    public partial class MainForm : Form
    {
        private AppDbContext _context;
        private TabControl _tabControl;
        private DataGridView _vendorsGrid;
        private DataGridView _ipMacGrid;
        private BindingSource _vendorsBindingSource;
        private BindingSource _ipMacBindingSource;
        private Timer _statusTimer;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeUI();
            LoadVendorsData();
            LoadIPMacData();
            StartStatusTimer();
        }

        private void InitializeComponent()
        {
            this.Text = "Vendor Manager - Управление базой данных";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(800, 600);
            this.BackColor = SystemColors.Control;
            this.Font = new Font("Segoe UI", 9F);
        }

        private void InitializeDatabase()
        {
            try
            {
                _context = new AppDbContext();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeUI()
        {
            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.FlatButtons,
                ItemSize = new Size(150, 30),
                SizeMode = TabSizeMode.Fixed,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            TabPage vendorsTab = new TabPage("Справочник вендоров")
            {
                BackColor = SystemColors.Control,
                Padding = new Padding(10)
            };

            TabPage ipMacTab = new TabPage("Редактор IP/MAC")
            {
                BackColor = SystemColors.Control,
                Padding = new Padding(10)
            };

            InitializeVendorsTab(vendorsTab);
            InitializeIPMacTab(ipMacTab);

            _tabControl.TabPages.Add(vendorsTab);
            _tabControl.TabPages.Add(ipMacTab);

            this.Controls.Add(_tabControl);
        }

        private void InitializeVendorsTab(TabPage tab)
        {
            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(240, 240, 245)
            };

            Button btnAdd = CreateControlButton("➕ Добавить", Color.FromArgb(46, 139, 87));
            btnAdd.Click += (s, e) => AddVendor();

            Button btnEdit = CreateControlButton("✏️ Редактировать", Color.FromArgb(30, 144, 255));
            btnEdit.Click += (s, e) => EditVendor();

            Button btnDelete = CreateControlButton("🗑 Удалить", Color.FromArgb(220, 53, 69));
            btnDelete.Click += (s, e) => DeleteVendor();

            Button btnRefresh = CreateControlButton("🔄 Обновить", Color.FromArgb(108, 117, 125));
            btnRefresh.Click += (s, e) => LoadVendorsData();

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(5)
            };

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnEdit);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnRefresh);

            controlPanel.Controls.Add(buttonPanel);

            _vendorsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 40,
                EnableHeadersVisualStyles = false
            };

            _vendorsGrid.Columns.Add("id", "ID");
            _vendorsGrid.Columns.Add("macs", "MAC адрес");
            _vendorsGrid.Columns.Add("brand", "Наименование");

            _vendorsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            _vendorsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            _vendorsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            _vendorsGrid.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            _vendorsGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            _vendorsGrid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    EditVendor();
            };

            _vendorsBindingSource = new BindingSource();
            _vendorsGrid.DataSource = _vendorsBindingSource;

            tab.Controls.Add(_vendorsGrid);
            tab.Controls.Add(controlPanel);
        }

        private void InitializeIPMacTab(TabPage tab)
        {
            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(240, 240, 245)
            };

            Button btnRefresh = CreateControlButton("🔄 Обновить", Color.FromArgb(108, 117, 125));
            btnRefresh.Click += (s, e) => LoadIPMacData();

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(5)
            };

            buttonPanel.Controls.Add(btnRefresh);

            controlPanel.Controls.Add(buttonPanel);

            _ipMacGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 40,
                EnableHeadersVisualStyles = false
            };

            _ipMacGrid.Columns.Add("Id", "ID");
            _ipMacGrid.Columns.Add("Mac", "MAC адрес");
            _ipMacGrid.Columns.Add("ip_cur", "IP адрес");
            _ipMacGrid.Columns.Add("Vendor", "Вендор");

            DataGridViewCheckBoxColumn inbaseColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Inbase",
                HeaderText = "Действует",
                Width = 80
            };
            _ipMacGrid.Columns.Add(inbaseColumn);

            _ipMacGrid.Columns.Add("last_dateupdate", "Последнее обновление");

            foreach (DataGridViewColumn column in _ipMacGrid.Columns)
            {
                if (column.Name != "Inbase")
                {
                    column.ReadOnly = true;
                }
            }

            _ipMacGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            _ipMacGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            _ipMacGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            _ipMacGrid.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            _ipMacGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            _ipMacGrid.CellValueChanged += IPMacGrid_CellValueChanged;
            _ipMacGrid.CurrentCellDirtyStateChanged += IPMacGrid_CurrentCellDirtyStateChanged;
            _ipMacGrid.CellFormatting += IPMacGrid_CellFormatting;

            _ipMacBindingSource = new BindingSource();
            _ipMacGrid.DataSource = _ipMacBindingSource;

            tab.Controls.Add(_ipMacGrid);
            tab.Controls.Add(controlPanel);
        }

        private Button CreateControlButton(string text, Color color)
        {
            Button button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.White,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Margin = new Padding(3),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(color.R + 20, 255),
                Math.Min(color.G + 20, 255),
                Math.Min(color.B + 20, 255));

            return button;
        }

        private void LoadVendorsData()
        {
            try
            {
                if (_context == null) return;

                var vendors = _context.Vendors
                    .OrderBy(v => v.brand)
                    .ToList();

                _vendorsBindingSource.DataSource = vendors;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных вендоров:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadIPMacData()
        {
            try
            {
                if (_context == null) return;

                var query = from ipmac in _context.IPMacs
                            join vendor in _context.Vendors
                            on ipmac.Mac.Substring(0, 8) equals vendor.macs into vendorGroup
                            from v in vendorGroup.DefaultIfEmpty()
                            orderby ipmac.last_dateupdate descending
                            select new
                            {
                                ipmac.Id,
                                ipmac.Mac,
                                ipmac.ip_cur,
                                Vendor = v != null ? v.brand : "Неизвестный вендор",
                                ipmac.Inbase,
                                ipmac.last_dateupdate
                            };

                var data = query.ToList();
                _ipMacBindingSource.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных IP/MAC:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddVendor()
        {
            using (var form = new VendorEditForm(null, _context))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadVendorsData();
                }
            }
        }

        private void EditVendor()
        {
            if (_vendorsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите вендор для редактирования", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var vendor = (Vendor)_vendorsBindingSource.Current;
            using (var form = new VendorEditForm(vendor, _context))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadVendorsData();
                }
            }
        }

        private void DeleteVendor()
        {
            if (_vendorsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите вендор для удаления", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var vendor = (Vendor)_vendorsBindingSource.Current;

            bool isUsed = _context.IPMacs.Any(ip =>
                ip.Mac.Replace(":", "").StartsWith(vendor.macs.Replace(":", "")));

            if (isUsed)
            {
                MessageBox.Show("Нельзя удалить вендор, который используется в таблице IP/MAC адресов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить вендор:\n\n" +
                                        $"MAC: {vendor.macs}\n" +
                                        $"Наименование: {vendor.brand}",
                                        "Подтверждение удаления",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _context.Vendors.Remove(vendor);
                    _context.SaveChanges();
                    LoadVendorsData();

                    MessageBox.Show("Вендор успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void IPMacGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (_ipMacGrid.IsCurrentCellDirty &&
                _ipMacGrid.CurrentCell.ColumnIndex == _ipMacGrid.Columns["Inbase"].Index)
            {
                _ipMacGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void IPMacGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == _ipMacGrid.Columns["Inbase"].Index)
            {
                try
                {
                    int id = (int)_ipMacGrid.Rows[e.RowIndex].Cells["Id"].Value;
                    bool? inbaseValue = _ipMacGrid.Rows[e.RowIndex].Cells["Inbase"].Value as bool?;

                    var ipmac = _context.IPMacs.Find(id);
                    if (ipmac != null)
                    {
                        ipmac.Inbase = inbaseValue;
                        ipmac.last_dateupdate = DateTime.Now;

                        _context.SaveChanges();

                        _ipMacGrid.Rows[e.RowIndex].Cells["last_dateupdate"].Value = ipmac.last_dateupdate;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadIPMacData();
                }
            }
        }

        private void IPMacGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == _ipMacGrid.Columns["Vendor"].Index)
            {
                string vendorName = e.Value?.ToString();
                if (vendorName == "Неизвестный вендор")
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(_ipMacGrid.Font, FontStyle.Italic);
                }
            }

            if (e.RowIndex >= 0 && e.ColumnIndex == _ipMacGrid.Columns["Inbase"].Index)
            {
                bool? inbaseValue = e.Value as bool?;
                if (inbaseValue == true)
                {
                    e.CellStyle.BackColor = Color.FromArgb(220, 255, 220);
                }
                else if (inbaseValue == false)
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 220, 220);
                }
            }
        }

        private void StartStatusTimer()
        {
            _statusTimer = new Timer { Interval = 1000 };
            _statusTimer.Tick += (s, e) =>
            {
                this.Text = $"Vendor Manager - Управление базой данных - {DateTime.Now:HH:mm:ss}";
            };
            _statusTimer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_statusTimer != null)
            {
                _statusTimer.Stop();
                _statusTimer.Dispose();
            }

            _context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}