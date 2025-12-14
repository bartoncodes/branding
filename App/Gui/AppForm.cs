using Brand.App.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace Branding.App.Gui {

  public class AppForm : Form{

    private ProfileManager ProfileManager { get; set; }

    private DataGridView ProfilesTable { get; set; }
    private Button ProfilesRefreshButton { get; set; }
    private Button ProfilesSelectAllButton { get; set; }
    private Button ProfilesSelectTypeButton { get; set; }

    public AppForm() {
      var defaultProfilesDirPath = @"C:\BartonCodes\Branding\Profiles";

      ProfileManager = new ProfileManager(defaultProfilesDirPath);

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

      // TESTING
      ProfilesTable.Rows.Add(true, "Test", "Banner", "Youtube", "TestYoutubeBanner.json");

    }

    private void InitPreviewBox(Panel parentPanel) {
      var previewBox = new GroupBox();
      previewBox.Text = "Preview";
      previewBox.Dock = DockStyle.Fill;
      parentPanel.Controls.Add(previewBox);


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

    private void LoadProfiles() {
      var profiles = ProfileManager.LoadProfiles();

      ProfilesTable.Rows.Clear();

      foreach(var profile in profiles) {
        ProfilesTable.Rows.Add(false, profile.Name, Enum.GetName(profile.Type) ?? "", profile.Theme, profile.FileName);
      }
    }

  }

}
