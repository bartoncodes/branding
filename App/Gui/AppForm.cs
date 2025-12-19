using Branding.App.Json;
using Branding.App.Brand;
using Branding.App.Renderers;
using Branding.App.Util;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace Branding.App.Gui {

  public class AppForm : Form{

    private ProfileManager ProfileManager { get; set; }
    private Dictionary<string, BrandProfile> ProfilesByFileName { get; set; }

    private object RenderLock { get; init; } = new object();
    private int CurrentRenderReqNum { get; set; } = -1;
    private int NextRenderReqNum { get; set; } = 1;

    private DataGridView ProfilesTable { get; set; }
    private Button ProfilesRefreshButton { get; set; }
    private Button ProfilesSelectAllButton { get; set; }
    private Button ProfilesSelectTypeButton { get; set; }

    private TextBox ProfileNameField { get; set; }
    private ComboBox ProfileTypeField { get; set; }
    private ComboBox ProfileThemeField { get; set; }
    private TextBox ProfileWidthField { get; set; }
    private TextBox ProfileHeightField { get; set; }
    private TextBox ProfileAoiXField { get; set; }
    private TextBox ProfileAoiYField { get; set; }
    private TextBox ProfileAoiWidthField { get; set; }
    private TextBox ProfileAoiHeightField { get; set; }

    private PictureBox PreviewPictureBox { get; set; }
    private Label PreviewLoadingLabel { get; set; }

    public AppForm() {
      var defaultProfilesDirPath = @"C:\BartonCodes\Branding\Profiles";

      ProfileManager = new ProfileManager(defaultProfilesDirPath);
      ProfilesByFileName = new Dictionary<string, BrandProfile>();

      InitForm();

      LoadProfiles();
    }

    private void InitForm() {
      Text = "Branding Generator App";
      SetClientSizeCore(1000, 600);
      WindowState = FormWindowState.Maximized;

      var appLayout = new SplitContainer();
      appLayout.Dock = DockStyle.Fill;
      appLayout.Orientation = Orientation.Vertical;
      appLayout.FixedPanel = FixedPanel.Panel2;
      appLayout.IsSplitterFixed = true;
      Controls.Add(appLayout);

      var leftLayout = new SplitContainer();
      leftLayout.Dock = DockStyle.Fill;
      leftLayout.Orientation = Orientation.Horizontal;
      appLayout.Panel1.Controls.Add(leftLayout);

      var rightLayout = new TableLayoutPanel();
      rightLayout.Dock = DockStyle.Fill;
      appLayout.Panel2.Controls.Add(rightLayout);

      rightLayout.ColumnCount = 1;
      rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));

      rightLayout.RowCount = 3;
      rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80.0f));
      rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
      rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80.0f));

      var topRightPanel = new Panel();
      topRightPanel.Dock = DockStyle.Fill;
      rightLayout.Controls.Add(topRightPanel, 0, 0);

      var middleRightPanel = new Panel();
      middleRightPanel.Dock = DockStyle.Fill;
      rightLayout.Controls.Add(middleRightPanel, 0, 1);

      var bottomRightPanel = new Panel();
      bottomRightPanel.Dock = DockStyle.Fill;
      rightLayout.Controls.Add(bottomRightPanel, 0, 2);

      InitProfilesBox(leftLayout.Panel1);
      InitPreviewBox(leftLayout.Panel2);
      InitConfigBox(topRightPanel);
      InitSettingsBox(middleRightPanel);
      InitActionsBox(bottomRightPanel);

      // Default Sizing
      appLayout.SplitterDistance = 600;
      leftLayout.SplitterDistance = 240;

      // Default preview
      RefreshPreview();
    }

    private void InitProfilesBox(Panel parentPanel) {
      var profilesBox = new GroupBox();
      profilesBox.Text = "Brand Profiles";
      profilesBox.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(profilesBox);

      var profilesLayout = new TableLayoutPanel();
      profilesLayout.Dock = DockStyle.Fill;
      profilesBox.Controls.Add(profilesLayout);

      profilesLayout.ColumnCount = 1;
      profilesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));

      profilesLayout.RowCount = 2;
      profilesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
      profilesLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35.0f));

      var tablePanel = new Panel();
      tablePanel.Dock = DockStyle.Fill;
      profilesLayout.Controls.Add(tablePanel, 0, 0);

      var buttonsPanel = new Panel();
      buttonsPanel.Dock = DockStyle.Fill;
      profilesLayout.Controls.Add(buttonsPanel, 0, 1);

      InitProfilesTable(tablePanel);
      InitProfilesButtons(buttonsPanel);
    }

    private void InitProfilesButtons(Panel parentPanel) {
      var buttonsTable = new TableLayoutPanel();
      buttonsTable.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(buttonsTable);

      buttonsTable.RowCount = 1;
      buttonsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));

      var nextColNum = 0;

      ProfilesSelectAllButton = new Button();
      ProfilesSelectAllButton.Text = "Select All";
      buttonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
      buttonsTable.Controls.Add(ProfilesSelectAllButton, nextColNum++, 0);

      ProfilesSelectTypeButton = new Button();
      ProfilesSelectTypeButton.Text = "Select Type";
      buttonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
      buttonsTable.Controls.Add(ProfilesSelectTypeButton, nextColNum++, 0);

      buttonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
      nextColNum++;

      ProfilesRefreshButton = new Button();
      ProfilesRefreshButton.Text = "Refresh";
      buttonsTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
      buttonsTable.Controls.Add(ProfilesRefreshButton, nextColNum++, 0);

      buttonsTable.ColumnCount = nextColNum;
    }

    private void InitProfilesTable(Panel parentPanel) {

      ProfilesTable = new DataGridView();
      ProfilesTable.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(ProfilesTable);

      // Configure Table
      ProfilesTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      ProfilesTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      ProfilesTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      ProfilesTable.MultiSelect = false;
      ProfilesTable.RowHeadersVisible = false;
      ProfilesTable.AllowUserToAddRows = false;
      ProfilesTable.AllowUserToDeleteRows = false;
      ProfilesTable.AllowUserToResizeRows = false;

      // Selected Column
      ProfilesTable.Columns.Add(new DataGridViewCheckBoxColumn() {
        Name = "Selected",
        HeaderText = "",
        MinimumWidth = 24,
        Width = 24,
        FillWeight = 10,
      });

      // Name Column
      ProfilesTable.Columns.Add(new DataGridViewTextBoxColumn() {
        Name = "Name",
        HeaderText = "Name",
        MinimumWidth = 100,
        FillWeight = 100,
        ReadOnly = true,

      });

      // Type Column
      ProfilesTable.Columns.Add(new DataGridViewTextBoxColumn() {
        Name = "Type",
        HeaderText = "Type",
        MinimumWidth = 50,
        FillWeight = 50,
        ReadOnly = true
      });

      // Theme Column
      ProfilesTable.Columns.Add(new DataGridViewTextBoxColumn() {
        Name = "Theme",
        HeaderText = "Theme",
        MinimumWidth = 50,
        FillWeight = 50,
        ReadOnly = true
      });

      // File Column
      ProfilesTable.Columns.Add(new DataGridViewTextBoxColumn() {
        Name = "File",
        HeaderText = "File",
        MinimumWidth = 100,
        FillWeight = 100,
        ReadOnly = true
      });

      // Selection Listener
      ProfilesTable.SelectionChanged += (o, e) => {
        var selectedProfile = GetSelectedProfile();
        OnProfileSelected(selectedProfile);
      };

    }

    private void InitPreviewBox(Panel parentPanel) {
      var previewBox = new GroupBox();
      previewBox.Text = "Preview";
      previewBox.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(previewBox);

      PreviewPictureBox = new PictureBox();
      PreviewPictureBox.Dock = DockStyle.Fill;
      PreviewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
      PreviewPictureBox.BackColor = Color.Black;
      previewBox.Controls.Add(PreviewPictureBox);

      PreviewLoadingLabel = new Label();
      PreviewLoadingLabel.Dock = DockStyle.Fill;
      PreviewLoadingLabel.Text = "Generating Preview...";
      PreviewLoadingLabel.Font = new Font("ByteBounce", 20.0f);
      PreviewLoadingLabel.BackColor = Color.Black;
      PreviewLoadingLabel.ForeColor = Color.LightGray;
      PreviewLoadingLabel.TextAlign = ContentAlignment.MiddleCenter;
      previewBox.Controls.Add(PreviewLoadingLabel);

      PreviewLoadingLabel.BringToFront();

      // Start loading label in hidden state
      PreviewLoadingLabel.Hide();
    }

    private void InitConfigBox(Panel parentPanel) {
      var configBox = new GroupBox();
      configBox.Text = "Configuration";
      configBox.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(configBox);


    }

    private void InitActionsBox(Panel parentPanel) {
      var actionsBox = new GroupBox();
      actionsBox.Text = "Actions";
      actionsBox.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(actionsBox);


    }

    private void InitSettingsBox(Panel parentPanel) {
      var settingsBox = new GroupBox();
      settingsBox.Text = "Profile";
      settingsBox.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(settingsBox);

      var table = new TableLayoutPanel();
      table.Dock = DockStyle.Fill;
      settingsBox.Controls.Add(table);

      table.ColumnCount = 4;
      for (var i = 0; i < 4; i++)
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0f));

      table.RowCount = 6;
      for (var i = 0; i < 5; i++)
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 28.0f));
      table.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));

      var typeOptions = new List<string>(Enum.GetNames<BrandType>());
      var themeOptions = ColorUtil.ColorThemes.Keys.ToList();

      // Row 0
      CreateFieldLabel("Profile Name:", table, 0, 0);
      ProfileNameField = CreateStringField(table, 1, 0, 3);

      // Row 1
      CreateFieldLabel("Type:", table, 0, 1);
      ProfileTypeField = CreateSelectField(typeOptions, table, 1, 1);
      CreateFieldLabel("Theme:", table, 2, 1);
      ProfileThemeField = CreateSelectField(themeOptions, table, 3, 1);

      // Row 2
      CreateFieldLabel("Width:", table, 0, 2);
      ProfileWidthField = CreateNumberField(table, 1, 2);
      CreateFieldLabel("Height:", table, 2, 2);
      ProfileHeightField = CreateNumberField(table, 3, 2);

      // Row 3
      CreateFieldLabel("AOI X:", table, 0, 3);
      ProfileAoiXField = CreateNumberField(table, 1, 3);
      CreateFieldLabel("AOI Y:", table, 2, 3);
      ProfileAoiYField = CreateNumberField(table, 3, 3);

      // Row 4
      CreateFieldLabel("AOI Width:", table, 0, 4);
      ProfileAoiWidthField = CreateNumberField(table, 1, 4);
      CreateFieldLabel("AOI Height:", table, 2, 4);
      ProfileAoiHeightField = CreateNumberField(table, 3, 4);

      // Row 5
      var typeSettingsBox = new GroupBox();
      typeSettingsBox.Dock = DockStyle.Fill;
      typeSettingsBox.Text = "Banner Settings"; // TODO: change when different type is selected
      table.Controls.Add(typeSettingsBox, 0, 5);
      table.SetColumnSpan(typeSettingsBox, 4);

    }

    private void AddToTable(Control control, TableLayoutPanel table, int col, int row, int colspan = -1, int rowspan = -1) {
      table.Controls.Add(control, col, row);
      if (colspan > 0)
        table.SetColumnSpan(control, colspan);
      if (rowspan > 0)
        table.SetRowSpan(control, rowspan);
    }

    private Label CreateFieldLabel(string text, TableLayoutPanel? table = null, int col = 0, int row = 0, int colspan = -1, int rowspan = -1) {
      var label = new Label();
      label.Dock = DockStyle.Fill;
      label.Text = text;
      label.TextAlign = ContentAlignment.MiddleRight;
      if (table != null)
        AddToTable(label, table, col, row, colspan, rowspan);
      return label;
    }

    private TextBox CreateStringField(TableLayoutPanel? table = null, int col = 0, int row = 0, int colspan = -1, int rowspan = -1) {
      var textBox = new TextBox();
      textBox.Dock = DockStyle.Fill;
      textBox.BorderStyle = BorderStyle.FixedSingle;

      if (table != null)
        AddToTable(textBox, table, col, row, colspan, rowspan);
      return textBox;
    }

    private TextBox CreateNumberField(TableLayoutPanel? table = null, int col = 0, int row = 0, int colspan = -1, int rowspan = -1) {
      var textBox = new TextBox();
      textBox.Dock = DockStyle.Fill;
      textBox.BorderStyle = BorderStyle.FixedSingle;

      if (table != null)
        AddToTable(textBox, table, col, row, colspan, rowspan);
      return textBox;
    }

    private ComboBox CreateSelectField(List<string> options, TableLayoutPanel? table = null, int col = 0, int row = 0, int colspan = -1, int rowspan = -1) {
      var comboBox = new ComboBox();
      comboBox.Dock = DockStyle.Fill;

      foreach (var option in options)
        comboBox.Items.Add(option);

      if (table != null)
        AddToTable(comboBox, table, col, row, colspan, rowspan);
      return comboBox;
    }

    private BrandProfile? GetSelectedProfile() {
      var selectedRows = ProfilesTable.SelectedRows;
      if (selectedRows.Count == 0)
        return null;

      var fileNameObj = selectedRows[0].Cells["File"].Value;
      if (fileNameObj == null || !(fileNameObj is string))
        return null;

      var fileName = (string)fileNameObj;
      if (!ProfilesByFileName.TryGetValue(fileName, out var profile))
        return null;

      return profile;
    }

    private BrandProfile CreateBlankProfile() {
      return new BrandProfile() {
        Name = "New Profile",
        Type = BrandType.Banner,
        Theme = "X",
        Colors = ColorUtil.ColorThemes["X"],
        Width = 100,
        Height = 100,
        AreaOfInterest = new Rectangle(0, 0, 100, 100)
      };
    }

    private void OnProfileSelected(BrandProfile? profile) {
      SetProfileInfo(profile);
      RefreshPreview();
    }

    private void LoadProfiles() {
      var profiles = ProfileManager.LoadProfiles();

      ProfilesByFileName.Clear();
      ProfilesTable.Rows.Clear();

      foreach(var profile in profiles) {
        ProfilesByFileName.Add(profile.FileName, profile);
        ProfilesTable.Rows.Add(false, profile.Name, Enum.GetName(profile.Type) ?? "", profile.Theme, profile.FileName);
      }
    }

    private void RefreshPreview() {
      // Dispose any currently shown preview image
      var currentBmp = PreviewPictureBox.Image;
      if (currentBmp != null)
        currentBmp.Dispose();

      PreviewPictureBox.Image = null;

      var profile = GetSelectedProfile();
      if (profile == null)
        return;

      PreviewLoadingLabel.Show();
      PreviewLoadingLabel.BringToFront();

      var renderReqNum = -1;
      lock (RenderLock) {
        renderReqNum = NextRenderReqNum++;
        CurrentRenderReqNum = renderReqNum;
      }
      Task.Run(() => {
        var renderer = Renderer.Create(profile);
        if(renderer == null) {
          SyncUi(() => {
            PreviewLoadingLabel.Hide();
          });
          return;
        }
        var previewBmp = renderer.Render();
        SyncUi(() => {
          if (CurrentRenderReqNum != renderReqNum) {
            previewBmp.Dispose();
            return;
          }
          PreviewPictureBox.Image = previewBmp;
          PreviewLoadingLabel.Hide();
          lock (RenderLock) {
            CurrentRenderReqNum = -1;
          }
        });
      });
    }

    private void SetProfileInfo(BrandProfile? profile) {
      if (profile == null)
        profile = CreateBlankProfile();

      ProfileNameField.Text = profile.Name;
      ProfileTypeField.SelectedItem = Enum.GetName(profile.Type) ?? "";
      ProfileThemeField.SelectedItem = profile.Theme;
      ProfileWidthField.Text = $"{profile.Width}";
      ProfileHeightField.Text = $"{profile.Height}";
      ProfileAoiXField.Text = $"{profile.AreaOfInterest.X}";
      ProfileAoiYField.Text = $"{profile.AreaOfInterest.Y}";
      ProfileAoiWidthField.Text = $"{profile.AreaOfInterest.Width}";
      ProfileAoiHeightField.Text = $"{profile.AreaOfInterest.Height}";

      // TODO: fill type specific settings
    }

    private void SyncUi(Action action) {
      if (InvokeRequired)
        BeginInvoke(action);
      else
        action();
    }

  }

}
