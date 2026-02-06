using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VendorManager.Data;
using VendorManager.Data.Models;

namespace VendorManager.Forms
{
    public partial class VendorEditForm : Form
    {
        private Vendor _vendor;
        private AppDbContext _context;
        private bool _isNew;

        private TextBox _txtMacs;
        private TextBox _txtBrand;
        private Button _btnSave;
        private Button _btnCancel;
        private Label _lblValidation;

        public VendorEditForm(Vendor vendor, AppDbContext context)
        {
            _vendor = vendor ?? new Vendor();
            _context = context;
            _isNew = vendor == null;

            InitializeComponent();
            InitializeControls();
            LoadData();

            this.StartPosition = FormStartPosition.CenterParent;
            this.AcceptButton = _btnSave;
            this.CancelButton = _btnCancel;
        }

        private void InitializeComponent()
        {
            this.Text = _isNew ? "Новый вендор" : "Редактирование вендора";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = SystemColors.Control;
            this.Padding = new Padding(10);
        }

        private void InitializeControls()
        {
            Label lblMacs = new Label
            {
                Text = "MAC адрес вендора:",
                Location = new Point(20, 20),
                Size = new Size(150, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _txtMacs = new TextBox
            {
                Location = new Point(180, 20),
                Size = new Size(180, 25),
                MaxLength = 8
            };
            _txtMacs.TextChanged += ValidateInput;

            Label lblBrand = new Label
            {
                Text = "Наименование:",
                Location = new Point(20, 60),
                Size = new Size(150, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _txtBrand = new TextBox
            {
                Location = new Point(180, 60),
                Size = new Size(180, 25),
                MaxLength = 255
            };
            _txtBrand.TextChanged += ValidateInput;

            _lblValidation = new Label
            {
                Location = new Point(20, 100),
                Size = new Size(340, 40),
                ForeColor = Color.Red,
                Font = new Font(this.Font, FontStyle.Italic)
            };

            _btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(180, 160),
                Size = new Size(85, 30),
                Enabled = false
            };
            _btnSave.Click += SaveVendor;

            _btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(275, 160),
                Size = new Size(85, 30)
            };
            _btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(lblMacs);
            this.Controls.Add(_txtMacs);
            this.Controls.Add(lblBrand);
            this.Controls.Add(_txtBrand);
            this.Controls.Add(_lblValidation);
            this.Controls.Add(_btnSave);
            this.Controls.Add(_btnCancel);
        }

        private void LoadData()
        {
            if (!_isNew)
            {
                _txtMacs.Text = _vendor.macs;
                _txtBrand.Text = _vendor.brand;
            }
        }

        private void ValidateInput(object sender, EventArgs e)
        {
            string macs = _txtMacs.Text.Trim().ToUpper();
            string brand = _txtBrand.Text.Trim();

            bool macValid = Regex.IsMatch(macs, @"^[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}$");

            bool macUnique = true;
            if (macValid)
            {
                macUnique = !_context.Vendors.Any(v =>
                    v.macs == macs && (_isNew || v.id != _vendor.id));
            }

            bool brandValid = !string.IsNullOrWhiteSpace(brand);

            if (!macValid)
            {
                _lblValidation.Text = "MAC адрес должен быть в формате XX:XX:XX (например: 02:42:BD)";
                _btnSave.Enabled = false;
            }
            else if (!macUnique)
            {
                _lblValidation.Text = "MAC адрес уже существует в базе данных";
                _btnSave.Enabled = false;
            }
            else if (!brandValid)
            {
                _lblValidation.Text = "Введите наименование вендора";
                _btnSave.Enabled = false;
            }
            else
            {
                _lblValidation.Text = "✓ Все поля заполнены корректно";
                _lblValidation.ForeColor = Color.Green;
                _btnSave.Enabled = true;
            }
        }

        private void SaveVendor(object sender, EventArgs e)
        {
            try
            {
                _vendor.macs = _txtMacs.Text.Trim().ToUpper();
                _vendor.brand = _txtBrand.Text.Trim();

                if (_isNew)
                {
                    _context.Vendors.Add(_vendor);
                }
                else
                {
                    _context.Entry(_vendor).State = System.Data.Entity.EntityState.Modified;
                }

                _context.SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}