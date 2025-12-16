using Brand.App.Json;
using Branding.App.Brand;
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

      var appLayout = new SplitContainer();
      appLayout.Dock = DockStyle.Fill;
      appLayout.Orientation = Orientation.Vertical;
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
        RefreshPreview();
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
      // PreviewLoadingLabel.Hide();
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
      settingsBox.Text = "Profile Render Settings";
      settingsBox.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(settingsBox);

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
        var renderer = new BrandRenderer(profile);
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

    private void SyncUi(Action action) {
      if (InvokeRequired)
        BeginInvoke(action);
      else
        action();
    }

  }

}
